using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Spine.Unity;

public class PlayerData : MonoBehaviourPunCallbacks 
{
    public Data<PlayerState> pState = new Data<PlayerState>();//플레이어 상태 변수
    public int pIndex;//플레이어의 인덱스
    public List<CardInfo> playerCards = new List<CardInfo>();
    public List<int> strikeCards = new List<int>();
    public int playerScore;
    public Sprite PlayerBirdSprite;
    public int strikeScore = 0;
    public int ComboCount = 0;
    public int ComboScore = 200;
    public SkeletonGraphic CharaIdle;
    public List <SkeletonDataAsset> charaAnim = new List<SkeletonDataAsset>();
    public Transform cardLeftTransform;
    public Transform cardRightTransform;
    public Transform playerPosition;
    public GameObject playerObject;
    public TMP_Text playerScoreText;
    public GameObject delayUI;
    
    [Header("Check")]
    public bool tripleCheck = false;
    public bool stairCheck = false;
    public bool doubleCheck = false;
    public List<int> Guardnums = new List<int>();
    public PhotonView PV;
    [Header("playerParticleSys")]
    public ParticleSystem p1TurnStart;
    public ParticleSystem p2TurnStart;
    public ParticleSystem p1TurnEnd;
    public ParticleSystem p2TurnEnd;
    [Header("BirdState")]
    public Sprite PlayerClickBird;
    public Sprite PlayerGuardFailBird;
    public Sprite PlayerBirdTemp;
    public GameObject myStateObj;
    public TMP_Text myStateTxt;
    public int RoundWinCount = 0;
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        pState.onChange += Draw;//메소드를 추가(onChange를 의도하는 순서대로 배치)
        pState.onChange += SelectTime;
        pState.onChange += DelayUI;
        pState.onChange += SelectCardFin;
        pState.onChange += SetIdle;
        TurnSys.Instance.sPlayerIndex.onChange += SetTurn;
    }
    private void Start()
    {
       if(pIndex !=GameManager.Instance.myIndex)
        {
            myStateObj.SetActive(false);
        }
    }

    [PunRPC]
    public void RPCPlayerSystem()
    {
        Debug.Log("WaitAction" + TurnSys.Instance.sPlayerIndex.Value);
        TurnSys.Instance.gState.Value = GameState.WaitAction;//게임 상태를 WaitAction으로 바꾼 후
    }
  
    [PunRPC]    
    public void RPCPlayerSelect()  
    {
        Debug.Log("playerSelect" + TurnSys.Instance.sPlayerIndex.Value);
    }
    
    IEnumerator PlayerSystem()
    {
        Debug.Log("GoplayerSystem");
        TurnSys.Instance.gState.Value = GameState.WaitAction;
        yield return new WaitForSeconds(0.5f);
        pState.Value = PlayerState.Select;
    }
    private void SelectTime(PlayerState _pState)
    {
        if( _pState == PlayerState.Select)
        {
            delayUI.SetActive(false);
            if (TurnSys.Instance.sPlayerIndex.Value == GameManager.Instance.myIndex)
            {
                myStateTxt.text = "My Turn";
            }
            StartCoroutine(SelectTimer());
        }
    }
    private void DelayUI(PlayerState _pState)
    {
        if(_pState == PlayerState.delay)
        {
            delayUI.SetActive(true);
        }
    }
    private void Draw(PlayerState _pState)//Player상태가 StartDraw인지 체크하고 PlayerSystem 시작 
    {
        if (_pState == PlayerState.StartDraw)
        {
            if (!p1TurnStart.isPlaying && TurnSys.Instance.sPlayerIndex.Value == 0 && GameManager.Instance.myIndex == 0)
            {
                SoundManager.Instance.PlaySfx(SoundManager.Sfx.DrawCard);
                p1TurnStart.Play();
            }
            else if (!p2TurnStart.isPlaying && TurnSys.Instance.sPlayerIndex.Value == 1 && GameManager.Instance.myIndex == 1)
            {
                SoundManager.Instance.PlaySfx(SoundManager.Sfx.DrawCard);
                p2TurnStart.Play();
            }
            StartCoroutine(DelayDraw());
            Debug.Log("StartDraw?");
        }
    }
    IEnumerator DelayDraw()
    {
        yield return new WaitForSeconds(0.5f);
        strikeScore = 0;
            for (int i = playerCards.Count; i < 8; i++)
            {
                if(!PhotonNetwork.IsMasterClient && TurnSys.Instance.sPlayerIndex.Value == 1 || PhotonNetwork.IsMasterClient && TurnSys.Instance.sPlayerIndex.Value == 0)
                CardManager.Instance.AddCard(TurnSys.Instance.sPlayerIndex.Value);
            yield return new WaitForSeconds(0.05f);
        }
        CardManager.Instance.SortCards(playerCards);
        CardManager.Instance.ArrangeCardsBetweenMyCards(playerCards, cardLeftTransform, cardRightTransform, 1.7f);//건필군이 카드 간격을 조절하고싶을때 맨마지막상수f값을건들면됨
        StartCoroutine(PlayerSystem());
    }
    private void SelectCardFin(PlayerState _pState)//카드 선택이 끝났을때
    {
        if (_pState == PlayerState.SelectFin)
        {
            StartCoroutine(SelectCardFin());
        }
    }
    IEnumerator SelectCardFin()
    {
        
        GameManager.Instance.turnFinishButton.SetActive(false);
        yield return new WaitForSecondsRealtime(0.5f);
        CardManager.Instance.SortCards(playerCards);
        CardManager.Instance.ArrangeCardsBetweenMyCards(playerCards, cardLeftTransform, cardRightTransform, 1.7f);//건필군이 카드 간격을 조절하고싶을때 맨마지막상수f값을건들면됨
        yield return new WaitForSecondsRealtime(0.5f);
        PV.RPC("RPCSelectCardFin", RpcTarget.All);
    }
    [PunRPC]
    public void RPCSelectCardFin()
    {
        ComboCount = 0;
        pState.Value = PlayerState.End; 
        StartCoroutine(CardClearDelay());
    }
   
    public void ReadyStrike()//스트라이크 버튼 작용
    {
        if (TurnSys.Instance.sPlayerIndex.Value == GameManager.Instance.myIndex && pState.Value == PlayerState.Select)
        {
            if (GameManager.Instance.S_State.Value == StrikeState.ReadyStrike && pIndex == GameManager.Instance.myIndex)
            {
                Debug.Log("Check");
                Check();
            }
            else if (GameManager.Instance.S_State.Value == StrikeState.Idle)
            {
                Debug.Log("SetStrike");
                //문구 호출
                PV.RPC("RPCReadyStrike", RpcTarget.All);
            }

        }
    }
    [PunRPC]
    public void RPCReadyStrike()
    {
        GameManager.Instance.S_State.Value = StrikeState.ReadyStrike;
    }

    IEnumerator CardClearDelay()
    {
        yield return new WaitForSeconds(0.05f);
        strikeCards.Clear();
        Guardnums.Clear();
        GameManager.Instance.S_State.Value = StrikeState.Idle;
        TurnSys.Instance.gState.Value = GameState.ActionEnd;  //게임상태를 ActionEnd로만듬
    }

    private void SetTurn(int _sIndex)
    {
        if (pIndex == _sIndex)//시스템 인덱스와 플레이어 인덱스가 같을경우 턴을 시작하게 하기위한 조건
        {
            pState.Value = PlayerState.StartDraw;
            strikeCards.Clear();
            Guardnums.Clear();
            Debug.Log("DrawTurn " + pIndex);//여기같은 경우 이미지띄우기연출같은거 넣으면 될듯
        }
    }
    private void SetIdle(PlayerState _pState)//player의 상태가 end일 경우에
    {
        if (_pState == PlayerState.End)
        {
            myStateTxt.text = "";
            for(int i = 0; i < playerCards.Count; i++)
            {
                playerCards[i].myCardState = false;
            }
            if (!p1TurnEnd.isPlaying && TurnSys.Instance.sPlayerIndex.Value == 0 && GameManager.Instance.myIndex == 0)
            {
                p1TurnEnd.Play();
            }
            else if (!p2TurnEnd.isPlaying && TurnSys.Instance.sPlayerIndex.Value == 1 && GameManager.Instance.myIndex == 1)
            {
                p2TurnEnd.Play();
            }
            if (PhotonNetwork.IsMasterClient)
              PV.RPC("RPCSetIdle", RpcTarget.All);
        }
        //이 순간에도 상대가 패를 냈을때 그패를 가져와 조합완성이 가능하다면 작동하도록 검사추가
    }
    [PunRPC]
    public void RPCSetIdle()
    {
          pState.Value = PlayerState.Idle;// 아무것도 안하는 동작으로 변경
    }
  
    public void Check()
    {
        strikeCards.Sort();
        int headCheck = 0;
        int bodyCheck = 0;
        doubleCheck = false;
        tripleCheck = false;
        stairCheck = false;
        int i = 0;

        if (strikeCards.Count < 2)
        {
            Debug.Log("CheckError");
            StartCoroutine(PlayerCheckFaildelay());
            return;
        }
        int temp = strikeCards[i];
        if (strikeCards.Count == 2)
        {
            if (temp == strikeCards[i + 1])

            {
                Debug.Log("HEAD");
                headCheck++;
                PV.RPC("RPCdoubleCheck", RpcTarget.All);
            }
        }
        if (strikeCards.Count == 3)
        {
            if (temp == strikeCards[i + 1])         // HEAD CHECK
            {
                if (temp == strikeCards[i + 2])//같은수 3개가 몸통일경우
                { 
                    bodyCheck++;
                    PV.RPC("RPCtripleCheck", RpcTarget.All);
                    Debug.Log("HEAD => BODY");
                }
            }
            else                             // BODY CHECK
            {
                if (temp + 1 == strikeCards[i + 1] && temp + 2 == strikeCards[i + 2])//연속수3개가 몸통일경우
                {
                    PV.RPC("RPCstairCheck", RpcTarget.All);
                    bodyCheck++;
                    Debug.Log("BODY");
                }

            }
        }

        if (bodyCheck == 0 && headCheck == 0)
        {
            strikeCards.Clear();
            StartCoroutine(PlayerCheckFaildelay());
            //문구 및 선택취소
        }

        if (bodyCheck == 1 || headCheck == 1)
        {
            pState.Value = PlayerState.delay;
            GameManager.Instance.BgdarkUI.SetActive(false);
            PV.RPC("RPCStrike", RpcTarget.All);
        }
    }
    IEnumerator PlayerCheckFaildelay()
    {
        GameManager.Instance.GuardFailUIdelay.SetActive(true);
        PlayerClickBird = PlayerGuardFailBird;
        SoundManager.Instance.PlaySfx(SoundManager.Sfx.CheckFail);
        yield return new WaitForSecondsRealtime(0.5f);
        for (int i = 0; i < playerCards.Count; i++)
        {
            playerCards[i].myCardState = false;
        }
        PlayerClickBird = PlayerBirdTemp;
        GameManager.Instance.GuardFailUIdelay.SetActive(false);
    }
    [PunRPC]
    public void RPCStrike()
    {
        StartCoroutine(DeleteDelay());
        ComboCount++;
        strikeCards.Clear();
        if(PhotonNetwork.IsMasterClient)
        PV.RPC("RPCCheckGuard", RpcTarget.All);
    }
    [PunRPC]
    public void GuardnumInfo(int Guardnum)
    {
        Guardnums.Add(Guardnum);
    }
    [PunRPC]
    public void GuardCheck()
    {
        int headCheck = 0;
        int bodyCheck = 0;
        doubleCheck = false;
        tripleCheck = false;
        stairCheck = false;
        int i = 0;

        if (Guardnums.Count < 2)
        {
            for (int a = 0; a < GameManager.Instance.guardBirdCount; a++)
            {
                PV.RPC("RPCBirdUIClear", RpcTarget.All, a);
            }
            PV.RPC("RPCGuardnumClear", RpcTarget.All);
            Debug.Log("CheckError");
            StartCoroutine(PlayerCheckFaildelay());
            return;
        }
        int temp = Guardnums[i];
        if (Guardnums.Count == 2)
        {
            if (temp == Guardnums[i + 1])
            {
                Debug.Log("HEAD");
                headCheck++;
                PV.RPC("RPCdoubleCheck", RpcTarget.All);
            }
        }
        if (Guardnums.Count == 3)
        {
            if (temp == Guardnums[i + 1])         
            {
                if (temp == Guardnums[i + 1]&&Guardnums[i+1]== Guardnums[i+2])//같은수 3개가 몸통일경우
                {
                    headCheck--;
                    bodyCheck++;
                    PV.RPC("RPCtripleCheck", RpcTarget.All);
                    Debug.Log("HEAD => BODY");
                }
            }
            else                             
            {
                if (temp + 1 == Guardnums[i + 1] && temp + 2 == Guardnums[i + 2])//연속수3개가 몸통일경우
                {
                    PV.RPC("RPCstairCheck", RpcTarget.All);
                    bodyCheck++;
                    Debug.Log("BODY");
                }

            }
        }
        if (bodyCheck == 0 && headCheck == 0)
        {
            for(int a = 0; a < GameManager.Instance.guardBirdCount; a++)
            {
                PV.RPC("RPCBirdUIClear", RpcTarget.All, a);
            }
            PV.RPC("RPCGuardnumClear", RpcTarget.All);   
            StartCoroutine(PlayerCheckFaildelay());
            PV.RPC("GuardCountClear", RpcTarget.All);
        }
        if (bodyCheck == 1 || headCheck == 1)
        {
        }
    }
    [PunRPC]
    public void GuardCountClear()
    {
        GameManager.Instance.guardBirdCount = 0;
    }
    [PunRPC]
    public void RPCBirdUIClear(int i)
    {
        if(TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            GameManager.Instance.p2GuardBirdUI[i].SetActive(false);
        }
        else if (TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            GameManager.Instance.p1GuardBirdUI[i].SetActive(false);
        }
    }
    [PunRPC]
    public void RPCGuardnumClear()
    {
        Guardnums.Clear();
    }
    [PunRPC]
    public void RPCGuardCheck()
    {
        StartCoroutine(DeleteDelay());
    }
    [PunRPC] public void RPCdoubleCheck() { doubleCheck = true; strikeScore = 100; }
    [PunRPC] public void RPCtripleCheck() { doubleCheck = false;  tripleCheck = true; strikeScore = 500; }
    [PunRPC] public void RPCstairCheck() { stairCheck = true; strikeScore = 300; }
    IEnumerator DeleteDelay()
    {
        for (int a = playerCards.Count - 1; a >= 0; a--)
        {
            if (playerCards[a].myCardState == true)
            {
                if (GameManager.Instance.G_State.Value != GuardState.GuardSelect)
                    PV.RPC("GuardnumInfo", RpcTarget.All, playerCards[a].cardnum);
                yield return new WaitForSecondsRealtime(0.15f);
                Destroy(playerCards[a].gameObject);
                yield return new WaitForSecondsRealtime(0.15f);
                playerCards.RemoveAt(a);
                yield return new WaitForSecondsRealtime(0.15f);
            }
        }
    }
   IEnumerator SelectTimer()
    {
        while (true)
        {
            if (GameManager.Instance.myIndex == TurnSys.Instance.sPlayerIndex.Value && pState.Value == PlayerState.Select)
            {
                GameManager.Instance.turnTime -= Time.deltaTime;
                GameManager.Instance.timerText.text = "" + Mathf.Round(GameManager.Instance.turnTime);
            }
            else
                break;
            if (GameManager.Instance.turnTime < 10 && GameManager.Instance.turnTime > 9.9f)
            {
                GameManager.Instance.timerCharacterImg();
            }
            if (GameManager.Instance.turnTime < 0)
            {
                GameManager.Instance.BgdarkUI.SetActive(false);
                for (int i = 0; i < playerCards.Count; i++)
                {
                    playerCards[i].myCardState = false;
                }
                if (ComboCount > 0)
                {
                    pState.Value = PlayerState.SelectFin;
                    break;
                }
                else if (ComboCount == 0)
                {
                    GameManager.Instance.CardBuffering.SetActive(pState.Value == PlayerState.Select);   
                    StartCoroutine(CheckDelay(playerCards[7].gameObject));
                    break;
                }
            }
            
            if (TurnSys.Instance.gState.Value == GameState.GameEnd)
                break;
            yield return null;
        }
    }

    IEnumerator CheckDelay(GameObject obj)
    {
        Destroy(obj);
        yield return new WaitForSeconds(0.15f); // 0.2초 지연
        playerCards.RemoveAt(7);
        pState.Value = PlayerState.SelectFin;

        // 오브젝트 파괴
    }

    IEnumerator GuardCheckdelay()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        Debug.Log("Guard Check2");
        GameManager.Instance.G_State.Value = GuardState.DoCheckGuard;
        Debug.Log("Guard Check3");
    }
    [PunRPC]
    public void RPCCheckGuard()
    {
        Debug.Log("Guard Check1");
        StartCoroutine(GuardCheckdelay());
    }
   
    [PunRPC]
    public void CardStateChange(int i, int Count)
    {
       for(int a = 0; a < Count; a++)
        {
            strikeCards.Add(playerCards[i+a].cardnum);      
            playerCards[i+a].myCardState = true;
        }
    }

}
