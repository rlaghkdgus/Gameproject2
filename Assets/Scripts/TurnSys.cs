using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using TMPro;
public class TurnSys : MonoBehaviourPun
{

    public Data<int> sPlayerIndex = new Data<int>();
    //�÷��̾� �ε����� ������ �ý��� �ε����� ���� ���� �����ϱ����� �������Ҵ�
    //ex. player 1 , 2, 3 �� �ε����� �� 0,1,2 �� ���
    //splayerIndex�� ���� 0�϶� player1�� ��
    public Data<GameState> gState = new Data<GameState>();//���� ���� ����� ����

    public int gameTurn = 0;//�÷��̾ �ѹ��� �������� +1�߰�, ���� ī�� �ε����� ���� ����
    public bool gameStart = false;
    int endCount = 0;
    private static TurnSys _instance;
    public PhotonView PV;

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
        sPlayerIndex.Value = -1;//player1���ͽ���, 
        gState.Value = GameState.Idle;//���� ���¸� �ƹ��͵����Ҷ��� ����
        gState.onChange += NextPlayer;
    }
    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        PV.RPC("StartGame", RpcTarget.All);
    }



    private void NextPlayer(GameState _gState)//���� test�� ��ư�� �� �޼ҵ�,
    {
        //gState��Value�� PlayerData�� PlayerSystem�� ����
        if (_gState == GameState.ActionEnd)//PlayerSystem���� gState.Value�� ActionEnd���°� �� ���
        {
            if (CardManager.Instance.cardBuffer.Count == 0)
                endCount++;
            if (endCount == 2)
                gState.Value = GameState.GameEnd;
            ShootingManager.Instance.DestroyTurnEndBird();
            GameManager.Instance.timerText.text = "";
            Debug.Log("NextPlayer");
            if(PhotonNetwork.IsMasterClient)
            PV.RPC("RPCActionEnd", RpcTarget.All);
        }
    }
    [PunRPC]
    public void RPCActionEnd()
    {
            Debug.Log("ActionEnd");
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
            if (sPlayerIndex.Value >= 1)//sPlayer�� �ε����� 2�̻��ϰ��
            {
                sPlayerIndex.Value = 0;//0���� �ʱ�ȭ(player1����)
                gameTurn++;//������ ��ü �� ����
                Debug.Log("NextTurn " + gameTurn);
            }
            else if (sPlayerIndex.Value == -1)
            {
                sPlayerIndex.Value = 0;//0���� �ʱ�ȭ(player1����)
                gameTurn++;//������ ��ü �� ����
                Debug.Log("NextTurn " + gameTurn);
            }
            else
            {
                sPlayerIndex.Value = 1;//sPlayerIndex�� ����� �������� �� ��ȯ ���� 
            }
            GameManager.Instance.turnTime = 20.0f;       
    }

    IEnumerator StartGameCoroutine()//���� ���۽�
    {
        Debug.Log("StartC");
        yield return new WaitForSeconds(1.0f);//1���� �����̸� �ְ� 
        gState.Value = GameState.ActionEnd;// 0
        gameStart = true;
        Debug.Log("GameStart");
    }

    [PunRPC]
    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }
}
