using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;
    CircleCollider2D coll;

    int nextMove;
    public int speed;

    WaitForSeconds deactiveTime = new WaitForSeconds(5);

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        Think();
    }


    void FixedUpdate()
    {
        // Move
        rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);

        // Platform Check
        Vector2 frontVec = new Vector2(transform.position.x + nextMove * 0.3f, transform.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0,1,0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1.5f, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            nextMove = -nextMove;
            CancelInvoke();
            Invoke("Think", 2);
        }
    }

    private void LateUpdate()
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

    public void OnDamaged()
    {
        // Sprite Alpha
        sprite.color = new Color(1, 1, 1, 0.4f);

        // Sprite Flip Y
        sprite.flipY = true;

        // Collider Disable
        coll.enabled = false;

        // Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // Object Deactivate
        StartCoroutine(Deactive());
    }

    IEnumerator Deactive()
    {
        yield return deactiveTime;
        gameObject.SetActive(false);
    }
}
