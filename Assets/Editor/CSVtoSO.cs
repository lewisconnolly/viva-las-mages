using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVtoSO
{
    private static string cardsCSVPath = "/Editor/CSVs/cards.csv";
    private static string cardMaterialsPath = "CardMaterials/";
    
    [MenuItem("Utilities/Generate Cards")]
    public static void GenerateCards()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + cardsCSVPath);

        foreach (string line in allLines)
        {
            string[] splitData = line.Split(',');

            CardScriptableObject card = ScriptableObject.CreateInstance<CardScriptableObject>();
            card.value = int.Parse(splitData[0]);
            card.suit = splitData[1];
            card.material = Resources.Load<Material>(cardMaterialsPath + splitData[2]);
            card.powerCardType = PowerCardController.PowerCardType.None;

            AssetDatabase.CreateAsset(card, "Assets/Resources/Cards/PlayerDeck/" + card.value + card.suit + ".asset");
        }

        AssetDatabase.SaveAssets();
    }

}
