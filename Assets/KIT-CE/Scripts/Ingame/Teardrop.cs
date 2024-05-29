using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teardrop : MonoBehaviour
{
    Rigidbody2D rigid;
    public float teardropSpeed;
    Vector2 dir;
    float disappearTime = 4f;
    float currentTime;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        dir = GameManager.instance.player.transform.position - transform.parent.position;
    }

    private void OnEnable()
    {
        rigid.AddForce(new Vector2(teardropSpeed * dir.normalized.x, teardropSpeed), ForceMode2D.Impulse);
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
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90)); // -90�� ����
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name.Equals("Player"))
        {
            transform.gameObject.SetActive(false);
        }
    }
}
