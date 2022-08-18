using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using UnityEngine.UI;

public class PlayerSkill : MonoBehaviour
{
    private enum State
    {
        Nothing,
        Charging,
    }

    [SerializeField] GameObject Mybody;

    [SerializeField] Player player;
    [SerializeField] AnimManager animManager;
    [SerializeField] MoveController moveController;

    [SerializeField] GameObject Projectile_Q;
    [SerializeField] GameObject Projectile_W;
    [SerializeField] GameObject Projectile_E;
    [SerializeField] GameObject Projectile_R;

    float Cooltime_Q = -1;   // 쿨타임 타이머
    float Cooltime_W = -1;
    float Cooltime_E = -1;
    float Cooltime_R = -1;

    [SerializeField] float Qooltime_Q; // 재사용대기시간
    [SerializeField] float Qooltime_W; 
    [SerializeField] float Qooltime_E;
    [SerializeField] float Qooltime_R;

    //[SerializeField] bool isUsable_Q = true; // 사용가능 유무
    //[SerializeField] bool isUsable_W = true;
    //[SerializeField] bool isUsable_E = true;
    //[SerializeField] bool isUsable_R = true;

    bool E_damageOnOff = false;

    [SerializeField] Image Q_UI;
    [SerializeField] Image W_UI;
    [SerializeField] Image E_UI;
    [SerializeField] Image R_UI;
    public void SkillReady(int direction, string key)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.Skill);
        message.AddUShort(player.Id);
        message.AddInt(direction);
        message.AddString(key);
        switch (key) {
            case "Q":
                if (Cooltime_Q < 0 && player.IsLocal)
                {
                    StartCoroutine(CooltimeReturner("Q"));              // 쿨타임 타이머 시작
                    StartCoroutine(CooltimeReturner("State", 0.3f));    // 정신집중(차징, 멈칫)
                    NetworkManager.Singleton.Client.Send(message);      // qwer일때만 보내자
                    R_CanUse(true); // R초기화
                }
                break;
            case "W":
                if (Cooltime_W < 0 && player.IsLocal)
                {
                    StartCoroutine(CooltimeReturner("W"));      
                    StartCoroutine(CooltimeReturner("State", 0.3f)); 
                    NetworkManager.Singleton.Client.Send(message); 
                }
                break;
            case "E":
                if (Cooltime_E < 0 && player.IsLocal)
                {
                    StartCoroutine(CooltimeReturner("E"));      
                    StartCoroutine(CooltimeReturner("State", 0.1f));      
                    NetworkManager.Singleton.Client.Send(message);
                    R_CanUse(true); // R초기화
                }
                break;
            case "R":
                if (Cooltime_R < 0 && player.IsLocal)
                {
                    StartCoroutine(CooltimeReturner("State", 0.3f)); 
                    NetworkManager.Singleton.Client.Send(message);
                    R_CanUse(false);  // R 쿨타임은 타이머 따로 없게함 R_CanUse(true)로 초기화
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
                    Spawned.GetComponent<ShortSword>().Setting(player, direction, this);
                }
                break;
            case "W":
                break;
            case "E":
                animManager.SendAttackAnim(key);
                StartCoroutine(Skill_E(direction));
                break;
            case "R":
                animManager.SendAttackAnim(key);
                break;
        }
    }
    // 건들지말기~
    IEnumerator CooltimeReturner(string key, float cooltime = 0)
    {
        //Debug.Log(key + "쿨타임중!");
        if(key == "Q")
            Cooltime_Q = Qooltime_Q;
        else if (key == "W")
            Cooltime_W = Qooltime_W;
        else if (key == "E")
            Cooltime_E = Qooltime_E;
        else if (key == "R")
            Cooltime_R = Qooltime_R;
        else if(key == "State")
            player.SetState("Charging");

        switch (key)
        {
            case "Q":
                while (Cooltime_Q >= 0)
                {
                    Cooltime_Q -= Time.fixedDeltaTime;
                    Q_UI.fillAmount = Cooltime_Q / Qooltime_Q;
                    yield return null;
                }
                break;
            case "W":
                while (Cooltime_W >= 0)
                {
                    Cooltime_W -= Time.fixedDeltaTime;
                    W_UI.fillAmount = Cooltime_W / Qooltime_W;
                    yield return null;
                }
                break;
            case "E":
                while (Cooltime_E >= 0)
                {
                    Cooltime_E -= Time.fixedDeltaTime;
                    E_UI.fillAmount = Cooltime_E / Qooltime_E;
                    yield return null;
                }
                break;
            case "R":
                while (Cooltime_R >= 0)
                {
                    Cooltime_R -= Time.fixedDeltaTime;
                    R_UI.fillAmount = Cooltime_R / Qooltime_R;
                    yield return null;
                }
                break;
            case "State":
                yield return new WaitForSeconds(cooltime);
                break;
        }

        if(key == "State")
            player.SetState("Nothing");
        //Debug.Log(key + "사용가능!");

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
                    Spawned.GetComponent<SShort>().Setting(player, targets[n].gameObject, this);
                }
            }
        }
    }

    public void R_CanUse(bool tof)
    {
        if (tof)
        {
            Cooltime_R = -1;
            R_UI.fillAmount = Cooltime_R / Qooltime_R;
        }
        else
        {
            Cooltime_R = 1;
            R_UI.fillAmount = Cooltime_R / Qooltime_R;
        }
    }
    public void CoolTimeReturner(string key, float time)
    {
        switch (key)
        {
            case "Q":
                Cooltime_Q -= time;
                Q_UI.fillAmount = Cooltime_Q / Qooltime_Q;
                break;
            case "W":
                Cooltime_W -= time;
                W_UI.fillAmount = Cooltime_W / Qooltime_W;
                break;
            case "E":
                Cooltime_E -= time;
                E_UI.fillAmount = Cooltime_E / Qooltime_E;
                break;
            case "R":
                Cooltime_R -= time;
                R_UI.fillAmount = Cooltime_R / Qooltime_R;
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && E_damageOnOff)
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
