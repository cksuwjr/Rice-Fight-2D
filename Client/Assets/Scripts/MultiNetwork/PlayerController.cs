using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform camTransform;

    private bool[] inputs;

    private void Start()
    {
        inputs = new bool[4];
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            inputs[0] = true;
        if (Input.GetKey(KeyCode.DownArrow))
            inputs[1] = true;
        if (Input.GetKey(KeyCode.LeftArrow))
            inputs[2] = true;
        if (Input.GetKey(KeyCode.RightArrow))
            inputs[3] = true;

    }

    private void FixedUpdate()
    {
        SendInput();

        for (int i = 0; i < inputs.Length; i++)
            inputs[i] = false;
    }

    #region Messages
    private void SendInput()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.input);
        message.AddBools(inputs, false);
        message.AddVector2(camTransform.forward);
        NetworkManager.Singleton.Client.Send(message);

    }
    #endregion
}
