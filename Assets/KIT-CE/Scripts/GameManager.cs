using UnityEngine;
using UnityEngine.UI;

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

    public VirtualCamera vCam;
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
        StageInit();
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
        UI_health.text = "X " + health;
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
            // 현재 스테이지 비활성화
            stages[stageIndex].SetActive(false);
            stageIndex++;

            // 다음 스테이지 활성화
            stages[stageIndex].SetActive(true);

            // 스테이지 하위 오브젝트들 전부 Active
            StageInit();

            // 플레이어 위치 재설정
            PlayerReposition();

            // 스테이지가 보스 스테이지면 보스bgm
            if (stages[stageIndex].name.Contains("Boss"))
            {
                AudioManager.instance.PlayBGM(AudioManager.instance.bgmClips[1]);
            }
            

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

            // 게임 클리어 후 재시작하고 죽어서 Retry하면 Clear! 가 나오게 됨
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

            // Retry Button UI
            UI_restartBtn.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 추락 감지
        if (collision.gameObject.tag == "Player")
        {
            // 플레이어가 무적상태일 때
            if(player.gameObject.layer == LayerMask.NameToLayer("PlayerDamaged"))
            {
                PlayerReposition();
            }
            // 플레이어가 무적상태가 아니고, 체력이 0보다 클 때
            else if(health > 0)
            {
                // Health Up 후 InvincibleCoroutine(여기에 Health Down있기때문)
                health++;
                player.StartCoroutine(player.InvincibleCoroutine(transform.position));
                PlayerReposition();
                HealthDown();
            }
            else
            {
                HealthDown();
            }
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, 0);
        player.VelocityZero();
    }

    public void Restart()
    {
        // 첫 스테이지로 돌아갈 시 일반 bgm재생
        AudioManager.instance.PlayBGM(AudioManager.instance.bgmClips[0]);
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
        btnText.text = "Retry?";
        
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
        vCam.MapBorder(stages[stageIndex]);
    }
}