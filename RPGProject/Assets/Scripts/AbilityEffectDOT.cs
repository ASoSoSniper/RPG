using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_features", menuName = "ScriptableObject/Ability Effect DOT")]
public class AbilityEffectDOT : AbilityEffect
{
    [Header("Damage Over Time")]
    public int damage = 5;

    public override void TriggerOnTurnStart(Fighter fighter)
    {
        DamageTarget(fighter, damage);
        if (hitStatusOnTurnStart) SpawnHitStatus(fighter, damage.ToString());
    }
}
