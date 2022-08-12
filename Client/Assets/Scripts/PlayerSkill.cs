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
                    StartCoroutine(CooltimeReturner("Q", Qooltime_Q));      // ��Ÿ�� ��� ����
                    StartCoroutine(CooltimeReturner("State", 0.3f));        // ��������(��¡, ��ĩ)
                    NetworkManager.Singleton.Client.Send(message); // qwer�϶��� ������
                }
                break;
            case "W":
                break;
            case "E":
                break;
            case "R":
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
                break;
            case "R":
                break;
        }
    }
    IEnumerator CooltimeReturner(string key, float cooltime)
    {
        Debug.Log(key + "��Ÿ����!");
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
        Debug.Log(key + "��밡��!");

    }
}