using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using UnityEngine.UI;

public class PlayerSkill : MonoBehaviour
{

    [SerializeField] Player player;
    [SerializeField] AnimManager animManager;
    [SerializeField] MoveController moveController;

    // 플레이어 추가시 추가하세요
    [SerializeField] Lection lection;
    [SerializeField] Kara kara;

    [SerializeField] GameObject Mybody;

    [SerializeField] Image Q_UI;
    [SerializeField] Image W_UI;
    [SerializeField] Image E_UI;
    [SerializeField] Image R_UI;

    [SerializeField] float Cooltime_A;
    [SerializeField] float Cooltime_Q;
    [SerializeField] float Cooltime_W;
    [SerializeField] float Cooltime_E;
    [SerializeField] float Cooltime_R;

    float Timer_A = -1;
    float Timer_Q = -1;   // 쿨타임 타이머
    float Timer_W = -1;
    float Timer_E = -1;
    float Timer_R = -1;

    bool SKillProcess = false; // 한번에 두 번 시전 방지
    // 스킬 레디시 전체 호출
    public void SkillReady(int direction, string key)
    {
        if (!SKillProcess && (key == "A" && Timer_A < 0 && player.IsLocal) || (key == "Q" && Timer_Q < 0 && player.IsLocal) || (key == "W" && Timer_W < 0 && player.IsLocal) || (key == "E" && Timer_E < 0 && player.IsLocal) || (key == "R" && Timer_R < 0 && player.IsLocal))
        {
            SKillProcess = true;
            Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.Skill);
            message.AddUShort(player.Id);
            message.AddInt(direction);
            message.AddString(key);
            NetworkManager.Singleton.Client.Send(message);
        }
    }

    // 스킬 호출됨
    public void Skill(int direction, string key)
    {
        switch (key)
        {
            case "A":
                SKillProcess = false;
                animManager.SendAttackAnim(key);
                StartCoroutine(CooltimeReturner("A"));              // 쿨타임 타이머 시작
                A(direction);
                break;
            case "Q":
                SKillProcess = false;
                animManager.SendAttackAnim(key);
                StartCoroutine(CooltimeReturner("Q"));             
                Q(direction);
                break;
            case "W":
                SKillProcess = false;
                animManager.SendAttackAnim(key);
                StartCoroutine(CooltimeReturner("W"));
                W(direction);
                break;
            case "E":
                SKillProcess = false;
                animManager.SendAttackAnim(key);
                StartCoroutine(CooltimeReturner("E"));
                E(direction);
                
                break;
            case "R":
                SKillProcess = false;
                animManager.SendAttackAnim(key);
                R(direction);
                
                break;
        }
    }

    // 캐릭터추가시 여기만 추가해주면 됨
    private void A(int direction)
    {
        if (player.Character == "Lection")
            lection.A(direction);
        else if (player.Character == "Kara")
            kara.A(direction);
    }
    private void Q(int direction)
    {
        if (player.Character == "Lection")
            lection.Q(direction);
        else if (player.Character == "Kara")
            kara.Q(direction);
    }
    private void W(int direction)
    {
        if (player.Character == "Lection")
            lection.W(direction);
        else if (player.Character == "Kara")
            kara.W(direction);
    }
    private void E(int direction)
    {
        if (player.Character == "Lection")
            lection.E(direction);
        else if (player.Character == "Kara")
            kara.E(direction);
    }
    private void R(int direction)
    {
        if (player.Character == "Lection")
            lection.R(direction);
        else if (player.Character == "Kara")
            kara.R(direction);
    }
    //////////////////////////////
    // 여기부턴 건들필요 없을것이야
    
    
    // 정상 쿨타임 타이머
    IEnumerator CooltimeReturner(string key, float cooltime = 0)
    {
        //Debug.Log(key + "쿨타임중!");
        if(key == "A")
            Timer_A = Cooltime_A;
        else if (key == "Q")
            Timer_Q = Cooltime_Q;
        else if (key == "W")
            Timer_W = Cooltime_W;
        else if (key == "E")
            Timer_E = Cooltime_E;
        else if (key == "R")
            Timer_R = Cooltime_R;
        switch (key)
        {
            case "A":
                while (Timer_A >= 0)
                {
                    Timer_A -= Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
                break;
            case "Q":
                while (Timer_Q >= 0)
                {
                    Timer_Q -= Time.fixedDeltaTime;
                    Q_UI.fillAmount = Timer_Q / Cooltime_Q;
                    yield return new WaitForFixedUpdate();
                }
                break;
            case "W":
                while (Timer_W >= 0)
                {
                    Timer_W -= Time.fixedDeltaTime;
                    W_UI.fillAmount = Timer_W / Cooltime_W;
                    yield return new WaitForFixedUpdate();
                }
                break;
            case "E":
                while (Timer_E >= 0)
                {
                    Timer_E -= Time.fixedDeltaTime;
                    E_UI.fillAmount = Timer_E / Cooltime_E;
                    yield return new WaitForFixedUpdate();
                }
                break;
            case "R":
                while (Timer_R >= 0)
                {
                    Timer_R -= Time.fixedDeltaTime;
                    R_UI.fillAmount = Timer_R / Cooltime_R;
                    yield return new WaitForFixedUpdate();
                }
                break;
        }
        //Debug.Log(key + "사용가능!");

    }

    // 스킬 쿨타임 감소 (외부 영향)
    public void CoolTimeReturner(string key, float time)
    {
        switch (key)
        {
            case "A":
                Timer_A -= time;
                break;
            case "Q":
                Timer_Q -= time;
                Q_UI.fillAmount = Timer_Q / Cooltime_Q;
                break;
            case "W":
                Timer_W -= time;
                W_UI.fillAmount = Timer_W / Cooltime_W;
                break;
            case "E":
                Timer_E -= time;
                E_UI.fillAmount = Timer_E / Cooltime_E;
                break;
            case "R":
                Timer_R -= time;
                R_UI.fillAmount = Timer_R / Cooltime_R;
                break;
        }
    }

    // 스킬 사용  (가능/불가능)  (ON/OFF 형식)
    public void CanUseNow(string key, bool tof)
    {
        if (tof)
        {
            if (key == "A")
            {
                Timer_A = -1;
            }
            if (key == "Q")
            {
                Timer_Q = -1;
                R_UI.fillAmount = Timer_Q / Cooltime_Q;
            }
            if (key == "W")
            {
                Timer_W = -1;
                R_UI.fillAmount = Timer_W / Cooltime_W;
            }
            if (key == "E")
            {
                Timer_E = -1;
                R_UI.fillAmount = Timer_E / Cooltime_E;
            }
            if (key == "R")
            {
                Timer_R = -1;
                R_UI.fillAmount = Timer_R / Cooltime_R;
            }
        }
        else
        {
            if (key == "A")
            {
                Timer_A = 1;
            }
            if (key == "Q")
            {
                Timer_Q = 1;
                R_UI.fillAmount = Timer_Q / Cooltime_Q;
            }
            if (key == "W")
            {
                Timer_W = 1;
                R_UI.fillAmount = Timer_W / Cooltime_W;
            }
            if (key == "E")
            {
                Timer_E = 1;
                R_UI.fillAmount = Timer_E / Cooltime_E;
            }
            if (key == "R")
            {
                Timer_R = 1;
                R_UI.fillAmount = Timer_R / Cooltime_R;
            }
        }
    }

}
