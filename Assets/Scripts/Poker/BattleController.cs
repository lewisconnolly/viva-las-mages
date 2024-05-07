using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
//using static HandEvaluator;

public class BattleController : MonoBehaviour
{
    public static BattleController instance;
    public DealerHealth activeEnemy;

    public AK.Wwise.Event enemyWin;
    public AK.Wwise.Event enemyLose;

    private void Awake()
    {
        instance = this;
        List<DealerHealth> dealers = FindObjectsOfType<DealerHealth>().Where(dealer => dealer.activeEnemy == true).ToList<DealerHealth>();
        activeEnemy = dealers.First();
    }

    public int startingCardsAmount = 7;    
    public int currentBet;

    public enum TurnOrder { playerBetting, playerActive, enemyActive, resolveHands}
    public TurnOrder currentPhase;

    public Transform playerDiscardPosition;
    public Transform enemyDiscardPosition;
    public bool checkCardsDiscarded;

    public int numAutoPairs;

    private int heartsToGain;
    private int ranksToGain;

    private int enemyHeartsToGain;
    private int enemyRanksToGain;

    // Start is called before the first frame update
    void Start()
    {
        PokerUIController.instance.placeBetButton.SetActive(true);        
        PokerUIController.instance.betSlider.gameObject.SetActive(true);
        if (SceneManager.GetActiveScene().name != "FinalBossPokerRoom") PokerUIController.instance.leaveButton.SetActive(true);
        PokerUIController.instance.playHandButton.SetActive(false);
        PokerUIController.instance.swapCardButton.SetActive(false);
        PokerUIController.instance.playAgainButton.SetActive(false);

        checkCardsDiscarded = false;

        numAutoPairs = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Get number of auto pairs in selected cards
        numAutoPairs = HandController.instance.selectedCards.Where(card => card.powerCardType == PowerCardController.PowerCardType.Duplicard).ToList().Count;

        if (HandController.instance.selectedCards.Count == (HandController.instance.numCardsRequiredToPlay - numAutoPairs))
        {
            PokerUIController.instance.playHandButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            PokerUIController.instance.playHandButton.GetComponent<Button>().interactable = false;
        }

        if (PokerUIController.instance.betSlider.GetComponent<Slider>().value > 0)
        {
            PokerUIController.instance.placeBetButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            PokerUIController.instance.placeBetButton.GetComponent<Button>().interactable = false;
        }

        // Get number of hand swaps and free swaps in selected cards
        int numHandSwaps = HandController.instance.selectedCards.Where(card => card.powerCardType == PowerCardController.PowerCardType.HandSwap).ToList().Count;
        int numFreeSwaps = HandController.instance.selectedCards.Where(card => card.powerCardType == PowerCardController.PowerCardType.FreeSwap).ToList().Count;
        int swapCost = (HandController.instance.selectedCards.Count - numFreeSwaps);

        // Allow player to swap cards if they have more health minus current bet than number of cards being swapped - free swaps
        if ((HandController.instance.selectedCards.Count > 0 && (PlayerHealth.instance.currentHealth - currentBet) >= swapCost) || numHandSwaps > 0)
        {
            string buttonText = "";

            // Change button text to display cost of swaps            
            if (numHandSwaps > 0)
            {
                buttonText = "Swap Hand";
            }
            else
            {
                buttonText = "Swap Selected\n(-" + swapCost.ToString() + " Heart";
                if (swapCost > 1) { buttonText += "s)"; } else { buttonText += ")"; }                
            }

            PokerUIController.instance.swapCardButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
            PokerUIController.instance.swapCardButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            PokerUIController.instance.swapCardButton.GetComponentInChildren<TextMeshProUGUI>().text = "Swap";
            PokerUIController.instance.swapCardButton.GetComponent<Button>().interactable = false;
        }

        if (checkCardsDiscarded)
        {            
            bool allCardsDiscarded = true;

            for (int i = 0; i < HandController.instance.playedCards.Count; i++)
            {
                double playerCardZPos = Mathf.Round(HandController.instance.playedCards[i].transform.position.z);
                double enemyCardZPos = Mathf.Round(EnemyController.instance.playedCards[i].transform.position.z);
                
                if (playerCardZPos != Mathf.Round(playerDiscardPosition.position.z) ||
                    enemyCardZPos != Mathf.Round(enemyDiscardPosition.position.z))
                {
                    allCardsDiscarded = false;
                }
            }

            if (allCardsDiscarded)
            {
                ResetPlayedCards();
                checkCardsDiscarded = false;
            }
        }
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
        DeckController.instance.DrawMultipleCards(startingCardsAmount - HandController.instance.heldCards.Count);
        EnemyController.instance.DrawMultipleCards(startingCardsAmount - EnemyController.instance.heldCards.Count);
        AdvanceTurn();
    }

    public void PlayAgain()
    {
        currentBet = 0;
        
        PokerUIController.instance.playAgainButton.GetComponent<Button>().interactable = false;

        DiscardPlayedCards();        
    }

    public void DiscardPlayedCards()
    {
        for (int i = 0; i < HandController.instance.playedCards.Count; i++)
        {
            if (HandController.instance.playedCards[i] != null)
            {
                HandController.instance.playedCards[i].moveSpeed = 1.75f;
                HandController.instance.playedCards[i].MakeTransparent();
                HandController.instance.playedCards[i].MoveToPoint(playerDiscardPosition.position, playerDiscardPosition.rotation);
            }
        }

        for (int i = 0; i < EnemyController.instance.playedCards.Count; i++)
        {
            if (EnemyController.instance.playedCards[i] != null)
            {
                EnemyController.instance.playedCards[i].moveSpeed = 1.75f;
                EnemyController.instance.playedCards[i].MakeTransparent();
                EnemyController.instance.playedCards[i].MoveToPoint(enemyDiscardPosition.position, enemyDiscardPosition.rotation);
            }
        }

        HandController.instance.playedCards.Clear();
        EnemyController.instance.playedCards.Clear();
        AdvanceTurn();

        //checkCardsDiscarded = true;
    }

    public void DiscardHeldCards()
    {
        for (int i = 0; i < EnemyController.instance.heldCards.Count; i++)
        {
            if (EnemyController.instance.heldCards[i] != null)
            {
                EnemyController.instance.heldCards[i].moveSpeed = 1.75f;
                EnemyController.instance.heldCards[i].MakeTransparent();
                EnemyController.instance.heldCards[i].MoveToPoint(enemyDiscardPosition.position, enemyDiscardPosition.rotation);
            }
        }
    }

    public void ResetPlayedCards()
    {
        for (int i = 0; i < HandController.instance.playedCards.Count; i++)
        {
            Destroy(HandController.instance.playedCards[i].gameObject);
            Destroy(EnemyController.instance.playedCards[i].gameObject);
        }

        HandController.instance.playedCards.Clear();
        EnemyController.instance.playedCards.Clear();
        AdvanceTurn();
    }

    public void ResolveHands(HandEvaluator.HandRank playerHandRank, HandEvaluator.HandRank enemyHandRank)
    {
        PokerUIController.instance.SetHandText(playerHandRank.ToString(), enemyHandRank.ToString());

        if (playerHandRank > enemyHandRank) // Player won hand
        {
            PokerUIController.instance.SetWinnerText("You Win!");
            PlayerHealth.instance.IncreaseHealth(currentBet + heartsToGain);           
            //activeEnemy.TakeDamage(currentBet);
            ChangeEnemyHealthOnLoss();
            enemyLose.Post(gameObject);
        }
        else if (playerHandRank < enemyHandRank) // Enemy won hand
        {
            PokerUIController.instance.SetWinnerText("Enemy Wins");
            enemyWin.Post(gameObject);
            activeEnemy.IncreaseHealth(enemyHeartsToGain);

            ChangePlayerHealthOnLoss();
        }
        else // Possible tie
        {
            if (ranksToGain > 0 && enemyRanksToGain == 0) // Enemy win because player used rank upgrade
            {
                PokerUIController.instance.SetWinnerText("Enemy Wins");
                enemyWin.Post(gameObject);
                activeEnemy.IncreaseHealth(enemyHeartsToGain);

                ChangePlayerHealthOnLoss();                

            }
            else if (ranksToGain == 0 && enemyRanksToGain > 0) // Player win because enemy used rank upgrade
            {
                PokerUIController.instance.SetWinnerText("You Win!");
                PlayerHealth.instance.IncreaseHealth(currentBet + heartsToGain);
                //activeEnemy.TakeDamage(currentBet);
                ChangeEnemyHealthOnLoss();

                enemyLose.Post(gameObject);
            }
            else if (ranksToGain > 0 && enemyRanksToGain > 0) // Both used rank upgrade
            {
                if (ranksToGain < enemyRanksToGain) // Player win because used fewer rank upgrades
                {
                    PokerUIController.instance.SetWinnerText("You Win!");
                    PlayerHealth.instance.IncreaseHealth(currentBet + heartsToGain);
                    //activeEnemy.TakeDamage(currentBet);
                    ChangeEnemyHealthOnLoss();

                    enemyLose.Post(gameObject);
                }
                else if (ranksToGain > enemyRanksToGain) // Enemy win because used fewer rank upgrades
                {
                    PokerUIController.instance.SetWinnerText("Enemy Wins");
                    enemyWin.Post(gameObject);
                    activeEnemy.IncreaseHealth(enemyHeartsToGain);

                    ChangePlayerHealthOnLoss();
                }
                else // Tie because used same amount
                {
                    PokerUIController.instance.SetWinnerText("Tie");

                    PlayerHealth.instance.IncreaseHealth(heartsToGain);
                    activeEnemy.IncreaseHealth(enemyHeartsToGain);
                }
            }
            else // No rank upgrades
            {
                HandEvaluator.TieWinner tieWinner = HandEvaluator.instance.BreakTie(HandController.instance.playedCards.OrderBy(c => c.value).ToList(),
                                    EnemyController.instance.playedCards.OrderBy(c => c.value).ToList(),
                                    playerHandRank);

                if (tieWinner == HandEvaluator.TieWinner.Player) // Player won hand
                {
                    PokerUIController.instance.SetWinnerText("You Win!");
                    PlayerHealth.instance.IncreaseHealth(currentBet + heartsToGain);
                    //activeEnemy.TakeDamage(currentBet);
                    ChangeEnemyHealthOnLoss();

                    enemyLose.Post(gameObject);
                }
                else if (tieWinner == HandEvaluator.TieWinner.Enemy) // Enemy won hand
                {
                    PokerUIController.instance.SetWinnerText("Enemy Wins");
                    enemyWin.Post(gameObject);
                    activeEnemy.IncreaseHealth(enemyHeartsToGain);

                    ChangePlayerHealthOnLoss();
                }
                else // Tie
                {
                    PokerUIController.instance.SetWinnerText("Tie");

                    PlayerHealth.instance.IncreaseHealth(heartsToGain);
                    activeEnemy.IncreaseHealth(enemyHeartsToGain);
                }
            }
        }
    }
    
    public void ChangePlayerHealthOnLoss()
    {
        if (currentBet - heartsToGain > 0)
        {
            PlayerHealth.instance.TakeDamage(currentBet - heartsToGain);
        }
        else
        {
            PlayerHealth.instance.IncreaseHealth((currentBet - heartsToGain) * -1);
        }
    }

    public void ChangeEnemyHealthOnLoss()
    {
        if (currentBet - enemyHeartsToGain > 0)
        {
            activeEnemy.TakeDamage(currentBet - enemyHeartsToGain);
        }
        else
        {
            activeEnemy.IncreaseHealth((currentBet - enemyHeartsToGain) * -1);
        }
    }

    public void AdvanceTurn()
    {
        currentPhase++;

        if((int)currentPhase >= System.Enum.GetValues(typeof(TurnOrder)).Length) { currentPhase = 0; }

        switch (currentPhase)
        {
            case TurnOrder.playerBetting:

                RewardCardUI.instance.rewardCardParentObject.SetActive(false);
                PokerUIController.instance.betSlider.GetComponent<BetSlider>().InitSlider();
                PokerUIController.instance.playerHandText.gameObject.SetActive(false);
                PokerUIController.instance.enemyHandText.gameObject.SetActive(false);
                PokerUIController.instance.winnerText.gameObject.SetActive(false);
                PokerUIController.instance.playAgainButton.SetActive(false);
                if (SceneManager.GetActiveScene().name != "FinalBossPokerRoom") PokerUIController.instance.leaveButton.SetActive(true);
                PokerUIController.instance.placeBetButton.SetActive(true);
                PokerUIController.instance.betSlider.gameObject.SetActive(true);                
                PokerUIController.instance.playHandButton.SetActive(false);
                PokerUIController.instance.swapCardButton.SetActive(false);

                break;

            case TurnOrder.playerActive:

                PokerUIController.instance.leaveButton.SetActive(false);
                PokerUIController.instance.placeBetButton.SetActive(false);
                PokerUIController.instance.betSlider.gameObject.SetActive(false);                
                PokerUIController.instance.playHandButton.SetActive(true);
                PokerUIController.instance.playHandButton.GetComponent<Button>().interactable = false;
                PokerUIController.instance.swapCardButton.SetActive(true);                                

                break;

            case TurnOrder.enemyActive:

                PokerUIController.instance.playHandButton.SetActive(false);
                EnemyController.instance.PlayHand();
                AdvanceTurn();
                
                break;

            case TurnOrder.resolveHands:

                heartsToGain = 0;
                ranksToGain = 0;
                enemyHeartsToGain = 0;
                enemyRanksToGain = 0;

                HandEvaluator.HandRank playerHandRank = HandEvaluator.instance.EvaluateHand(HandController.instance.playedCards, true);

                // Increase rank by number of UpgradeRank power cards
                ranksToGain = PowerCardController.instance.numRanksToUpgrade;

                if (ranksToGain > 0)
                {
                    if ((int)playerHandRank + ranksToGain > 9)
                    {
                        playerHandRank = (HandEvaluator.HandRank)9;
                    }
                    else
                    {
                        playerHandRank += ranksToGain;
                    }
                }

                //Gain heart for each GainHeart power card
                if (PowerCardController.instance.numHeartsToGain > 0)
                {
                    heartsToGain = PowerCardController.instance.numHeartsToGain;
                }

                HandEvaluator.HandRank enemyHandRank = HandEvaluator.instance.EvaluateHand(EnemyController.instance.playedCards, true);

                // Increase rank by number of UpgradeRank power cards
                enemyRanksToGain = PowerCardController.instance.numRanksToUpgrade;

                if (enemyRanksToGain > 0)
                {
                    if ((int)enemyHandRank + enemyRanksToGain > 9)
                    {
                        enemyHandRank = (HandEvaluator.HandRank)9;
                    }
                    else
                    {
                        enemyHandRank += enemyRanksToGain;
                    }
                }

                //Gain heart for each GainHeart power card
                if (PowerCardController.instance.numHeartsToGain > 0)
                {
                    enemyHeartsToGain = PowerCardController.instance.numHeartsToGain;
                }

                ResolveHands(playerHandRank, enemyHandRank);

                // Destroy player duplicates
                PowerCardController.instance.DestroyDuplicates();

                PokerUIController.instance.playerHandText.gameObject.SetActive(true);
                PokerUIController.instance.enemyHandText.gameObject.SetActive(true);                
                PokerUIController.instance.HideBetIcons();

                if (PlayerHealth.instance.isGameOver)
                {
                    string winnerText = PokerUIController.instance.winnerText.text;
                    PokerUIController.instance.SetWinnerText(winnerText + "\nYou've Gone Bust!");
                    PokerUIController.instance.winnerText.gameObject.SetActive(true);
                    PokerUIController.instance.swapCardButton.SetActive(false);
                    PokerUIController.instance.endGameButton.SetActive(true);                    
                    PokerUIController.instance.playerBet.SetActive(false);
                }
                else if (activeEnemy.GetHealth() != 0)
                {
                    PokerUIController.instance.winnerText.gameObject.SetActive(true);
                    PokerUIController.instance.playAgainButton.SetActive(true);
                    PokerUIController.instance.playAgainButton.GetComponent<Button>().interactable = true;
                    PokerUIController.instance.swapCardButton.SetActive(false);
                    if (SceneManager.GetActiveScene().name != "FinalBossPokerRoom") PokerUIController.instance.leaveButton.SetActive(true);
                }                                
                else // Enemy lost all health
                {
                    DiscardHeldCards();

                    if (SceneManager.GetActiveScene().name != "FinalBossPokerRoom")
                    {
                        VFXController.instance.sparkles.Play();
                        RewardCardUI.instance.rewardCardParentObject.SetActive(true);
                        PlayerInventory.instance.AddRewardCardtoDeck(RewardCardUI.instance.rewardCard.cardSO);
                        if (SceneManager.GetActiveScene().name != "FinalBossPokerRoom") PokerUIController.instance.leaveButton.SetActive(true);
                    }
                    else
                    {
                        PokerUIController.instance.endGameButton.SetActive(true);
                        activeEnemy.smokePuff.Play();
                        activeEnemy.model.SetActive(false);
                    }
                    
                    PokerUIController.instance.swapCardButton.SetActive(false);                    
                    PokerUIController.instance.enemyIconOverlayDefeated.SetActive(true);
                    PokerUIController.instance.enemyBet.SetActive(false);
                }

                break;
        }
    }
}
