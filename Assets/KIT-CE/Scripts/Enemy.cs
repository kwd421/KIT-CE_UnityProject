using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;
    CircleCollider2D coll;

    public int score;
    public int HP;
    int nextMove;
    public int speed;
    bool isDead = false;

    protected WaitForSeconds deactiveTime = new WaitForSeconds(3);  // 3�ʰ� ��� ���� ����

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<CircleCollider2D>();
    }

    protected virtual void Start()
    {
        Think();
    }


    protected virtual void FixedUpdate()
    {
        if (isDead)
            return;
        // Move
        rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);

        // Platform Check
        Vector2 frontVec = new Vector2(transform.position.x + nextMove * 0.5f, transform.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1.5f, LayerMask.GetMask("Platform"));
        // ray�� ���������� ray�� �������� Layer�� Ž������ ������ distance�� 0�̶�� ������� ��
        // collider == null �� distance == 0�� �ϴ� �����ϰ� �۵��Ѵ�
        if (rayHit.collider == null || rayHit.distance == 0)
        {
            nextMove = -nextMove;
            CancelInvoke();
            Invoke("Think", 2);
        }
    }

    protected virtual void LateUpdate()
    {
        // Flip Sprite
        if (rigid.velocity.x != 0)
        {
            sprite.flipX = rigid.velocity.x > 0;
        }

    }

    // Recursive
    void Think()
    {
        // RandomRange(����, ������)
        // Set Next Act
        nextMove = Random.Range(-1, 2);

        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);

        // Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);
    }

    public virtual void OnDamaged()
    {
        // �ǰ� �ִϸ��̼� ���
        HP -= 1;
        if(HP <= 0)
        {
            OnDead();
        }
    }

    // ��� �� ȣ��
    public virtual void OnDead()
    {
        // isDead to true, Update ����
        isDead = true;

        // Velocity Zero, �ӵ� �ʱ�ȭ
        rigid.velocity = Vector3.zero;

        // Sprite Alpha, ����������, ��� �ִϸ��̼� �ִٸ� ���X
        sprite.color = new Color(1, 1, 1, 0.4f);

        // Sprite Flip Y, ���Ϲ���, ��� �ִϸ��̼� �ִٸ� ���X
        sprite.flipY = true;

        // Collider Disable, �Ʒ��� �߶�, ��� �ִϸ��̼� �ִٸ� ���X
        coll.enabled = false;

        // Die Effect Jump, ��� �ִϸ��̼� �ִٸ� ���X
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // ��� �ִϸ��̼� ���

        // Object Deactivate
        StartCoroutine(Deactive());
    }

    IEnumerator Deactive()
    {
        yield return deactiveTime;
        gameObject.SetActive(false);
    }
}
