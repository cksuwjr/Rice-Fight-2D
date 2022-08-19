using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimSkillEvent : MonoBehaviour
{
    [SerializeField] Player player;
    
    public void Q() {
        
    }
    public void W() {
        
    }
    public void E() {
        
    }
    public void R()
    {
        if(player.Character == "Lection")
            player.GetComponent<Lection>().Skill_R();
    }

    public void On()
    {
        if (player.Character == "Lection")
            player.GetComponent<Lection>().R_RangeOn();
    }
    public void Off()
    {
        if (player.Character == "Lection")
            player.GetComponent<Lection>().R_RangeOff();
    }
}
