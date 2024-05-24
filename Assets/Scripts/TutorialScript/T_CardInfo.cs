using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class T_CardInfo : Singleton<T_CardInfo>
{
    public SpriteRenderer cardSprite;
    public TMP_Text text;

    public T_Card card;
    public bool isFront;
    public PRS originPRS;
    public int cardnum;
    public bool myCardState;
    public void Setup(T_Card card)
    {
        this.card = card;
        cardSprite.sprite = this.card.sprite;
        text.text = this.card.num.ToString();
        if (this.card.num == 20)
            text.text = "R";
        else if (this.card.num == 30)
            text.text = "B";
        cardnum = this.card.num;
        myCardState = this.card.CardState;
    }


    void OnMouseDown()//클릭시
    {
        if (T_GameManager.Instance.player[0].pState.Value == PlayerState.Select)
        {
            if (T_TurnSys.Instance.sPlayerIndex.Value == 0)//심판이 0번이라면
            {
                if (T_GameManager.Instance.S_State.Value == StrikeState.ReadyStrike)
                {
                    if (T_GameManager.Instance.player[0].strikeCards.Count >= 3)
                    {
                        Debug.Log("Don't Do that");
                        return;
                    }
                    if (myCardState == false)
                    {
                        T_GameManager.Instance.player[0].strikeCards.Add(this.cardnum);

                        myCardState = true;
                    }

                }

                else
                {
                    if (this.transform.parent == T_GameManager.Instance.player[0].playerObject.transform)//부모의 위치로 내카드인지 아닌지 판정
                    {
                        T_GameManager.Instance.CardBuffering.SetActive(T_GameManager.Instance.player[0].pState.Value == PlayerState.Select);
                        this.myCardState = true;
                        T_GameManager.Instance.player[0].playerCards.Remove(this);//내카드리스트에 삭제
                        this.text.text = "";
                        this.cardSprite.sprite = null;
                        T_GameManager.Instance.player[0].pState.Value = PlayerState.Idle;
                        StartCoroutine(CheckDelay(gameObject));
                        T_ShootingManager.Instance.DestroyTurnEndBird();
                    }
                }

            }
        }
        else if (T_GameManager.Instance.player[1].pState.Value == PlayerState.Select)
        {
            if (T_TurnSys.Instance.sPlayerIndex.Value == 1)//심판이 1번이라면
            {
                if (T_GameManager.Instance.S_State.Value == StrikeState.ReadyStrike)
                {
                    if (T_GameManager.Instance.player[1].strikeCards.Count >= 3)
                    {
                        Debug.Log("Don't Do that");
                        return;
                    }
                    if (myCardState == false)
                    {
                        T_GameManager.Instance.player[1].strikeCards.Add(this.cardnum);
                        myCardState = true;
                    }

                }

                else
                {
                    if (this.transform.parent == T_GameManager.Instance.player[1].playerObject.transform)//부모의 위치로 내카드인지 아닌지 판정
                    {
                        T_GameManager.Instance.CardBuffering.SetActive(T_GameManager.Instance.player[1].pState.Value == PlayerState.Select);
                        this.myCardState = true;
                        T_GameManager.Instance.player[1].playerCards.Remove(this);//내카드리스트에 삭제
                        this.text.text = "";
                        this.cardSprite.sprite = null;
                        T_GameManager.Instance.player[1].pState.Value = PlayerState.Idle;
                        StartCoroutine(CheckDelay(gameObject));
                        T_ShootingManager.Instance.DestroyTurnEndBird();
                    }
                }
            }
        }
        else if (T_GameManager.Instance.G_State.Value == GuardState.GuardSelect && T_TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            if (transform.parent == T_GameManager.Instance.player[0].playerObject.transform)
            {
                if (T_GameManager.Instance.player[0].Guardnums.Count >= 4)
                {
                    Debug.Log("Don't Do that");
                    return;
                }
                if (myCardState == false)
                {
                    T_GameManager.Instance.player[0].Guardnums.Add(cardnum);
                    myCardState = true;
                }
            }
        }
    }
    IEnumerator CheckDelay(GameObject obj)
    {
        yield return new WaitForSeconds(0.2f); // 0.2초 지연
        if (T_TurnSys.Instance.sPlayerIndex.Value == 0)
            T_GameManager.Instance.player[0].pState.Value = PlayerState.SelectFin;
        else if (T_TurnSys.Instance.sPlayerIndex.Value == 1)
            T_GameManager.Instance.player[1].pState.Value = PlayerState.SelectFin;
        yield return new WaitForSeconds(0.05f);
        Destroy(obj);
        // 오브젝트 파괴
    }
}
