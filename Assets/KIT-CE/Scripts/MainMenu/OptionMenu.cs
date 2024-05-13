using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    FullScreenMode screenMode;
    public Toggle fullscreenBtn;

    public TMP_Dropdown resolutionDropdown;
    List<Resolution> resolutions = new List<Resolution>();
    [SerializeField] int resolutionNum;

    public TMP_Dropdown fpsDropdown;
    List<int> fpsList = new List<int> { 30, 60, 120, 144, 240 };
    [SerializeField] int fpsNum;

    public Slider bgmSlider;
    public Text bgmNumber;
    private float bgmVolume;

    public Slider sfxSlider;
    public Text sfxNumber;
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
        foreach (Resolution item in Screen.resolutions)
        {
            if (item.width < 800) continue;
            resolutions.Add(item);
        }
        resolutionDropdown.options.Clear();

        int optionNum = 0;
        foreach (Resolution item in resolutions)
        {
            if (item.width < 800) continue;
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = item.width + " x " + item.height;
            resolutionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
            {
                resolutionDropdown.value = optionNum;
            }
            optionNum++;
        }
        resolutionDropdown.RefreshShownValue();

        fullscreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }

    public void FPSInit()
    {
        fpsDropdown.options.Clear();

        int optionNum = 0;
        int currentFPS = (int)Screen.currentResolution.refreshRateRatio.value;

        foreach(int item in fpsList)
        {
            if (item > currentFPS) break;
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = item + " FPS";
            fpsDropdown.options.Add(option);

            if(item == currentFPS)
            {
                fpsDropdown.value = optionNum;
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

        bgmNumber.text = ((int)(bgmVolume * 100)).ToString();
        sfxNumber.text = ((int)(sfxVolume * 100)).ToString();
    }

    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width,
            resolutions[resolutionNum].height, screenMode);

        Application.targetFrameRate = fpsList[fpsNum];  // fps설정은 전역적으로 적용됨

        AudioManager.instance.bgmVolume = bgmVolume;
        AudioManager.instance.sfxVolume = sfxVolume;
    }
}
