using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : Enemy
{
    public Transform player;
    public float range = 8.0f;
    public int moveSpeed = 1;

    bool istarget = false; // �÷��̾��� Ÿ�� ����
    bool isMovingToPlayer = false; // �÷��̾� ������ �̵� ������ ����
    bool hasCollidedWithPlatform = false; // platform�� �浹�ߴ��� ����
    Vector2 startPos; // Skull�� ���� ��ġ
    Vector2 playerDirection; // �÷��̾� ����

    protected override void Start()
    {
        startPos = transform.position;

        Think();
    }
    protected void Update()
    {
        if (isDead)
            return;

        CheckDistance();

        // �÷��̾� �������� �̵� ���̸鼭 ���� �浹���� ���� ��쿡�� �̵��� ����մϴ�.
        if (isMovingToPlayer && !hasCollidedWithPlatform)
        {
            // Flip Sprite
            if (transform.position.x - playerDirection.x > 0)
                sprite.flipX = true;
            else
                sprite.flipX = false;

            rigid.velocity = playerDirection * speed;
        }
    }
    protected override void FixedUpdate()
    {
        if (!isMovingToPlayer)
        {
            // ���� ��ġ���� range ���� �ȿ� �ִ��� Ȯ��
            if (Mathf.Abs(transform.position.x - startPos.x) >= range)
            {
                // range ���� �ȿ� ���� ������ �̵� ������ �ݴ�� ����
                nextMove *= -1;
            }

            // Move
            rigid.velocity = new Vector2(nextMove * moveSpeed, rigid.velocity.y);
        }
    }
    protected override void LateUpdate()
    {
        // Flip Sprite
        if (nextMove == -1)
        {
            sprite.flipX = true;
        }
        else if (nextMove == 1)
        {
            sprite.flipX = false;
        }
    }

    // player�� skull�� ���� ���� �ȿ� ���Դ��� �Ǻ��ϴ� �Լ�
    void CheckDistance()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= range)
        {
            if (!istarget)
            {
                playerDirection = (player.position - transform.position).normalized;
                isMovingToPlayer = true;
            }

            istarget = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            // �÷��̾� ������ platform�� �浹�� ��� �̵��� ����
            hasCollidedWithPlatform = true;

            anim.SetBool("isHitWall", true);

            HP -= 1;
            if (HP <= 0)
            {
                OnDead();
            }

            GameManager.instance.stagePoint += score;
        }
    }
    protected override void Think()
    {
        if (!isMovingToPlayer)
        {
            // Set Next Act
            nextMove = Random.Range(-1, 2);

            float nextThinkTime = Random.Range(2f, 5f);
            Invoke("Think", nextThinkTime);
        }
    }
    public override void Init()
    {
        base.Init();

        istarget = false;
        isMovingToPlayer = false;
        hasCollidedWithPlatform = false;
    }
    public override void OnDead()
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
}
