using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyMgr : MonoBehaviourPunCallbacks
{
    private static LobbyMgr _instance;
    public PhotonView PV;
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
    public void GoGameScene()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Master client. Loading Game_HH scene...");
                PhotonNetwork.LoadLevel("Game_HH");
            }
        }
    }
}
