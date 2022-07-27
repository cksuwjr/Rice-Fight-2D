using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float playerMoveSpeed;

    private Vector2 lastPosition;

    private void Start()
    {
        
    }

    public void AnimateBasedOnSpeed()
    {
        lastPosition.y = transform.position.y;
        float distanceMoved = Vector2.Distance(transform.position, lastPosition);
        animator.SetFloat("Speed", distanceMoved);

        lastPosition = transform.position;
    }


}
