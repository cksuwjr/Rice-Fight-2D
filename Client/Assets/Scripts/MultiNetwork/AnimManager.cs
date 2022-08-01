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
        lastPosition.y = transform.position.y;
        float distanceMoved = Vector2.Distance(transform.position, lastPosition);

        anim.SetBool("isWalk", distanceMoved > 0.01f);
        anim.SetBool("isIdle", !(distanceMoved > 0.01f));

        lastPosition = transform.position;
    }
}
