using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName ="Card", order = 1)]
public class CardScriptableObject : ScriptableObject
{
    public int value;
    public string suit;
    public Mesh mesh;
}
