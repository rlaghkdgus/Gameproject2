using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class TurnSys : MonoBehaviourPun
{

    public Data<int> sPlayerIndex = new Data<int>();
    //플레이어 인덱스와 가상의 시스템 인덱스를 비교해 턴을 구별하기위한 데이터할당
    //ex. player 1 , 2, 3 의 인덱스가 각 0,1,2 일 경우
    //splayerIndex의 값이 0일때 player1의 턴
    public Data<GameState> gState = new Data<GameState>();//게임 상태 저장용 변수

    public int gameTurn = 0;//플레이어별 한바퀴 돌때마다 +1추가, 추후 카드 인덱스에 넣을 예정
    public bool gameStart = false;
    int endCount = 0;
    private static TurnSys _instance;
    public PhotonView PV;
    public GameObject GameEndUI;
    public static TurnSys Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TurnSys>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(TurnSys).Name);
                    _instance = singleton.AddComponent<TurnSys>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        sPlayerIndex.Value = -1;//player1부터시작, 
        gState.Value = GameState.Idle;//게임 상태를 아무것도안할때로 변경
        gState.onChange += NextPlayer;
        gState.onChange += GameEnd;
    }
    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        PV.RPC("StartGame", RpcTarget.All);
    }



    private void NextPlayer(GameState _gState)//현재 test용 버튼에 들어간 메소드,
    {
        //gState의Value는 PlayerData의 PlayerSystem과 연계
        if (_gState == GameState.ActionEnd)//PlayerSystem에서 gState.Value가 ActionEnd상태가 될 경우
        { 
            if ((sPlayerIndex.Value == 0  &&  ShootingManager.Instance.p1Bird.Count == 8)|| (sPlayerIndex.Value == 1 && ShootingManager.Instance.p2Bird.Count == 8))
            {
                ShootingManager.Instance.DestroyTurnEndBird();
            }
            GameManager.Instance.GuardnumTextClear();
            GameManager.Instance.timerText.text = "";
            Debug.Log("NextPlayer");
            if(PhotonNetwork.IsMasterClient)
            PV.RPC("RPCActionEnd", RpcTarget.All);
        }
    }
    [PunRPC]
    public void RPCActionEnd()
    {

        if (CardManager.Instance.cardBuffer.Count == 0)
            endCount++;
        if (endCount == 2)
            gState.Value = GameState.GameEnd;
        StartCoroutine(TurnStartCo());
        if (GameManager.Instance.myIndex == 0)
        {
            GameManager.Instance.player[0].characterImg.sprite = GameManager.Instance.player[0].characterUI[3];
            GameManager.Instance.player[1].characterImg.sprite = GameManager.Instance.player[1].characterUI[3];
        }
        else if (GameManager.Instance.myIndex == 1)
        {
            GameManager.Instance.player[1].characterImg.sprite = GameManager.Instance.player[0].characterUI[3];
            GameManager.Instance.player[0].characterImg.sprite = GameManager.Instance.player[1].characterUI[3];
        }
    }
    
    IEnumerator TurnStartCo()
    {
            Debug.Log("TurnStart");
            yield return new WaitForSeconds(1.0f);
            if (sPlayerIndex.Value >= 1)//sPlayer의 인덱스가 2이상일경우
            {
                sPlayerIndex.Value = 0;//0으로 초기화(player1의턴)
                gameTurn++;//게임의 전체 턴 증가
                Debug.Log("NextTurn " + gameTurn);
            }
            else if (sPlayerIndex.Value == -1)
            {
                sPlayerIndex.Value = 0;//0으로 초기화(player1의턴)
                gameTurn++;//게임의 전체 턴 증가
                Debug.Log("NextTurn " + gameTurn);
            }
            else
            {
                sPlayerIndex.Value = 1;//sPlayerIndex의 밸류를 증가시켜 턴 전환 구현 
            }
            GameManager.Instance.turnTime = 20.0f;       
    }

    IEnumerator StartGameCoroutine()//게임 시작시
    {
        Debug.Log("StartC");
        yield return new WaitForSeconds(1.0f);//1초의 딜레이를 주고 
        gState.Value = GameState.ActionEnd;// 0
        Debug.Log("GameStart");
    }

    [PunRPC]
    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }
    private void GameEnd(GameState _gState)
    {
        if(_gState == GameState.GameEnd)
        {
            GameManager.Instance.turnFinishButton.SetActive(false);
            if (PhotonNetwork.IsMasterClient)
            PV.RPC("RPCGameEnd1", RpcTarget.All);
        }
    }
    [PunRPC]
    public void RPCGameEnd1()
    {
        Debug.Log("A");
        for (int i = 0; i < 2; i++)
        {
            GameManager.Instance.player[i].pState.Value = PlayerState.Idle;
        }
        GameManager.Instance.S_State.Value = StrikeState.Idle;
        GameManager.Instance.G_State.Value = GuardState.Idle;
        if (RoundManager.Instance.roundCount == 1)
        {
            if(PhotonNetwork.IsMasterClient)
            PV.RPC("RPCNewRound", RpcTarget.All);
        }
        else if (RoundManager.Instance.roundCount == 2)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                PV.RPC("RPCGameEndUI", RpcTarget.All);
            }
        }
    }
    [PunRPC]
    public void RPCGameEndUI()
    {
        StartCoroutine(GameEndUIset());
    }
    IEnumerator GameEndUIset()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        GameEndUI.SetActive(true);
    }
    [PunRPC]
    public void RPCNewRound()
    {
        
        Debug.Log("B");
        StartCoroutine(NewRound());
    }
    IEnumerator NewRound()
    {
        GameManager.Instance.GuardnumTextClear();
        GameManager.Instance.timerText.text = "";
        GameManager.Instance.turnFinishButton.SetActive(false);
        yield return new WaitForSecondsRealtime(0.5f);
        GameManager.Instance.player[0].playerScore = 3000;
        GameManager.Instance.player[1].playerScore = 3000;
        GameManager.Instance.player[0].ComboCount = 0;
        GameManager.Instance.player[1].ComboCount = 0;
        Debug.Log("C");
        yield return new WaitForSecondsRealtime(1.0f);
        for(int i = 0;  i<2; i++)
        {
            for (int j = 0; j < GameManager.Instance.player[i].playerCards.Count; j++)
            {
                GameManager.Instance.player[i].playerCards[j].gameObject.SetActive(false);
            }
            GameManager.Instance.player[i].playerCards.Clear();
        }
        Debug.Log("D");
        yield return new WaitForSecondsRealtime(1.5f);
        for(int i = 0; i < 2; i++)
        {
            if(i == 0)
            {
                for(int j = 0; j < ShootingManager.Instance.p1Bird.Count; j++)
                {
                    ShootingManager.Instance.p1Bird[j].gameObject.SetActive(false);
                }
                ShootingManager.Instance.p1Bird.Clear();
            }
            else if(i == 1)
            {
                for (int j = 0; j < ShootingManager.Instance.p2Bird.Count; j++)
                {
                    ShootingManager.Instance.p2Bird[j].gameObject.SetActive(false);
                }
                ShootingManager.Instance.p2Bird.Clear();
            }
        }
        Debug.Log("E");
        CardManager.Instance.cardBuffer.Clear();
        yield return new WaitForSecondsRealtime(0.5f);
        if (PhotonNetwork.IsMasterClient)
            CardManager.Instance.RoundSetupCard();
        Debug.Log("F");
        yield return new WaitForSecondsRealtime(0.5f);
        sPlayerIndex.Value = -1;
        Debug.Log("G");
        yield return new WaitForSecondsRealtime(2.0f);
        if (PhotonNetwork.IsMasterClient)
            PV.RPC("GmrStart", RpcTarget.All);
        Debug.Log("H");
        if(PhotonNetwork.IsMasterClient)
            PV.RPC("RPCActionEnd", RpcTarget.All);
    }
}
