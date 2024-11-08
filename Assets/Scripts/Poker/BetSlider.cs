using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BetSlider : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI currentBet;  

    // Start is called before the first frame update
    void Start()
    {
        InitSlider();
    }

    public void InitSlider()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null && GameObject.FindGameObjectWithTag("Enemy") != null)
        {            
            slider.maxValue = Mathf.Min(PlayerHealth.instance.GetHealth(), BattleController.instance.activeEnemy.GetHealth());
        }
        else
        {
            slider.maxValue = Mathf.Min(int.Parse(PokerUIController.instance.healthValueText.text), int.Parse(PokerUIController.instance.enemyHealthValueText.text));
        }

        slider.minValue = 0;
    }

    // Update is called once per frame
    void Update()
    {        
        currentBet.text = slider.value.ToString();
    }

    
}
