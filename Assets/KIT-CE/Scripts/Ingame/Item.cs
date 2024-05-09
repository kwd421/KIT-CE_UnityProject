using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Animator anim;

    public bool collected = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // OnTrigger는 실제적 충돌이 없을 때 사용(isTrigger On)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // collision.tag 와 collision.gameObject.tag는 동일하다
        if (collision.gameObject.tag == "Player")
        {
            switch (transform.tag)
            {
                case ("Coin"):
                    Coins(transform);
                    break;

                case ("Item"):
                    Items(collision.transform);
                    break;

                case ("Trap"):
                    Traps(collision.transform);
                    break;

                case ("Finish"):
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Finish);
                    // To Next Stage
                    GameManager.instance.NextStage();
                    break;
            }
        }
    }

    public void Init()
    {
        collected = false;
        transform.gameObject.SetActive(true);
    }

    public void Coins(Transform coin)
    {
        // Sound
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Item);

        // Point
        if (coin.name.Contains("Bronze"))
            GameManager.instance.stagePoint += 50;
        else if (coin.name.Contains("Silver"))
            GameManager.instance.stagePoint += 100;
        else if (coin.name.Contains("Gold"))
            GameManager.instance.stagePoint += 200;

        // Deactive Item
        collected = true;
        transform.gameObject.SetActive(false);
    }

    public void Items(Transform player)
    {
        // Sound
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Item);

        // Point

        // Deactive Item
        transform.gameObject.SetActive(false);
    }

    public void Traps(Transform player)
    {
        if (transform.name.Contains("JumpPad") && player.GetComponent<Rigidbody2D>().velocity.y < 0)
        {
            JumpPad();
        }
    }

    public void JumpPad()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
        GameManager.instance.player.anim.SetBool("isJumping", true);
        //GameManager.instance.player.isJumping = true;
        anim.SetTrigger("doJump");
        GameManager.instance.player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 40, ForceMode2D.Impulse);
        //GameManager.instance.player.isJumping = false;
    }
}
