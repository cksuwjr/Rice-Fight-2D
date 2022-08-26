using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class PlayerKeyinput : MonoBehaviour
{
    [SerializeField] private Player player;
    private bool[] inputs;

    int direction = 1;
    int finaldirection = 1;
    private void OnValidate()
    {
        if (player == null)
            player = GetComponent<Player>();
    }

    private void Start()
    {
        inputs = new bool[8];
    }

    private void FixedUpdate()
    {
        /// ============= 움직임 ==============
        // if(inputs[0]) 상(점프)
        // if(inputs[1]) 하(없음)
        direction = 0;

        if (inputs[2]) // 좌
            direction -= 1;
        if (inputs[3]) // 우
            direction += 1;

        SendMove(direction, inputs[0]);
        /// ==================================
        if (direction != 0)
            finaldirection = direction;

        if (inputs[4])
            SendSkillReady("A");
        if (inputs[5])
            SendSkillReady("Q");
        if (inputs[6])
            SendSkillReady("W");
        if (inputs[7])
            SendSkillReady("E");
        if (inputs[8])
            SendSkillReady("R");
    }
    
    public void SetInput(bool[] inputs)
    {
        this.inputs = inputs;
    }
    private void SendMove(int direction, bool jump)
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.Move);
        message.AddUShort(player.Id);
        message.AddInt(direction);
        message.AddBool(jump);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    private void SendSkillReady(string Key)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.SkillReady);
        message.AddInt(finaldirection);
        message.AddString(Key);
        NetworkManager.Singleton.Server.Send(message, player.Id);
    }

    
}
