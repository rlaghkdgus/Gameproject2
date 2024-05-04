using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SingleTon : MonoBehaviour
{
    private static SingleTon instance = null;
    
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static SingleTon Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    [System.Serializable]
    private class CharacterData
    {
        public string charName;
        public Sprite lobbySprite;
        public Sprite sdSprite;
        public Sprite iconSprite;
        public Sprite backSprite;

        
    }
    [SerializeField]
    private CharacterData[] playableCharacters = new CharacterData[3];
    [SerializeField]
    private int charID;
    public int GetCharID()
    {
        return charID;
    }

    private Image mainCharImage;
    private Image playerIconImage;
    private Image playerCharSDImage;

    private void Start()
    {
        SetChar(charID);
    }
    private void Update()
    {
        
    }

    public void SetChar(int index)
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "MainMenu_Yagu")
        {
            if (mainCharImage == null)
            {
                mainCharImage = GameObject.Find("MainCharacterImage").GetComponent<Image>();
            }
            charID = index;
            mainCharImage.sprite = playableCharacters[index].lobbySprite;
        }

        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (playerIconImage == null)
            {
                playerIconImage = GameObject.Find("ScoreImage2").GetComponent<Image>();
            }
            if (playerCharSDImage == null)
            {
                playerCharSDImage = GameObject.Find("SDImage2").GetComponent<Image>();
            }
            playerIconImage.sprite = playableCharacters[index].iconSprite;
            playerCharSDImage.sprite = playableCharacters[index].sdSprite;
            //playerCharSDImage.SetNativeSize();
        }
    }
}