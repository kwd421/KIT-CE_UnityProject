using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;
using static UnityEditor.Progress;

public class Item : MonoBehaviour
{
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
                    Items(transform);
                    break;

                case ("Finish"):
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Finish);
                    // To Next Stage
                    GameManager.instance.NextStage();
                    break;
            }
        }
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
        transform.gameObject.SetActive(false);
    }

    public void Items(Transform item)
    {
        // Sound
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Item);

        // Point

        // Deactive Item
        transform.gameObject.SetActive(false);
    }
}
