using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveController : MonoBehaviour
{
    [SerializeField] float JumpForce = 400f;
    [SerializeField] LayerMask Ground;
    [SerializeField] Transform GroundCheck;
    [Range(0, .3f)][SerializeField] float Smooth = .05f;

    bool isGround; // 바닥인지 아닌지
    float GroundCheckRadius = .01f;  // 바닥체크 원 반지름

    Rigidbody2D rb;
    int Direction = 1; // 바라보는 방향 -1 / 1

    Vector2 velocity = Vector2.zero;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    private void FixedUpdate()
    {
        bool wasGround = isGround;
        isGround = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, GroundCheckRadius, Ground);
        for(int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                isGround = true;
                if (!wasGround)
                {
                    OnLandEvent.Invoke();
                }
            }

        }
    }

    public void Move(float move, bool jump)
    {
        //if (isGround)
        //{
            Vector2 targetVelocity = new Vector2(move * 10f, rb.velocity.y);

            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, Smooth);

            if(move > 0 && Direction == 1)
                Flip();
            else if(move < 0 && Direction == -1)
                Flip();

        //}
        if(jump) /* && isGround   // 지상에 있을때만 점프하고 싶으면추가  */
        {
            rb.velocity = Vector2.zero; // 연속점프 위해 추가함 가속도초기화
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
