using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public class T_CardManager : Singleton<T_CardManager>
{
    [SerializeField] T_CardSO cardSO;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform cardSpawnPoint;

    List<T_Card> cardBuffer;//ī�带 ���� ����


    public T_Card PopCard()//ī�� ������
    {
        T_Card card = cardBuffer[0];
        cardBuffer.RemoveAt(0);
        return card;
    }


    void SetupCardBuffer()//ī���� ����
    {
        cardBuffer = new List<T_Card>();
        for (int i = 0; i < cardSO.card.Length; i++)//ī�带 �ְ�
        {
            T_Card card = cardSO.card[i];
            for (int j = 0; j < card.percent; j++)
                cardBuffer.Add(card);
        }
        for (int i = 0; i < cardBuffer.Count; i++)//�����ϰ� ����
        {
            int rand = Random.Range(i, cardBuffer.Count);
            T_Card temp = cardBuffer[i];
            cardBuffer[i] = cardBuffer[rand];
            cardBuffer[rand] = temp;
        }
    }
    public void SortCards(List<T_CardInfo> playerCards)
    {
        playerCards.Sort((card1, card2) => card1.cardnum.CompareTo(card2.cardnum)); ;
    }


    private void Awake()
    {

        //SingleTon.Instance.SetChar(SingleTon.Instance.GetCharID());
        SetupCardBuffer();
    }


    private void Start()
    {

    }

    // Update is called once per frame

    public void AddCard(List<T_CardInfo> _pCard, Transform playerPosition, GameObject playerObject)
    {
        if (cardBuffer.Count == 0)
        {
            return;
        }
        var cardObject = Instantiate(cardPrefab, playerPosition.position, Utills.QI);
        cardObject.transform.parent = playerObject.transform;
        var card = cardObject.GetComponent<T_CardInfo>();
        card.Setup(PopCard());
        _pCard.Add(card);
    }

    public void ArrangeCardsBetweenMyCards(List<T_CardInfo> cards, Transform leftTransform, Transform rightTransform, float gap)
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
