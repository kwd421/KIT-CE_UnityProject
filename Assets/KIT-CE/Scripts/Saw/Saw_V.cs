using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw_V : Enemy
{
    public Transform topPoint; // ��� �̵� ����
    public Transform bottomPoint; // �ϴ� �̵� ����

    int movingUp = 1; // ��� ������ ����

    protected void Update()
    {
        // ���� �̵�
        rigid.velocity = new Vector2(rigid.velocity.x, movingUp * speed);

        // Chain ������ �Ѿ�ٸ� ������ ����
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