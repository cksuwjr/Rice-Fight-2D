using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class PlayerSkill : MonoBehaviour
{
    private enum State
    {
        Nothing,
        Charging,
    }

    [SerializeField] Player player;
    [SerializeField] AnimManager animManager;
    [SerializeField] MoveController moveController;

    [SerializeField] GameObject Projectile_Q;
    [SerializeField] GameObject Projectile_W;
    [SerializeField] GameObject Projectile_E;
    [SerializeField] GameObject Projectile_R;

    [SerializeField] float Qooltime_Q = 10f;
    [SerializeField] float Qooltime_W = 10f;
    [SerializeField] float Qooltime_E = 10f;
    [SerializeField] float Qooltime_R = 10f;

    [SerializeField] bool isUsable_Q = true;
    [SerializeField] bool isUsable_W = true;
    [SerializeField] bool isUsable_E = true;
    [SerializeField] bool isUsable_R = true;

    bool E_damageOnOff = false;
    public void SkillReady(int direction, string key)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.Skill);
        message.AddUShort(player.Id);
        message.AddInt(direction);
        message.AddString(key);
        switch (key) {
            case "Q":
                if (isUsable_Q && player.IsLocal)
                {
                    StartCoroutine(CooltimeReturner("Q", Qooltime_Q));      // 쿨타임 대기 시작
                    StartCoroutine(CooltimeReturner("State", 0.3f));        // 정신집중(차징, 멈칫)
                    NetworkManager.Singleton.Client.Send(message); // qwer일때만 보내자
                }
                break;
            case "W":
                if (isUsable_W && player.IsLocal)
                {
                    StartCoroutine(CooltimeReturner("W", Qooltime_W));      // 쿨타임 대기 시작
                    StartCoroutine(CooltimeReturner("State", 0.3f));        // 정신집중(차징, 멈칫)
                    NetworkManager.Singleton.Client.Send(message); // qwer일때만 보내자
                }
                break;
            case "E":
                if (isUsable_E && player.IsLocal)
                {
                    StartCoroutine(CooltimeReturner("E", Qooltime_E));      // 쿨타임 대기 시작
                    StartCoroutine(CooltimeReturner("State", 0.1f));        // 정신집중(차징, 멈칫)
                    NetworkManager.Singleton.Client.Send(message); // qwer일때만 보내자
                }
                break;
            case "R":
                if (isUsable_R && player.IsLocal)
                {
                    StartCoroutine(CooltimeReturner("R", Qooltime_R));      // 쿨타임 대기 시작
                    StartCoroutine(CooltimeReturner("State", 0.3f));        // 정신집중(차징, 멈칫)
                    NetworkManager.Singleton.Client.Send(message); // qwer일때만 보내자
                }
                break;
        }
    }
    public void Skill(int direction, string key)
    {
        switch (key)
        {
            case "Q":
                animManager.SendAttackAnim(key);
                GameObject Spawned;
                if (Projectile_Q != null)
                {
                    Spawned = Instantiate(Projectile_Q, player.transform.position, Quaternion.identity);
                    Spawned.GetComponent<ShortSword>().Setting(player, direction);
                }
                break;
            case "W":
                break;
            case "E":
                animManager.SendAttackAnim(key);
                StartCoroutine(Skill_E(direction));
                break;
            case "R":
                break;
        }
    }
    IEnumerator CooltimeReturner(string key, float cooltime)
    {
        Debug.Log(key + "쿨타임중!");
        switch (key)
        {
            case "Q":
                isUsable_Q = false;
                break;
            case "W":
                isUsable_W = false;
                break;
            case "E":
                isUsable_E = false;
                break;
            case "R":
                isUsable_R = false;
                break;
            case "State":
                player.SetState("Charging");
                break;
        }
        yield return new WaitForSeconds(cooltime);
        switch (key)
        {
            case "Q":
                isUsable_Q = true;
                break;
            case "W":
                isUsable_W = true;
                break;
            case "E":
                isUsable_E = true;
                break;
            case "R":
                isUsable_R = true;
                break;
            case "State":
                player.SetState("Nothing");
                break;
        }
        Debug.Log(key + "사용가능!");

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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && E_damageOnOff)
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
