using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw_H : Enemy
{
    public Transform leftPoint; // 좌측 이동 지점
    public Transform rightPoint; // 우측 이동 지점

    int movingRight = 1; // 우측 이동 중인지 여부

    protected void Update()
    {
        // 좌우 이동
        rigid.velocity = new Vector2(movingRight * speed, rigid.velocity.y);

        // Chain 범위를 넘어갔다면 방향을 변경
        if (transform.position.x >= rightPoint.position.x)
        {
            movingRight = -1;
        }
        else if (transform.position.x <= leftPoint.position.x)
        {
            movingRight = 1;
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
