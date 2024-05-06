using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.Search;
using System.Linq;
using static UnityEditor.Progress;

public class GameManager : MonoBehaviour, IDataPersistence
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

    public Text UI_point;
    public Text UI_stage;
    public Text UI_time;
    public Text UI_health;
    public GameObject UI_restartBtn;

    Text btnText;
    int min;
    int sec;
    bool isTimeOver = false;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("이미 GameManager가 존재합니다.");
        }
        instance = this;
        btnText = UI_restartBtn.GetComponentInChildren<Text>();
    }

    private void Start()
    {
        // 추후 옵션으로 프레임 설정 가능하게
        Application.targetFrameRate = 60;
        UI_health.text = "X " + health;
    }

    void Update()
    {
        UI_point.text = (totalPoint + stagePoint).ToString();
        gameTime += Time.deltaTime;
        min = (int)(stageTime - gameTime) / 60;
        sec = (int)(stageTime - gameTime) % 60;
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

    public void LoadData(GameData data)
    {
        this.health = data.health;
    }

    public void SaveData(ref GameData data)
    {
        data.health = this.health;
    }

    public void NextStage()
    {
        // Stage Change
        if (stageIndex < stages.Length - 1)
        {
            stages[stageIndex].SetActive(false);
            stageIndex++;

            stages[stageIndex].SetActive(true);
            // 스테이지 하위 오브젝트들 전부 Active
            StageInit();

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
            UI_health.text = "X " + health;
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

        UI_restartBtn.SetActive(false);

        PlayerReposition();

        totalPoint = 0;
        stagePoint = 0;
        stageIndex = 0;
        health = 3;
        stageTime = 180;
        gameTime = 0;

        UI_health.text = "X " + health;
        UI_stage.text = "STAGE " + (stageIndex + 1);

        // stages 전부 비활성화
        System.Array.ForEach(stages, stage => stage.SetActive(false));
        StageInit();

        player.Init();
    }

    // 스테이지가 하나 넘어갈 때마다 넘어간 스테이지의 Item, Enemy 전체 활성화
    void StageInit()
    {
        // 스테이지 활성화
        stages[stageIndex].SetActive(true);
        // 스테이지 하위 Item 활성화
        Item[] items = stages[stageIndex].GetComponentsInChildren<Item>(true);
        for (int i = 0; i < items.Length; i++)
        {
            items[i].gameObject.SetActive(true);
        }
        // 스테이지 하위 Enemy 활성화
        Enemy[] enemies = stages[stageIndex].GetComponentsInChildren<Enemy>(true);
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].gameObject.SetActive(true);
        }
    }
}