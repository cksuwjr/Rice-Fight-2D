using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortSword : MonoBehaviour
{
    Player player;
    PlayerSkill playerSkill;
    public float Speed = 15f;
    public float DestroyTime = 1f;
    float damage = 50f;

    public void Setting(Player player, int direction, PlayerSkill playerSkill)
    {
        this.player = player;
        transform.localScale = new Vector3(-direction * 0.8f, 0.8f, 1);
        this.playerSkill = playerSkill;
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Speed * direction, 0);
    }
    void Start()
    {
        Destroy(gameObject, DestroyTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject != player.gameObject)
            {
                if (player.IsLocal)
                {
                    collision.GetComponent<Player>().Gethurt(damage);
                    playerSkill.CoolTimeReturner("E", 10f);
                }
                Destroy(gameObject);
            }
        }
    }

}
