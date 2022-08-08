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
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void Move(int direction, bool jump)
    {
        Vector2 targetVelocity = new Vector2(direction * MoveSpeed, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, Smooth);

        if (jump)
        {
            Debug.Log("���� ȣ��!");
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, JumpForce),ForceMode2D.Impulse);
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
