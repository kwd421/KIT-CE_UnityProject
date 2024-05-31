using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Boss : MonoBehaviour
{
    SpriteRenderer sprite;
    // 1페이즈용 1번안대
    Transform eyePatch1;
    // 1페이즈용 눈
    public Transform eye;
    // 2페이즈용 2번안대
    Transform eyePatch2;
    Rigidbody2D rigid;
    Vector2 initPos;
    bool phase2 = false;

    enum Pattern { A, B, C };

    // 화면 밖으로 안나가게 체크
    Vector2 inView;

    [Header ("Boss Infos")]
    public int maxHp;
    int hp;
    // 이동속도
    public float moveSpeed;
    // 이동유지 시간
    WaitForSeconds moveTime = new WaitForSeconds(3f);
    // 수평/수직 이동방향
    int dirX, dirY;
    bool isDead = false;
    // 패턴 끝난 후 다음패턴 시작 시간
    WaitForSeconds nextPattern = new WaitForSeconds(3f);

    [Header ("Pattern A")]
    public GameObject teardrop;
    bool eyeFlip = false;
    // 눈물 오브젝트 풀
    List<GameObject> tears = new List<GameObject>();
    WaitForSeconds tearCreateTime = new WaitForSeconds(0.5f);
    public int tearCount;
    public float tearSpeed;
    int attackCount;
    public AudioClip patternASound;

    [Header("Pattern B")]
    public GameObject pillarParent;
    public AudioClip patternBSound;

    [Header ("Pattern C")]
    public GameObject[] attackBars;
    public AudioClip patternCSound;
    

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        eyePatch1 = transform.GetChild(0);
        eye = transform.GetChild(1);
        rigid = GetComponent<Rigidbody2D>();
        initPos = transform.position;

        // 공격용 눈물 생성(오브젝트풀)
        MakeTears();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        if(isDead)
            StopAllCoroutines();
    }


    private void LateUpdate()
    {
        // 플레이어를 항상 바라보게
        Vector2 dir = GameManager.instance.player.transform.position - transform.position;
        sprite.flipX = dir.x > 0 ? true : false;
        // 머리가 flip되면 눈 대칭 이동
        if(eyeFlip != sprite.flipX)
        {
            eyeFlip = sprite.flipX;
            float eyePosX = transform.position.x - (eyePatch1.position.x - transform.position.x);
            eyePatch1.position = new Vector2(eyePosX, eyePatch1.position.y);
            eyePosX = transform.position.x - (eye.position.x - transform.position.x);
            eye.position = new Vector2(eyePosX, eye.position.y);
            //eye.GetComponent<SpriteRenderer>().flipX = sprite.flipX ? true : false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌했을 때
        if(collision.name.Equals("Player"))
        {
            Player player = GameManager.instance.player;
            // 플레이어가 밟았을 때
            if(player.GetComponent<Rigidbody2D>().velocity.y < 0 && player.transform.position.y > eyePatch1.position.y)
            {
                // 플레이어가 점프하게
                player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 22, ForceMode2D.Impulse);
                OnDamaged();
            }
        }
    }

    private void FixedUpdate()
    {
        // 화면 끝에 다다르면 반대로 이동
        inView = Camera.main.WorldToViewportPoint(transform.position);
        if (inView.x < 0.13f || inView.x > 0.87f)
        {
            // 수평 이동
            dirX = -dirX;
        }
        if (inView.y < 0.28f || inView.y > 0.7f)
        {
            // 수직 이동
            dirY = -dirY;
        }

        // 최종 이동속도
        rigid.velocity = new Vector2(moveSpeed * dirX, moveSpeed * dirY);
    }

    IEnumerator Move()
    {
        // -1, 0, 1 중 0을 제외한 값을 받아와 방향값으로 활용
        do
        {
            dirX = Random.Range(-1, 2);
        }
        while (dirX == 0);
        do
        {
            dirY = Random.Range(-1, 2);
        }
        while (dirY == 0);

        yield return moveTime;
        StartCoroutine(Move());
    }

    void OnDamaged()
    {
        hp--;
        StartCoroutine(DamagedCoroutine());
        // 2페이즈 ON
        if(hp <= 0.5f * maxHp)
        {
            phase2 = true;
        }
        else if(hp <= 0)
        {
            OnDead();
        }
    }

    IEnumerator DamagedCoroutine()
    {
        // hp가 6 이상일 때 한 눈만 피격
        if(!phase2)
        {
            eye.GetComponent<Collider2D>().enabled = false;
            sprite.color = new Color(1, 0, 0, 0.8f);
            yield return new WaitForSeconds(2f);
            eye.GetComponent<Collider2D>().enabled = true;
            sprite.color = new Color(1, 1, 1);
        }
        // hp가 5 이하일 때 두 눈 모두 피격
        else
        {
            eyePatch1.GetComponent<Collider2D>().enabled = false;
            eye.GetComponent<Collider2D>().enabled = false;
            sprite.color = new Color(1, 0, 0, 0.8f);
            yield return new WaitForSeconds(2f);
            eyePatch1.GetComponent<Collider2D>().enabled = true;
            eye.GetComponent<Collider2D>().enabled = true;
            sprite.color = new Color(1, 1, 1);
        }        
    }

    
    void MakeTears()
    {
        // 오브젝트 풀에 눈물 추가
        for (int i = 0; i < tearCount; i++)
        {
            // 눈물 생성, 부모는 눈
            GameObject temp = Instantiate(teardrop, eye);
            if (i % 3 == 0) temp.GetComponent<Teardrop>().teardropSpeed = 4f;
            else if (i % 2 == 1) temp.GetComponent<Teardrop>().teardropSpeed = 5f;
            else temp.GetComponent<Teardrop>().teardropSpeed = 6f;
            // 눈물 비활성화
            temp.SetActive(false);
            // 눈물 오브젝트 풀에 담기
            tears.Add(temp);
        }
    }

    IEnumerator BossPatterns()
    {
        // 잠깐 기다리고 패턴 시작, 패턴 끝난 후 재귀
        yield return nextPattern;
        int select;
        select = Random.Range(0, 3);
        switch(select)
        {
            case ((int)Pattern.A):
                yield return StartCoroutine(PatternA());
                break;
            case ((int)Pattern.B):
                yield return StartCoroutine(PatternB());
                break;
            case ((int)Pattern.C):
                yield return StartCoroutine(PatternC());
                break;
        }        
        StartCoroutine(BossPatterns());
    }


    IEnumerator PatternA()
    {
        AudioManager.instance.PlaySfx(patternASound);
        int count = 0;
        if (!phase2)
        {
            // attackCount만큼 눈물 생성
            attackCount = 3;
            while (count < attackCount)
            {
                // 현재 눈의 위치로 시작점 설정 후 생성
                tears[count].transform.SetParent(eye);
                tears[count].SetActive(true);
                count++;
                yield return tearCreateTime;
            }
        }
        else
        {
            attackCount = 6;
            while (count < attackCount)
            {
                tears[count].SetActive(true);
                count++;
                yield return tearCreateTime;
            }
        }
               
    }
    
    
    IEnumerator PatternB()
    {
        AudioManager.instance.PlaySfx(patternBSound);
        eye.GetChild(0).gameObject.SetActive(false);
        // 패턴 경고용 붉은 안광 활성화, 1.5초 후 눈 주위로 공격패턴 생성
        // 패턴B(pillarParent)는 eye의 자식으로 있음
        eye.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);        
        pillarParent.SetActive(true);
        // 페이즈 1일 때 속도
        if(!phase2)
        {
            pillarParent.GetComponent<PatternB>().SetSpeed(4f);
        }
        else
        // 페이즈 2일 때 속도
        {
            pillarParent.GetComponent<PatternB>().SetSpeed(7f);
        }
        // 2초간 페이즈당 속도만큼 회전 후 비활성화. 다음 패턴으로 넘어간다.
        yield return new WaitForSeconds(2f);
        eye.GetChild(0).gameObject.SetActive(false);
        pillarParent.SetActive(false);
    }

    // 1~3층까지의 발판에 공격패턴을 생성
    IEnumerator PatternC()
    {
        AudioManager.instance.PlaySfx(patternCSound);
        // 1페이즈 패턴: 무작위 층 하나에 공격패턴 생성
        if (!phase2)
        {
            int temp = Random.Range(0, 3);
            GameObject bar = attackBars[temp];
            // 경고
            yield return StartCoroutine(Warning(bar)); 
            // 공격
            yield return StartCoroutine(BarAttack(bar));
        }
        // 2페이즈 패턴: 무자구이 층 둘에 공격패턴 생성
        else
        {
            int tempA, tempB;
            // tempA와 tempB가 중복이면 tempB를 다시 받아옴
            tempA = Random.Range(0, 3);
            do
            {
                tempB = Random.Range(0, 3);
            }
            while (tempA == tempB);
            GameObject barA = attackBars[tempA];
            GameObject barB = attackBars[tempB];
            
            // 경고
            StartCoroutine(Warning(barA));
            yield return StartCoroutine(Warning(barB));
            // 공격
            StartCoroutine(BarAttack(barA));
            yield return StartCoroutine(BarAttack(barB));
        }
    }

    IEnumerator Warning(GameObject bar)
    {
        WaitForSeconds flickTime = new WaitForSeconds(0.1f);
        Light2D glow = bar.GetComponentInChildren<Light2D>();
        glow.color = new Color(1, 0, 0);
        glow.volumeIntensity = 0;

        // 처음은 경고
        bar.GetComponent<Collider2D>().enabled = false;
        bar.SetActive(true);

        // 공격 경보 깜빡이게
        for (int count = 0; count < 2; count++)
        {
            // 색 증가
            for (int i = 1; i <= 10; i++)
            {
                bar.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.1f * i);                
                glow.volumeIntensity += 0.1f;
                yield return flickTime;
            }
            // 색 감소
            for (int i = 9; i >= 0; i--)
            {
                bar.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.1f * i);
                glow.volumeIntensity -= 0.1f;
                yield return flickTime;
            }
        }
    }

    IEnumerator BarAttack(GameObject bar)
    {
        WaitForSeconds flickTime = new WaitForSeconds(0.1f);
        Light2D glow = bar.GetComponentInChildren<Light2D>();
        glow.color = new Color(0.933f, 0.867f, 0.616f);
        glow.volumeIntensity = 0;

        // 점점 색 선명해지다가 0.5초 후 공격
        for (int i=1; i<=5; i++)
        {
            bar.GetComponent<SpriteRenderer>().color = new Color(0.933f, 0.867f, 0.616f, 0.2f * i);
            glow.volumeIntensity += 0.2f;
            yield return flickTime;
        }
        glow.volumeIntensity = 2f;
        bar.GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        bar.SetActive(false);
    }

    void OnDead()
    {
        isDead = true;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Finish);
        // To Next Stage
        GameManager.instance.NextStage();
    }

    public virtual void Init()
    {
        // HP 초기화
        hp = maxHp;

        // isDead to false, Update 실행
        isDead = false;

        // 위치 초기화
        transform.position = initPos;

        // 패턴A 눈물 초기화
        foreach(GameObject tear in tears)
        {
            tear.SetActive(false);
        }
        // 패턴B 안광 및 공격 기둥 초기화
        // !!!!!!!!!!!!문제의코드!!!!!!!!!!!!
        //eye.GetComponent<Light2D>().gameObject.SetActive(false);
        pillarParent.SetActive(false);
        // 패턴C bar들 초기화
        foreach(GameObject bar in attackBars)
        {
            bar.SetActive(false);
        }

        // 이동 코루틴 시작
        StartCoroutine(Move());
        StartCoroutine(BossPatterns());
    }
}
