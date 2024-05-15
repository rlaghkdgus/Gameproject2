using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks 
{
    public List<PlayerData> player = new List<PlayerData>();//모든 플레이어 데이터 리스트(자신포함)
    public Data<GuardState> G_State = new Data<GuardState>();
    public Data<StrikeState> S_State = new Data<StrikeState>();
    public int myIndex;//자신의 인덱스 저장용
    public GameObject p1Camera;
    public GameObject p2Camera;
    public GameObject CardBuffering;
    public GameObject ActiveJudge;
    public GameObject StrikeObject;
    public GameObject ResultImage;
    public GameObject EntityManage;
    public GameObject ResultCharacter1;
    public GameObject ResultCharacter2;
    public GameObject turnFinishButton;
    public GameObject CheckGuardButton;
    public GameObject DoGuardButton;
    public GameObject NoGuardButton;
    public GameObject GuardUI;
    public GameObject p1prefab;
    public GameObject p2prefab;
    public List<GameObject> GuardBirdUI = new List<GameObject>();
    public List<TMP_Text> GuardBirdTxt = new List<TMP_Text>();
    public List<GameObject> GuardBirdUIExc = new List<GameObject>();
    public float turnTime;
    public TMP_Text timerText;
    public TMP_Text strikeScoreText;
    public List<TMP_Text> GuardNumTxt = new List<TMP_Text>();
    public bool AnotherStrike = false;
    bool guardOk = false;
    Vector3 bird1rot = new Vector3(-90f, 360f, 0f);
    Vector3 bird2rot = new Vector3(-90f, 180f, 0f);

    public PhotonView PV;

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(GameManager).Name);
                    _instance = singleton.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

  
    public void FinishTurn()
    {
        if (TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            if (player[0].ComboCount == 0)
                StartCoroutine(CheckDelay(player[0].playerCards[7].gameObject));
            else
                player[0].pState.Value = PlayerState.SelectFin;
        }
        else if (TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            if (player[1].ComboCount == 0)
                StartCoroutine(CheckDelay(player[1].playerCards[7].gameObject));
            else
                player[1].pState.Value = PlayerState.SelectFin;
        }
    }
  
    private void CheckGuard(GuardState _gState)
    {
        if (_gState == GuardState.DoCheckGuard)
        {
          if (TurnSys.Instance.sPlayerIndex.Value == 0)
            {
                player[0].Guardnums.Sort();
                if (player[0].Guardnums.Count == 3)
                {
                    for (int i = 0; i < player[0].Guardnums.Count; i++)
                    {
                        GuardNumTxt[i].text = "" + player[0].Guardnums[i];
                        if (player[0].Guardnums[i] == 20)
                            GuardNumTxt[i].text = "R";
                        if (player[0].Guardnums[i] == 30)
                            GuardNumTxt[i].text = "B";
                    }
                }
                if (player[0].Guardnums.Count == 2)
                {
                    for (int i = 0; i < player[1].Guardnums.Count; i++)
                    {
                        GuardNumTxt[i].text = "" + player[0].Guardnums[i];
                        if (player[0].Guardnums[i] == 20)
                            GuardNumTxt[i].text = "R";
                        if (player[0].Guardnums[i] == 30)
                            GuardNumTxt[i].text = "B";
                    }
                    GuardNumTxt[2].text = "";
                }
                CheckGuardButton.SetActive(myIndex == 1 && G_State.Value == GuardState.DoCheckGuard);
                NoGuardButton.SetActive(myIndex == 1 && G_State.Value == GuardState.DoCheckGuard);
            }
          
            if (TurnSys.Instance.sPlayerIndex.Value == 1)
            {
                player[1].Guardnums.Sort();
                if (player[1].Guardnums.Count == 3)
                {
                    for (int i = 0; i < player[1].Guardnums.Count; i++)
                    {
                        GuardNumTxt[i].text = "" + player[1].Guardnums[i];
                        if (player[1].Guardnums[i] == 20)
                            GuardNumTxt[i].text = "R";
                        if (player[1].Guardnums[i] == 30)
                            GuardNumTxt[i].text = "B"; 
                    }
                }
                if(player[1].Guardnums.Count == 2)
                {
                    for(int i = 0; i< player[1].Guardnums.Count; i++)
                    {
                        GuardNumTxt[i].text = "" + player[1].Guardnums[i];
                        if (player[1].Guardnums[i] == 20)
                            GuardNumTxt[i].text = "R";
                        if (player[1].Guardnums[i] == 30)
                            GuardNumTxt[i].text = "B";
                    }
                    GuardNumTxt[2].text = "";
                }
                CheckGuardButton.SetActive(myIndex == 0 && G_State.Value == GuardState.DoCheckGuard);
                NoGuardButton.SetActive(myIndex == 0 && G_State.Value == GuardState.DoCheckGuard);
            }
        }
    }
    public void CheckGuardSelect()
    {
        CheckGuardButton.SetActive(false);
        NoGuardButton.SetActive(false);
        G_State.Value = GuardState.GuardSelect;
    }
    public void NoGuard()
    {
        G_State.Value = GuardState.Idle;
        if (TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            CheckGuardButton.SetActive(false);
            NoGuardButton.SetActive(false);
            StartCoroutine(SetStrikeDelay());
        }
        
        if (TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            CheckGuardButton.SetActive(false);
            NoGuardButton.SetActive(false);
            StartCoroutine(SetStrikeDelay());
        }
    }
    IEnumerator SetStrikeDelay()
    {
        S_State.Value = StrikeState.SetStrike;
        yield return new WaitForSeconds(0.5f);
        player[TurnSys.Instance.sPlayerIndex.Value].pState.Value = PlayerState.Select;
    }
    private void SelectGuardCard(GuardState _gState)
    {
        if (_gState == GuardState.GuardSelect)
        {
            if (myIndex == 0 && TurnSys.Instance.sPlayerIndex.Value == 1)
            {
                DoGuardButton.SetActive(_gState == GuardState.GuardSelect);
            }
        }
    }
    public void DoGuard()
    {
        if (TurnSys.Instance.sPlayerIndex.Value == 0)
        {
          
            if (player[1].strikeCards[0] >= player[0].Guardnums[0])

            {
                if (player[0].doubleCheck == true && player[1].doubleCheck == true)
                {
                    player[0].strikeScore = 0;
                }
                else if (player[0].tripleCheck == true && player[1].tripleCheck == true)
                {
                    player[0].strikeScore = 0;
                }
                else if (player[0].stairCheck == true && player[1].stairCheck == true)
                {
                    player[0].strikeScore = 0;
                }
            }
            else
            {
                S_State.Value = StrikeState.SetStrike;
            }
        }
        else if (TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            player[0].Guardnums.Sort();
            if (player[0].Guardnums[0] >= player[1].Guardnums[0])
            {
                if ((player[0].doubleCheck == true && player[1].doubleCheck == true) || (player[0].tripleCheck == true && player[1].tripleCheck == true) || (player[0].stairCheck == true && player[1].stairCheck == true))
                    StartCoroutine(GuardUIDelay());
                else
                {
                    for (int i = 0; i < player[0].playerCards.Count; i++)
                    {
                        player[0].playerCards[i].myCardState = false;
                    }
                    player[0].Guardnums.Clear();
                    return;
                }
            }
            else
            {
                for (int i = 0; i < player[0].playerCards.Count; i++)
                {
                    player[0].playerCards[i].myCardState = false;
                }
                player[0].Guardnums.Clear();
                return;
            }
        }

    }

    IEnumerator GuardUIDelay()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        DoGuardButton.SetActive(false);
        GuardUI.SetActive(true);
        player[0].Guardnums.Sort();
        player[0].GuardCheck();
        ShootingManager.Instance.DestoyGuardBird();
        for(int i = 0; i < player[0].Guardnums.Count; i++)
        {
            GuardBirdTxt[i].text = player[0].Guardnums[i].ToString();
            if (player[0].Guardnums[i] == 20)
                GuardBirdTxt[i].text = "R";
            else if (player[0].Guardnums[i] == 30)
                GuardBirdTxt[i].text = "B";
        }
        for(int i = 0; i< player[1].Guardnums.Count; i++)
        {
            GuardBirdTxt[i+3].text = player[1].Guardnums[i].ToString();
            if (player[1].Guardnums[i] == 20)
                GuardBirdTxt[i+3].text = "R";
            else if (player[1].Guardnums[i] == 30)
                GuardBirdTxt[i+3].text = "B";
        }
        if (player[0].Guardnums[0] >= player[1].Guardnums[0])
        {
            if (player[0].doubleCheck == true && player[1].doubleCheck == true)
            {
                for(int i = 0; i < GuardBirdUIExc.Count; i++)
                {
                    GuardBirdUIExc[i].SetActive(false);
                }
                player[1].strikeScore = 0;
                guardOk = true;
            }
            else if (player[0].tripleCheck == true && player[1].tripleCheck == true)
            {
                player[1].strikeScore = 0;
                guardOk = true;
            }
            else if (player[0].stairCheck == true && player[1].stairCheck == true)
            {
                player[1].strikeScore = 0;
                guardOk = true;
            }
            player[1].Guardnums.Clear();
        }
        else
        {
            guardOk = false;
        }
        yield return new WaitForSeconds(1.0f);
        GuardBirdUI[0].SetActive(true);
        yield return new WaitForSeconds(1.0f);
        GuardBirdUI[1].SetActive(true);
        yield return new WaitForSeconds(3.0f);
        for(int i = 0; i< 2; i++)
        {
            GuardBirdUI[i].SetActive(false);
            GuardBirdUIExc[i].SetActive(true);
        }
        GuardUI.SetActive(false);
        if (guardOk == true)
        {
            player[1].pState.Value = PlayerState.SelectFin;
            G_State.Value = GuardState.Idle;
        }
        else if (guardOk == false)
            S_State.Value = StrikeState.SetStrike; 
    }
    private void FinishButton(StrikeState _sState)
    {
        if (_sState == StrikeState.ReadyStrike)
        { 
            if(TurnSys.Instance.sPlayerIndex.Value == 0)
             turnFinishButton.SetActive(true);
        }
       
    }
    private void StrikeDamage(StrikeState _sState)
    {
        if (_sState == StrikeState.SetStrike)
        {
            
            if (TurnSys.Instance.sPlayerIndex.Value == 0)
            {
                player[0].strikeCards.Clear();
                player[0].characterImg.sprite = player[0].characterUI[1];
                player[1].characterImg.sprite = player[1].characterUI[2];
                player[1].playerScore -= player[0].strikeScore;
                player[1].playerScore -= player[0].ComboScore * player[0].ComboCount;
                player[0].strikeScore = 0;
                player[1].playerScoreText.text = "" + player[1].playerScore;
                if(player[0].playerCards.Count <8)
                {
                    CardManager.Instance.AddCard(0);
                    CardManager.Instance.SortCards(0);
                    CardManager.Instance.ArrangeCardsBetweenMyCards(0,1.7f);
                    ShootingManager.Instance.AddBird(ShootingManager.Instance.p1birdTransform[player[0].playerCards.Count-1], ShootingManager.Instance.birdparentobj, ShootingManager.Instance.birdPrefab, ShootingManager.Instance.p1Bird, bird1rot);
                }
                if (player[1].playerCards.Count < 8)
                {
                    CardManager.Instance.AddCard(1);
                    CardManager.Instance.SortCards(1);
                    CardManager.Instance.ArrangeCardsBetweenMyCards(1, 1.7f);
                    ShootingManager.Instance.AddBird(ShootingManager.Instance.p2birdTransform[player[1].playerCards.Count-1], ShootingManager.Instance.birdparentobj2, ShootingManager.Instance.bird2Prefab, ShootingManager.Instance.p2Bird, bird2rot);
                }
            }
            else if (TurnSys.Instance.sPlayerIndex.Value == 1)
            {
                player[1].strikeCards.Clear();
                player[1].characterImg.sprite = player[1].characterUI[1];
                player[0].characterImg.sprite = player[0].characterUI[2];
                player[0].playerScore -= player[1].strikeScore;
                player[0].playerScore -= player[1].ComboScore * player[1].ComboCount;
                player[1].strikeScore = 0;
                player[0].playerScoreText.text = "" + player[0].playerScore;
                if (player[1].playerCards.Count < 8)
                {
                    CardManager.Instance.AddCard(1);
                    CardManager.Instance.SortCards(1);
                    CardManager.Instance.ArrangeCardsBetweenMyCards(1, 1.7f);
                    ShootingManager.Instance.AddBird(ShootingManager.Instance.p2birdTransform[player[1].playerCards.Count-1], ShootingManager.Instance.birdparentobj2, ShootingManager.Instance.bird2Prefab,ShootingManager.Instance.p2Bird ,bird2rot);
                }
                if (player[0].playerCards.Count < 8)
                {
                    CardManager.Instance.AddCard(0);
                    CardManager.Instance.SortCards(0);
                    CardManager.Instance.ArrangeCardsBetweenMyCards(0, 1.7f);
                    ShootingManager.Instance.AddBird(ShootingManager.Instance.p1birdTransform[player[0].playerCards.Count-1], ShootingManager.Instance.birdparentobj, ShootingManager.Instance.birdPrefab, ShootingManager.Instance.p1Bird, bird1rot);
                }
                //player[1].pState.Value = PlayerState.Select; 

            }
            S_State.Value = StrikeState.ReadyStrike;
            
        }
       
    }
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PV = GetComponent<PhotonView>();
        PV.RPC("SetCamera", RpcTarget.All);
        S_State.onChange += FinishButton;
        S_State.onChange += StrikeDamage;
        G_State.onChange += CheckGuard;
        G_State.onChange += SelectGuardCard;
        
        /*sPlayerIndex에 구독을 하는 이유 : 
         * 그냥 플레이어 인덱스를 넣어버리면 다른 사람차례에 버튼이 호출 되지않을 것.
         * 시스템플레이어 인덱스에서 호출을 해야 다른사람에게도 호출이됨
         * 시스템인덱스와 내인덱스가 동일할때라는 조건에 따라 각 차례에 버튼 호출
         * 활용방안으로 메소드안에 if(나의 인덱스 == 시스템인덱스){ 턴에 작용하고 싶은 행동 }
        */
    }
    [PunRPC]
    public void SetCamera()
    {
        if (PhotonNetwork.IsMasterClient)
            p2Camera.SetActive(false);
        else if (!PhotonNetwork.IsMasterClient)
            p1Camera.SetActive(false);
    }
    void Update()
    {
        /*
        if(player[0].pState.Value == PlayerState.Select)//시간체크용
        {
            turnTime -= Time.deltaTime;
            timerText.text = ""+Mathf.Round(turnTime);
        
            if(turnTime < 10 && turnTime >9.9f)
            { 
                player[0].characterImg.sprite = player[0].characterUI[0];
            }
            if(turnTime < 0 )
            {
                if (player[0].ComboCount > 0)
                {
                    player[0].pState.Value = PlayerState.SelectFin;
                }
                else if(player[0].ComboCount == 0)
                {
                    CardBuffering.SetActive(player[0].pState.Value == PlayerState.Select);
                    player[0].playerCards[7].myCardState = true;
                    StartCoroutine(CheckDelay(player[0].playerCards[7].gameObject));
                    ShootingManager.Instance.DestroyTurnEndBird();
                }
            }      
        }
       
        if (player[1].pState.Value == PlayerState.Select)//시간체크용
        */
        {
            /*
            turnTime -= Time.deltaTime;
            timerText.text = "" + Mathf.Round(turnTime);
            if (turnTime < 10 && turnTime > 9.9f)
            {
                player[1].characterImg.sprite = player[1].characterUI[0];
            }
            if (turnTime < 0)
            {
                if (player[1].ComboCount > 0)
                {
                    player[1].pState.Value = PlayerState.SelectFin;
                }
                else
                {
                    CardBuffering.SetActive(player[1].pState.Value == PlayerState.Select);
                    player[1].playerCards[7].myCardState = true;
                    StartCoroutine(CheckDelay(player[1].playerCards[7].gameObject));
                }
            }
            */
        }
    }
    void Start()
    {
      if(PhotonNetwork.IsMasterClient)  PV.RPC("GmrStart", RpcTarget.All); 
    }
    [PunRPC]
    public void GmrStart()
    {
        Debug.Log("GmrStart");
            S_State.Value = StrikeState.Idle;
            G_State.Value = GuardState.Idle;
            player[0].playerScoreText.text = "" + player[0].playerScore;
            player[1].playerScoreText.text = "" + player[1].playerScore;
            for (int i = 0; i < player.Count; i++)//플레이어의 수따라
            {    
                player[i].pState.Value = PlayerState.Idle;//플레이어 데이터를 초기화
                player[i].pIndex = i;//각플레이어에게 턴 인덱스를 배정
                for (int j = 0; j < 8; j++)
                {
                if(PhotonNetwork.IsMasterClient)
                    CardManager.Instance.AddCard(i);
                }
                CardManager.Instance.SortCards(i);
                CardManager.Instance.ArrangeCardsBetweenMyCards(i, 1.7f);
            }
        
    }
    IEnumerator CheckDelay(GameObject obj)
    {
        Destroy(obj);
        yield return new WaitForSeconds(0.15f); // 0.2초 지연
        if (TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            player[0].playerCards.RemoveAt(7);//내카드리스트에 삭제
            player[0].pState.Value = PlayerState.SelectFin;
        }
        else if (TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            player[1].playerCards.RemoveAt(7);//내카드리스트에 삭제
           player[1].pState.Value = PlayerState.SelectFin;
        }
       
        // 오브젝트 파괴
    }
    
  
}

