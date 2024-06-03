using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip[] bgmClips;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;   // �������� ������ sfxPlayers�� Index

    public enum Sfx { Attack, Damaged, Die, Finish, Item, Jump }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("�̹� AudioManager�� �����մϴ�.");
        }
        instance = this;
        Init();
        DontDestroyOnLoad(gameObject);
    }

    void Init()
    {
        // ����� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = true;  // ���۽� �Ҹ� on
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        // ó���� �Ϲ� bgm ���
        bgmPlayer.clip = bgmClips[0];
        bgmPlayer.Play();
        
        

        // ȿ���� �÷��̾� �ʱ�ȭ
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

    public void SetVolumes(float bgm, float sfx)
    {
        bgmVolume = bgm;
        sfxVolume = sfx;
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

    public void PlayBGM(AudioClip bgm)
    {
        bgmPlayer.Stop();
        bgmPlayer.clip = bgm;
        bgmPlayer.Play();
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

    public void PlaySfx(AudioClip sfx)
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfx;
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
