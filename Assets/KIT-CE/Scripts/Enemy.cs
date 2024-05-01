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

    protected WaitForSeconds deactiveTime = new WaitForSeconds(3);  // 3초간 사망 관련 진행

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
        // ray의 시작점에서 ray의 끝점까지 Layer가 탐지되지 않으면 distance가 0이라고 나오기는 함
        // collider == null 과 distance == 0은 일단 동일하게 작동한다
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
        // RandomRange(포함, 미포함)
        // Set Next Act
        nextMove = Random.Range(-1, 2);

        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);

        // Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);
    }

    public virtual void OnDamaged()
    {
        // 피격 애니메이션 재생
        HP -= 1;
        if(HP <= 0)
        {
            OnDead();
        }
    }

    // 사망 시 호출
    public virtual void OnDead()
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

    IEnumerator Deactive()
    {
        yield return deactiveTime;
        gameObject.SetActive(false);
    }
}
