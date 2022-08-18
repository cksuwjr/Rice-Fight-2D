using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SShort : MonoBehaviour
{
    Player player;
    public float Speed = 15f;
    float damage = 10f;
    GameObject AttackTarget;
    PlayerSkill playerSkill;
    Rigidbody2D rb;
    public void Setting(Player player, GameObject AttackTarget, PlayerSkill playerSkill)
    {
        if (AttackTarget == null)
            Destroy(gameObject);
        this.player = player;
        this.AttackTarget = AttackTarget;
        this.playerSkill = playerSkill;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if(AttackTarget != null)
        {
            Vector2 dir = (AttackTarget.transform.position - transform.position).normalized;
            rb.velocity = dir * Speed;
            transform.rotation = Quaternion.FromToRotation(Vector3.left, dir);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == AttackTarget)
        {
            if (player.IsLocal)
            {
                collision.GetComponent<Player>().Gethurt(damage);
                playerSkill.CoolTimeReturner("Q", 0.84f); // Q쿨타임 0.84초 감소
            }
            Destroy(gameObject);
        }
    }

}
