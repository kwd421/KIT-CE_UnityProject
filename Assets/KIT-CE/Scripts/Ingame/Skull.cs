using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : Enemy
{
    public Transform player;
    public float range = 8.0f;
    public int moveSpeed = 1;

    bool istarget = false; // 플레이어의 타겟 여부
    bool isMovingToPlayer = false; // 플레이어 쪽으로 이동 중인지 여부
    bool hasCollidedWithPlatform = false; // platform에 충돌했는지 여부
    Vector2 startPos; // Skull의 시작 위치
    Vector2 playerDirection; // 플레이어 방향

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

        // 플레이어 방향으로 이동 중이면서 아직 충돌하지 않은 경우에만 이동을 계속합니다.
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
            // 현재 위치에서 range 범위 안에 있는지 확인
            if (Mathf.Abs(transform.position.x - startPos.x) >= range)
            {
                // range 범위 안에 있지 않으면 이동 방향을 반대로 변경
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

    // player가 skull의 공격 범위 안에 들어왔는지 판별하는 함수
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
            // 플레이어 방향의 platform에 충돌한 경우 이동을 멈춤
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
        // isDead to true, Update 중지
        isDead = true;

        // Velocity Zero, 속도 초기화
        rigid.velocity = Vector3.zero;

        // Sprite Alpha, 반투명해짐, 사망 애니메이션 있다면 사용X
        sprite.color = new Color(1, 1, 1, 0.4f);

        // Sprite Flip Y, 상하반전, 사망 애니메이션 있다면 사용X
        sprite.flipY = true;

        // Collider Disable, 아래로 추락, 사망 애니메이션 있다면 사용X
        coll.enabled = false;

        // Die Effect Jump, 사망 애니메이션 있다면 사용X
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // 사망 애니메이션 재생

        // Object Deactivate
        StartCoroutine(Deactive());
    }
}
