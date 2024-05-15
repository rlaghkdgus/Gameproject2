using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System;
using System.IO;



public class CardInfo : MonoBehaviourPunCallbacks
{

    public PhotonView PV;
    public Card card;
    public int cardnum;
    public bool myCardState;
    public SpriteRenderer sprite;
    public TMP_Text text;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void Setup(Card card)
    {
        this.card = card;
        cardnum = this.card.num;
        myCardState = this.card.CardState;
        text.text = this.card.num.ToString();
        if (this.card.num == 20)
            text.text = "R";
        else if (this.card.num == 30)
            text.text = "B";
    }
    void OnMouseDown()//클릭시
    {
        PV.RPC("RPCClick", RpcTarget.MasterClient);
    }
    [PunRPC]
    public void RPCClick()
    {
        Debug.Log("Click");
        if (GameManager.Instance.player[0].pState.Value == PlayerState.Select)
        {
            if (TurnSys.Instance.sPlayerIndex.Value == 0)//심판이 0번이라면
            {
                if (GameManager.Instance.S_State.Value == StrikeState.ReadyStrike)
                {
                    if (GameManager.Instance.player[0].strikeCards.Count >= 3)
                    {
                        Debug.Log("Don't Do that");
                        return;
                    }
                    if (myCardState == false)
                    {
                        GameManager.Instance.player[0].strikeCards.Add(this.cardnum);
                        myCardState = true;
                        PV.RPC("RPCAddStrike", RpcTarget.Others, 0);
                    }

                }

                else
                {
                    if (this.transform.parent == GameManager.Instance.player[0].playerObject.transform)//부모의 위치로 내카드인지 아닌지 판정
                    {
                        GameManager.Instance.CardBuffering.SetActive(GameManager.Instance.player[0].pState.Value == PlayerState.Select);
                        this.myCardState = true;
                        GameManager.Instance.player[0].playerCards.Remove(this);//내카드리스트에 삭제
                        GameManager.Instance.player[0].pState.Value = PlayerState.Idle;
                        StartCoroutine(CheckDelay(gameObject));
                        ShootingManager.Instance.DestroyTurnEndBird();
                        PV.RPC("RPCDeleteCard", RpcTarget.Others,0);
                    }
                }

            }
        }
        else if (GameManager.Instance.player[1].pState.Value == PlayerState.Select)
        {
            if (TurnSys.Instance.sPlayerIndex.Value == 1)//심판이 1번이라면
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
                        GameManager.Instance.player[1].strikeCards.Add(this.cardnum);
                        myCardState = true;
                        PV.RPC("RPCAddStrike", RpcTarget.Others, 1);
                    }

                }

                else
                {
                    if (this.transform.parent == GameManager.Instance.player[1].playerObject.transform)//부모의 위치로 내카드인지 아닌지 판정
                    {
                        GameManager.Instance.CardBuffering.SetActive(GameManager.Instance.player[1].pState.Value == PlayerState.Select);
                        this.myCardState = true;
                        GameManager.Instance.player[1].playerCards.Remove(this);//내카드리스트에 삭제
                        GameManager.Instance.player[1].pState.Value = PlayerState.Idle;
                        StartCoroutine(CheckDelay(gameObject));
                        ShootingManager.Instance.DestroyTurnEndBird();
                        PV.RPC("RPCDeleteCard", RpcTarget.Others, 1);
                    }
                }
            }
        }
        else if (GameManager.Instance.G_State.Value == GuardState.GuardSelect && TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            if (transform.parent == GameManager.Instance.player[0].playerObject.transform)
            {
                if (GameManager.Instance.player[0].Guardnums.Count >= 4)
                {
                    Debug.Log("Don't Do that");
                    return;
                }
                if (myCardState == false)
                {
                    GameManager.Instance.player[0].Guardnums.Add(cardnum);
                    myCardState = true;
                    PV.RPC("RPCAddGuard", RpcTarget.Others, 0);
                }
            }
        }
        else if (GameManager.Instance.G_State.Value == GuardState.GuardSelect && TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            if (transform.parent == GameManager.Instance.player[1].playerObject.transform)
            {
                if (GameManager.Instance.player[1].Guardnums.Count >= 4)
                {
                    Debug.Log("Don't Do that");
                    return;
                }
                if (myCardState == false)
                {
                    GameManager.Instance.player[1].Guardnums.Add(cardnum);
                    myCardState = true;
                    PV.RPC("RPCAddGuard", RpcTarget.Others, 1);
                }
            }
        }
    }

    [PunRPC]
    public void RPCAddStrike(int playerIndex)
    {
        GameManager.Instance.player[playerIndex].strikeCards.Add(cardnum);
        myCardState = true;
    }
    [PunRPC]
    public void RPCAddGuard(int playerIndex)
    {
        GameManager.Instance.player[playerIndex].Guardnums.Add(cardnum);
        myCardState = true;
    }
    [PunRPC]
    public void RPCDeleteCard(int playerIndex)
    {
        GameManager.Instance.CardBuffering.SetActive(GameManager.Instance.player[playerIndex].pState.Value == PlayerState.Select);
        this.myCardState = true;
        GameManager.Instance.player[playerIndex].playerCards.Remove(this);//내카드리스트에 삭제
        GameManager.Instance.player[playerIndex].pState.Value = PlayerState.Idle;
        StartCoroutine(CheckDelay(gameObject));
        ShootingManager.Instance.DestroyTurnEndBird();
    }
    IEnumerator CheckDelay(GameObject obj)
    {
        yield return new WaitForSeconds(0.2f); // 0.2초 지연
        if (TurnSys.Instance.sPlayerIndex.Value == 0)
           GameManager.Instance.player[0].pState.Value = PlayerState.SelectFin;
        else if (TurnSys.Instance.sPlayerIndex.Value == 1)
          GameManager.Instance.player[1].pState.Value = PlayerState.SelectFin;
        yield return new WaitForSeconds(0.05f);
        Destroy(obj);
        // 오브젝트 파괴
    }
}
