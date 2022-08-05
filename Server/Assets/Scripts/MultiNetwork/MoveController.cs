using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{

    Rigidbody2D rb;
    Vector2 velocity;

    [Range(0, .3f)] [SerializeField] float Smooth = .05f;
    [SerializeField] float JumpForce = 10f;
    [SerializeField] float MoveSpeed = 10f;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Move(int direction, bool jump)
    {
        Vector2 targetVelocity = new Vector2(direction * MoveSpeed, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, Smooth);

        if (jump)
        {
            Debug.Log("점프 호출!");
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, JumpForce),ForceMode2D.Impulse);
        }
    }
}
