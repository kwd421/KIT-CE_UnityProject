using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RockEnemy : Enemy
{
    UnityEngine.Vector3 originPosition;
    [SerializeField] bool isDown = false;

    protected override void Start()
    {
        originPosition = transform.position;
    }

    protected override void OnEnable()
    {

        StopAllCoroutines();
        CancelInvoke();
        isDown = false;

        base.OnEnable();
    }

    protected override void FixedUpdate()
    {
        if (isDown)
            return;

        // 밑으로 raycast를 발사하여 플레이어가 잇는지 확인 
        Debug.DrawRay(transform.position, UnityEngine.Vector3.down * 10, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, UnityEngine.Vector3.down, 10f, LayerMask.GetMask("Player"));

        if (rayHit.collider != null)
        {
            isDown = true;

            rigid.bodyType = RigidbodyType2D.Dynamic;

            anim.SetTrigger("Bottom");
            
            Invoke("Reset", 3.0f);
        }
    }

    // 원래 자리로 이동
    void Reset()
    {
        rigid.bodyType = RigidbodyType2D.Static;

        StartCoroutine(ResetCourtine());
    }

    // 원래 자리로 돌아가게 하는 코루틴
    IEnumerator ResetCourtine()
    {
        while(true)
        {
            UnityEngine.Vector3 moveDir = (originPosition - transform.position).normalized;
            transform.Translate(moveDir * speed * Time.deltaTime);

            if (Mathf.Abs(UnityEngine.Vector3.Distance(transform.position, originPosition)) <= 0.1f )
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        isDown = false;
    }
}
