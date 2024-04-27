using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.VFX;

public class ExitController : MonoBehaviour
{
    public static ExitController instance;
    public GameObject floatingTextPrefab;
    public GameObject padlockModel;

    public VisualEffect hit;
    public VisualEffect smokePuff;

    private void Awake()
    {
        instance = this;
    }

    public TextMeshPro healthValueText;

    // Start is called before the first frame update
    void Start()
    {
        if (ExitCost.instance != null)
        {
            if (ExitCost.instance.GetHealth() <= 0)
            {
                padlockModel.SetActive(false);
            }
            else
            {
                SetHealthText(ExitCost.instance.GetHealth());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetHealthText(int health)
    {
        healthValueText.text = health.ToString();

        if (health <= 0)
        {
            smokePuff.Play();
            //VFXController.instance.smokePuff.Play();
            padlockModel.SetActive(false);
        }
    }

    public void ShowFloatingText(int damage)
    {
        GameObject floatingText;

        if (floatingTextPrefab != null)
        {
            floatingText = Instantiate(floatingTextPrefab);

            floatingText.GetComponent<TextMeshPro>().text = "-" + damage.ToString();
        }
        
    }
}
