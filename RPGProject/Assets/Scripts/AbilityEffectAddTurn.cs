using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_features", menuName = "ScriptableObject/Ability Effect Add Turn")]
public class AbilityEffectAddTurn : AbilityEffect
{
    [Header("Turn Mod")]
    bool AddTurn = false;

    public override bool Trigger(Fighter fighter)
    {
        //If fighter to cast has not yet moved, do not cast
        if (!fighter.turnEnded && AddTurn)
        {
            Debug.Log("Cannot cast, no turn to add");
            return false;
        }

        //If max stacks reached, do not cast
        if (!base.Trigger(fighter)) return false;

        if (fighter.turnEnded)
        {
            fighter.turnEnded = false;
        }

        return true;
    }
}
