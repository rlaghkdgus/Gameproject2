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

    List<T_Card> cardBuffer;//카드를 담을 공간


    public T_Card PopCard()//카드 꺼내기
    {
        T_Card card = cardBuffer[0];
        cardBuffer.RemoveAt(0);
        return card;
    }


    void SetupCardBuffer()//카드모둠 세팅
    {
        cardBuffer = new List<T_Card>();
        for (int i = 0; i < cardSO.card.Length; i++)//카드를 넣고
        {
            T_Card card = cardSO.card[i];
            for (int j = 0; j < card.percent; j++)
                cardBuffer.Add(card);
        }
        for (int i = 0; i < cardBuffer.Count; i++)//랜덤하게 섞기
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
        float totalWidth = gap * (cardCount - 1); // 카드들이 차지할 총 가로 길이 계산
        float startX = (leftTransform.position.x + rightTransform.position.x - totalWidth) / 2f; // 첫 번째 카드의 x 위치 계산

        // 카드를 일정한 간격으로 배치
        for (int i = 0; i < cardCount; i++)
        {
            Vector3 cardPosition = new Vector3(startX + i * gap, leftTransform.position.y, leftTransform.position.z);
            cards[i].transform.position = cardPosition;
        }
    }
}
