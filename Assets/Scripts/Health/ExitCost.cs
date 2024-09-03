using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public GameObject heartLockUIIcon;

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


            if (!SceneManager.GetActiveScene().name.Contains("Poker"))
            {
                Destroy(GameObject.Find("HeartLockIcon(Clone)"));
                heartLockUIIcon.GetComponentInChildren<TextMeshPro>().text = exitHealthCost.ToString();
                Instantiate(heartLockUIIcon, GameObject.Find("WorldSpaceCanvas1").GetComponentInChildren<CanvasRenderer>().gameObject.transform);
            }
        }
        
        if (exitHealthCost <= 0)
        {            
            Door doorInteractable = FindObjectOfType<Door>();
            doorInteractable.prompt = "Open Door";
        }
    }
}
