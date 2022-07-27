using RiptideNetworking;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform camTransform;

    private bool[] inputs;

    private void Start()
    {
        inputs = new bool[3];
    }
    private void Update()
    {
        if (Input.GetAxisRaw("Horizontal") == -1)
            inputs[0] = true;
        else if (Input.GetAxisRaw("Horizontal") == 1)
            inputs[1] = true;

        if (Input.GetButtonDown("Jump"))
            inputs[2] = true;
        //if (Input.GetButtonDown("Jump"))
        //{
        //    jump = true;
        //    anim.SetBool("IsJumping", true);
        //    anim.SetTrigger("Jump");
        //}

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
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.input);
        message.AddBools(inputs, false);
        message.AddVector2(camTransform.forward);
        NetworkManager.Singleton.Client.Send(message);
        
        // Server 프로젝트의 Player.cs 의 메시지 핸들러에서 처리
    }
    #endregion
}
