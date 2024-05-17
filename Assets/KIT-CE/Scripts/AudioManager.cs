using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;   // 마지막에 실행한 sfxPlayers의 Index

    public enum Sfx { Attack, Damaged, Die, Finish, Item, Jump }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("이미 AudioManager가 존재합니다.");
        }
        instance = this;
        Init();
        DontDestroyOnLoad(gameObject);
    }

    void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = true;  // 시작시 소리 on
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        bgmPlayer.Play();

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for(int i=0; i<sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = sfxVolume;
        }
    }
   
    public void BGMVolumeChange(float volume)
    {
        bgmPlayer.volume = volume;
    }

    public void SFXVolumeChange(float volume)
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i].volume = volume;
        }
    }

    public void PlaySfx(Sfx sfx)
    {
        for(int i=0; i<sfxPlayers.Length; i++)
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
}
