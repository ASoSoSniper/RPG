using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveCompanion : MonoBehaviour
{
    public Vector2 destination;
    Rigidbody2D rigidBody;
    [SerializeField] float moveSpeed = 10;
    ExploreSpriteAnims anims;
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
        rigidBody.velocity = Vector2.Distance(destination, (Vector2)transform.position) > 0.1f ? direction.normalized * moveSpeed : Vector2.zero;
        anims.UpdateDirection(rigidBody.velocity != Vector2.zero);
    }
}
