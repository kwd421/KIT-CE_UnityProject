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

    public Slider bgmSlider;
    public Text bgmText;
    private float bgmVolume;

    public Slider sfxSlider;
    public Text sfxText;
    private float sfxVolume;

    private void Start()
    {
        InitUI();
    }

    public void InitUI()
    {
        foreach(Resolution item in Screen.resolutions)
        {
            if (item.width < 800) continue;
            resolutions.Add(item);
        }
        resolutionDropdown.options.Clear();

        int optionNum = 0;
        foreach(Resolution item in resolutions)
        {
            if (item.width < 800) continue;
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = item.width + " x " + item.height + " "
                + Mathf.Round((float)item.refreshRateRatio.value) + "Hz";
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

    public void DropboxOptionChange(int x)
    {
        resolutionNum = x;
    }

    public void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void SetVolumes()
    {
        bgmVolume = bgmSlider.value;
        sfxVolume = sfxSlider.value;
        bgmText.text = ((int)bgmVolume * 100).ToString();
        sfxText.text = ((int)sfxVolume * 100).ToString();
    }

    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width,
            resolutions[resolutionNum].height, screenMode);

        AudioManager.instance.bgmVolume = bgmVolume;
        AudioManager.instance.sfxVolume = sfxVolume;
    }
}
