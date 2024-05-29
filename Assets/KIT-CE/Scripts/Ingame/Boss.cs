using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Boss : MonoBehaviour
{
    SpriteRenderer sprite;
    // 1페이즈용 눈, 1번안대
    Transform eyePatch1;
    Transform eye;
    // 2페이즈용 2번안대
    Transform eyePatch2;
    Rigidbody2D rigid;
    public GameObject teardrop;
    public float teardropSpeed;

    bool temp = false;
    public float moveSpeed;
    // 화면 밖으로 안나가게 체크
    Vector2 inView;

    // 이동유지 시간
    WaitForSeconds moveTime = new WaitForSeconds(3f);
    int dirX, dirY;

    int hp = 10;
    bool isDead = false;
    // 눈물 오브젝트 풀
    List<GameObject> tears = new List<GameObject>();
    WaitForSeconds tearTIme = new WaitForSeconds(0.5f);
    int tearCount = 3;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        eyePatch1 = transform.GetChild(0);
        eye = transform.GetChild(1);
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // 공격용 눈물 생성(오브젝트풀)
        MakeTears();
        // 이동 코루틴 시작
        StartCoroutine(Move());
    }


    private void LateUpdate()
    {
        // 플레이어를 항상 바라보게
        Vector2 dir = GameManager.instance.player.transform.position - transform.position;
        sprite.flipX = dir.x > 0 ? true : false;
        // 머리가 flip되면 눈 대칭 이동
        if(temp != sprite.flipX)
        {
            temp = sprite.flipX;
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

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // 화면 끝에 다다르면 반대로 이동
        inView = Camera.main.WorldToViewportPoint(transform.position);
        if (inView.x < 0.08f || inView.x > 0.92f)
        {
            // 수평 이동
            dirX = -dirX;
        }
        if (inView.y < 0.25f || inView.y > 0.75f)
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
        if(hp <= 0)
        {
            OnDead();
        }
    }

    IEnumerator DamagedCoroutine()
    {
        // hp가 6 이상일 때 한 눈만 피격
        if(hp > 5)
        {
            eye.GetComponent<Collider2D>().enabled = false;
            sprite.color = new Color(1, 0, 0, 0.8f);
            yield return new WaitForSeconds(2f);
            eye.GetComponent<Collider2D>().enabled = true;
            sprite.color = new Color(1, 1, 1);
        }
        // hp가 5 이하일 때 두 눈 모두 피격
        else if(hp <= 5)
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
        // 오브젝트 풀에 비활성화된 복셀을 담고 싶다.
        for (int i = 0; i < tearCount; i++)
        {
            // 1. 복셀 공장에서 복셀 생성하기
            GameObject temp = Instantiate(teardrop, eye.position, Quaternion.Euler(0, 0, 180));
            // 2. 복셀 비활성화하기
            temp.SetActive(false);
            // 3. 복셀을 오브젝트 풀에 담고 싶다.
            tears.Add(temp);
        }
    }

    void PatternA()
    {
        StartCoroutine(TearSet());
    }

    IEnumerator TearSet()
    {
        foreach (GameObject tear in tears)
        {
            tear.SetActive(true);
            yield return tearTIme;
        }
    }
    


    void OnDead()
    {
        isDead = true;
    }
}
