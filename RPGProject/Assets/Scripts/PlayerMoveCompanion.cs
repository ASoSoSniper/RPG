using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveCompanion : MonoBehaviour
{
    public Vector2 destination;
    Rigidbody2D rigidBody;
    [SerializeField] float moveSpeed = 10;
    ExploreSpriteAnims anims;
    public GameObject following;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        anims = GetComponentInChildren<ExploreSpriteAnims>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector2 direction = destination - (Vector2)transform.position;

        if (Vector2.Distance(destination, (Vector2)transform.position) > 0.1f && Vector3.Distance(following.transform.position, transform.position) > PlayerMovement.footStepInterval)
        {
            rigidBody.velocity = direction.normalized * moveSpeed;
        }
        else
        {
            rigidBody.velocity = Vector2.zero;
        }
        anims.UpdateDirection(rigidBody.velocity != Vector2.zero);
    }
}
