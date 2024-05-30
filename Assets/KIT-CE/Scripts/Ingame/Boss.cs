using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss : MonoBehaviour
{
    SpriteRenderer sprite;
    // 1������� 1���ȴ�
    Transform eyePatch1;
    // 1������� ��
    Transform eye;
    // 2������� 2���ȴ�
    Transform eyePatch2;
    Rigidbody2D rigid;

    // ȭ�� ������ �ȳ����� üũ
    Vector2 inView;

    [Header ("Boss Infos")]
    public int maxHp;
    int hp;
    // �̵��ӵ�
    public float moveSpeed;
    // �̵����� �ð�
    WaitForSeconds moveTime = new WaitForSeconds(3f);
    // ����/���� �̵�����
    int dirX, dirY;
    bool isDead = false;

    [Header ("Pattern A")]
    public GameObject teardrop;
    bool eyeFlip = false;
    // ���� ������Ʈ Ǯ
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
        // ���ݿ� ���� ����(������ƮǮ)
        MakeTears();
        // �̵� �ڷ�ƾ ����
        StartCoroutine(Move());
        StartCoroutine(PatternC());
    }


    private void LateUpdate()
    {
        // �÷��̾ �׻� �ٶ󺸰�
        Vector2 dir = GameManager.instance.player.transform.position - transform.position;
        sprite.flipX = dir.x > 0 ? true : false;
        // �Ӹ��� flip�Ǹ� �� ��Ī �̵�
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
        // �÷��̾�� �浹���� ��
        if(collision.name.Equals("Player"))
        {
            Player player = GameManager.instance.player;
            // �÷��̾ ����� ��
            if(player.GetComponent<Rigidbody2D>().velocity.y < 0 && player.transform.position.y > eyePatch1.position.y)
            {
                // �÷��̾ �����ϰ�
                player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 22, ForceMode2D.Impulse);
                OnDamaged();
            }
        }
    }

    private void FixedUpdate()
    {
        // ȭ�� ���� �ٴٸ��� �ݴ�� �̵�
        inView = Camera.main.WorldToViewportPoint(transform.position);
        if (inView.x < 0.13f || inView.x > 0.87f)
        {
            // ���� �̵�
            dirX = -dirX;
        }
        if (inView.y < 0.28f || inView.y > 0.7f)
        {
            // ���� �̵�
            dirY = -dirY;
        }

        // ���� �̵��ӵ�
        rigid.velocity = new Vector2(moveSpeed * dirX, moveSpeed * dirY);
    }

    IEnumerator Move()
    {
        // -1, 0, 1 �� 0�� ������ ���� �޾ƿ� ���Ⱚ���� Ȱ��
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
        // 2������ ON
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
        // hp�� 6 �̻��� �� �� ���� �ǰ�
        if(hp > 0.5f * maxHp)
        {
            eye.GetComponent<Collider2D>().enabled = false;
            sprite.color = new Color(1, 0, 0, 0.8f);
            yield return new WaitForSeconds(2f);
            eye.GetComponent<Collider2D>().enabled = true;
            sprite.color = new Color(1, 1, 1);
        }
        // hp�� 5 ������ �� �� �� ��� �ǰ�
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
        // ������Ʈ Ǯ�� ���� �߰�
        for (int i = 0; i < tearCount; i++)
        {
            // ���� ����, �θ�� ��
            GameObject temp = Instantiate(teardrop, eye);
            // ���� ��Ȱ��ȭ
            temp.SetActive(false);
            // ���� ������Ʈ Ǯ�� ���
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
        // 1������ ����
        if(hp < 0.5f * maxHp)
        {
            int temp = Random.Range(0, 3);
            GameObject bar = attackBars[temp];
            yield return StartCoroutine(Warning(bar)); 
            yield return StartCoroutine(BarAttack(bar));
        }
        // 2������ ����
        else
        {
            int tempA, tempB;
            // tempA�� tempB�� �ٸ� ������
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

        // ó���� ���
        bar.GetComponent<Collider2D>().enabled = false;
        bar.SetActive(true);

        // ���� �溸 �����̰�
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

        // ���� �� ���������ٰ� 1�� �� ����
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
