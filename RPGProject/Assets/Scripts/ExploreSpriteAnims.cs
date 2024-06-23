using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreSpriteAnims : MonoBehaviour
{
    Animator animator;
    Rigidbody2D movement;

    Vector2 facingDirection = Vector2.down;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponentInParent<Rigidbody2D>();
    }

    public void UpdateDirection(bool moving)
    {
        if (movement.velocity != Vector2.zero)
        {
            facingDirection = movement.velocity.normalized;
        }

        animator.SetBool("Moving", moving);
        animator.SetInteger("X", Mathf.RoundToInt(facingDirection.x));
        animator.SetInteger("Y", Mathf.RoundToInt(facingDirection.y));
    }
}
