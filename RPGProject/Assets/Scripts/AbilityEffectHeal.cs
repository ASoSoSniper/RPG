using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_features", menuName = "ScriptableObject/Ability Effect Heal")]
public class AbilityEffectHeal : AbilityEffect
{
    [Header("Heal")]
    public int healthGain = 20;
    public bool healOverTime = false;

    public override bool Trigger(Fighter fighter)
    {
        if (healOverTime)
        {
            if (!base.Trigger(fighter)) return false;
        }

        TriggerOnTurnStart(fighter);
        return true;
    }
    public override void TriggerOnTurnStart(Fighter fighter)
    {
        DamageTarget(fighter, -healthGain);
        if (hitStatusOnTurnStart) SpawnHitStatus(fighter, "+" + healthGain.ToString());
    }
}
