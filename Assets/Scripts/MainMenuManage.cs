using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class MainMenuManage : MonoBehaviourPun
{
    // Start is called before the first frame update
  
    public void GoMainMenu()
    {
        LoadingSceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
    public void PNLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void GoLobby()
    {
        LoadingSceneManager.LoadScene("RoomList_HH");
    }
    public void LeaveLobby()
    {
        if(PhotonNetwork.InLobby)
        PhotonNetwork.LeaveLobby();
    }


}
