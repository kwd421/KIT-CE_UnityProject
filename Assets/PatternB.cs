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
        // 자식 오브젝트 순회
        for(int i=0; i<transform.childCount; i++)
        {
            pillar = transform.GetChild(i);
            // 기둥의 위치와 회전을 초기화
            pillar.localPosition = Vector3.zero;
            pillar.localRotation = Quaternion.identity;
            // 기둥을 활성화
            pillar.gameObject.SetActive(true);

            // 기둥을 일정한 간격으로 회전 (2D 회전 사용)
            float angle = 360f * i / transform.childCount;
            pillar.Rotate(Vector3.forward, angle);
            // 기둥을 회전한 방향으로 이동 (2D 이동 사용)
            pillar.Translate(Vector2.up * 3f, Space.Self);
        }
    }
}
