using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    List<Resolution> resolutions = new List<Resolution>();
    [SerializeField] int resolutionNum;

    public Dropdown screenDropdown;
    List<FullScreenMode> screenModes = new List<FullScreenMode> { FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.FullScreenWindow, FullScreenMode.Windowed };
    [SerializeField] int screenNum;

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
        screenCheck.text = "Current ScreenMode: " + Screen.fullScreenMode + "\n"
            + "Change to " + screenModes[screenNum];
    }

    public void InitUI()
    {
        ResInit();

        ScreenInit();

        FPSInit();

        SoundInit();
    }

    // �ػ� �ʱ⼳��
    public void ResInit()
    {
        Debug.Log("ResInit");
        Resolution temp = Screen.currentResolution;
        
        foreach (Resolution item in Screen.resolutions)
        {
            // item.width == ~�� �ػ󵵰� 800x600�� �� 800x600�� �ѹ� �� ������ �� ����
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
    }

    // ȭ�� ��� �ʱ⼳��
    public void ScreenInit()
    {
        Debug.Log("ScreenInit");
        screenDropdown.options.Clear();

        int optionNum = 0;

        foreach (FullScreenMode item in screenModes)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            if(item.ToString() == "ExclusiveFullScreen")
            {
                option.text = "��üȭ����";
            }
            else if (item.ToString() == "FullScreenWindow")
            {
                option.text = "��üâ���";
            }
            else if (item.ToString() == "Windowed")
            {
                option.text = "â���";
            }
            screenDropdown.options.Add(option);

            if (item == Screen.fullScreenMode)
            {
                screenDropdown.value = optionNum;
            }
            optionNum++;
        }
        screenDropdown.RefreshShownValue();
    }

    // FPS �ʱ⼳��
    public void FPSInit()
    {
        Debug.Log("FPSInit");
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

            // ������ �������� ������ ���� ���� ��(�ɼǸ޴� ù ����): ���� ����� �ֻ��� ǥ��
            if (currentFPS == -1)
            {
                if (item == monitorHz)
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
        Debug.Log("SoundInit");
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

    public void ScreenDropboxOptionChange(int z)
    {
        screenNum = z;
    }

    public void FPSDropboxOptionChange(int y)
    {
        fpsNum = y;
    }


    // ���� ���ð��� AudioManager�� ����
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

    // ���� ����
    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width,
            resolutions[resolutionNum].height, screenModes[screenNum]);

        Application.targetFrameRate = fpsList[fpsNum];  // fps������ ���������� �����
    }
}
