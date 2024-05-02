using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Card
{
    public string color;
    public int num;
    public Sprite sprite;
    public int percent;
    public bool CardState = false;
}

[CreateAssetMenu(fileName ="CardSO",menuName ="Scriptable Object/CardSO")]
public class CardSO : ScriptableObject
{
    public Card[] cards;

}


