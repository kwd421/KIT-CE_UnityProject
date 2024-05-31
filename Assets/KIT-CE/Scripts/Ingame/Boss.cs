using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Boss : MonoBehaviour
{
    SpriteRenderer sprite;
    // 1������� 1���ȴ�
    Transform eyePatch1;
    // 1������� ��
    public Transform eye;
    // 2������� 2���ȴ�
    Transform eyePatch2;
    Rigidbody2D rigid;
    Vector2 initPos;
    bool phase2 = false;

    enum Pattern { A, B, C };

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
    // ���� ���� �� �������� ���� �ð�
    WaitForSeconds nextPattern = new WaitForSeconds(3f);

    [Header ("Pattern A")]
    public GameObject teardrop;
    bool eyeFlip = false;
    // ���� ������Ʈ Ǯ
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

        // ���ݿ� ���� ����(������ƮǮ)
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
            phase2 = true;
        }
        else if(hp <= 0)
        {
            OnDead();
        }
    }

    IEnumerator DamagedCoroutine()
    {
        // hp�� 6 �̻��� �� �� ���� �ǰ�
        if(!phase2)
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
            if (i % 3 == 0) temp.GetComponent<Teardrop>().teardropSpeed = 4f;
            else if (i % 2 == 1) temp.GetComponent<Teardrop>().teardropSpeed = 5f;
            else temp.GetComponent<Teardrop>().teardropSpeed = 6f;
            // ���� ��Ȱ��ȭ
            temp.SetActive(false);
            // ���� ������Ʈ Ǯ�� ���
            tears.Add(temp);
        }
    }

    IEnumerator BossPatterns()
    {
        // ��� ��ٸ��� ���� ����, ���� ���� �� ���
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
            // attackCount��ŭ ���� ����
            attackCount = 3;
            while (count < attackCount)
            {
                // ���� ���� ��ġ�� ������ ���� �� ����
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
        // ���� ���� ���� �ȱ� Ȱ��ȭ, 1.5�� �� �� ������ �������� ����
        // ����B(pillarParent)�� eye�� �ڽ����� ����
        eye.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);        
        pillarParent.SetActive(true);
        // ������ 1�� �� �ӵ�
        if(!phase2)
        {
            pillarParent.GetComponent<PatternB>().SetSpeed(4f);
        }
        else
        // ������ 2�� �� �ӵ�
        {
            pillarParent.GetComponent<PatternB>().SetSpeed(7f);
        }
        // 2�ʰ� ������� �ӵ���ŭ ȸ�� �� ��Ȱ��ȭ. ���� �������� �Ѿ��.
        yield return new WaitForSeconds(2f);
        eye.GetChild(0).gameObject.SetActive(false);
        pillarParent.SetActive(false);
    }

    // 1~3�������� ���ǿ� ���������� ����
    IEnumerator PatternC()
    {
        AudioManager.instance.PlaySfx(patternCSound);
        // 1������ ����: ������ �� �ϳ��� �������� ����
        if (!phase2)
        {
            int temp = Random.Range(0, 3);
            GameObject bar = attackBars[temp];
            // ���
            yield return StartCoroutine(Warning(bar)); 
            // ����
            yield return StartCoroutine(BarAttack(bar));
        }
        // 2������ ����: ���ڱ��� �� �ѿ� �������� ����
        else
        {
            int tempA, tempB;
            // tempA�� tempB�� �ߺ��̸� tempB�� �ٽ� �޾ƿ�
            tempA = Random.Range(0, 3);
            do
            {
                tempB = Random.Range(0, 3);
            }
            while (tempA == tempB);
            GameObject barA = attackBars[tempA];
            GameObject barB = attackBars[tempB];
            
            // ���
            StartCoroutine(Warning(barA));
            yield return StartCoroutine(Warning(barB));
            // ����
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

        // ó���� ���
        bar.GetComponent<Collider2D>().enabled = false;
        bar.SetActive(true);

        // ���� �溸 �����̰�
        for (int count = 0; count < 2; count++)
        {
            // �� ����
            for (int i = 1; i <= 10; i++)
            {
                bar.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.1f * i);                
                glow.volumeIntensity += 0.1f;
                yield return flickTime;
            }
            // �� ����
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

        // ���� �� ���������ٰ� 0.5�� �� ����
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
        // HP �ʱ�ȭ
        hp = maxHp;

        // isDead to false, Update ����
        isDead = false;

        // ��ġ �ʱ�ȭ
        transform.position = initPos;

        // ����A ���� �ʱ�ȭ
        foreach(GameObject tear in tears)
        {
            tear.SetActive(false);
        }
        // ����B �ȱ� �� ���� ��� �ʱ�ȭ
        // !!!!!!!!!!!!�������ڵ�!!!!!!!!!!!!
        //eye.GetComponent<Light2D>().gameObject.SetActive(false);
        pillarParent.SetActive(false);
        // ����C bar�� �ʱ�ȭ
        foreach(GameObject bar in attackBars)
        {
            bar.SetActive(false);
        }

        // �̵� �ڷ�ƾ ����
        StartCoroutine(Move());
        StartCoroutine(BossPatterns());
    }
}
