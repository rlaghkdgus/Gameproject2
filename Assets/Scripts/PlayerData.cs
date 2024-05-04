using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



public class PlayerData : MonoBehaviour
{
    public Data<PlayerState> pState = new Data<PlayerState>();//플레이어 상태 변수
    public int pIndex;//플레이어의 인덱스
    public List<CardInfo> playerCards = new List<CardInfo>();
    public List<int> strikeCards = new List<int>();
    public int playerScore;
    public int strikeScore = 0;
    public int ComboCount = 0;
    public Transform cardLeftTransform;
    public Transform cardRightTransform;
    public Transform playerPosition;
    public GameObject playerObject;
    public TMP_Text playerScoreText;
    public bool tripleCheck = false;
    public bool stairCheck = false;
    public bool doubleCheck = false;
    public List<int> Guardnums = new List<int>();
    
    private void Awake()
    {
        pState.onChange += Draw;//메소드를 추가(onChange를 의도하는 순서대로 배치)
        pState.onChange += AISelect;
        pState.onChange += SelectCardFin;
        pState.onChange += SetIdle;
        TurnSys.Instance.sPlayerIndex.onChange += SetTurn;
    }
    private void AISelect(PlayerState _pState)
    {
        if (_pState == PlayerState.Select)
        {
            if (pIndex == 1 && TurnSys.Instance.sPlayerIndex.Value == 1)
            {
                AICheck();
            }
        }
    }
    IEnumerator PlayerSystem()
    {
        //카드주기를 여기에 넣거나
        TurnSys.Instance.gState.Value = GameState.WaitAction;//게임 상태를 WaitAction으로 바꾼 후
        yield return new WaitForSeconds(1.0f);
        CardManager.Instance.SortCards(playerCards);
        yield return new WaitForSeconds(0.5f);
        pState.Value = PlayerState.Select;
        //yield return new WaitForSeconds(0.5f); 혹시몰름
        /*턴에 들어가야 하는 것
         * 가상으로 20초를 세고, 20초가 지나가면 턴이 넘어가거나 혹은 패를 냈을때 턴이 넘어가도록
         * 20초를 세는동안 패를 내는 동작이 작동하도록
         * 검사 함수를 따로 만들어 조건이 맞을경우 UI버튼을 SetActive로 만들기까지?
         * 남은시간 표기까지?
         */
    }
    private void Draw(PlayerState _pState)//Player상태가 StartDraw인지 체크하고 PlayerSystem 시작 
    {
        if (_pState == PlayerState.StartDraw)
        {
            strikeScore = 0;

            for (int i = playerCards.Count; i < 8; i++)

            {
                if (playerCards.Count == 7)
                    break;
                CardManager.Instance.AddCard(playerCards, playerPosition, playerObject);
            }
            CardManager.Instance.SortCards(playerCards);
            CardManager.Instance.ArrangeCardsBetweenMyCards(playerCards, cardLeftTransform, cardRightTransform, 1.7f);
            CardManager.Instance.AddCard(playerCards, playerPosition, playerObject);
            StartCoroutine(PlayerSystem());
        }
    }
    private void SelectCardFin(PlayerState _pState)//카드 선택이 끝났을때
    {
        if (_pState == PlayerState.SelectFin)
        {

            ComboCount = 0;
            GameManager.Instance.turnFinishButton.SetActive(false);
            CardManager.Instance.SortCards(playerCards);
            CardManager.Instance.ArrangeCardsBetweenMyCards(playerCards, cardLeftTransform, cardRightTransform, 1.7f);//건필군이 카드 간격을 조절하고싶을때 맨마지막상수f값을건들면됨
            pState.Value = PlayerState.End;
            StartCoroutine(CardClearDelay());

        }
    }
    public void ReadyStrike()//스트라이크 버튼 작용
    {
        if (TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            if (GameManager.Instance.S_State.Value == StrikeState.ReadyStrike)
            {
                Debug.Log("Check");
                Check();
            }
            else if (GameManager.Instance.S_State.Value == StrikeState.Idle)
            {
                Debug.Log("SetStrike");
                //문구 호출
                GameManager.Instance.S_State.Value = StrikeState.ReadyStrike;
            }

        }

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
            pState.Value = PlayerState.StartDraw;//StartDraw상태로 변경

            strikeCards.Clear();
            Guardnums.Clear();

            Debug.Log("DrawTurn " + pIndex);//여기같은 경우 이미지띄우기연출같은거 넣으면 될듯
        }
    }
    private void SetIdle(PlayerState _pState)//player의 상태가 end일 경우에
    {
        if (_pState == PlayerState.End)
        {
            _pState = PlayerState.Idle;// 아무것도 안하는 동작으로 변경
        }
        //이 순간에도 상대가 패를 냈을때 그패를 가져와 조합완성이 가능하다면 작동하도록 검사추가
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
            return;
        }
        int temp = strikeCards[i];
        if (strikeCards.Count == 2)
        {
            if (temp == strikeCards[i + 1])

            {
                Debug.Log("HEAD");
                headCheck++;
                doubleCheck = true;
                strikeScore = 200;
            }
        }
        if (strikeCards.Count == 3)
        {

            if (temp == strikeCards[i + 1])         // HEAD CHECK
            {
                if (temp == strikeCards[i + 2])//같은수 3개가 몸통일경우
                { 
                    bodyCheck++;
                    tripleCheck = true;

                    Debug.Log("HEAD => BODY");
                    strikeScore = 300;
                }
            }
            else                             // BODY CHECK
            {
                if (temp + 1 == strikeCards[i + 1] && temp + 2 == strikeCards[i + 2])//연속수3개가 몸통일경우
                {
                    stairCheck = true;
                    bodyCheck++;
                    strikeScore = 300;
                    Debug.Log("BODY");
                }

            }
        }

        if (bodyCheck == 0 && headCheck == 0)
        {
            for (int a = 0; a < strikeCards.Count; a++)
            {
                strikeCards.Clear();
            }
            for (int a = 0; a < playerCards.Count; a++)

            {
                playerCards[a].myCardState = false;
            }
            //문구 및 선택취소
        }

        if (bodyCheck == 1 || headCheck == 1)
        {
            for (int a = playerCards.Count - 1; a >= 0; a--)
            {
                if (playerCards[a].myCardState == true)
                {
                    if (GameManager.Instance.G_State.Value != GuardState.GuardSelect)
                        Guardnums.Add(playerCards[a].cardnum);
                    StartCoroutine(RemoveCards(playerCards[a].gameObject, a));

                }
            }
            ComboCount++;
            strikeCards.Clear();
            pState.Value = PlayerState.delay;
            StartCoroutine(GuardCheckdelay()); 
        }
    }
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
            Debug.Log("CheckError");
            return;
        }
        int temp = Guardnums[i];
        if (Guardnums.Count == 2)
        {
            if (temp == Guardnums[i + 1])
            {
                Debug.Log("HEAD");
                headCheck++;
                doubleCheck = true;
            }
        }
        if (Guardnums.Count == 3)
        {
            if (temp == Guardnums[i + 1])         
            {
                if (temp == Guardnums[i + 1]&&Guardnums[i+1]== Guardnums[i+2])//같은수 3개가 몸통일경우
                {
                    headCheck--;
                    doubleCheck = false;
                    bodyCheck++;
                    tripleCheck = true;
                    Debug.Log("HEAD => BODY");
                }
            }
            else                             
            {
                if (temp + 1 == Guardnums[i + 1] && temp + 2 == Guardnums[i + 2])//연속수3개가 몸통일경우
                {
                    stairCheck = true;
                    bodyCheck++;
                    Debug.Log("BODY");
                }

            }
        }
        if (bodyCheck == 0 && headCheck == 0)
        {
            for (int a = 0; a < strikeCards.Count; a++)
            {
                Guardnums.Clear();
            }
            for (int a = 0; a < playerCards.Count; a++)
            {
                playerCards[a].myCardState = false;
            }
            //문구 및 선택취소
        }
        if (bodyCheck == 1 || headCheck == 1)
        {
            for (int a = playerCards.Count - 1; a >= 0; a--)
            {
                if (playerCards[a].myCardState == true)
                {
                    if (GameManager.Instance.G_State.Value != GuardState.GuardSelect)
                        Guardnums.Add(playerCards[a].cardnum);
                  StartCoroutine(RemoveCards(playerCards[a].gameObject,a));     
                 
                }
            }

            strikeCards.Clear();
        }
    }
    IEnumerator RemoveCards(GameObject obj, int a)
    {
        yield return new WaitForSeconds(0.03f);
        Destroy(obj);
        yield return new WaitForSeconds(0.05f);
        playerCards.RemoveAt(a);
        yield return new WaitForSeconds(0.05f);
    }
    IEnumerator AIDelayCheck()
    {
        Guardnums.Clear();
        yield return new WaitForSeconds(0.05f);
        int headCheck = 0;
        int bodyCheck = 0;
        doubleCheck = false;
        tripleCheck = false;
        stairCheck = false;
        for (var i = 0; i < playerCards.Count - 1; i++)
        {
            yield return new WaitForSeconds(0.1f);


            int temp = playerCards[i].cardnum;
            if (temp == playerCards[i + 1].cardnum)         // HEAD CHECK
            {
                if (headCheck == 1)
                    break;
                if (bodyCheck == 1)
                    break;
                if (i == playerCards.Count - 2)
                {
                    Debug.Log("HEAD");
                    doubleCheck = true;
                    headCheck++;
                    CardStateChange(i, 2);
                    strikeScore = 200;
                    break;
                }
                if (temp == playerCards[i + 2].cardnum)//같은수 3개가 몸통일경우
                {

                    headCheck--;
                    doubleCheck = false;
                    bodyCheck++;
                    tripleCheck = true;
                    CardStateChange(i, 3);
                    Debug.Log("HEAD => BODY");
                    strikeScore = 300;
                    break;

                }
                else
                {
                    Debug.Log("HEAD");
                    doubleCheck = true;
                    headCheck++;
                    CardStateChange(i, 2);
                    strikeScore = 200;
                    break;
                }

            }
            else                             // BODY CHECK
            {
                if (i  == playerCards.Count - 2)
                    break;
                if (headCheck == 1)
                    break;
                if (bodyCheck == 1)
                    break;
                if (temp + 1 == playerCards[i + 1].cardnum && temp + 2 == playerCards[i + 2].cardnum)//연속수3개가 몸통일경우
                {
                    stairCheck = true;
                    CardStateChange(i, 3);

                    bodyCheck++;
                    strikeScore = 300;
                    Debug.Log("BODY");
                    break;
                }

            }

        }
        if (headCheck == 1 || bodyCheck == 1)
        {
            for (int a = playerCards.Count - 1; a >= 0; a--)
            {
                if (playerCards[a].myCardState == true)
                {
                    if (GameManager.Instance.G_State.Value != GuardState.GuardSelect)
                        Guardnums.Add(playerCards[a].cardnum);
                   StartCoroutine(RemoveCards(playerCards[a].gameObject, a));

                }
            }
            ComboCount++;
            strikeCards.Clear();
            pState.Value = PlayerState.delay;
            StartCoroutine(GuardCheckdelay());
        }
        if (bodyCheck == 0 && headCheck == 0)
        {
            pState.Value = PlayerState.SelectFin;
        }

    }

    IEnumerator GuardCheckdelay()
    {
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.G_State.Value = GuardState.DoCheckGuard;
    }

    public void AICheck()
    {
        StartCoroutine(AIDelayCheck());
    }
    private void CardStateChange(int i, int Count)
    {
       for(int a = 0; a < Count; a++)
        {
            strikeCards.Add(playerCards[i+a].cardnum);      
            playerCards[i+a].myCardState = true;
        }
    }

}
