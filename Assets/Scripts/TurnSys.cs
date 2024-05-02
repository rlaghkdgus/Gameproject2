using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TurnSys : Singleton<TurnSys>
{

    public Data<int> sPlayerIndex = new Data<int>();
    //�÷��̾� �ε����� ������ �ý��� �ε����� ���� ���� �����ϱ����� �������Ҵ�
    //ex. player 1 , 2, 3 �� �ε����� �� 0,1,2 �� ���
    //splayerIndex�� ���� 0�϶� player1�� ��
    public Data<GameState> gState = new Data<GameState>();//���� ���� ����� ����

    public int gameTurn = 0;//�÷��̾ �ѹ��� �������� +1�߰�, ���� ī�� �ε����� ���� ����
    

   

    private void Awake()
    {
        sPlayerIndex.Value = -1;//player1���ͽ���, 
        gState.Value = GameState.Idle;//���� ���¸� �ƹ��͵����Ҷ��� ����
        gState.onChange += NextPlayer;
    }
    private void Start()
    {
        StartCoroutine(StartGameCo());
    }

 

    private void NextPlayer(GameState _gState)//���� test�� ��ư�� �� �޼ҵ�,
    {
        //gState��Value�� PlayerData�� PlayerSystem�� ����
        if (_gState == GameState.ActionEnd)//PlayerSystem���� gState.Value�� ActionEnd���°� �� ���
        {
            StartCoroutine(TurnStartCo());
        }
    }
   IEnumerator TurnStartCo()
    {
        yield return new WaitForSeconds(2.0f);
        if (sPlayerIndex.Value + 1 >= 2)//sPlayer�� �ε����� 2�̻��ϰ��
        {

            sPlayerIndex.Value = 0;//0���� �ʱ�ȭ(player1����)
            gameTurn++;//������ ��ü �� ����
            GameManager.Instance.turnTime = 10.0f;
            Debug.Log("NextTurn " + gameTurn);
        }
        else if (sPlayerIndex.Value == -1)
        {

            sPlayerIndex.Value = 0;//0���� �ʱ�ȭ(player1����)
            gameTurn++;//������ ��ü �� ����
            GameManager.Instance.turnTime = 10.0f;
            Debug.Log("NextTurn " + gameTurn);
        }
        else
        {

            sPlayerIndex.Value++;//sPlayerIndex�� ����� �������� �� ��ȯ ����
            GameManager.Instance.turnTime = 2.0f;
        }
    }
    IEnumerator StartGameCo()//���� ���۽�
    {
        yield return new WaitForSeconds(1.0f);//1���� �����̸� �ְ�
        gState.Value = GameState.ActionEnd;//
        Debug.Log("GameStart");
    }
}
