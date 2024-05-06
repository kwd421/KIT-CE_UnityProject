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

    // ���� �̵�
    float moveInput;

    public bool isJumping = false;
    bool grounded = true;   // ����, ���� �Ǻ� ����
    bool isStop = false;
    bool isHit = false; // �ǰ� �� noJumpTime�ʸ�ŭ ���� �Ұ��� �ϱ� ���� ����
    int dir;

    // ���� �ð�
    WaitForSeconds invinTime = new WaitForSeconds(1.5f);
    // �ǰݽ� ���� �Ұ� �ð�
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
        // Ű�Է��� Update��, ���� �̵��� FixedUpdate����

        // JumpŰ ������ �� + ���� + �ǰ� ���� �ƴ� ��
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
        // �� �� ���� ������ �� ����ũ ����
        if (moveInput != 0)
            sprite.flipX = moveInput < 0;

        if (Mathf.Abs(rigid.velocity.x) < 1)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }


    void FixedUpdate()
    {
        // velocity: ���������� ����Ͽ� (Rigidbody��)�浹 �� ��ȣ�ۿ� O
        // Friction(������)�� ������� �����Ƿ� Friction ����Ϸ��� AddForce ����Ұ�
        //rigid.velocity = new Vector2(moveSpeed * moveInput, rigid.velocity.y);


        // Translate: �������� �̻��, �浹 �� ��ȣ�ۿ� X, ���� Rigidbody�� �Բ� ��� X
        // transform.Translate( * moveSpeed * Time.deltaTime);


        // AddForce: ������Ʈ�� ���� ������. �ַ� �÷��̾� �̵����ٴ� ����ü�� ���
        // AddForce�� ������ �ۿ������� Update�� ��� ȣ��ȴٸ� ���� �����ȴ�.
        // AddForce���� �ð��� �⺻����Ǿ� deltaTime��� �ȴ�.
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

        // Ray�� �߾ӿ� 1���̸� ���ο��� ����� �ν� �Ұ�, �¿� 2���� ����
        RaycastHit2D rightRayHit = Physics2D.Raycast(transform.position + new Vector3(0.2f, 0, 0), Vector3.down, 0.3f, LayerMask.GetMask("Platform"));
        RaycastHit2D leftRayHit = Physics2D.Raycast(transform.position + new Vector3(-0.2f, 0, 0), Vector3.down, 0.3f, LayerMask.GetMask("Platform"));

        // �¿� ray ��� ���� �ν� ��
        if (rightRayHit.collider == null && leftRayHit.collider == null)
        {
            grounded = false;   // ü����, Jump �Ұ�
        }
        else
        {
            // �� ������ ������ �÷����� �ν��ϴ� ���� ������ �������� ���� �������� + - �Ǿ� ���� ���� �߻�
            // 0�� �ƴ� �ٸ����϶��� �߻�(eg. distance < 0.05f)
            if (rightRayHit.distance == 0 || leftRayHit.distance == 0)
            {
                grounded = true;    // ����, Jump ����
            }
        }

        // ���� grounded�ʹ� ������, ���� ���۽� collider != null�� distance == 0 ������ �����ϱ� ������
        // ������ �� ������ ������ ����� ���� �ִϸ��̼� ���/����� ����
        if (rigid.velocity.y <= 0)
        {
            // ray�� Platform�� Hit�� distance�� 0�� �� ���� �ִϸ��̼� ����(���� �پ����� ��)
            if (rightRayHit.collider != null || leftRayHit.collider != null)
            {
                if (rightRayHit.distance == 0 || leftRayHit.distance == 0)
                {
                    anim.SetBool("isJumping", false);
                }
            }
        }
        dir = rigid.velocity.x > 0 ? -1 : 1;    // hit�� ���� ����
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
        // ���߿����� ���� �ӵ� ���� ���� velocity.x ���
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        isJumping = false;
    }

    void Stop()
    {
        rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        isStop = false;
    }

    // OnCollision�� ������ �浹�� ���� �� ���(isTrigger Off)
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
        // Enemy�� ���� ��ӵ� _enemy�� Get. Override�� OnDamaged()�Լ��� �ִٸ� �װ��� ����
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
        // TimeMap Collider 2D�� �������� ���ֵ� ��ü�� ���� �ݶ��̴��� ����ϱ� ������ ����, ������ Spike��
        // �������ִٰ� �� �� �����ʿ��� ������ Spike�� �浹�� �� TileMap Position���� ���ʿ� ��ġ�Ͽ� �������� ƨ��� �ȴ�.
        // ���� dir�� Spike�� �浹 �� ���� player �ӵ� ����, Enemy�� �浹 �� position �������� ����
        // AddForce ��� �� �������� �Ǿ� velocity�� ����
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
