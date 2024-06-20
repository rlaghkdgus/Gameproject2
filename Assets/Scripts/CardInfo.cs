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
    public SpriteRenderer Cardsprite;
    public TMP_Text text;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (Cardsprite == null)
        {
            Cardsprite = GetComponent<SpriteRenderer>();
            if (Cardsprite == null)
            {
                Debug.LogError("SpriteRenderer 컴포넌트가 없습니다.");
            }
        }
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void Setup(Card card)
    {
        this.card = card;
        if (GameManager.Instance.myIndex == 0)
            Cardsprite.sprite = GameManager.Instance.player[0].PlayerBirdSprite;
        else if(GameManager.Instance.myIndex == 1)
            Cardsprite.sprite = GameManager.Instance.player[1].PlayerBirdSprite;
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
        if (PhotonNetwork.IsMasterClient)
            RPCp1Click();
        else if (!PhotonNetwork.IsMasterClient)
            RPCp2Click();
    }
    private void Update()
    {
        if(myCardState == true)
        {
            if (GameManager.Instance.myIndex == 0)
                Cardsprite.sprite = GameManager.Instance.player[0].PlayerClickBird;
            if (GameManager.Instance.myIndex == 1)
                Cardsprite.sprite = GameManager.Instance.player[1].PlayerClickBird;
        }
        else if(myCardState == false)
        {
            if (GameManager.Instance.myIndex == 0)
                Cardsprite.sprite = GameManager.Instance.player[0].PlayerBirdSprite;
            else if (GameManager.Instance.myIndex == 1)
                Cardsprite.sprite = GameManager.Instance.player[1].PlayerBirdSprite;
        }
    }
    public void RPCp1Click()
    {
        Debug.Log("Click");
        if (GameManager.Instance.player[0].pState.Value == PlayerState.Select)
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
                    
                        SoundManager.Instance.PlaySfx(SoundManager.Sfx.SelectBirdSfx);
                        GameManager.Instance.player[0].strikeCards.Add(cardnum);
                        myCardState = true;
                    
                }
                }
                else
                {
                            GameManager.Instance.CardBuffering.SetActive(GameManager.Instance.player[0].pState.Value == PlayerState.Select);
                        this.myCardState = true;
                SoundManager.Instance.PlaySfx(SoundManager.Sfx.SelectBirdSfx);
                GameManager.Instance.player[0].pState.Value = PlayerState.Idle;
                        StartCoroutine(DeleteDelay(0));
                            ShootingManager.Instance.DestroyTurnEndBird();
            }
        }
        else if (GameManager.Instance.G_State.Value == GuardState.GuardSelect && TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            if (GameManager.Instance.player[0].Guardnums.Count > 3)
                return;
            SoundManager.Instance.PlaySfx(SoundManager.Sfx.SelectBirdSfx);
            PV.RPC("RPCGuardClick",RpcTarget.All, 0, cardnum);
            myCardState = true;
        }
    }
    [PunRPC]
    public void RPCGuardClick(int playerIndex,int cardnum)
    {
        GameManager.Instance.player[playerIndex].Guardnums.Add(cardnum);
        GameManager.Instance.guardBirdCount++;
        if(TurnSys.Instance.sPlayerIndex.Value == 0 && myCardState == false)
        {
            GameManager.Instance.p2GuardBirdTxt[GameManager.Instance.guardBirdCount- 1].text = "" + cardnum;
            if(cardnum  == 20)
            {
                GameManager.Instance.p2GuardBirdTxt[GameManager.Instance.guardBirdCount - 1].text = "R"; 
            }
            else if (cardnum == 30)
            {
                GameManager.Instance.p2GuardBirdTxt[GameManager.Instance.guardBirdCount - 1].text = "B";
            }
            GameManager.Instance.p2GuardBirdUI[GameManager.Instance.guardBirdCount - 1].SetActive(true);
        }
        if (TurnSys.Instance.sPlayerIndex.Value == 1 && myCardState == false)
        {
            GameManager.Instance.p1GuardBirdTxt[GameManager.Instance.guardBirdCount - 1].text = "" + cardnum;
            if (cardnum == 20)
            {
                GameManager.Instance.p1GuardBirdTxt[GameManager.Instance.guardBirdCount - 1].text = "R";
            }
            else if (cardnum == 30)
            {
                GameManager.Instance.p1GuardBirdTxt[GameManager.Instance.guardBirdCount - 1].text = "B";
            }
            GameManager.Instance.p1GuardBirdUI[GameManager.Instance.guardBirdCount - 1].SetActive(true);
        }
    }
   
    public void RPCp2Click()
    {
        Debug.Log("Click");
         if (GameManager.Instance.player[1].pState.Value == PlayerState.Select)
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
                    SoundManager.Instance.PlaySfx(SoundManager.Sfx.SelectBirdSfx);
                    GameManager.Instance.player[1].strikeCards.Add(cardnum);
                    myCardState = true;

                }
            }
            else
            {
                GameManager.Instance.CardBuffering.SetActive(GameManager.Instance.player[1].pState.Value == PlayerState.Select);
                myCardState = true;
                SoundManager.Instance.PlaySfx(SoundManager.Sfx.SelectBirdSfx);
                GameManager.Instance.player[1].pState.Value = PlayerState.Idle;
                StartCoroutine(DeleteDelay(1));
                ShootingManager.Instance.DestroyTurnEndBird();   
            }
        }
         else if(GameManager.Instance.G_State.Value == GuardState.GuardSelect && TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            if (GameManager.Instance.player[1].Guardnums.Count > 3)
                return;
            SoundManager.Instance.PlaySfx(SoundManager.Sfx.SelectBirdSfx);
            PV.RPC("RPCGuardClick",RpcTarget.All, 1, cardnum);
            myCardState = true;
        }
    }

  
    [PunRPC]
    public void RPCAddGuard(int playerIndex)
    {
        GameManager.Instance.player[playerIndex].Guardnums.Add(cardnum);
        myCardState = true;
    }
    IEnumerator DeleteDelay(int playerIndex)
    {
        GameManager.Instance.player[playerIndex].pState.Value = PlayerState.SelectFin;
        for (int a = GameManager.Instance.player[playerIndex].playerCards.Count - 1; a >= 0; a--)
        {
            if (GameManager.Instance.player[playerIndex].playerCards[a].myCardState == true)
            {
                GameManager.Instance.player[playerIndex].playerCards.Remove(this);  
                yield return new WaitForSeconds(0.15f);
                gameObject.SetActive(false);
            }
        }
    }
    [PunRPC]
    public void SelectFin(int playerIndex)
    {
        GameManager.Instance.player[playerIndex].pState.Value = PlayerState.SelectFin;
    }
}
