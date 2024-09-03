using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static PowerCardController;

public class SlotMachine : MonoBehaviour, IInteractable
{
    [SerializeField] public string prompt;

    public string InteractionPrompt => prompt;

    public CardScriptableObject rewardCard;
    public int numSpins = 0;
    public bool paidOut = false;
    public AK.Wwise.Event slotMachineWin;
    public AK.Wwise.Event betSound;

    public bool isTheOriginal;

    private Vector3 startingPos;

    public Material[] materials;
    private int currentMat;

    private bool cardGenerated;

    void Awake()
    {
        //GenerateRewardCard();
        cardGenerated = false;

        // For shake effect
        startingPos.x = transform.position.x;
        startingPos.y = transform.position.y;
        startingPos.z = transform.position.z;

        // For changing material on interact
        currentMat = 0;
    }

    public bool Interact(Interactor interactor)
    {
        // Generate reward card on first interaction
        if (!cardGenerated)
        {
            GenerateRewardCard();
            cardGenerated = true;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (prompt != "Out of Order" || prompt != "Paid Out!")
        {            
            if (PlayerHealth.instance.currentHealth > 1 && numSpins < 3 && !paidOut)
            {
                StartCoroutine(ShakeEffect());
                ChangeMaterial();

                numSpins++;

                PlayerHealth.instance.TakeDamage(1);
                ExitCost.instance.TakeDamage(1);

                int randomNum = UnityEngine.Random.Range(1, 4);
                if (randomNum == 1)
                {
                    RewardCardUI.instance.SlotMachineReward(rewardCard);
                    slotMachineWin.Post(gameObject);

                    prompt = "";
                    paidOut = true;

                    player.GetComponent<Interactor>().interactionPromptUI.Close();
                }
                else
                {                    

                    if (numSpins == 3)
                    {
                        prompt = "Out of Order. Unlucky!";
                        betSound.Post(gameObject);
                    }
                    else
                    {
                        if (numSpins == 2)
                        {
                            prompt = $"No Dice! {3 - numSpins} Spin Remaining (-1 Heart)";
                            betSound.Post(gameObject);
                        }
                        else
                        {
                            prompt = $"No Dice! {3 - numSpins} Spins Remaining (-1 Heart)";
                            betSound.Post(gameObject);
                        }
                    }

                    player.GetComponent<Interactor>().interactionPromptUI.prompText.text = prompt;
                }
            }
            else if (PlayerHealth.instance.currentHealth <= 1)
            {             
                prompt = "Not Enough Hearts";

                player.GetComponent<Interactor>().interactionPromptUI.prompText.text = prompt;
            }
            else if (numSpins >= 3)
            {
                prompt = "Out of Order";

                player.GetComponent<Interactor>().interactionPromptUI.prompText.text = prompt;
            }
            else
            {
                prompt = "Paid Out!";                

                player.GetComponent<Interactor>().interactionPromptUI.prompText.text = prompt;
            }
        }

        return true;
    }

    public bool ResetInteractable()
    {
        return true;
    }

    void GenerateRewardCard()
    {
        List<CardScriptableObject> normalCards = PlayerInventory.instance.playerDeck.Where(card => card.powerCardType == PowerCardType.None).ToList();

        if (normalCards.Count < 0)
        {
            normalCards = PlayerInventory.instance.playerDeck;
        }
        
        PowerCardType[] powerCardTypes = (PowerCardType[])Enum.GetValues(typeof(PowerCardType));
        List<PowerCardType> powerCardTypesFiltered = powerCardTypes.Where(powerCard => !powerCard.ToString().StartsWith("TwoInOne") && powerCard.ToString() != "None").ToList();

        CardScriptableObject baseRewardCard = normalCards[UnityEngine.Random.Range(0, normalCards.Count)];
        PowerCardType powerup = powerCardTypesFiltered[UnityEngine.Random.Range(0, powerCardTypesFiltered.Count)];
        
        rewardCard = Instantiate(baseRewardCard);
        string name = baseRewardCard.value.ToString() + baseRewardCard.suit + powerup.ToString();
        rewardCard.name = name;
        rewardCard.value = baseRewardCard.value;
        rewardCard.suit = baseRewardCard.suit;
        rewardCard.material = baseRewardCard.material;
        rewardCard.powerCardType = powerup;        
    }

    private IEnumerator ShakeEffect()
    {
        float shakeDuration = 1f;

        while (shakeDuration > 0)
        {
            shakeDuration -= 0.1f;

            float newX = startingPos.x + (Mathf.Sin(Time.frameCount * 1) * 0.01f);
            float newY = startingPos.y + (Mathf.Sin(Time.frameCount * 1) * 0.01f);
            float newZ = startingPos.z + (Mathf.Sin(Time.frameCount * 1) * 0.01f);

            transform.position = new Vector3(newX, newY, newZ);

            yield return new WaitForSeconds(0.1f);
        }

        transform.position = startingPos;

        yield break;
    }

    void ChangeMaterial()
    {
        currentMat++;

        if (currentMat >= materials.Length)
        {
            currentMat = 0;
        }

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer mr in meshRenderers)
        {
            mr.material = materials[currentMat];
        }
    }
}

