using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player player;
    private bool[] inputs;

    int direction = 1;
    private void OnValidate()
    {
        if (player == null)
            player = GetComponent<Player>();
    }

    private void Start()
    {
        inputs = new bool[4];
    }

    private void FixedUpdate()
    {
        // if(inputs[0]) 상(점프)
        // if(inputs[1]) 하(없음)
        direction = 0;

        if (inputs[2]) // 좌
            direction -= 1;
        if (inputs[3]) // 우
            direction += 1;

        SendMove(direction, inputs[0]);
    }
    
    public void SetInput(bool[] inputs)
    {
        this.inputs = inputs;
    }

    private void SendMove(int direction, bool jump)
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.Move);
        message.AddInt(direction);
        message.AddBool(jump);
        NetworkManager.Singleton.Server.Send(message, player.Id);
    }


}
