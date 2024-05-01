using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitCost : MonoBehaviour
{
    public static ExitCost instance;
    private void Awake()
    {
        instance = this;
    }

    public int exitHealthCost = 2;

    private void Update()
    {        
    }

    void Start()
    {               
        ExitController.instance.SetHealthText(exitHealthCost);
    }

    public int GetHealth() { return exitHealthCost; }

    public void SetHealth(int health) { exitHealthCost = health; }

    public void TakeDamage(int damage)
    {
        if (exitHealthCost > 0)
        {
            exitHealthCost -= damage;

            ExitController.instance.hit.Play();

            ExitController.instance.ShowFloatingText(damage);

            if (exitHealthCost <= 0)
            {
                exitHealthCost = 0;
            }

            ExitController.instance.SetHealthText(exitHealthCost);

        }
        
        if (exitHealthCost <= 0)
        {            
            Door doorInteractable = FindObjectOfType<Door>();
            doorInteractable.prompt = "Open Door";
        }
    }
}
