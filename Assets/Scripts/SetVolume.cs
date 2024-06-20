using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVolume : MonoBehaviour
{
   public void SetSfxVolume(float volume)
    {
        for (int i = 0; i < SoundManager.Instance.sfxPlayers.Length; i++)
        {
            SoundManager.Instance.sfxPlayers[i].volume = volume;
        }
    }
    public void SetBgmVolume(float volume)
    {
        SoundManager.Instance.bgmPlayer.volume = volume;
    }
}
