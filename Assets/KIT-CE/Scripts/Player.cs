using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float jumpPower;
    public WalkingEnemy enemy;

    Rigidbody2D rigid;
    SpriteRenderer sprite;
    public Animator anim;
    CapsuleCollider2D coll;

    // 수평 이동
    float moveInput;

    public bool isJumping = false;
    bool grounded = true;   // 공중, 지상 판별 변수
    bool isStop = false;
    bool isHit = false; // 피격 시 noJumpTime초만큼 점프 불가케 하기 위한 변수
    int dir;

    // 무적 시간
    WaitForSeconds invinTime = new WaitForSeconds(1.5f);
    // 피격시 점프 불가 시간
    WaitForSeconds noJumpTime = new WaitForSeconds(0.5f);

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

        // Jump키 눌렀을 때 + 지상 + 피격 상태 아닐 때
        if (Input.GetButtonDown("Jump") && grounded && !isHit)
        {
            isJumping = true;
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
        //rigid.velocity = new Vector2(moveSpeed * moveInput, rigid.velocity.y);


        // Translate: 물리엔진 미사용, 충돌 시 상호작용 X, 보통 Rigidbody와 함께 사용 X
        // transform.Translate( * moveSpeed * Time.deltaTime);


        // AddForce: 오브젝트에 힘을 가해줌. 주로 플레이어 이동보다는 투사체에 사용
        // AddForce는 순간적 작용이지만 Update시 계속 호출된다면 힘이 누적된다.
        // AddForce에는 시간이 기본적용되어 deltaTime없어도 된다.
        rigid.AddForce(Vector2.right * moveInput, ForceMode2D.Impulse);

        if (rigid.velocity.x > moveSpeed) // Right Max Speed
        {
            rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y );
        }
        else if (rigid.velocity.x < -moveSpeed) // Left Max Speed
        {
            rigid.velocity = new Vector2(-moveSpeed, rigid.velocity.y);
        }

        if (isStop)
            Stop();

        if (isJumping)
            Jump();

        Debug.DrawRay(transform.position + new Vector3(0.2f, 0, 0), Vector3.down * 0.3f, new Color(0, 1, 0));
        Debug.DrawRay(transform.position + new Vector3(-0.2f, 0, 0), Vector3.down * 0.3f, new Color(0, 1, 0));

        // Ray가 중앙에 1개이면 경사로에서 제대로 인식 불가, 좌우 2개로 변경
        RaycastHit2D rightRayHit = Physics2D.Raycast(transform.position + new Vector3(0.2f, 0, 0), Vector3.down, 0.3f, LayerMask.GetMask("Platform"));
        RaycastHit2D leftRayHit = Physics2D.Raycast(transform.position + new Vector3(-0.2f, 0, 0), Vector3.down, 0.3f, LayerMask.GetMask("Platform"));

        // 좌우 ray 모두 공중 인식 시
        if (rightRayHit.collider == null && leftRayHit.collider == null)
        {
            grounded = false;   // 체공중, Jump 불가
        }
        else
        {
            // 이 조건이 없으면 플랫폼을 인식하는 순간 점프시 내려오는 힘과 점프힘이 + - 되어 낮은 점프 발생
            // 0이 아닌 다른값일때도 발생(eg. distance < 0.05f)
            if (rightRayHit.distance == 0 || leftRayHit.distance == 0)
            {
                grounded = true;    // 지상, Jump 가능
            }
        }

        // 위의 grounded와는 별개로, 점프 시작시 collider != null과 distance == 0 조건을 만족하기 때문에
        // 떨어질 때 조건을 별개로 만들어 점프 애니메이션 재생/미재생 구분
        if (rigid.velocity.y <= 0)
        {
            // ray가 Platform에 Hit한 distance가 0일 때 점프 애니메이션 해제(땅에 붙어있을 때)
            if (rightRayHit.collider != null || leftRayHit.collider != null)
            {
                if (rightRayHit.distance == 0 || leftRayHit.distance == 0)
                {
                    anim.SetBool("isJumping", false);
                }
            }
        }
        dir = rigid.velocity.x > 0 ? -1 : 1;    // hit시 방향 결정
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
        anim.SetBool("isJumping", true);
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
            StartCoroutine(InvincibleCoroutine(new Vector2(-1000, -1000)));
        }
    }

    void OnAttack(Transform _enemy)
    {
        Enemy enemy = _enemy.GetComponent<Enemy>();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Attack);
        // Point
        GameManager.instance.stagePoint += enemy.score;

        // Player Jump Force
        rigid.AddForce(Vector2.up * 12, ForceMode2D.Impulse);

        // Enemy Die
        // Enemy에 의해 상속된 _enemy를 Get. Override된 OnDamaged()함수가 있다면 그것이 사용됨
        enemy.OnDamaged();
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
        // 따라서 dir은 Spike와 충돌 시 현재 player 속도 기준, Enemy와 충돌 시 position 기준으로 변경
        // AddForce 사용 시 슈퍼점프 되어 velocity로 변경
        /*
                if(rigid.velocity.y > 0 && !grounded)
                {
                    rigid.AddForce(new Vector2(dir * 2, 0) * 20, ForceMode2D.Impulse);
                }
                else
                    rigid.AddForce(new Vector2(dir * 2, 1) * 20, ForceMode2D.Impulse);
        */
        if(targetPos ==  new Vector2(-1000, -1000)) 
        {
            rigid.velocity = new Vector2(dir * 1.5f, 1) * 20;
        }
        else
        {
            int dirE = transform.position.x - targetPos.x > 0 ? 1 : -1;
            rigid.velocity = new Vector2(dirE * 1.5f, 1) * 20;
        }
        

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
        isHit = true;   
        yield return noJumpTime;
        isHit = false;
        yield return invinTime;
        OffDamaged();
    }

    public void Init()
    {
        // Sprite Alpha
        sprite.color = new Color(1, 1, 1);

        // Sprite Flip Y
        sprite.flipY = false;

        // Collider Disable
        coll.enabled = true;
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
