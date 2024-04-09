using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PowerCardController;

public class SlotMachine : MonoBehaviour, IInteractable
{
    [SerializeField] public string prompt;

    public string InteractionPrompt => prompt;

    public CardScriptableObject rewardCard;
    public int numSpins = 0;
    public bool paidOut = false;

    public bool Interact(Interactor interactor)
    {        
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (prompt != "Out of Order" || prompt != "Paid Out!")
        {
            if (PlayerHealth.instance.currentHealth > 1 && numSpins < 3 && !paidOut)
            {
                numSpins++;

                GenerateRewardCard();

                PlayerHealth.instance.TakeDamage(1);

                int randomNum = UnityEngine.Random.Range(1, 4);
                if (randomNum == 1)
                {
                    RewardCardUI.instance.SlotMachineReward(rewardCard);

                    player.GetComponent<Interactor>().interactionPromptUI.Close();
                    
                    prompt = "";
                    paidOut = true;
                }
                else
                {
                    player.GetComponent<Interactor>().interactionPromptUI.Close();

                    if (numSpins == 3)
                    {
                        prompt = "Out of Order. Unlucky!";
                    }
                    else
                    {
                        if (numSpins == 2)
                        {
                            prompt = $"No Dice! {3 - numSpins} Spin Remaining (-1 Heart)";
                        }
                        else
                        {
                            prompt = $"No Dice! {3 - numSpins} Spins Remaining (-1 Heart)";
                        }
                    }
                }
            }
            else if (PlayerHealth.instance.currentHealth <= 1)
            {
                player.GetComponent<Interactor>().interactionPromptUI.Close();
                prompt = "Not Enough Hearts";
            }
            else if (numSpins >= 3)
            {
                player.GetComponent<Interactor>().interactionPromptUI.Close();                
                prompt = "Out of Order";
            }
            else
            {
                player.GetComponent<Interactor>().interactionPromptUI.Close();
                prompt = "Paid Out!";
            }
        }

        return true;
    }

    void GenerateRewardCard()
    {
        List<CardScriptableObject> normalCards = PlayerInventory.instance.playerDeck.Where(card => card.powerCardType == PowerCardType.None).ToList();

        if (normalCards.Count > 0)
        {
            PowerCardType[] powerCardTypes = (PowerCardType[])Enum.GetValues(typeof(PowerCardType));
            List<PowerCardType> powerCardTypesFiltered = powerCardTypes.Where(powerCard => !powerCard.ToString().StartsWith("Half") && powerCard.ToString() != "None").ToList();

            CardScriptableObject baseRewardCard = normalCards[UnityEngine.Random.Range(0, normalCards.Count)];
            PowerCardType powerup = powerCardTypesFiltered[UnityEngine.Random.Range(0, powerCardTypesFiltered.Count)];

            rewardCard = ScriptableObject.CreateInstance<CardScriptableObject>();
            string name = baseRewardCard.value.ToString() + baseRewardCard.suit + powerup.ToString();
            rewardCard.name = name;
            rewardCard.value = baseRewardCard.value;
            rewardCard.suit = baseRewardCard.suit;
            rewardCard.material = baseRewardCard.material;
            rewardCard.powerCardType = powerup;
        }
    }
}
