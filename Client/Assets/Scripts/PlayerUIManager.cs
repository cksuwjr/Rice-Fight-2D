using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private Text Nickname;
    [SerializeField] private Image HPbar;

    public void SetNickname(string name)
    {
        Nickname.text = name;
    }
    public void UIUpdate(float Maxhp, float Currenthp, float Attackpower)
    {
        Debug.Log(player.Id + " UI¾÷µ¥ÀÌÆ®µÊ");
        HPbar.fillAmount = Currenthp / Maxhp;
    }
}
