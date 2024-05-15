using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Photon.Pun;
using ExitGames.Client.Photon;


[System.Serializable]
public class Card
{
    public int num;
    public int percent;
    public bool CardState = false;
    public static byte[] Serialize(object customobject)
    {
        Card cd = (Card)customobject;

        MemoryStream ms = new MemoryStream(sizeof(int) + sizeof(bool) + sizeof(int));

        ms.Write(BitConverter.GetBytes(cd.num), 0, sizeof(int));
        ms.Write(BitConverter.GetBytes(cd.CardState), 0, sizeof(bool));
        ms.Write(BitConverter.GetBytes(cd.percent), 0, sizeof(int));

        return ms.ToArray();
    }

    public static object Deserialize(byte[] bytes)
    {
        Card cd = new Card();

        cd.num = BitConverter.ToInt32(bytes, 0);
        cd.percent = BitConverter.ToInt32(bytes, 4);
        cd.CardState = BitConverter.ToBoolean(bytes, 8);
        return cd;
    }
}

[CreateAssetMenu(fileName ="CardSO",menuName ="Scriptable Object/CardSO")]
public class CardSO : ScriptableObject
{
    public Card[] cards;
   

}


