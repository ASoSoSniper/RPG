using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_features", menuName = "ScriptableObject/Ability Effect Buff")]
public class AbilityEffectBuff : AbilityEffect
{
    [Header("Buff")]
    public int value = 1;

    public override int StatMod(Fighter fighter, BuffType type)
    {
        if (type == buffType)
        {
            return value;
        }

        return 0;
    }
}
