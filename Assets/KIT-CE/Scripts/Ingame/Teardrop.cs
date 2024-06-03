using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Teardrop : MonoBehaviour
{
    Rigidbody2D rigid;
    public float teardropSpeed;
    Vector2 dir;
    float disappearTime = 4f;
    float currentTime;
    Vector2 playerPos;
    Vector2 spawnPos;
    // 첫 활성화 시 부모오브젝트 받아옴
    GameObject parent;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        parent = transform.parent.gameObject;
    }
    private void OnEnable()
    {
        // 현재 플레이어의 위치
        playerPos = GameManager.instance.player.transform.position;
        // 눈물이 나올 위치는 Awake에서 받아왔던 오브젝트(eye)의 위치
        spawnPos = parent.transform.position;
        // 위치, 속도값 초기화
        transform.position = spawnPos;
        rigid.velocity = Vector3.zero;
        // 방향, 속도 설정
        dir = (playerPos - spawnPos).normalized;
        rigid.AddForce(new Vector2(teardropSpeed * dir.x, teardropSpeed), ForceMode2D.Impulse);

        // 부모 해제(부모의 Y축 flip따라 눈물도 flip되지 않게)
        transform.SetParent(null);
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime > disappearTime)
        {
            currentTime = 0;
            transform.gameObject.SetActive(false);
        }
        // 속도 벡터의 방향으로 회전 설정
        float angle = Mathf.Atan2(rigid.velocity.y, rigid.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name.Equals("Player"))
        {
            transform.gameObject.SetActive(false);
        }
    }
}
