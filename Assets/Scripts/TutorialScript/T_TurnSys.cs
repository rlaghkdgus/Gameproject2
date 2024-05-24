using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class T_TurnSys : Singleton<T_TurnSys>
{
    public Data<int> sPlayerIndex = new Data<int>();
    //�÷��̾� �ε����� ������ �ý��� �ε����� ���� ���� �����ϱ����� �������Ҵ�
    //ex. player 1 , 2, 3 �� �ε����� �� 0,1,2 �� ���
    //splayerIndex�� ���� 0�϶� player1�� ��
    public Data<GameState> gState = new Data<GameState>();//���� ���� ����� ����

    public int gameTurn = 0;//�÷��̾ �ѹ��� �������� +1�߰�, ���� ī�� �ε����� ���� ����
    public bool gameStart = false;



    private void Awake()
    {
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
        StartGame();
    }



    private void NextPlayer(GameState _gState)//���� test�� ��ư�� �� �޼ҵ�,
    {
        //gState��Value�� PlayerData�� PlayerSystem�� ����
        if (_gState == GameState.ActionEnd)//PlayerSystem���� gState.Value�� ActionEnd���°� �� ���
        {
            StartCoroutine(TurnStartCo());
            T_GameManager.Instance.player[0].characterImg.sprite = T_GameManager.Instance.player[0].characterUI[3];
            T_GameManager.Instance.player[1].characterImg.sprite = T_GameManager.Instance.player[1].characterUI[3];
        }
    }
    IEnumerator TurnStartCo()
    {
        yield return new WaitForSeconds(2.0f);
        if (sPlayerIndex.Value + 1 >= 2)//sPlayer�� �ε����� 2�̻��ϰ��
        {

            sPlayerIndex.Value = 0;//0���� �ʱ�ȭ(player1����)
            gameTurn++;//������ ��ü �� ����
            T_GameManager.Instance.turnTime = 20.0f;
            Debug.Log("NextTurn " + gameTurn);
        }
        else if (sPlayerIndex.Value == -1)
        {
            sPlayerIndex.Value = 0;//0���� �ʱ�ȭ(player1����)
            gameTurn++;//������ ��ü �� ����
            T_GameManager.Instance.turnTime = 20.0f;
            Debug.Log("NextTurn " + gameTurn);
        }
        else
        {
            sPlayerIndex.Value++;//sPlayerIndex�� ����� �������� �� ��ȯ ����
            T_GameManager.Instance.turnTime = 2.0f;
        }
    }
    IEnumerator StartGameCoroutine()//���� ���۽�
    {
        Debug.Log("StartC");
        yield return new WaitForSeconds(1.0f);//1���� �����̸� �ְ� 
        gState.Value = GameState.ActionEnd;// 0
        gameStart = true;
        Debug.Log("GameStart");
    }

    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }
}
