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

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        playerPos = GameManager.instance.player.transform.position;
        spawnPos = transform.parent.position;
        // ��ġ, �ӵ��� �ʱ�ȭ
        transform.position = spawnPos;
        rigid.velocity = Vector3.zero;
        // ����, �ӵ� ����
        dir = (playerPos - spawnPos).normalized;
        rigid.AddForce(new Vector2(teardropSpeed * dir.x, teardropSpeed), ForceMode2D.Impulse);

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
        // �ӵ� ������ �������� ȸ�� ����
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
