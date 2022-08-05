using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimManager : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private float playerMoveSpeed;

    private Vector2 lastPosition;

    public void AnimateBasedOnSpeed()
    {
        Vector2 lastXpos = lastPosition;
        Vector2 lastYpos = lastPosition;

        lastXpos.y = transform.position.y;
        float distanceMoved = Vector2.Distance(transform.position, lastXpos);

        anim.SetBool("isWalk", distanceMoved > 0.01f);
        anim.SetBool("isIdle", !(distanceMoved > 0.01f));
        

        lastYpos.x = transform.position.x;
        distanceMoved = Vector2.Distance(transform.position, lastYpos);
        anim.SetBool("isJump", distanceMoved > 0.01f);

        if (distanceMoved > 0.01f)
        {
            int updown = (lastYpos.y - transform.position.y < 0) ? 1 : -1;
            anim.SetInteger("Jump", updown);
        }
        lastPosition = transform.position;
    }
}
