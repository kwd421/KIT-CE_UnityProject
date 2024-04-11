using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Changed: Singleton GameManager
    public static GameManager instance;

    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public int stageTime = 180;
    public float gameTime;

    public Player player;
    public GameObject[] stages;

    public Image[] UI_health;
    public Text UI_point;
    public Text UI_stage;
    public Text UI_time;
    public GameObject UI_restartBtn;

    Text btnText;
    int min;
    int sec;
    bool isTimeOver = false;

    void Awake()
    {
        instance = this;
        btnText = UI_restartBtn.GetComponentInChildren<Text>();
    }

    private void Start()
    {
        // 추후 옵션으로 프레임 설정 가능하게
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        UI_point.text = (totalPoint + stagePoint).ToString();
        gameTime += Time.deltaTime;
        min = Mathf.FloorToInt((stageTime - gameTime) / 60);
        sec = Mathf.CeilToInt((stageTime - gameTime) % 60);
        if(min < 0) min = 0;
        if (sec < 0) sec = 0;
        UI_time.text = min.ToString("D2") + ":" + sec.ToString("00");

        if (stageTime - gameTime < 0 && !isTimeOver)
        {
            player.Dead();
            isTimeOver = true;
            UI_restartBtn.SetActive(true);
        }
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
            gameTime = 0;

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
            btnText.text = "Clear!";
            UI_restartBtn.SetActive(true);
        }

    }

    public void HealthDown()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Damaged);
        if (health > 0)
        {
            health--;
            UI_health[health].color = new Color(1, 0, 0, 0.4f);
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
            if(health > 0)
            {
                // Health Down
                PlayerReposition();
            }
            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, 0);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        isTimeOver = false;
        SceneManager.LoadScene(0);
    }

}