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

    [SerializeField] Lection lection;

    [SerializeField] GameObject Mybody;

    [SerializeField] Image Q_UI;
    [SerializeField] Image W_UI;
    [SerializeField] Image E_UI;
    [SerializeField] Image R_UI;


    [SerializeField] float Cooltime_Q;
    [SerializeField] float Cooltime_W;
    [SerializeField] float Cooltime_E;
    [SerializeField] float Cooltime_R;
    
    float Timer_Q = -1;   // ��Ÿ�� Ÿ�̸�
    float Timer_W = -1;
    float Timer_E = -1;
    float Timer_R = -1;


    // ��ų ����� ��ü ȣ��
    public void SkillReady(int direction, string key)
    {
        if ((key == "Q" && Timer_Q < 0 && player.IsLocal) || (key == "W" && Timer_W < 0 && player.IsLocal) || (key == "E" && Timer_E < 0 && player.IsLocal) || (key == "R" && Timer_R < 0 && player.IsLocal))
        {
            Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.Skill);
            message.AddUShort(player.Id);
            message.AddInt(direction);
            message.AddString(key);
            NetworkManager.Singleton.Client.Send(message);
        }
    }

    // ��ų ȣ���
    public void Skill(int direction, string key)
    {
        switch (key)
        {
            case "Q":
                animManager.SendAttackAnim(key);
                StartCoroutine(CooltimeReturner("Q"));              // ��Ÿ�� Ÿ�̸� ����
                Q(direction);
                
                break;
            case "W":
                animManager.SendAttackAnim(key);
                StartCoroutine(CooltimeReturner("W"));
                W(direction);
                break;
            case "E":
                animManager.SendAttackAnim(key);
                StartCoroutine(CooltimeReturner("E"));
                E(direction);
                
                break;
            case "R":
                animManager.SendAttackAnim(key);
                R(direction);
                
                break;
        }
    }

    // ĳ�����߰��� ���⸸ �߰����ָ� ��
    private void Q(int direction)
    {
        if (player.Character == "Lection")
            lection.Q(direction);
    }
    private void W(int direction)
    {
        if (player.Character == "Lection")
            lection.W(direction);
    }
    private void E(int direction)
    {
        if (player.Character == "Lection")
            lection.E(direction);
    }
    private void R(int direction)
    {
        if (player.Character == "Lection")
            lection.R(direction);
    }
    //////////////////////////////
    
    
    
    // ���� ��Ÿ�� Ÿ�̸�
    IEnumerator CooltimeReturner(string key, float cooltime = 0)
    {
        //Debug.Log(key + "��Ÿ����!");
        if(key == "Q")
            Timer_Q = Cooltime_Q;
        else if (key == "W")
            Timer_W = Cooltime_W;
        else if (key == "E")
            Timer_E = Cooltime_E;
        else if (key == "R")
            Timer_R = Cooltime_R;
        switch (key)
        {
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
        //Debug.Log(key + "��밡��!");

    }

    // ��ų ��Ÿ�� ���� (�ܺ� ����)
    public void CoolTimeReturner(string key, float time)
    {
        switch (key)
        {
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

    // ��ų ���  (����/�Ұ���)  (ON/OFF ����)
    public void CanUseNow(string key, bool tof)
    {
        if (tof)
        {
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
