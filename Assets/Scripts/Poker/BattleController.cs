using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HandEvaluator;

public class BattleController : MonoBehaviour
{
    public static BattleController instance;

    private void Awake()
    {
        instance = this;
    }

    public int startingCardsAmount = 7;    
    public int currentBet;

    public enum TurnOrder { playerBetting, playerActive, enemyActive, resolveHands}
    public TurnOrder currentPhase;

    
    // Start is called before the first frame update
    void Start()
    {
        PokerUIController.instance.placeBetButton.SetActive(true);
        PokerUIController.instance.betSlider.gameObject.SetActive(true);
        PokerUIController.instance.playHandButton.SetActive(false);
        PokerUIController.instance.swapCardButton.SetActive(false);
        PokerUIController.instance.playAgainButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (HandController.instance.selectedCards.Count == 5)
        {
            PokerUIController.instance.playHandButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            PokerUIController.instance.playHandButton.GetComponent<Button>().interactable = false;
        }
    }

    public void DrawHand()
    {
        int cardsToDraw = startingCardsAmount - HandController.instance.heldCards.Count;
        DeckController.instance.DrawMultipleCards(cardsToDraw);
    }

    public void DrawEnemyHand()
    {
        int cardsToDraw = startingCardsAmount - HandController.instance.enemyHeldCards.Count;
        DeckController.instance.DrawEnemyCards(cardsToDraw);
    }

    public void PlayHand()
    {
        HandController.instance.PlayHand();       
        AdvanceTurn();
    }

    public void PlaceBet(int bet)
    {
        currentBet = bet;
        PlayerHealth.instance.PlaceBet(bet);
        DrawHand();
        DrawEnemyHand();
        AdvanceTurn();
    }

    public void PlayAgain()
    {
        currentBet = 0;

        for (int i = 0; i < HandController.instance.playedCards.Count; i++)
        {
            Destroy(HandController.instance.playedCards[i].gameObject);
            Destroy(HandController.instance.enemyPlayedCards[i].gameObject);
        }

        HandController.instance.playedCards.Clear();
        HandController.instance.enemyPlayedCards.Clear();        
        AdvanceTurn();
    }

    public void ResolveHands(HandEvaluator.HandRank playerHandRank, HandEvaluator.HandRank enemyHandRank)
    {
        PokerUIController.instance.SetHandText(playerHandRank.ToString(), enemyHandRank.ToString());

        if (playerHandRank > enemyHandRank)
        {
            PokerUIController.instance.SetWinnerText("Player Wins!");
            PlayerHealth.instance.IncreaseHealth(currentBet);
            DealerHealth.instance.TakeDamage(currentBet);

        }
        else if (playerHandRank < enemyHandRank)
        {
            PokerUIController.instance.SetWinnerText("Enemy Wins");
            PlayerHealth.instance.TakeDamage(currentBet);
        }
        else
        {
            PokerUIController.instance.SetWinnerText("Tie");
        }
    }

    public void AdvanceTurn()
    {
        currentPhase++;

        if((int)currentPhase >= System.Enum.GetValues(typeof(TurnOrder)).Length)
        {
            currentPhase = 0;
        }

        switch (currentPhase)
        {
            case TurnOrder.playerBetting:
                PokerUIController.instance.betSlider.GetComponent<BetSlider>().InitSlider();
                PokerUIController.instance.playerHandText.gameObject.SetActive(false);
                PokerUIController.instance.enemyHandText.gameObject.SetActive(false);
                PokerUIController.instance.winnerText.gameObject.SetActive(false);
                PokerUIController.instance.playAgainButton.SetActive(false);
                PokerUIController.instance.leaveButton.SetActive(false);
                PokerUIController.instance.placeBetButton.SetActive(true);
                PokerUIController.instance.betSlider.gameObject.SetActive(true);
                PokerUIController.instance.playHandButton.SetActive(false);
                //PokerUIController.instance.swapCardButton.SetActive(false);

                break;

            case TurnOrder.playerActive:

                PokerUIController.instance.placeBetButton.SetActive(false);
                PokerUIController.instance.betSlider.gameObject.SetActive(false);
                PokerUIController.instance.playHandButton.SetActive(true);
                PokerUIController.instance.playHandButton.GetComponent<Button>().interactable = false;
                //PokerUIController.instance.swapCardButton.SetActive(true);                                

                break;

            case TurnOrder.enemyActive:
                PokerUIController.instance.playHandButton.SetActive(false);
                HandController.instance.PlayEnemyHand();
                AdvanceTurn();
                break;

            case TurnOrder.resolveHands:
                var playerHandRank = HandEvaluator.instance.EvaluateHand(HandController.instance.playedCards);
                var enemyHandRank = HandEvaluator.instance.EvaluateHand(HandController.instance.enemyPlayedCards);

                ResolveHands(playerHandRank, enemyHandRank);

                PokerUIController.instance.playerHandText.gameObject.SetActive(true);
                PokerUIController.instance.enemyHandText.gameObject.SetActive(true);
                PokerUIController.instance.winnerText.gameObject.SetActive(true);
                PokerUIController.instance.HideBetIcons();

                if (DealerHealth.instance.GetHealth() != 0)
                {
                    PokerUIController.instance.playAgainButton.SetActive(true);
                    PokerUIController.instance.leaveButton.SetActive(true);
                }
                else
                {
                    PokerUIController.instance.leaveButton.SetActive(true);
                }

                break;
        }
    }    
}
