using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Boss : MonoBehaviour
{
    SpriteRenderer sprite;
    // 1������� ��, 1���ȴ�
    Transform eyePatch1;
    Transform eye;
    // 2������� 2���ȴ�
    Transform eyePatch2;
    Rigidbody2D rigid;
    public GameObject teardrop;

    bool temp = false;
    public float moveSpeed;
    // ȭ�� ������ �ȳ����� üũ
    Vector2 inView;

    // �̵����� �ð�
    WaitForSeconds moveTime = new WaitForSeconds(3f);
    int dirX, dirY;

    int hp = 10;
    bool isDead = false;
    // ���� ������Ʈ Ǯ
    List<GameObject> tears = new List<GameObject>();
    WaitForSeconds tearCreateTime = new WaitForSeconds(0.5f);
    public int tearCount;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        eyePatch1 = transform.GetChild(0);
        eye = transform.GetChild(1);
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // ���ݿ� ���� ����(������ƮǮ)
        MakeTears();
        // �̵� �ڷ�ƾ ����
        StartCoroutine(Move());
        StartCoroutine(PatternA());
    }


    private void LateUpdate()
    {
        // �÷��̾ �׻� �ٶ󺸰�
        Vector2 dir = GameManager.instance.player.transform.position - transform.position;
        sprite.flipX = dir.x > 0 ? true : false;
        // �Ӹ��� flip�Ǹ� �� ��Ī �̵�
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
        if(hp <= 0)
        {
            OnDead();
        }
    }

    IEnumerator DamagedCoroutine()
    {
        // hp�� 6 �̻��� �� �� ���� �ǰ�
        if(hp > 5)
        {
            eye.GetComponent<Collider2D>().enabled = false;
            sprite.color = new Color(1, 0, 0, 0.8f);
            yield return new WaitForSeconds(2f);
            eye.GetComponent<Collider2D>().enabled = true;
            sprite.color = new Color(1, 1, 1);
        }
        // hp�� 5 ������ �� �� �� ��� �ǰ�
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
    


    void OnDead()
    {
        isDead = true;

    }

}
