using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolleyProjectile : Projectile
{
    // Update is called once per frame
    void Update()
    {
        if (!target) return;
        if (Vector2.Distance(transform.position, targetPos) <= minDamageRange)
        {
            owner.Damage(target);
            Destroy(this.gameObject);
        }
    }

    public override void Launch(Fighter fighter, Fighter targetFighter, Vector2 spawnPos)
    {
        owner = fighter;
        target = targetFighter;
        targetPos = target.transform.position + new Vector3(0, owner.projectileYOffset);

        float angle = CalculateAngle(spawnPos, targetPos);

        transform.rotation = Quaternion.Euler(0, 0, angle);

        rigidbody2D.AddForce(transform.right * fighter.activeAbility.projectileSpeed, ForceMode2D.Impulse);
    }

    float CalculateAngle(Vector2 originPosition, Vector2 targetPosition)
    {

        float gravity = rigidbody2D.gravityScale * -Physics2D.gravity.y;
        float dx = targetPosition.x - originPosition.x;
        float dy = targetPosition.y - originPosition.y;

        float z = owner.activeAbility.projectileSpeed * owner.activeAbility.projectileSpeed;
        float D = z * z - gravity * (gravity * dx * dx + 2 * dy * z);

        if (D < 0)
        {
            return 0;
        }

        float angle = Mathf.Atan((z - Mathf.Sqrt(D)) / (gravity * dx));
        angle *= Mathf.Rad2Deg;

        return angle;
    }
}
