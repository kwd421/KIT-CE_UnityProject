using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rino : Enemy
{
    [SerializeField] private bool isDash = false;
    private Vector3 dashDir = Vector3.zero;
    protected override void FixedUpdate()
    {
        if(isDead)
            return;

        if (isDash)
        {
            rigid.velocity = new Vector2(dashDir.x * speed * 5, rigid.velocity.y); 

            float moveXDir = (dashDir.x / Mathf.Abs(dashDir.x));

            // 다음 이동지가 맵 밖을 벗어나는지 확인
            Vector2 frontVec = new Vector2(transform.position.x + moveXDir, transform.position.y);
            Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1.5f, LayerMask.GetMask("Platform"));

            if (rayHit.collider == null)
            {
                isDash = false;
            }


            // 앞으로 달려나가는 중 벽에 부딧히면 데미지
            Vector2 hitCheckPoisition = new Vector2(transform.position.x, transform.position.y + 0.7f);
            Debug.DrawRay(hitCheckPoisition, Vector3.right * moveXDir, new Color(0, 1, 0));
            RaycastHit2D wallHit = Physics2D.Raycast(hitCheckPoisition, Vector3.right * moveXDir, 0.7f, LayerMask.GetMask("Platform"));
            if (wallHit.collider != null)
            {
                isDash = false;

                anim.SetTrigger("isHitWall");

                HP -= 1;
                if (HP <= 0)
                {
                    rigid.velocity = Vector2.zero;
                    OnDead();
                }

                GameManager.instance.stagePoint += score;
            }
        }
        else
        {
            base.FixedUpdate();
            FindPlayer();
        }
    }

    private void FindPlayer()
    {
        var hit = Physics2D.CircleCast(transform.position, 5.0f, Vector2.up, 0.0f, LayerMask.GetMask("Player"));

        if (hit.collider != null)
        {
            dashDir = (hit.collider.transform.position - transform.position).normalized;
            isDash = true;
        }        
    }
}
