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
        SoundManager.Instance.PlaySfx(SoundManager.Sfx.ClickButton);
        LoadingSceneManager.LoadScene("MainMenu");
    }
    public void PNLeaveRoom()
    {
        SoundManager.Instance.PlayBgm(SoundManager.Bgm.MainMenuBgm);
        PhotonNetwork.LeaveRoom();
    }
    public void GoLobby()
    {
        SoundManager.Instance.PlaySfx(SoundManager.Sfx.ClickButton);
        LoadingSceneManager.LoadScene("RoomList_HH");
    }
    public void LeaveLobby()
    {
        if(PhotonNetwork.InLobby)
        PhotonNetwork.LeaveLobby();
    }


}
