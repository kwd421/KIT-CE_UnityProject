using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw_V : Enemy
{
    public Transform topPoint; // 상단 이동 지점
    public Transform bottomPoint; // 하단 이동 지점

    int movingUp = 1; // 상승 중인지 여부

    protected void Update()
    {
        // 상하 이동
        rigid.velocity = new Vector2(rigid.velocity.x, movingUp * speed);

        // Chain 범위를 넘어갔다면 방향을 변경
        if (transform.position.y >= topPoint.position.y)
        {
            movingUp = -1;
        }
        else if (transform.position.y <= bottomPoint.position.y)
        {
            movingUp = 1;
        }
        
    }
    protected override void Start()
    {

    }
    protected override void FixedUpdate()
    {

    }
    protected override void LateUpdate()
    {

    }
}