using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public MoveController controller;
    public Animator anim;

    public float Speed = 40f;
    float horizontalMove = 0f;
    
    bool jump = false;
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * Speed;

        anim.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            anim.SetBool("IsJumping", true);
            anim.SetTrigger("Jump");
        }
    }
    public void OnLanding()
    {
        anim.SetBool("IsJumping", false);
    }
    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }
}
