using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternB : MonoBehaviour
{
    float speed;
    void OnEnable()
    {
        Init();
    }

    void Update()
    {
        transform.Rotate(Vector3.back * speed * 10f * Time.deltaTime);
    }

    public void SetSpeed(float value)
    {
        speed = value;
    }

    void Init()
    {
        Transform pillar;
        // �ڽ� ������Ʈ ��ȸ
        for(int i=0; i<transform.childCount; i++)
        {
            pillar = transform.GetChild(i);
            // ����� ��ġ�� ȸ���� �ʱ�ȭ
            pillar.localPosition = Vector3.zero;
            pillar.localRotation = Quaternion.identity;
            // ����� Ȱ��ȭ
            pillar.gameObject.SetActive(true);

            // ����� ������ �������� ȸ�� (2D ȸ�� ���)
            float angle = 360f * i / transform.childCount;
            pillar.Rotate(Vector3.forward, angle);
            // ����� ȸ���� �������� �̵� (2D �̵� ���)
            pillar.Translate(Vector2.up * 3f, Space.Self);
        }
    }
}
