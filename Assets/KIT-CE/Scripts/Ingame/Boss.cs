using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss : MonoBehaviour
{
    SpriteRenderer sprite;
    // 1페이즈용 1번안대
    Transform eyePatch1;
    // 1페이즈용 눈
    Transform eye;
    // 2페이즈용 2번안대
    Transform eyePatch2;
    Rigidbody2D rigid;

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

    [Header ("Pattern A")]
    public GameObject teardrop;
    bool eyeFlip = false;
    // 눈물 오브젝트 풀
    List<GameObject> tears = new List<GameObject>();
    WaitForSeconds tearCreateTime = new WaitForSeconds(0.5f);
    public int tearCount;

    [Header("Pattern C")]
    public GameObject[] attackBars;
    

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        eyePatch1 = transform.GetChild(0);
        eye = transform.GetChild(1);
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        hp = maxHp;
        // 공격용 눈물 생성(오브젝트풀)
        MakeTears();
        // 이동 코루틴 시작
        StartCoroutine(Move());
        StartCoroutine(PatternC());
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
            
        }
        else if(hp <= 0)
        {
            OnDead();
        }
    }

    IEnumerator DamagedCoroutine()
    {
        // hp가 6 이상일 때 한 눈만 피격
        if(hp > 0.5f * maxHp)
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
            // 눈물 비활성화
            temp.SetActive(false);
            // 눈물 오브젝트 풀에 담기
            tears.Add(temp);
        }
    }



    IEnumerator PatternA()
    {
        yield return new WaitForSeconds(2f);
        int count = 0;
        while(count < tearCount)
        {
            tears[count].SetActive(true);
            count++;
            yield return tearCreateTime;
        }        
    }
    
    IEnumerator PatternB()
    {
        yield return null;
    }

    IEnumerator PatternC()
    {
        // 1페이즈 패턴
        if(hp < 0.5f * maxHp)
        {
            int temp = Random.Range(0, 3);
            GameObject bar = attackBars[temp];
            yield return StartCoroutine(Warning(bar)); 
            yield return StartCoroutine(BarAttack(bar));
        }
        // 2페이즈 패턴
        else
        {
            int tempA, tempB;
            // tempA와 tempB를 다른 값으로
            tempA = Random.Range(0, 3);
            do
            {
                tempB = Random.Range(0, 3);
            }
            while (tempA == tempB);
            GameObject barA = attackBars[tempA];
            GameObject barB = attackBars[tempB];
            StartCoroutine(Warning(barA));
            yield return StartCoroutine(Warning(barB));
            StartCoroutine(BarAttack(barA));
            yield return StartCoroutine(BarAttack(barB));
        }
    }

    IEnumerator Warning(GameObject bar)
    {
        WaitForSeconds flickTime = new WaitForSeconds(0.1f);

        // 처음은 경고
        bar.GetComponent<Collider2D>().enabled = false;
        bar.SetActive(true);

        // 공격 경보 깜빡이게
        for (int count = 0; count < 2; count++)
        {
            for (int i = 1; i <= 10; i++)
            {
                bar.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.1f * i);
                yield return flickTime;
            }
            for (int i = 9; i >= 0; i--)
            {
                bar.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.1f * i);
                yield return flickTime;
            }
        }
    }

    IEnumerator BarAttack(GameObject bar)
    {
        WaitForSeconds flickTime = new WaitForSeconds(0.1f);

        // 점점 색 선명해지다가 1초 후 공격
        for(int i=1; i<=10; i++)
        {
            bar.GetComponent<SpriteRenderer>().color = new Color(0.933f, 0.867f, 0.616f, 0.1f * i);
            yield return flickTime;
        }
        bar.GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        bar.SetActive(false);
    }

    void OnDead()
    {
        isDead = true;

    }

}
