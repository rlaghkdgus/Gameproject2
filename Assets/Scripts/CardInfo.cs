using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class CardInfo : MonoBehaviour
{
    public SpriteRenderer cardSprite;
    public TMP_Text text;

    public Card card;
    public bool isFront;
    public PRS originPRS;
    public int cardnum;
    public bool myCardState;
    public void Setup(Card card)
    {
        this.card = card;

        cardSprite.sprite = this.card.sprite;
        text.text = this.card.num.ToString();
        cardnum = this.card.num;
        myCardState = this.card.CardState;
    }

 
 void OnMouseDown()//Ŭ����
    {
        if (GameManager.Instance.player[0].pState.Value == PlayerState.Select)
        {
            if (TurnSys.Instance.sPlayerIndex.Value == 0)//������ 0���̶��
            {
                if (GameManager.Instance.S_State.Value == StrikeState.ReadyStrike )
                {
                    if (GameManager.Instance.player[0].strikeCards.Count >= 3)
                    {
                        Debug.Log("Don't Do that");
                        return;
                    }
                    if (myCardState == false)
                    {
                        GameManager.Instance.player[0].strikeCards.Add(this);
                        myCardState = true;
                    }
                    
                }
               
                else
                {
                    if (this.transform.parent == GameManager.Instance.player[0].playerObject.transform)//�θ��� ��ġ�� ��ī������ �ƴ��� ����
                    {
                        GameManager.Instance.CardBuffering.SetActive(GameManager.Instance.player[0].pState.Value == PlayerState.Select);
                        this.myCardState = true;
                        GameManager.Instance.player[0].playerCards.Remove(this);//��ī�帮��Ʈ�� ����
                        this.text.text = "";
                        this.cardSprite.sprite = null;
                        GameManager.Instance.player[0].pState.Value = PlayerState.Idle;
                        StartCoroutine(CheckDelay(gameObject));
                    }
                }
                
            }
        }
        else if (GameManager.Instance.player[1].pState.Value == PlayerState.Select)
        {
            if (TurnSys.Instance.sPlayerIndex.Value == 1)//������ 1���̶��
            {
                if (GameManager.Instance.S_State.Value == StrikeState.ReadyStrike)
                {
                    if (GameManager.Instance.player[1].strikeCards.Count >= 3)
                    {
                        Debug.Log("Don't Do that");
                        return;
                    }
                    if (myCardState == false)
                    {
                        GameManager.Instance.player[1].strikeCards.Add(this);
                        myCardState = true;
                    }

                }
             
                else
                {
                    if (this.transform.parent == GameManager.Instance.player[1].playerObject.transform)//�θ��� ��ġ�� ��ī������ �ƴ��� ����
                    {
                        GameManager.Instance.CardBuffering.SetActive(GameManager.Instance.player[1].pState.Value == PlayerState.Select);
                        this.myCardState = true;
                        GameManager.Instance.player[1].playerCards.Remove(this);//��ī�帮��Ʈ�� ����
                        this.text.text = "";
                        this.cardSprite.sprite = null;
                        GameManager.Instance.player[1].pState.Value = PlayerState.Idle;
                        StartCoroutine(CheckDelay(gameObject));
                    }
                }
            }
        }
        else if (GameManager.Instance.G_State.Value == GuardState.GuardSelect && TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            if (transform.parent == GameManager.Instance.player[0].playerObject.transform)
            {
                GameManager.Instance.player[0].strikeCards.Add(this);
                myCardState = true;
            }
        }
    }
    IEnumerator CheckDelay(GameObject obj)
    {
        yield return new WaitForSeconds(0.2f); // 0.2�� ����
        if (TurnSys.Instance.sPlayerIndex.Value == 0)
            GameManager.Instance.player[0].pState.Value = PlayerState.SelectFin;
        else if (TurnSys.Instance.sPlayerIndex.Value == 1)
            GameManager.Instance.player[1].pState.Value = PlayerState.SelectFin;
        yield return new WaitForSeconds(0.05f);
        Destroy(obj);
        // ������Ʈ �ı�
    }



}
