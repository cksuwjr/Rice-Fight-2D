using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class AnimManager : MonoBehaviour
{
    [SerializeField] private Animator anim;
    bool[] AnimationParameter = new bool[3];
    private void Start()
    {
        for(int i = 0; i < AnimationParameter.Length; i++)
        {
            AnimationParameter[i] = false;
        }
    }
    public void AnimSet(string name, bool TF)
    {
        anim.SetBool(name, TF);
        if (name == "isIdle")
            AnimationParameter[0] = TF;
        if (name == "isWalk")
            AnimationParameter[1] = TF;
        if (name == "isJump")
            AnimationParameter[2] = TF;
    }
    public void SendAnim()
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.MyAnim);
        message.AddBools(AnimationParameter, false);
        NetworkManager.Singleton.Client.Send(message);
    }
}
