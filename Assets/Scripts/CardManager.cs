using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CardManager : Singleton<CardManager>
{
    

    [SerializeField] CardSO cardSO;
    [SerializeField] GameObject cardPrefab;
   
   
    public List<CardInfo> BonusCards;
    [SerializeField] GameObject bonusCardParent;
    [SerializeField] Transform cardSpawnPoint;
    public TMP_Text BonusText; 

    List<Card> cardBuffer;//ī�带 ���� ����

    

    enum ECardState {Nothing, CanMouseOver, CanMouseDrag }
    public Card PopCard()//ī�� ������
    {
        Card card = cardBuffer[0];
        cardBuffer.RemoveAt(0);
        return card;
    }
    void SetupCardBuffer()//ī���� ����
    {
        cardBuffer = new List<Card>();
        for( int i = 0; i < cardSO.cards.Length; i++)//ī�带 �ְ�
        {
            Card card = cardSO.cards[i];
            for (int j = 0; j < card.percent; j++)
                cardBuffer.Add(card);
        }
        for(int i = 0; i< cardBuffer.Count; i ++)//�����ϰ� ����
        {
            int rand = Random.Range(i, cardBuffer.Count);
            Card temp = cardBuffer[i];
            cardBuffer[i] = cardBuffer[rand]; 
            cardBuffer[rand] = temp;
        }
    }
    public void SortCards(List<CardInfo> playerCards)
    {
        playerCards.Sort((card1, card2) => card1.cardnum.CompareTo(card2.cardnum));
    }


    private void Awake()
    {
        SetupCardBuffer();
    }


    private void Start()
    {
        AddCard(BonusCards, cardSpawnPoint, bonusCardParent);
        BonusText.text = "Bonus :" + BonusCards[0].cardnum.ToString();
    }

    // Update is called once per frame
   

    

  
    public void AddCard(List<CardInfo> _pCard, Transform playerPosition, GameObject playerObject)//ī���߰�
    {
            var cardObject = Instantiate(cardPrefab, playerPosition.position, Utills.QI);
            cardObject.transform.parent = playerObject.transform;
            var card = cardObject.GetComponent<CardInfo>();
            card.Setup(PopCard());
            _pCard.Add(card);
    }
    public void ReturnCard(List<CardInfo> _pCard, Transform playerPosition, GameObject playerObject)//ī���߰�
    {
        var cardObject = Instantiate(cardPrefab, playerPosition.position, Utills.QI);
        cardObject.transform.parent = playerObject.transform;
        var card = cardObject.GetComponent<CardInfo>();
        if (TurnSys.Instance.sPlayerIndex.Value == 0)
        {
            card.Setup(GameManager.Instance.player[0].strikeCards[0].card);
        }
        if(TurnSys.Instance.sPlayerIndex.Value == 1)
        {
            card.Setup(GameManager.Instance.player[1].strikeCards[0].card);
        }
            _pCard.Add(card);
    }

    public void ArrangeCardsBetweenMyCards(List<CardInfo> cards, Transform leftTransform, Transform rightTransform, float gap)
    {
        int cardCount = cards.Count;
        float totalWidth = gap * (cardCount - 1); // ī����� ������ �� ���� ���� ���
        float startX = (leftTransform.position.x + rightTransform.position.x - totalWidth) / 2f; // ù ��° ī���� x ��ġ ���

        // ī�带 ������ �������� ��ġ
        for (int i = 0; i < cardCount; i++)
        {
            Vector3 cardPosition = new Vector3(startX + i * gap, leftTransform.position.y, leftTransform.position.z);
            cards[i].transform.position = cardPosition;
        }
    }



}