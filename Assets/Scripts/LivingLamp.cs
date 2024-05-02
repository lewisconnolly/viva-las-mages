using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class LivingLamp : MonoBehaviour, IInteractable
{
    [SerializeField] public string prompt;

    public AK.Wwise.Event lampSpeak;

    public string InteractionPrompt => prompt;

    private string initPrompt = "Hi friend, d'you want some help getting out of here? [E]";

    private string[] tutorialPrompts = {
        "I'm afraid the only way to escape is in the same way you ended up here...",
        "Yep, you have to continue gambling...",
        "However, this time chips won't cut it...",
        "I'm sorry buddy, but you'll need to risk your life if you want to unlock that door...",
        "Throughout this tower you'll encounter dealers like that mega muroid over there...",
        "Approach them and you can stake your health on a game of Short Deck Poker...",
        "I know pal, it's daunting, but it's the only way to progress...",
        "If you're able to wipe the dealer out, you might get something that'll tip the odds in your favour...",
        "Also, keep an eye out for other ways to chip away at those locks...",
        "Here's a deck to get you started, you can view it by pressing [I]...",
        "And take this guide too [H]...",
        "Play your cards right, friend, and with a bit of luck this'll be our first and last conversation"
    };

    private int currentPrompt;

    void Start()
    {
        currentPrompt = 0;
        //tutorialRead = false;

        if (PlayerInventory.instance != null && PlayerInventory.instance.fullTutorialRead)
        {
            prompt = "Good luck! Remember: [E] to interact, [I] to view your deck, and [H] to view the guide";
            initPrompt = prompt;
        }
    }
    void Update()
    {
        transform.position = transform.position + Vector3.up * Mathf.Sin(Time.frameCount/100) / 30f * Time.deltaTime;
    }

    public bool Interact(Interactor interactor)
    {
        PlayerInventory.instance.hasTalkedToLamp = true;

        if (currentPrompt >= tutorialPrompts.Length || PlayerInventory.instance.fullTutorialRead)
        {
            prompt = "Good luck! Remember: [E] to interact, [I] to view your deck, and [H] to view the guide";
            initPrompt = prompt;
            PlayerInventory.instance.fullTutorialRead = true;
            lampSpeak.Stop(gameObject);
            lampSpeak.Post(gameObject);
        }
        else
        {
            prompt = tutorialPrompts[currentPrompt];
            lampSpeak.Stop(gameObject);
            lampSpeak.Post(gameObject);
            currentPrompt++;
        }

        
        return true;
    }

    public bool ResetInteractable()
    {
        prompt = initPrompt;
        currentPrompt = 0;

        return true;
    }
}
