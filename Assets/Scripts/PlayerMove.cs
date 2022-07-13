using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public MoveController controller;
    public float Speed = 40f;
    float horizontalMove = 0f;
    
    bool jump = false;
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * Speed;
        if (Input.GetButtonDown("Jump"))
            jump = true;
    }
    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }
}
