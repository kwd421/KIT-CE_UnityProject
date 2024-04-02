using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] stages;

    public Image[] UI_health;
    public Text UI_point;
    public Text UI_stage;
    public GameObject UI_restartBtn;

    Text btnText;

    void Awake()
    {
        btnText = UI_restartBtn.GetComponentInChildren<Text>();
    }

    void Update()
    {
        UI_point.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        // Stage Change
        if (stageIndex < stages.Length - 1)
        {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();

            // Calculate Point
            totalPoint += stagePoint;
            stagePoint = 0;

            UI_stage.text = "STAGE " + (stageIndex + 1);
        }
        // Game Clear
        else
        {
            // Player Control Lock
            Time.timeScale = 0;

            // Result UI
            Debug.Log("게임 클리어");

            // Restart Button UI
            btnText.text = "Game Clear!";
            UI_restartBtn.SetActive(true);
        }

    }

    public void HealthDown()
    {
        if (health > 0)
        {
            health--;
            UI_health[health].color = new Color(1, 1, 1, 0.2f);
        }
        else
        {
            // Player Die Effect
            player.Dead();

            // Result UI
            Debug.Log("죽었습니다!");

            // Retry Button UI
            UI_restartBtn.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(health > 1)
            {
                // Health Down
                player.VelocityZero();
                collision.transform.position = new Vector3(-6, -1, 0);
            }
            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-6, -1, 0);
        player.VelocityZero();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
