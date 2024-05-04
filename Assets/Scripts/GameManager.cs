using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public List<PlayerData> player = new List<PlayerData>();//모든 플레이어 데이터 리스트(자신포함)
    public Data<GuardState> G_State = new Data<GuardState>();
    public Data<StrikeState> S_State = new Data<StrikeState>();
    public int myIndex;//자신의 인덱스 저장용
    public GameObject Judge;//버튼 불러오기
    public GameObject Judge1;//버튼 불러오기
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

    private void ViewButton(int _sIndex)//턴마다 나의 버튼이 보이게 하도록
    {
        Judge.SetActive(_sIndex == 0);//내 인덱스가 시스템인덱스와 동일할때 true
    }

    private void ViewButton1(int _sIndex)//턴마다 나의 버튼이 보이게 하도록
    {
        Judge1.SetActive(_sIndex == 1);//내 인덱스가 시스템인덱스와 동일할때 true

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
        
        /*sPlayerIndex에 구독을 하는 이유 : 
         * 그냥 플레이어 인덱스를 넣어버리면 다른 사람차례에 버튼이 호출 되지않을 것.
         * 시스템플레이어 인덱스에서 호출을 해야 다른사람에게도 호출이됨
         * 시스템인덱스와 내인덱스가 동일할때라는 조건에 따라 각 차례에 버튼 호출
         * 활용방안으로 메소드안에 if(나의 인덱스 == 시스템인덱스){ 턴에 작용하고 싶은 행동 }
        */
    }

    void Update()
    {
       /* if(player[0].pState.Value == PlayerState.Select)//시간체크용
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
        if (player[1].pState.Value == PlayerState.Select)//시간체크용
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
        for (int i = 0; i < player.Count; i++)//플레이어의 수따라
        {
            player[i].pState.Value = PlayerState.Idle;//플레이어 데이터를 초기화
            player[i].pIndex = i;//각플레이어에게 턴 인덱스를 배정
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
        yield return new WaitForSeconds(0.2f); // 0.2초 지연
        if (TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            player[0].playerCards.Remove(player[0].playerCards[7]);//내카드리스트에 삭제
            player[0].pState.Value = PlayerState.SelectFin;
        }
        else if (TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            player[1].playerCards.Remove(player[1].playerCards[7]);//내카드리스트에 삭제
           player[1].pState.Value = PlayerState.SelectFin;
        }
        Destroy(obj);
        // 오브젝트 파괴
    }
}
