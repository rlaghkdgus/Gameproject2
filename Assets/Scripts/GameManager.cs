using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public List<PlayerData> player = new List<PlayerData>();//��� �÷��̾� ������ ����Ʈ(�ڽ�����)
    public Data<GuardState> G_State = new Data<GuardState>();
    public Data<StrikeState> S_State = new Data<StrikeState>();
    public int myIndex;//�ڽ��� �ε��� �����
    public GameObject Judge;//��ư �ҷ�����
    public GameObject Judge1;//��ư �ҷ�����
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
    public float turnTime;
    public TMP_Text timerText;
    public TMP_Text strikeScoreText;
    public TMP_Text FianlScoretext;
    public TMP_Text FianlScoretext1;
    public bool AnotherStrike = false;

    private void ViewButton(int _sIndex)//�ϸ��� ���� ��ư�� ���̰� �ϵ���
    {
        Judge.SetActive(_sIndex == 0);//�� �ε����� �ý����ε����� �����Ҷ� true
    }

    private void ViewButton1(int _sIndex)//�ϸ��� ���� ��ư�� ���̰� �ϵ���
    {
        Judge1.SetActive(_sIndex == 1);//�� �ε����� �ý����ε����� �����Ҷ� true

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
    public void TestButton()
    {
        Debug.Log("Idle");
        S_State.Value = StrikeState.Idle;
    }
    private void CheckGuard(GuardState _gState)
    {
        if (_gState == GuardState.DoCheckGuard)
        {
          if (TurnSys.Instance.sPlayerIndex.Value == 0)
            {
                G_State.Value = GuardState.Idle;
                S_State.Value = StrikeState.SetStrike;
                player[0].pState.Value = PlayerState.Select;
                //CheckGuardButton.SetActive(myIndex == 1 && G_State.Value == GuardState.DoCheckGuard);
                //NoGuardButton.SetActive(myIndex == 1 && G_State.Value == GuardState.DoCheckGuard);
            }
          
            if (TurnSys.Instance.sPlayerIndex.Value == 1)
            {
                player[1].Guardnums.Sort();
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
        /*if (TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            S_State.Value = StrikeState.SetStrike;
            player[0].pState.Value = PlayerState.Select;
        }
        */
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
        player[1].pState.Value = PlayerState.Select;
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
            player[1].strikeCards.Sort();
            player[1].Check();
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
            player[0].GuardCheck();
            if (player[0].Guardnums[0] >= player[1].Guardnums[0])
            {
                if (player[0].doubleCheck == true && player[1].doubleCheck == true)
                {
                    player[1].strikeScore = 0;
                    player[1].pState.Value = PlayerState.SelectFin;
                    G_State.Value = GuardState.Idle;
                }
                else if (player[0].tripleCheck == true && player[1].tripleCheck == true)
                {
                    player[1].strikeScore = 0;
                    player[1].pState.Value = PlayerState.SelectFin;
                    G_State.Value = GuardState.Idle;
                }
                else if (player[0].stairCheck == true && player[1].stairCheck == true)
                {
                    player[1].strikeScore = 0;
                    player[1].pState.Value = PlayerState.SelectFin;
                    G_State.Value = GuardState.Idle;
                }
                player[1].Guardnums.Clear();
            }
            else
            {
                S_State.Value = StrikeState.SetStrike;
            }
        }
        DoGuardButton.SetActive(false);
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
                player[1].playerScore -= player[0].strikeScore;
                player[0].strikeScore = 0;
                player[1].playerScoreText.text = "" + player[1].playerScore;
            }
            else if (TurnSys.Instance.sPlayerIndex.Value == 1)
            {
                player[1].strikeCards.Clear();
                player[0].playerScore -= player[1].strikeScore;
                player[1].strikeScore = 0;
                player[0].playerScoreText.text = "" + player[0].playerScore;
            }
            S_State.Value = StrikeState.ReadyStrike;
        }
       
    }
    private void Awake()
    {
        TurnSys.Instance.sPlayerIndex.onChange += ViewButton;
        TurnSys.Instance.sPlayerIndex.onChange += ViewButton1;
        S_State.onChange += FinishButton;
        S_State.onChange += StrikeDamage;
        G_State.onChange += CheckGuard;
        G_State.onChange += SelectGuardCard;
        
        /*sPlayerIndex�� ������ �ϴ� ���� : 
         * �׳� �÷��̾� �ε����� �־������ �ٸ� ������ʿ� ��ư�� ȣ�� �������� ��.
         * �ý����÷��̾� �ε������� ȣ���� �ؾ� �ٸ�������Ե� ȣ���̵�
         * �ý����ε����� ���ε����� �����Ҷ���� ���ǿ� ���� �� ���ʿ� ��ư ȣ��
         * Ȱ�������� �޼ҵ�ȿ� if(���� �ε��� == �ý����ε���){ �Ͽ� �ۿ��ϰ� ���� �ൿ }
        */
    }

    void Update()
    {
       /* if(player[0].pState.Value == PlayerState.Select)//�ð�üũ��
        {
            turnTime -= Time.deltaTime;
            timerText.text = ""+Mathf.Round(turnTime);
            if(turnTime < 0 )
            {
                CardBuffering.SetActive(player[0].pState.Value == PlayerState.Select);
                player[0].playerCards[7].myCardState = true;
                CardManager.Instance.SortCards(player[1].playerCards);
                player[0].playerCards[7].text.text = "";
                player[0].playerCards[7].cardSprite.sprite = null;
               player[0].pState.Value = PlayerState.Idle;
                StartCoroutine(CheckDelay(player[0].playerCards[7].gameObject));   
            }      
        }
       */
        if (player[1].pState.Value == PlayerState.Select)//�ð�üũ��
        {
            turnTime -= Time.deltaTime;
            timerText.text = "" + Mathf.Round(turnTime);
            if (turnTime < 0)
            {
                CardBuffering.SetActive(player[1].pState.Value == PlayerState.Select);
                player[1].playerCards[7].myCardState = true;
                CardManager.Instance.SortCards(player[0].playerCards);
                player[1].pState.Value = PlayerState.Idle;
                StartCoroutine(CheckDelay(player[1].playerCards[7].gameObject));
            }
        }
    }
    void Start()
    {
        S_State.Value = StrikeState.Idle;
        G_State.Value = GuardState.Idle;
        player[0].playerScoreText.text = "" + player[0].playerScore;
        player[1].playerScoreText.text = "" + player[1].playerScore;
        for (int i = 0; i < player.Count; i++)//�÷��̾��� ������
        {
            player[i].pState.Value = PlayerState.Idle;//�÷��̾� �����͸� �ʱ�ȭ
            player[i].pIndex = i;//���÷��̾�� �� �ε����� ����
            for(int j = 0; j < 7; j++)
            {
                CardManager.Instance.AddCard(player[i].playerCards,player[i].playerPosition,player[i].playerObject);
            }
            CardManager.Instance.SortCards(player[i].playerCards);
            CardManager.Instance.ArrangeCardsBetweenMyCards(player[i].playerCards, player[i].cardLeftTransform, player[i].cardRightTransform, 1.7f);
        }
    }
    IEnumerator CheckDelay(GameObject obj)
    {
        yield return new WaitForSeconds(0.2f); // 0.2�� ����
        if (TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            player[0].playerCards.Remove(player[0].playerCards[7]);//��ī�帮��Ʈ�� ����
            player[0].pState.Value = PlayerState.SelectFin;
        }
        else if (TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            player[1].playerCards.Remove(player[1].playerCards[7]);//��ī�帮��Ʈ�� ����
           player[1].pState.Value = PlayerState.SelectFin;
        }
        Destroy(obj);
        // ������Ʈ �ı�
    }
}
