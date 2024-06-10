using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;
using TMPro;
using ExitGames.Client.Photon;

public class CardManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public Card p1BufferCard;
    public Card p2BufferCard;
    [SerializeField] CardSO cardSO;
    [SerializeField] GameObject cardPrefab;
    public List<CardInfo> BonusCards;
    [SerializeField] GameObject bonusCardParent;
    [SerializeField] Transform cardSpawnPoint;
    [SerializeField] TMP_Text cardCountTxt;
   public List<Card> cardBuffer;//카드를 담을 공간

    private static CardManager _instance;

    public static CardManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CardManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(CardManager).Name);
                    _instance = singleton.AddComponent<CardManager>();
                }
            }
            return _instance;
        }
    }

    public void PopCard(int playerIndex)//카드 꺼내기
    {
        PV.RPC("RPCCard", RpcTarget.All, playerIndex);
    }
    [PunRPC]
    public void RPCCard(int playerIndex)
    {
        if (cardBuffer.Count == 0)
            return;
        if (playerIndex == 0)
        {
            p1BufferCard = cardBuffer[0];
            cardBuffer.RemoveAt(0);
        }
        else if (playerIndex == 1)
        {
            p2BufferCard = cardBuffer[0];
            cardBuffer.RemoveAt(0);
        }

    }

    public void RoundSetupCard()
    {
        PV.RPC("SetupCardBuffer", RpcTarget.All);
    }
    [PunRPC]
    public void SetupCardBuffer()//카드모둠 세팅
    {
        if (PhotonNetwork.IsMasterClient)
        {
            cardBuffer = new List<Card>();
            for (int i = 0; i < cardSO.cards.Length; i++)//카드를 넣고
            {
                Card card = cardSO.cards[i];
                for (int j = 0; j < card.percent; j++)
                    cardBuffer.Add(card);
            }
            for (int i = 0; i < cardBuffer.Count; i++)//랜덤하게 섞기
            {
                int rand = Random.Range(i, cardBuffer.Count);
                Card temp = cardBuffer[i];
                cardBuffer[i] = cardBuffer[rand];
                cardBuffer[rand] = temp;
               
            }
            PV.RPC("RPCSyncCardBuffer", RpcTarget.Others, cardBuffer.ToArray());
        }
    }
    [PunRPC]
    public void RPCSyncCardBuffer(Card[] syncedCardBuffer)
    {
        cardBuffer = new List<Card>(syncedCardBuffer);
    }


    public void SortCards(List<CardInfo> playerCards)
    {
        if(playerCards.Count == 0)
        Debug.Log("Sort");
        playerCards.Sort((card1, card2) => card1.cardnum.CompareTo(card2.cardnum)); ;    
    }

   
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonPeer.RegisterType(typeof(Card), 0, Card.Serialize, Card.Deserialize);
        PV = GetComponent<PhotonView>();
        //SingleTon.Instance.SetChar(SingleTon.Instance.GetCharID());
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("SetupCardBuffer", RpcTarget.All);
        }
    }


    private void Start()
    {
        
    }

    // Update is called once per frame

    public void AddCard(int playerIndex)
    {
            PV.RPC("RPCAddCard", RpcTarget.All, playerIndex);
    }

    [PunRPC]
    public void RPCAddCard(int playerIndex)
    {
        if (cardBuffer.Count == 0)
              return;   
        if (PhotonNetwork.IsMasterClient && playerIndex == 0)
        {
            var cardObject = PhotonNetwork.Instantiate("Card1", GameManager.Instance.player[playerIndex].playerPosition.position, Utills.QI);
            var card = cardObject.GetComponent<CardInfo>();
            cardObject.transform.parent = GameManager.Instance.player[playerIndex].playerObject.transform;
            Debug.Log("p1card");
            PopCard(0);
            card.Setup(p1BufferCard);
            GameManager.Instance.player[playerIndex].playerCards.Add(card);
            PV.RPC("RPCCardCount", RpcTarget.All);
        }
            else if (!PhotonNetwork.IsMasterClient  &&  playerIndex == 1)
        {
            var cardObject = PhotonNetwork.Instantiate("Card1", GameManager.Instance.player[playerIndex].playerPosition.position, Utills.QI);
            var card = cardObject.GetComponent<CardInfo>();
            cardObject.transform.parent = GameManager.Instance.player[playerIndex].playerObject.transform;
            Debug.Log("p2Card");
            PopCard(1);
            card.Setup(p2BufferCard);
            GameManager.Instance.player[playerIndex].playerCards.Add(card);
            PV.RPC("RPCCardCount", RpcTarget.All);
        }
    }
  [PunRPC]
  public void RPCCardCount()
    {
        cardCountTxt.text = "" + cardBuffer.Count;
    }

    public void ArrangeCardsBetweenMyCards(List<CardInfo> cards, Transform leftTransform, Transform rightTransform, float gap)
    {
        if (cards.Count == 0)
            return;
        Debug.Log("Arrange");
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