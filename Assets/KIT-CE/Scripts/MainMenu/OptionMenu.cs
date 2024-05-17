using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Unity.Collections.LowLevel.Unsafe;

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

    public Slider bgmSlider;
    public Text bgmVolumeSize;
    private float bgmVolume;

    public Slider sfxSlider;
    public Text sfxVolumeSize;
    private float sfxVolume;

    private void Start()
    {
        InitUI();
    }

    public void InitUI()
    {
        ResInit();

        FPSInit();
    }

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
                resolutionNum = optionNum;
            }
            optionNum++;
        }
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }

    public void FPSInit()
    {
        fpsDropdown.options.Clear();

        int optionNum = 0;
        int currentFPS = (int)Math.Round(Screen.currentResolution.refreshRateRatio.value);

        foreach(int item in fpsList)
        {
            if (item > currentFPS) break;
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item + " FPS";
            fpsDropdown.options.Add(option);

            if(item == currentFPS)
            {
                fpsDropdown.value = optionNum;
                fpsNum = optionNum;
            }
            optionNum++;
        }
        fpsDropdown.RefreshShownValue();
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

    public void SetVolumes()
    {
        bgmVolume = bgmSlider.value;
        sfxVolume = sfxSlider.value;
        AudioManager.instance.SendMessage("BGMVolumeChange", bgmVolume);
        AudioManager.instance.SendMessage("SFXVolumeChange", sfxVolume);

        bgmVolumeSize.text = ((int)(bgmVolume * 100)).ToString();
        sfxVolumeSize.text = ((int)(sfxVolume * 100)).ToString();
    }

    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width,
            resolutions[resolutionNum].height, screenMode);
        Debug.Log(resolutions[resolutionNum].width + " " + 
            resolutions[resolutionNum].height + " " + screenMode);

        Application.targetFrameRate = fpsList[fpsNum];  // fps설정은 전역적으로 적용됨
        Debug.Log(fpsList[fpsNum]);

    }
}
