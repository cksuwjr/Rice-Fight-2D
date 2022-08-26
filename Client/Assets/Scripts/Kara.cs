using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kara : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] PlayerSkill playerSkill;

    //[SerializeField] GameObject Projectile_Q;
    //[SerializeField] GameObject Projectile_W;
    //[SerializeField] GameObject Projectile_E;
    //[SerializeField] GameObject Projectile_R;

    int Astack = 0;
    void AstackUP()
    {
        ++Astack;
        if (Astack > 2)
            Astack = 0;
        GetComponent<AnimManager>().AnimSetInt("Atype", Astack);
    }

    public void A(int direction)
    {
        StartCoroutine(State("AttackMoving",0.3f));
        AstackUP();
    }
    public void Q(int direction)
    {
        StartCoroutine(State("Charging",0.3f));    // 정신집중(차징, 멈칫)
    }
    public void W(int direction)
    {
        StartCoroutine(State("Charging",0.3f));

    }
    public void E(int direction)
    {
        StartCoroutine(State("Charging",0.1f));
    }
    public void R(int direction)
    {
        StartCoroutine(State("Charging",0.3f));
    }
    IEnumerator State(string what, float time)
    {
        player.SetState(what);
        yield return new WaitForSeconds(time);
        player.SetState("Nothing");
    }






    // PlayerAnimSkillEvent 에서만 호출하기위한 메서드!!!
    public void Skill_Q() { }
    public void Skill_W() { }
    public void Skill_E() { }
    public void Skill_R() { }
    ///////////////////////////////////////////


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}

