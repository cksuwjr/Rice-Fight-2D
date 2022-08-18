using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimSkillEvent : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] PlayerSkill playerSkill;
    [SerializeField] GameObject R_Range;
    GameObject SpawnedR_Range;
    public void Q() { }
    public void W() { }
    public void E() { }
    public void R()
    {
        playerSkill.Skill_R();
    }

    public void Ron()
    {
        if(SpawnedR_Range == null)
            SpawnedR_Range = Instantiate(R_Range, player.transform);
    }
    public void Roff()
    {
        if(SpawnedR_Range != null)
            Destroy(SpawnedR_Range);
    }
}
