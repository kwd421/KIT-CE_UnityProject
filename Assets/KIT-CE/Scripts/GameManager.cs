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
            Debug.LogError("�̹� GameManager�� �����մϴ�.");
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
            // ���� �������� ��Ȱ��ȭ
            stages[stageIndex].SetActive(false);
            stageIndex++;

            // ���� �������� Ȱ��ȭ
            stages[stageIndex].SetActive(true);

            // �������� ���� ������Ʈ�� ���� Active
            StageInit();

            // �÷��̾� ��ġ �缳��
            PlayerReposition();

            // ���������� ���� ���������� ����bgm
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

            // ���� Ŭ���� �� ������ϰ� �׾ Retry�ϸ� Clear! �� ������ ��
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
        // �߶� ����
        if (collision.gameObject.tag == "Player")
        {
            // �÷��̾ ���������� ��
            if(player.gameObject.layer == LayerMask.NameToLayer("PlayerDamaged"))
            {
                PlayerReposition();
            }
            // �÷��̾ �������°� �ƴϰ�, ü���� 0���� Ŭ ��
            else if(health > 0)
            {
                // Health Up �� InvincibleCoroutine(���⿡ Health Down�ֱ⶧��)
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
        // ù ���������� ���ư� �� �Ϲ� bgm���
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

        // stages ���� ��Ȱ��ȭ
        System.Array.ForEach(stages, stage => stage.SetActive(false));
        StageInit();

        player.Init();
    }

    // ���������� �ϳ� �Ѿ ������ �Ѿ ���������� Item, Enemy ��ü Ȱ��ȭ
    void StageInit()
    {
        // �������� Ȱ��ȭ
        stages[stageIndex].SetActive(true);
        btnText.text = "Retry?";
        
        // �������� ���� Item Ȱ��ȭ
        Item[] items = stages[stageIndex].GetComponentsInChildren<Item>(true);
        for (int i = 0; i < items.Length; i++)
        {
            items[i].gameObject.SetActive(true);
        }
        // �������� ���� Enemy Ȱ��ȭ
        Enemy[] enemies = stages[stageIndex].GetComponentsInChildren<Enemy>(true);
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].gameObject.SetActive(true);
        }
        vCam.MapBorder(stages[stageIndex]);
    }
}