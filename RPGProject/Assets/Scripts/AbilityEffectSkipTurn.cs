using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_features", menuName = "ScriptableObject/Ability Effect Skip Turn")]
public class AbilityEffectSkipTurn : AbilityEffect
{
    public override bool Trigger(Fighter fighter)
    {
        //If max stacks reached, do not cast
        if (!base.Trigger(fighter)) return false;

        //***End here if casting is triggered on turn start***
        if (castOnTurnStart) return true;

        //If fighter has not moved and this effect removes turns, remove turn
        TriggerOnTurnStart(fighter);

        return true;
    }

    public override void TriggerOnTurnStart(Fighter fighter)
    {
        if (!fighter.turnEnded)
        {
            fighter.turnEnded = true;
            if (hitStatusOnTurnStart) SpawnHitStatus(fighter, turnStartMessage);
        }
    }
}
