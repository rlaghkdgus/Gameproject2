using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharaChange : MonoBehaviour
{
    public List<Sprite> CharaSprite = new List<Sprite>();
    public Image MainChara;
    public void ChangeEmber()
    {
        MainChara.sprite = CharaSprite[0];
    }
    public void ChangeEmil()
    {
        MainChara.sprite = CharaSprite[1];
    }
}
