using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    [SerializeField] float JumpForce = 400f;
    [SerializeField] LayerMask Ground;
    [SerializeField] Transform GroundCheck;
    [Range(0, .3f)][SerializeField] float Smooth = .05f;

    bool isGround; // �ٴ����� �ƴ���
    float GroundCheckRadius = .2f;  // �ٴ�üũ �� ������

    Rigidbody2D rb;
    int Direction = 1; // �ٶ󺸴� ���� -1 / 1

    Vector2 velocity = Vector2.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        isGround = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, GroundCheckRadius, Ground);
        for(int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                isGround = true;
        }
    }

    public void Move(float move, bool jump)
    {
        if (isGround)
        {
            Vector2 targetVelocity = new Vector2(move * 10f, rb.velocity.y);

            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, Smooth);

            if(move > 0 && Direction == 1)
                Flip();
            else if(move < 0 && Direction == -1)
                Flip();

        }
        if(isGround && jump)
        {
            isGround = false;
            rb.AddForce(new Vector2(0f, JumpForce));
        }
    }
    void Flip()
    {
        Direction *= -1;

        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
}
