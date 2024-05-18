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

        SoundInit();
    }

    // �ػ� �ʱ⼳��
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

        fullscreenToggle.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }

    // FPS �ʱ⼳��
    public void FPSInit()
    {
        fpsDropdown.options.Clear();

        int optionNum = 0;
        int monitorFPS = (int)Math.Round(Screen.currentResolution.refreshRateRatio.value);
        int currentFPS = Application.targetFrameRate;
        Debug.Log(currentFPS);

        foreach(int item in fpsList)
        {
            if (item > monitorFPS) break;
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item + " FPS";
            fpsDropdown.options.Add(option);

            // ������ �������� ������ ���� ���� ��(�ɼǸ޴� ù ����): ���� ����� ������ ǥ��
            if (currentFPS == -1)
            {
                if (item == monitorFPS)
                {
                    fpsDropdown.value = optionNum;
                }
            }
            // ������ �������� ������ ������(�ɼǸ޴����� ������ ���� �Ϸ�)
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

    // AudioManager���� ���� �������� �޾ƿ� �����̴��� Text�� ����
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

    // ���� ���ð��� AudioManager�� ����
    public void SetVolumes()
    {
        bgmVolume = bgmSlider.value;
        sfxVolume = sfxSlider.value;
        AudioManager.instance.SetVolumes(bgmVolume, sfxVolume);
        AudioManager.instance.SendMessage("BGMVolumeChange", bgmVolume);
        AudioManager.instance.SendMessage("SFXVolumeChange", sfxVolume);

        bgmVolumeSize.text = ((int)(bgmVolume * 100)).ToString();
        sfxVolumeSize.text = ((int)(sfxVolume * 100)).ToString();
    }

    // ���� ����
    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width,
            resolutions[resolutionNum].height, screenMode);

        Debug.Log(fpsList[fpsNum].ToString());
        Application.targetFrameRate = fpsList[fpsNum];  // fps������ ���������� �����
        Debug.Log(fpsList[fpsNum]);

    }
}
