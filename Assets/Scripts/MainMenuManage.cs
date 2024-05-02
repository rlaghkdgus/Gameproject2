using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenuManage : MonoBehaviour
{
    // Start is called before the first frame update
   public void GameStart()
    {
        LoadingSceneManager.LoadScene("Game");
    }
    public void GoMainMenu()
    {
        LoadingSceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
}
