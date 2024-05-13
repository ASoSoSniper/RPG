using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected Fighter owner;
    protected Fighter target;
    protected Vector3 targetPos;
    protected Rigidbody2D rigidbody2D;
    [SerializeField] protected float minDamageRange = 1.0f;
    [SerializeField] protected bool endAttackOnHit = false;
    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!target) return;
        if (Vector2.Distance(transform.position, targetPos) <= minDamageRange)
        {
            owner.Damage(target);
            if (endAttackOnHit) owner.EndAttack();
            Destroy(this.gameObject);
        }
    }

    public virtual void Launch(Fighter fighter, Fighter targetFighter, Vector2 spawnPos)
    {
        owner = fighter;
        target = targetFighter;
        targetPos = target.transform.position + new Vector3(0, owner.projectileYOffset);

        Vector3 direction = targetPos - transform.position;
        rigidbody2D.AddForce(direction.normalized * fighter.activeAbility.projectileSpeed, ForceMode2D.Impulse);
    }
}
