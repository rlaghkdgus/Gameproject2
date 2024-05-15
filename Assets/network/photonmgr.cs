using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class photonmgr : MonoBehaviourPunCallbacks
{
    public Text PlayerCountText;
    private void Awake()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.AutomaticallySyncScene = true;
        DontDestroyOnLoad(gameObject);
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

        //PhotonNetwork.LoadLevel("Game_HH");
        Debug.Log("a");
        //int randnum = Random.RandomRange(1000, 9999);
        int randnum = 1000;

        string roomName = "Room" + randnum.ToString();
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 2}, null);
        //*/
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("b");
        Debug.Log("OnJoinedRoom called. Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        // PhotonNetwork.CurrentRoom.PlayerCount*PhotonNetwork.LocalPlayer.GetPlayerNumber();
        //float usernum = (float)PhotonNetwork.LocalPlayer.GetPlayerNumber();
        //float usernum = (float)(PhotonNetwork.CurrentRoom.PlayerCount * PhotonNetwork.LocalPlayer.GetPlayerNumber());
        //Debug.Log("usernum : " + usernum.ToString());
        //PhotonNetwork.Instantiate("unitobj", new Vector3((-3f - (usernum)), 1f, 0f), Quaternion.identity, 0);
        //PhotonNetwork.Instantiate("unit", Vector3.zero, Quaternion.identity, 0);

        
    }
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
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            PlayerCountText.text = "Players: " + playerCount;
        }
        
        
    }
}
