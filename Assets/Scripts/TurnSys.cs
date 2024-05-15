using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class TurnSys : MonoBehaviourPun
{

    public Data<int> sPlayerIndex = new Data<int>();
    //�÷��̾� �ε����� ������ �ý��� �ε����� ���� ���� �����ϱ����� �������Ҵ�
    //ex. player 1 , 2, 3 �� �ε����� �� 0,1,2 �� ���
    //splayerIndex�� ���� 0�϶� player1�� ��
    public Data<GameState> gState = new Data<GameState>();//���� ���� ����� ����

    public int gameTurn = 0;//�÷��̾ �ѹ��� �������� +1�߰�, ���� ī�� �ε����� ���� ����
    public bool gameStart = false;

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
        // if (PhotonNetwork.IsMasterClient) // ������ ��쿡�� ��� �÷��̾��� �ε带 Ȯ��
        //  {
        //     StartCoroutine(CheckAllPlayersLoaded());
        // }
        if(PhotonNetwork.IsMasterClient)
        PV.RPC("StartGame", RpcTarget.All);
    }



    private void NextPlayer(GameState _gState)//���� test�� ��ư�� �� �޼ҵ�,
    {
        //gState��Value�� PlayerData�� PlayerSystem�� ����
        if (_gState == GameState.ActionEnd)//PlayerSystem���� gState.Value�� ActionEnd���°� �� ���
        {
            Debug.Log("NextPlayer");
            if(PhotonNetwork.IsMasterClient)
            PV.RPC("RPCActionEnd", RpcTarget.All);
        }
    }
    [PunRPC]
    public void RPCActionEnd()
    {
        Debug.Log("ActionEnd");
        if(PhotonNetwork.IsMasterClient)    StartCoroutine(TurnStartCo());
        if(sPlayerIndex.Value == 0)
        {
            GameManager.Instance.player[0].characterImg.sprite = GameManager.Instance.player[0].characterUI[3];
            GameManager.Instance.player[1].characterImg.sprite = GameManager.Instance.player[1].characterUI[3];
        }
        else if(sPlayerIndex.Value == 1)
        {
            GameManager.Instance.player[0].characterImg.sprite = GameManager.Instance.player[0].characterUI[3];
            GameManager.Instance.player[1].characterImg.sprite = GameManager.Instance.player[1].characterUI[3];
        }
    }
   IEnumerator TurnStartCo()
    {
        Debug.Log("TurnStart");
        yield return new WaitForSeconds(2.0f);
        if (sPlayerIndex.Value + 1 >= 2)//sPlayer�� �ε����� 2�̻��ϰ��
        {
            sPlayerIndex.Value = 0;//0���� �ʱ�ȭ(player1����)
            gameTurn++;//������ ��ü �� ����
            GameManager.Instance.turnTime = 20.0f;
            Debug.Log("NextTurn " + gameTurn);
        }
        else if (sPlayerIndex.Value == -1)
        {
            sPlayerIndex.Value = 0;//0���� �ʱ�ȭ(player1����)
            gameTurn++;//������ ��ü �� ����
            GameManager.Instance.turnTime = 20.0f;
            Debug.Log("NextTurn " + gameTurn);
        }
        else
        {
            sPlayerIndex.Value++;//sPlayerIndex�� ����� �������� �� ��ȯ ����
            GameManager.Instance.turnTime = 2.0f;
        }
        PV.RPC("RPCsPIndex", RpcTarget.Others, sPlayerIndex.Value);
    }
    [PunRPC]
    public void RPCsPIndex(int _sPlayerIndex)
    {
        sPlayerIndex.Value = _sPlayerIndex;
        if(_sPlayerIndex == 0)
            gameTurn++;
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
