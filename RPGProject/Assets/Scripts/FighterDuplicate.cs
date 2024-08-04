using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterDuplicate : Fighter
{
    AbilityEffectDuplicate effectReference;
    Fighter parentFighter;
    float damagePercent = 0.25f;
    float deathDespawnDelay = 1f;

    public void InitializeDuplicate(AbilityEffectDuplicate effect, Fighter host)
    {
        effectReference = effect;
        parentFighter = host;

        fighterInfo = parentFighter.fighterInfo;
        currentHP = effect.health;
        damagePercent = effect.damagePercent;
        currentFP = 10000;

        if (fighterInfo.model)
        {
            GameObject sprite = Instantiate(effect.visual, this.transform);
            animator = sprite.GetComponent<Animator>();
        }
    }

    protected override void MoveToIdle()
    {
        if (!move) return;

        Vector3 target = idlePosition;
        Vector3 direction = target - transform.position;

        float distance = Vector3.Distance(initReturnPos, target);
        float speed = distance / Mathf.Clamp(returnTime, 1, returnTime);
        transform.position += direction.normalized * speed * Time.deltaTime;

        if (Vector3.Distance(target, transform.position) < 0.01f)
        {
            move = false;
            transform.position = target;
            actionState = ActionStates.Idle;
        }
    }
    public override void BeginAttack()
    {
        battle.attackerAttacking = true;

        if (actionState == ActionStates.Idle && activeAbility.moveToSpectaclePos)
        {
            move = true;
            actionState = ActionStates.Move;
            attackDelay = activeAbility.spectacleAttackDelay;
            return;
        }

        returnDelay = activeAbility.attackReturnDelay;
        actionState = ActionStates.Attack;
        animator.SetTrigger(activeAbility.animation);
        if (activeEffects.Count > 0)
        {
            foreach (KeyValuePair<AbilityEffect, int> effect in activeEffects)
            {
                effect.Key.AttackMod(this);
            }
        }
        projectilesToSpawn = (activeAbility.castsPerTarget * targets.Count);
    }

    public override void Damage(Fighter target)
    {
        int randomDamage = Random.Range(activeAbility.damageMin, activeAbility.damageMax);

        int damage = Mathf.RoundToInt((float)randomDamage * damagePercent);

        target.RecieveDamage(damage);
    }

    public override void RecieveDamage(int damage, bool noHitStatus = false)
    {
        if (actionState == ActionStates.Dead) return;

        currentHP = Mathf.Clamp(currentHP - damage, 0, currentHP);
        if (currentHP <= 0)
        {
            animator.SetTrigger("Death");
            StartCoroutine(DestroyDuplicate());
            actionState = ActionStates.Dead;
        }
    }

    IEnumerator DestroyDuplicate()
    {
        if (actionState == ActionStates.Dead) yield return null;

        yield return new WaitForSeconds(deathDespawnDelay);

        effectReference.RemoveEffect(parentFighter, 1);

        Destroy(this.gameObject);
    }
}
