using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class MoveController : MonoBehaviour
{
    public enum State
    {
        Nothing,
        Charging,
        AttackMoving,

    }

    public State state = State.Nothing;

    Rigidbody2D rb;
    Vector2 velocity;

    [Range(0, .3f)] [SerializeField] float Smooth = .05f;
    [SerializeField] float JumpForce = 10f;
    [SerializeField] float MoveSpeed = 10f;

    [SerializeField] private Player player;

    [SerializeField] private AnimManager animManager;

    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask MyGround;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void Move(int direction, bool jump)
    {
        if (player.IsLocal && state == State.Charging)
            rb.velocity = new Vector2(0, rb.velocity.y);
        if (player.IsLocal && (state == State.Nothing || state == State.AttackMoving))
        {
            Vector2 targetVelocity = new Vector2(direction * MoveSpeed, rb.velocity.y);
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, Smooth);

            bool isIdle;
            bool isWalk;
            bool isJump;

            // 점프 검사
            if (Physics2D.OverlapBox(GroundCheck.position, new Vector2(0.7f, 0.2f), 0, MyGround))//Physics2D.OverlapCircle(GroundCheck.position, 0.05f, MyGround)
                isJump = false;
            else
                isJump = true;

            if (direction != 0)
            {
                isWalk = true;
                isIdle = false;
            }
            else
            {
                isWalk = false;
                isIdle = true;
            }

            if (!isJump && jump) // 점프를 눌렀는가
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
            }

            animManager.AnimSetMove(isIdle,isWalk,isJump);  // 내 애니메이션 업데이트
            animManager.SendMoveAnim();                     // 내 애니메이션 다른놈들에게 전송
        }
    }


    private void Update()
    {
        if(player.IsLocal)
            SendMyPose();
    }
    private void SendMyPose()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.MyPose);
        message.AddVector2(transform.position);
        NetworkManager.Singleton.Client.Send(message);
    }


}
