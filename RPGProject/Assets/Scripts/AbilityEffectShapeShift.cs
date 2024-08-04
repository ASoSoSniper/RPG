using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_features", menuName = "ScriptableObject/Ability Effect ShapeShift")]
public class AbilityEffectShapeShift : AbilityEffect
{
    [SerializeField] bool endTurn;
    [SerializeField] Ability revertAbility;

    public override bool Trigger(Fighter fighter)
    {
        if (!base.Trigger(fighter)) return false;

        Fighter target = fighter.targets[0];
        bool playerSide = fighter.battle.playerSprites.Contains(fighter);

        Fighter shape = fighter.battle.CreateFighter(target.fighterInfo, fighter.idlePosition, playerSide);

        shape.currentHP = fighter.currentHP;
        shape.maxHP = fighter.maxHP;
        shape.currentFP = fighter.currentFP;
        shape.maxFP = fighter.maxFP;

        List<Fighter> fighters = playerSide ? fighter.battle.playerSprites : fighter.battle.enemySprites;
        int index = fighters.IndexOf(fighter);
        fighters[index] = shape;

        fighter.battle.CreateSmokeBomb(shape);
        Destroy(fighter.gameObject);

        if (revertAbility)
        {
            shape.specialAbilities.Add(revertAbility);
            shape.otherInfoRef = fighter.fighterInfo;
        }

        shape.EndAttack();
        shape.turnEnded = endTurn;

        return true;
    }
}
