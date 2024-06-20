using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("BGM")]
    public AudioClip[] bgmClips;
    public float bgmVolume;
    public AudioSource bgmPlayer;
    [Header("SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    public AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Sfx { timerWarning, ClickButton, MakeRoom, EnterRoom, SelectBirdSfx, DrawCard, CheckFail,Combo1,Combo2,Combo3}
    public enum Bgm { MainMenuBgm,InGameBgm}
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Init();
    }
    void Init()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClips[0];
        bgmPlayer.Play();
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = sfxVolume;
        }
    }
    public void PlaySfx(Sfx sfx)
    {
        for (int i = 0; i < channels; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;
            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
    public void PlayBgm(Bgm bgm)
    {
        bgmPlayer.clip = bgmClips[(int)bgm];
        bgmPlayer.Play();
    }
}
