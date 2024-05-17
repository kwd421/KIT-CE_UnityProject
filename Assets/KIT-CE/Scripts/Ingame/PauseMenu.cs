using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CallPauseMenu();
        }        
    }

    void CallPauseMenu()
    {
        if (!pauseMenu.activeSelf)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            //pauseMenu.GetComponentsInChildren<Transform>()[1].gameObject.SetActive(false);
            foreach(Transform item in pauseMenu.GetComponentsInChildren<Transform>(true))
            {
                if(item.name == "Exit Warning" || item.name == "Option Menu")
                {
                    item.gameObject.SetActive(false);
                }
                if(item.name == "Pause Menu Panel")
                {
                    item.gameObject.SetActive(true);
                }
            }
            pauseMenu.SetActive(false);
        }
    }
    public void Resume()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
