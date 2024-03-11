using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVtoMaterials
{
    private static string materialsCSVPath = "/Editor/CSVs/card_materials.csv";
    private static string texturesPath = "CardTextures/";
    
    [MenuItem("Utilities/Generate Card Materials")]
    public static void GenerateCardMaterials()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + materialsCSVPath);

        foreach (string line in allLines)
        {
            string[] splitData = line.Split(',');

            string materialName = splitData[0] + splitData[1] + "Material";
            string cardFolder = splitData[2] + "/" + splitData[3] + "/";

            Material cardMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));            

            Texture2D albedoTex = Resources.Load<Texture2D>(texturesPath + cardFolder + splitData[4]);
            Texture2D aoTex = Resources.Load<Texture2D>(texturesPath + cardFolder + splitData[5]);
            Texture2D heightTex = Resources.Load<Texture2D>(texturesPath + cardFolder + splitData[6]);
            Texture2D metallicTex = Resources.Load<Texture2D>(texturesPath + cardFolder + splitData[7]);
            Texture2D normalTex = Resources.Load<Texture2D>(texturesPath + cardFolder + splitData[8]);            

            cardMaterial.SetTexture("_BaseMap", albedoTex);
            cardMaterial.SetTexture("_OcclusionMap", aoTex);
            cardMaterial.SetTexture("_ParallaxMap", heightTex);
            cardMaterial.SetTexture("_MetallicGlossMap", metallicTex);
            cardMaterial.SetTexture("_BumpMap", normalTex);

            AssetDatabase.CreateAsset(cardMaterial, "Assets/Materials/CardMaterials/" + materialName + ".mat");
        }

        AssetDatabase.SaveAssets();
    }

}
