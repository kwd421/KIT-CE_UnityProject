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

    // ���� �̵�
    float moveInput;

    bool isJumping = false;
    bool isStop = false;


    // ���� �ð�
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
        // Ű�Է��� Update��, ���� �̵��� FixedUpdate����

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
        // rigid.velocity = new Vector2(moveSpeed * moveInput, rigid.velocity.y);


        // Translate: �������� �̻��, �浹 �� ��ȣ�ۿ� X, ���� Rigidbody�� �Բ� ��� X
        // transform.Translate( * moveSpeed * Time.deltaTime);


        // AddForce: ������Ʈ�� ���� ������. �ַ� �÷��̾� �̵����ٴ� ����ü�� ���
        // AddForce�� ������ �ۿ������� Update�� ��� ȣ��ȴٸ� ���� �����ȴ�.
        // AddForce���� �ð��� �⺻����Ǿ� deltaTime��� �ȴ�.
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

            // Ray�� �߾ӿ� 1���̸� ���ο��� ����� �ν� �Ұ���
            RaycastHit2D rightRayHit = Physics2D.Raycast(transform.position + new Vector3(0.2f, 0, 0), Vector3.down, 1, LayerMask.GetMask("Platform"));
            RaycastHit2D leftRayHit = Physics2D.Raycast(transform.position + new Vector3(-0.2f, 0, 0), Vector3.down, 1, LayerMask.GetMask("Platform"));


            // ray�� Platform�� Hit�� distance�� 0.5 �̸��� �� ���� Ȱ��ȭ �� ���� �ִϸ��̼� ����
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
            StartCoroutine(InvincibleCoroutine(collision.transform.position));
        }
    }

    // OnTrigger�� ������ �浹�� ���� �� ���(isTrigger On)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // collision.tag �� collision.gameObject.tag�� �����ϴ�
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
        // TimeMap Collider 2D�� �������� ���ֵ� ��ü�� ���� �ݶ��̴��� ����ϱ� ������ ����, ������ Spike��
        // �������ִٰ� �� �� �����ʿ��� ������ Spike�� �浹�� �� TileMap Position���� ���ʿ� ��ġ�Ͽ� �������� ƨ��� �ȴ�.
        // ���� dir�� ������ ����, ���������� ƨ��� �� ���� �ǹ̰� ����.
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
