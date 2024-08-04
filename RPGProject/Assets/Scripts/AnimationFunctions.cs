using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimationFunctions : MonoBehaviour
{
    Fighter fighter;
    [SerializeField] List<Transform> spawnPoints;
    
    // Start is called before the first frame update
    void Start()
    {
        fighter = GetComponentInParent<Fighter>();
    }
    // Trigger when in the animation the fighter moves toward a spectacle position or the enemy target
    public void StartMoving()
    {
        fighter.move = true;
    }
    // For spawn-based abilities, selects which ability type to use based on the active ability
    public void TriggerActiveAbility(int spawnPointIndex)
    {
        switch(fighter.activeAbility.type)
        {
            case Ability.AbilityType.Damage:
                FireProjectile(spawnPointIndex);
                break;
            case Ability.AbilityType.Heal:
                CastEffect(fighter.targetIndex);
                break;
            case Ability.AbilityType.Buff:
                CastEffect(fighter.targetIndex);
                break;
        }
    }
    void FireProjectile(int spawnPointIndex)
    {
        switch (fighter.activeAbility.firePattern)
        {
            // Loop Targets: attacks each target once and loops until the number of attacks is met
            case Ability.FirePattern.LoopTargets:
                StartCoroutine(BurstFire(spawnPointIndex, fighter.targetIndex));
                AdvanceTargetIndex();
                break;
            // Simultaneous: attacks every target with the same attack simultaneously
            case Ability.FirePattern.Simultaneous:
                for(int i = 0; i < fighter.targets.Count; i++)
                {
                    StartCoroutine(BurstFire(spawnPointIndex, fighter.targetIndex));
                    AdvanceTargetIndex();
                }
                break;
        }
    }

    IEnumerator BurstFire(int spawnPointIndex, int targetIndex)
    {
        fighter.projectilesToSpawn--;

        if (!fighter.activeAbility.burstCast)
        {
            SingleFire(spawnPointIndex, targetIndex);
        }
        else
        {
            for (int i = 0; i < fighter.activeAbility.burstCount; i++)
            {
                SingleFire(spawnPointIndex, targetIndex);

                yield return new WaitForSeconds(fighter.activeAbility.burstRate);
            }
        }
    }

    void SingleFire(int spawnPointIndex, int targetIndex)
    {
        Battle battle = FindObjectOfType<Battle>();
        Transform spawnPos = fighter.transform;
        if (spawnPointIndex <= spawnPoints.Count - 1)
        {
            spawnPos = spawnPoints[spawnPointIndex];
        }

        GameObject spawn = Instantiate(fighter.activeAbility.visualEffect.gameObject, battle.transform);
        spawn.transform.position = spawnPos.position;

        Projectile projectile = spawn.GetComponent<Projectile>();
        projectile.Launch(fighter, fighter.targets[targetIndex], spawnPos.position);
    }

    void CastEffect(int targetIndex)
    {
        fighter.projectilesToSpawn--;
        if (!fighter.gameObject.GetComponent<FighterDuplicate>() ||
            (fighter.gameObject.GetComponent<FighterDuplicate>() && fighter.activeAbility.abilityEffect.duplicatesCanCast))
        {
            if (!fighter.WillHitTarget(fighter.targets[targetIndex], fighter)) return;

            Fighter target = fighter.activeAbility.targetSelection != Ability.TargetSelection.RefTarget ? fighter.targets[targetIndex] : fighter;
            fighter.activeAbility.abilityEffect.Trigger(target);
            if (fighter.activeAbility.visualEffect)
            {
                Battle battle = FindObjectOfType<Battle>();
                GameObject visual = Instantiate(fighter.activeAbility.visualEffect.gameObject, battle.transform);
                visual.transform.position = fighter.targets[targetIndex].transform.position + Vector3.up * fighter.projectileYOffset;
            }
        }
            
    }
    void AdvanceTargetIndex()
    {
        fighter.targetIndex++;
        if (fighter.targetIndex >= fighter.targets.Count)
        {
            fighter.targetIndex = 0;
        }
    }
    public void DamageTarget()
    {
        fighter.Damage(fighter.targets[fighter.targetIndex]);
    }

    public void ReturnToIdle()
    {
        fighter.EndAttack();
    }

    public void RepeatAnimation()
    {
        if (fighter.actionState == Fighter.ActionStates.Attack && fighter.projectilesToSpawn > 0)
        {
            fighter.animator.SetTrigger(fighter.activeAbility.animation);
            /*if (fighter.activeEffects.Count > 0)
            {
                foreach (KeyValuePair<AbilityEffect, int> effect in fighter.activeEffects)
                {
                    effect.Key.AttackMod(fighter);
                }
            }*/
        }
        else
        {
            fighter.EndAttack();
        }
    }

    public void KillFighter(GameObject marble)
    {
        fighter.actionState = Fighter.ActionStates.Dead;

        Battle battle = FindObjectOfType<Battle>();
        for (int i = 0; i < 15; i++)
        {
            GameObject spawn = Instantiate(marble, battle.transform);
            spawn.transform.position = fighter.transform.position; //+ Vector3.up * fighter.projectileYOffset;
        }

        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }
}
