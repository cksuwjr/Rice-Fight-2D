using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class MoveController : MonoBehaviour
{
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
        if (player.IsLocal)
        {
            Vector2 targetVelocity = new Vector2(direction * MoveSpeed, rb.velocity.y);
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, Smooth);

            bool[] AnimParameter = new bool[3];
            bool isIdle;
            bool isWalk;
            bool isJump;

            // 점프 검사
            if (Physics2D.OverlapCircle(GroundCheck.position, 0.05f, MyGround))
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

            AnimParameter[0] = isIdle;
            AnimParameter[1] = isWalk;
            AnimParameter[2] = isJump;
            AnimSetting(AnimParameter);
        }
    }
    private void AnimSetting(bool[] AnimParameter)
    {
        animManager.AnimSet("isIdle", AnimParameter[0]);
        animManager.AnimSet("isWalk", AnimParameter[1]);
        animManager.AnimSet("isJump", AnimParameter[2]);
        animManager.SendAnim();
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
