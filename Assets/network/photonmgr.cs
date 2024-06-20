using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class photonmgr : MonoBehaviourPunCallbacks
{
    public Text PlayerCountText;
    
   
    private static photonmgr instance;
    public PhotonView PV;
    public static photonmgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<photonmgr>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<photonmgr>();
                    singletonObject.name = typeof(photonmgr).ToString() + " (Singleton)";
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void OnConnet()
    {
        int randnum = Random.RandomRange(1000, 9999);
        string playerName = "user" + randnum.ToString();

        if (!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }

    public override void OnConnectedToMaster()
    {

        //PhotonNetwork.LoadLevel("Lobby_HH");
        Debug.Log("a");
        //int randnum = Random.RandomRange(1000, 9999);
        /* int randnum = 1000;

         string roomName = "Room" + randnum.ToString();
         RoomOptions roomOptions = new RoomOptions();
         roomOptions.MaxPlayers = 2;
         PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
         */
    }
   
    public void LeaveRoomAndDestroy()
    {
        
    }


    public override void OnLeftRoom()
    {
        LoadingSceneManager.LoadScene("MainMenu");  // 메인 메뉴로 이동
        // 방을 나간 후 처리할 작업 추가
    }
   

    public override void OnJoinedRoom()
    {
        LoadingSceneManager.LoadScene("Lobby_HH");
        Debug.Log("b");
        Debug.Log("OnJoinedRoom called. Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        // PhotonNetwork.CurrentRoom.PlayerCount*PhotonNetwork.LocalPlayer.GetPlayerNumber();
        //float usernum = (float)PhotonNetwork.LocalPlayer.GetPlayerNumber();
        //float usernum = (float)(PhotonNetwork.CurrentRoom.PlayerCount * PhotonNetwork.LocalPlayer.GetPlayerNumber());
        //Debug.Log("usernum : " + usernum.ToString());
        //PhotonNetwork.Instantiate("unitobj", new Vector3((-3f - (usernum)), 1f, 0f), Quaternion.identity, 0);
        //PhotonNetwork.Instantiate("unit", Vector3.zero, Quaternion.identity, 0);


    }
    public override void OnLeftLobby()
    {
        LoadingSceneManager.LoadScene("MainMenu");
    }


    // Start is called before the first frame update
    void Start()
    {
       
    }
   
    // Update is called once per frame

}
