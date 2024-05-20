using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    FullScreenMode screenMode;
    public Toggle fullscreenToggle;

    public Dropdown resolutionDropdown;
    List<Resolution> resolutions = new List<Resolution>();
    [SerializeField] int resolutionNum;

    public Dropdown fpsDropdown;
    List<int> fpsList = new List<int> { 30, 60, 120, 144, 240 };
    [SerializeField] int fpsNum;

    public Slider masterSlider;
    public Text masterVolumeSize;
    private float masterVolume;

    public Slider bgmSlider;
    public Text bgmVolumeSize;
    private float bgmVolume;

    public Slider sfxSlider;
    public Text sfxVolumeSize;
    private float sfxVolume;

    public Text screenCheck;

    private void Start()
    {
        InitUI();
    }

    private void Update()
    {
        screenCheck.text = Screen.fullScreenMode.ToString();
    }

    public void InitUI()
    {
        ResInit();

        FPSInit();

        SoundInit();
    }

    // 해상도 초기설정
    public void ResInit()
    {
        Resolution temp = Screen.currentResolution;
        
        foreach (Resolution item in Screen.resolutions)
        {
            if (item.width < 800 || item.width == temp.width) continue;
            temp = item;
            resolutions.Add(item);
        }
        resolutionDropdown.options.Clear();

        int optionNum = 0;
        
        foreach (Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.width + " x " + item.height;
            resolutionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
            {
                resolutionDropdown.value = optionNum;
            }
            optionNum++;
        }
        resolutionDropdown.RefreshShownValue();

        Debug.Log(Screen.fullScreenMode);   // Windowed

        fullscreenToggle.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
        Debug.Log(fullscreenToggle.isOn);
    }

    // FPS 초기설정
    public void FPSInit()
    {
        fpsDropdown.options.Clear();

        int optionNum = 0;
        int monitorHz = (int)Math.Round(Screen.currentResolution.refreshRateRatio.value);
        int currentFPS = Application.targetFrameRate;

        foreach(int item in fpsList)
        {
            if (item > monitorHz) break;
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item + " FPS";
            fpsDropdown.options.Add(option);

            // 게임의 프레임이 설정돼 있지 않을 때(옵션메뉴 첫 진입): 현재 모니터 주사율 표시
            if (currentFPS == -1)
            {
                if (item == monitorHz)
                {
                    fpsDropdown.value = optionNum;
                }
            }
            // 게임의 프레임이 설정돼 있을때(옵션메뉴에서 프레임 설정 완료)
            else if (currentFPS != -1)
            {
                if (item == currentFPS)
                {
                    fpsDropdown.value = optionNum;
                }
            }            
            optionNum++;
        }
        fpsDropdown.RefreshShownValue();
    }

    // AudioManager에서 사운드 설정값을 받아와 슬라이더와 Text에 적용
    public void SoundInit()
    {
        bgmVolume = AudioManager.instance.bgmVolume;
        sfxVolume = AudioManager.instance.sfxVolume;
        bgmSlider.value = AudioManager.instance.bgmVolume;
        sfxSlider.value = AudioManager.instance.sfxVolume;
        bgmVolumeSize.text = ((int)(bgmVolume * 100)).ToString();
        sfxVolumeSize.text = ((int)(sfxVolume * 100)).ToString();
    }

    public void ResDropboxOptionChange(int x)
    {
        resolutionNum = x;
    }

    public void FPSDropboxOptionChange(int y)
    {
        fpsNum = y;
    }

    public void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    // 현재 세팅값을 AudioManager에 적용
    public void SetVolumes()
    {
        masterVolume = masterSlider.value;
        bgmVolume = bgmSlider.value;
        sfxVolume = sfxSlider.value;

        AudioListener.volume = masterVolume;
        AudioManager.instance.SetVolumes(bgmVolume, sfxVolume);
        AudioManager.instance.BGMVolumeChange(bgmVolume);
        AudioManager.instance.SFXVolumeChange(sfxVolume);

        masterVolumeSize.text = ((int)(masterVolume * 100)).ToString();
        bgmVolumeSize.text = ((int)(bgmVolume * 100)).ToString();
        sfxVolumeSize.text = ((int)(sfxVolume * 100)).ToString();
    }

    // 설정 적용
    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width,
            resolutions[resolutionNum].height, screenMode);

        Application.targetFrameRate = fpsList[fpsNum];  // fps설정은 전역적으로 적용됨
    }
}
