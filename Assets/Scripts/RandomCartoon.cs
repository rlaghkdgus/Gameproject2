using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RandomCartoon : MonoBehaviour
{
   public List<Sprite> LoadingCartoon = new List<Sprite>();
    public Image CartoonImg;

    private void Awake()
    {
        int rand = Random.Range(0, LoadingCartoon.Count);
        RandomCartoonImg(rand);
    }
    void RandomCartoonImg(int randnum)
    {
        CartoonImg.sprite = LoadingCartoon[randnum];
    }
}
