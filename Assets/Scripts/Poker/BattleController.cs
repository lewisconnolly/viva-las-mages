using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public static BattleController instance;

    private void Awake()
    {
        instance = this;
    }

    public int startingCardsAmount = 7;    
    public int currentBet;

    public enum TurnOrder { playerBetting, playerActive, enemyActive}
    public TurnOrder currentPhase;

    
    // Start is called before the first frame update
    void Start()
    {
        PokerUIController.instance.placeBetButton.SetActive(true);
        PokerUIController.instance.betSlider.gameObject.SetActive(true);
        PokerUIController.instance.playHandButton.SetActive(false);
        PokerUIController.instance.swapCardButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void DrawHand()
    {
        int cardsToDraw = startingCardsAmount - HandController.instance.heldCards.Count;

        DeckController.instance.DrawMultipleCards(cardsToDraw);
    }

    public void PlayHand()
    {
        AdvanceTurn();
    }

    public void PlaceBet(int bet)
    {
        PlayerHealth.instance.PlaceBet(bet);
        AdvanceTurn();
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
                
                PokerUIController.instance.placeBetButton.SetActive(true);
                PokerUIController.instance.betSlider.gameObject.SetActive(true);
                PokerUIController.instance.playHandButton.SetActive(false);
                PokerUIController.instance.swapCardButton.SetActive(false);

                break;

            case TurnOrder.playerActive:

                PokerUIController.instance.placeBetButton.SetActive(false);
                PokerUIController.instance.betSlider.gameObject.SetActive(false);
                PokerUIController.instance.playHandButton.SetActive(true);
                PokerUIController.instance.swapCardButton.SetActive(true);

                DrawHand();

                break;
                
            case TurnOrder.enemyActive:

                Debug.Log("Skipping enemy actions");
                AdvanceTurn();
                break;
        }
    }    
}
