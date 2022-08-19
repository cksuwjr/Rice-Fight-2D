using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lection : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] PlayerSkill playerSkill;

    [SerializeField] GameObject Projectile_Q;
    //[SerializeField] GameObject Projectile_W;
    //[SerializeField] GameObject Projectile_E;
    [SerializeField] GameObject Projectile_R;

    [SerializeField] GameObject R_Range;
    GameObject SpawnedR_Range;

    bool E_damageOnOff = false;

    public void Q(int direction)
    {
        StartCoroutine(State(0.3f));    // 정신집중(차징, 멈칫)
        playerSkill.CanUseNow("R", true); // R초기화
        GameObject Spawned;
        if (Projectile_Q != null)
        {
            Spawned = Instantiate(Projectile_Q, player.transform.position, Quaternion.identity);
            Spawned.GetComponent<ShortSword>().Setting(player, direction, playerSkill);
        }
    }
    public void W(int direction)
    {
        StartCoroutine(State(0.3f));

    }
    public void E(int direction)
    {
        StartCoroutine(State(0.1f));
        playerSkill.CanUseNow("R", true); // R초기화
        StartCoroutine(Skill_E(direction));
    }
    public void R(int direction)
    {
        StartCoroutine(State(0.3f));
        playerSkill.CanUseNow("R", false);

    }
    IEnumerator State(float time)
    {
        player.SetState("Charging");
        yield return new WaitForSeconds(time);
        player.SetState("Nothing");
    }

    IEnumerator Skill_E(int direction)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        E_damageOnOff = true;

        int second = 0;
        while (second < 5)
        {
            rb.MovePosition(new Vector2(transform.position.x + direction, transform.position.y));
            second++;
            yield return new WaitForSeconds(0.001f);
        }

        rb.gravityScale = 3;
        E_damageOnOff = false;
    }

    public void R_RangeOn()
    {
        if (SpawnedR_Range == null)
            SpawnedR_Range = Instantiate(R_Range, player.transform);
    }
    public void R_RangeOff()
    {
        if (SpawnedR_Range != null)
            Destroy(SpawnedR_Range);
    }






    // PlayerAnimSkillEvent 에서만 호출하기위한 메서드!!!
    public void Skill_Q() { }
    public void Skill_W() { }
    public void Skill_E() { }
    public void Skill_R()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(player.transform.position, 4.5f, 1 << LayerMask.NameToLayer("Player"));
        for (int n = 0; n < targets.Length; n++)
        {
            if (targets[n].gameObject != gameObject)
            {
                GameObject Spawned;
                if (Projectile_R != null)
                {
                    Spawned = Instantiate(Projectile_R, player.transform.position, Quaternion.identity);
                    Spawned.GetComponent<SShort>().Setting(player, targets[n].gameObject, playerSkill);
                }
            }
        }
    }
    ///////////////////////////////////////////
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && E_damageOnOff)
        {
            if (collision.gameObject != player.gameObject)
            {
                if (player.IsLocal)
                {
                    collision.GetComponent<Player>().Gethurt(150);
                    E_damageOnOff = false;
                }
            }
        }
    }
}
