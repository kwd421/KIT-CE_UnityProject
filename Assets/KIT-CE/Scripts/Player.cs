using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float jumpPower;
    public Enemy enemy;

    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;
    CapsuleCollider2D coll;

    // 수평 이동
    float moveInput;

    bool isJumping = false;
    bool isStop = false;


    // 무적 시간
    WaitForSeconds invinTime = new WaitForSeconds(3);

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        // 키입력은 Update에, 실제 이동은 FixedUpdate에서

        // Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            isJumping = true;
            anim.SetBool("isJumping", true);
        }

        // Move Speed
        moveInput = Input.GetAxisRaw("Horizontal");

        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
            isStop = true;

        // Sprite Direction
        // 좌 우 같이 눌렀을 때 문워크 방지
        if (moveInput != 0)
            sprite.flipX = moveInput < 0;

        if (Mathf.Abs(rigid.velocity.x) < 1)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }


    void FixedUpdate()
    {
        // velocity: 물리엔진을 사용하여 (Rigidbody와)충돌 시 상호작용 O
        // Friction(마찰력)에 영향받지 않으므로 Friction 사용하려면 AddForce 사용할것
        // rigid.velocity = new Vector2(moveSpeed * moveInput, rigid.velocity.y);


        // Translate: 물리엔진 미사용, 충돌 시 상호작용 X, 보통 Rigidbody와 함께 사용 X
        // transform.Translate( * moveSpeed * Time.deltaTime);


        // AddForce: 오브젝트에 힘을 가해줌. 주로 플레이어 이동보다는 투사체에 사용
        // AddForce는 순간적 작용이지만 Update시 계속 호출된다면 힘이 누적된다.
        // AddForce에는 시간이 기본적용되어 deltaTime없어도 된다.
        rigid.AddForce(Vector2.right * moveInput, ForceMode2D.Impulse);

        if (rigid.velocity.x > moveSpeed) // Right Max Speed
        {
            rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < -moveSpeed) // Left Max Speed
        {
            rigid.velocity = new Vector2(-moveSpeed, rigid.velocity.y);
        }

        if (isStop)
            Stop();

        if (isJumping)
            Jump();


        // Landing Platform
        if (rigid.velocity.y <= 0)
        {
            Debug.DrawRay(transform.position + new Vector3(0.2f, 0, 0), Vector3.down, new Color(0, 1, 0));
            Debug.DrawRay(transform.position + new Vector3(-0.2f, 0, 0), Vector3.down, new Color(0, 1, 0));

            // Ray가 중앙에 1개이면 경사로에서 제대로 인식 불가함
            RaycastHit2D rightRayHit = Physics2D.Raycast(transform.position + new Vector3(0.2f, 0, 0), Vector3.down, 1, LayerMask.GetMask("Platform"));
            RaycastHit2D leftRayHit = Physics2D.Raycast(transform.position + new Vector3(-0.2f, 0, 0), Vector3.down, 1, LayerMask.GetMask("Platform"));


            // ray가 Platform에 Hit한 distance가 0.5 미만일 시 점프 활성화 및 점프 애니메이션 해제
            if (rightRayHit.collider != null || leftRayHit.collider != null)
            {
                if (rightRayHit.distance < 0.5f || leftRayHit.distance < 0.5f)
                {
                    anim.SetBool("isJumping", false);
                }
                    
            }
        }
    }

    /*
    private void LateUpdate()
    {
        if (moveInput != 0)
            sprite.flipX = moveInput < 0;
    }
    */
    void Jump()
    {
        // 공중에서도 기존 속도 유지 위해 velocity.x 사용
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        isJumping = false;
    }

    void Stop()
    {
        rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        isStop = false;
    }

    // OnCollision은 실제적 충돌이 있을 때 사용(isTrigger Off)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            // Attack
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y
                && Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(collision.transform.position.x)) < 0.75)
            {
                OnAttack(collision.transform);
            }
            // Damaged
            else
                StartCoroutine(InvincibleCoroutine(collision.transform.position));
        }
        else if (collision.gameObject.tag == "Spike")
        {
            StartCoroutine(InvincibleCoroutine(collision.transform.position));
        }
    }

    // OnTrigger는 실제적 충돌이 없을 때 사용(isTrigger On)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // collision.tag 와 collision.gameObject.tag는 동일하다
        if (collision.gameObject.tag == "Item")
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Item);
            // Point
            string coinType = collision.name;
            if (coinType.Contains("Bronze"))
                GameManager.instance.stagePoint += 50;
            else if (coinType.Contains("Silver"))
                GameManager.instance.stagePoint += 100;
            else if (coinType.Contains("Gold"))
                GameManager.instance.stagePoint += 200;

            // Deactive Item
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Finish);
            // Next Stage
            GameManager.instance.NextStage();
        }
    }


    void OnAttack(Transform enemy)
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Attack);
        // Point
        GameManager.instance.stagePoint += 100;

        // Player Jump Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        // Enemy Die
        Enemy enemyMove = enemy.GetComponent<Enemy>();
        enemyMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {

        // Health Down
        GameManager.instance.HealthDown();

        // Invincible Layer
        gameObject.layer = 11;

        // View Alpha
        sprite.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        // TimeMap Collider 2D는 여러개를 놔둬도 전체를 단일 콜라이더로 취급하기 때문에 왼쪽, 오른쪽 Spike가
        // 나눠져있다고 할 때 오른쪽에서 왼쪽의 Spike에 충돌할 시 TileMap Position기준 왼쪽에 위치하여 왼쪽으로 튕기게 된다.
        // 따라서 dir을 나누어 왼쪽, 오른쪽으로 튕기게 한 것은 의미가 없다.
        int dir = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dir, 1) * 7, ForceMode2D.Impulse);

        // Animation
        anim.SetTrigger("isDamaged");
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        sprite.color = new Color(1, 1, 1, 1);
    }

    IEnumerator InvincibleCoroutine(Vector2 targetPos)
    {
        OnDamaged(targetPos);
        yield return invinTime;
        OffDamaged();
    }

    public void Dead()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Die);
        // Sprite Alpha
        sprite.color = new Color(1, 1, 1, 0.4f);

        // Sprite Flip Y
        sprite.flipY = true;

        // Collider Disable
        coll.enabled = false;

        // Die Effect Jump
        VelocityZero();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
