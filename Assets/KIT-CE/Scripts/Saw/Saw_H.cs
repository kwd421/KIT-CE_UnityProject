using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw_H : Enemy
{
    public Transform leftPoint; // ���� �̵� ����
    public Transform rightPoint; // ���� �̵� ����

    int movingRight = 1; // ���� �̵� ������ ����

    protected void Update()
    {
        // �¿� �̵�
        rigid.velocity = new Vector2(movingRight * speed, rigid.velocity.y);

        // Chain ������ �Ѿ�ٸ� ������ ����
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
