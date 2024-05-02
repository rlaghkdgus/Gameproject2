using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



public class PlayerData : MonoBehaviour
{
    public Data<PlayerState> pState = new Data<PlayerState>();//�÷��̾� ���� ����
    public int pIndex;//�÷��̾��� �ε���
    public List<CardInfo> playerCards = new List<CardInfo>();
    public List<CardInfo> strikeCards = new List<CardInfo>();
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
        pState.onChange += Draw;//�޼ҵ带 �߰�(onChange�� �ǵ��ϴ� ������� ��ġ)
        pState.onChange += AISelect;
        pState.onChange += SelectCardFin;
        pState.onChange += SetIdle;
        TurnSys.Instance.sPlayerIndex.onChange += SetTurn;
    }
    private void AISelect(PlayerState _pState)
    {
        if (_pState == PlayerState.Select)
        {
            if(pIndex == 1 &&  TurnSys.Instance.sPlayerIndex.Value == 1)
            {
                AICheck();
            }
        }
    }
    IEnumerator PlayerSystem()
    {
        //ī���ֱ⸦ ���⿡ �ְų�
        TurnSys.Instance.gState.Value = GameState.WaitAction;//���� ���¸� WaitAction���� �ٲ� ��
        yield return new WaitForSeconds(1.0f);
        CardManager.Instance.SortCards(playerCards);
        yield return new WaitForSeconds(0.5f);
        pState.Value = PlayerState.Select;
        yield return new WaitForSeconds(0.5f);
        /*�Ͽ� ���� �ϴ� ��
         * �������� 20�ʸ� ����, 20�ʰ� �������� ���� �Ѿ�ų� Ȥ�� �и� ������ ���� �Ѿ����
         * 20�ʸ� ���µ��� �и� ���� ������ �۵��ϵ���
         * �˻� �Լ��� ���� ����� ������ ������� UI��ư�� SetActive�� ��������?
         * �����ð� ǥ�����?
         */
    }
    private void Draw(PlayerState _pState)//Player���°� StartDraw���� üũ�ϰ� PlayerSystem ���� 
    {
        if (_pState == PlayerState.StartDraw)
        {
            strikeScore = 0;
            for(int i = playerCards.Count; i < 8 ; i++)
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
    private void SelectCardFin(PlayerState _pState)//ī�� ������ ��������
    {
        if (_pState == PlayerState.SelectFin)
        {
            CardManager.Instance.SortCards(playerCards);
            CardManager.Instance.ArrangeCardsBetweenMyCards(playerCards, cardLeftTransform, cardRightTransform, 1.7f);//���ʱ��� ī�� ������ �����ϰ������ �Ǹ��������f�����ǵ���
            pState.Value = PlayerState.End;
            GameManager.Instance.S_State.Value = StrikeState.Idle;
            TurnSys.Instance.gState.Value = GameState.ActionEnd;//���ӻ��¸� ActionEnd�θ���
        }
    }
    public void ReadyStrike()//��Ʈ����ũ ��ư �ۿ�
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
                //���� ȣ��
                GameManager.Instance.S_State.Value = StrikeState.ReadyStrike;
            }
           
        }
        
    }
    
    private void SetTurn(int _sIndex)
    {
        if (pIndex == _sIndex)//�ý��� �ε����� �÷��̾� �ε����� ������� ���� �����ϰ� �ϱ����� ����
        {
            pState.Value = PlayerState.StartDraw;//StartDraw���·� ����
            Debug.Log("DrawTurn " + pIndex);//���ⰰ�� ��� �̹������⿬�ⰰ���� ������ �ɵ�
        }
    }
    private void SetIdle(PlayerState _pState)//player�� ���°� end�� ��쿡
    {
        if (_pState == PlayerState.End)
        {
            _pState = PlayerState.Idle;// �ƹ��͵� ���ϴ� �������� ����
        }
        //�� �������� ��밡 �и� ������ ���и� ������ ���տϼ��� �����ϴٸ� �۵��ϵ��� �˻��߰�
    }

    public void Check()
    {
        CardManager.Instance.SortCards(strikeCards);
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
        int temp = strikeCards[i].cardnum;
        if(strikeCards.Count == 2)
        {
            if(temp == strikeCards[i+1].cardnum)
            {
                Debug.Log("HEAD");
                headCheck++;
                doubleCheck = true;
                i++;
                strikeScore = 200;
            }
        }
        if (strikeCards.Count == 3)
        {
            if (temp == strikeCards[i + 1].cardnum)         // HEAD CHECK
            {
                if (temp == strikeCards[i + 2].cardnum)//������ 3���� �����ϰ��
                {
                    headCheck--;
                    doubleCheck = false;
                    bodyCheck++;
                    tripleCheck = true;
                    i++;
                    i++;
                    Debug.Log("HEAD => BODY");
                    strikeScore = 300;
                }
            }
            else                             // BODY CHECK
            {
                if (temp + 1 == strikeCards[i + 1].cardnum && temp + 2 == strikeCards[i + 2].cardnum)//���Ӽ�3���� �����ϰ��
                {
                    i++;
                    i++;
                    stairCheck = true;
                    bodyCheck++;
                    strikeScore = 300;
                    Debug.Log("BODY");
                }

            }
        }
        if(bodyCheck == 0 && headCheck == 0)
        {
            for(int a = 0; a < strikeCards.Count; a++)
            {
                strikeCards.Clear();
            }
            for(int a = 0; a < playerCards.Count; a++)
            {
                playerCards[a].myCardState = false;
            }
            //���� �� �������
        }
        if(bodyCheck == 1 || headCheck == 1)
        {
            StartCoroutine(RemoveCards());
            ComboCount++;
            pState.Value = PlayerState.delay;
            GameManager.Instance.G_State.Value = GuardState.DoCheckGuard;
        }
    }
    IEnumerator RemoveCards()
    {
        for (int a = playerCards.Count - 1; a >= 0; a--)
        {
            if (playerCards[a].myCardState == true)
            {
                Guardnums.Add(playerCards[a].cardnum);
                yield return new WaitForSeconds(0.05f);
                Destroy(playerCards[a].gameObject);
                yield return new WaitForSeconds(0.15f);
                playerCards.Remove(playerCards[a]);
                yield return new WaitForSeconds(0.15f);                
            }
        }
    }

    public void AICheck()
    {
        int headCheck = 0;
        int bodyCheck = 0;
      
        for (var i = 0; i < playerCards.Count - 1; i++)
        {
            if (i == playerCards.Count - 1)
            {
                break;
            }

            int temp = playerCards[i].cardnum;
            if (temp == playerCards[i + 1].cardnum)         // HEAD CHECK
            {
                if (headCheck == 1)
                    break;
                if (i + 1 >= playerCards.Count - 1)
                {

                    if(playerCards[i].cardnum == playerCards[i+1].cardnum)
                    {
                        
                        headCheck++;
                        doubleCheck = true;
                        strikeCards.Add(playerCards[i]);
                        strikeCards.Add(playerCards[i + 1]);
                        playerCards[i].myCardState = true;
                        playerCards[i + 1].myCardState = true;
                       
                    }
                }
                    if (temp == playerCards[i + 2].cardnum)//������ 3���� �����ϰ��
                    {
                    headCheck--;
                    doubleCheck = false;
                    bodyCheck++;
                    tripleCheck = true;
                    strikeCards.Add(playerCards[i]);
                    strikeCards.Add(playerCards[i + 1]);
                    strikeCards.Add(playerCards[i + 2]);
                    playerCards[i].myCardState = true;
                    playerCards[i + 1].myCardState = true;
                    playerCards[i+2].myCardState = true;
                    i++;
                        i++;
                        Debug.Log("HEAD => BODY");
                        strikeScore = 300;
                    break;

                    }
                    else
                    {
                    Debug.Log("HEAD");
                    doubleCheck = true;
                    headCheck++;
                    strikeCards.Add(playerCards[i]);
                    strikeCards.Add(playerCards[i + 1]);
                    playerCards[i].myCardState = true;
                    playerCards[i + 1].myCardState = true;
                    strikeScore = 200;
                        i++;
                    }
                
            }
            else                             // BODY CHECK
            {
                if (headCheck == 1)
                    break;
                if (i + 2 >= playerCards.Count -1 )
                {
                    if (playerCards[i].cardnum == playerCards[i+1].cardnum - 1 && playerCards[i+1].cardnum == playerCards[i+2].cardnum - 1)
                    {
                        stairCheck = true;
                        bodyCheck++;
                        strikeCards.Add(playerCards[i]);
                        strikeCards.Add(playerCards[i + 1]);
                        strikeCards.Add(playerCards[i + 2]);
                        playerCards[i].myCardState = true;
                        playerCards[i + 1].myCardState = true;
                        playerCards[i + 2].myCardState = true;
                        strikeScore = 300;
                        break;
                    }
                }
                
                if (temp + 1 == playerCards[i + 1].cardnum && temp + 2 == playerCards[i + 2].cardnum)//���Ӽ�3���� �����ϰ��
                {
                    stairCheck = true;
                    strikeCards.Add(playerCards[i]);
                    strikeCards.Add(playerCards[i + 1]);
                    strikeCards.Add(playerCards[i + 2]);
                    playerCards[i].myCardState = true;
                    playerCards[i + 1].myCardState = true;
                    playerCards[i + 2].myCardState = true;
                    i++;
                    i++;
                    bodyCheck++;
                    strikeScore = 300;
                    Debug.Log("BODY");
                    break;
                }

            }

        }
      if(headCheck == 1 || bodyCheck == 1)
        {
            StartCoroutine(RemoveCards());
            ComboCount++;
            pState.Value = PlayerState.delay;
            GameManager.Instance.G_State.Value = GuardState.DoCheckGuard;
        }
      if(bodyCheck == 0 && headCheck == 0)
        {
            
        }

    }
}
