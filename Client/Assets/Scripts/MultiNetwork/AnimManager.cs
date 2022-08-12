using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class AnimManager : MonoBehaviour
{
    [SerializeField] private Animator anim;
    bool[] AnimationMoveParameter = new bool[3];
    private void Start()
    {
        for(int i = 0; i < AnimationMoveParameter.Length; i++)
        {
            AnimationMoveParameter[i] = false;
        }
    }
    public void AnimSetMove(bool isidle, bool iswalk, bool isjump)
    {
        anim.SetBool("isIdle", isidle);
        AnimationMoveParameter[0] = isidle;
        anim.SetBool("isWalk", iswalk);
        AnimationMoveParameter[1] = iswalk;
        anim.SetBool("isJump", isjump);
        AnimationMoveParameter[2] = isjump;
    }
    public void SendMoveAnim()
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.MyAnim);
        message.AddBools(AnimationMoveParameter, false);
        NetworkManager.Singleton.Client.Send(message);
    }
    public void SendAttackAnim(string key)
    {
        anim.SetTrigger(key);
    }
}
