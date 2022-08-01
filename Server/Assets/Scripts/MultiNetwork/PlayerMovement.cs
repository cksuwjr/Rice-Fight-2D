using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private MoveController moveController;
    private bool[] inputs;

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
        int direction = 0;

        if (inputs[2]) // 좌
            direction -= 1;
        if (inputs[3]) // 우
            direction += 1;
            

        moveController.Move(direction, inputs[0]);
        SendMove();
    }
    private void SendMove()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.playerMovement);
        message.AddUShort(player.Id);
        message.AddVector2(transform.position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    public void SetInput(bool[] inputs)
    {
        this.inputs = inputs;
    }



}
