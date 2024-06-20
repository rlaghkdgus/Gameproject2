using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyMgr : MonoBehaviourPunCallbacks
{
    private static LobbyMgr _instance;
    public PhotonView PV;
    public int playerCount = 0;
    public bool readySign;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public Text ListText;
    public Text RoomInfoText;
    public Text[] ChatText;
    public InputField ChatInput;
    public static LobbyMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LobbyMgr>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(LobbyMgr).Name);
                    _instance = singleton.AddComponent<LobbyMgr>();
                }
            }
            return _instance;
        }
    }
    public void ReadyCheck()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            PV.RPC("DoReady", RpcTarget.All);
        }
    }
    public void GoRoom()
    {
        photonmgr.Instance.OnConnet();
    }
    public void GoGameScene()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (PhotonNetwork.IsMasterClient && readySign == true)
            {
                PhotonNetwork.LoadLevel("Game_HH");
            }
        }
    }
    public void leftRoom()
    {
        if(PhotonNetwork.InRoom)
        PhotonNetwork.LeaveRoom();
    }
    [PunRPC]
    public void DoReady()
    {
        if (readySign == false)
        {
            readySign = true;
        }
        else if (readySign == true)
        {
            readySign = false;
        }
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public GameObject StartButton;
    public GameObject ReadyButton;
    public List<Image> Images = new List<Image>();
    public List<Sprite> Sprites = new List<Sprite>();
    public GameObject p1UI;
    public GameObject p2UI;
    public GameObject Mask;
    public Transform ReadyButtontransform;
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartButton.SetActive(true);
            ReadyButton.SetActive(false);
        }
        else
        {
            StartButton.SetActive(false);
            ReadyButton.SetActive(true);
        }
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                p1UI.SetActive(true);
                p2UI.SetActive(false);
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                p2UI.SetActive(true);

        }

        if (readySign == true)
        {
            Mask.SetActive(false);
        }
        else if(readySign == false)
        {
            Mask.SetActive(true);
        }

    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SoundManager.Instance.PlaySfx(SoundManager.Sfx.EnterRoom);
        RoomRenewal();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        readySign = false;
    }
    void RoomRenewal()
    {
       
    }
}
