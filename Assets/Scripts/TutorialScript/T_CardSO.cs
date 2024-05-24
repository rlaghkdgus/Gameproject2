using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class T_Card
{
    public int num;
    public Sprite sprite;
    public int percent;
    public bool CardState = false;
}
[CreateAssetMenu(fileName = "T_CardSO", menuName = "Scriptable Object/T_CardSO")]
public class T_CardSO : ScriptableObject
{
    public T_Card[] card;
}
