using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_features", menuName = "ScriptableObject/Ability Effect ShapeShift Return")]
public class AbilityEffectShapeShift_Return : AbilityEffect
{
    [SerializeField] bool endTurn;

    public override bool Trigger(Fighter fighter)
    {
        if (!base.Trigger(fighter)) return false;

        if (!fighter.otherInfoRef) return false;

        bool playerSide = fighter.battle.playerSprites.Contains(fighter);

        Fighter shape = fighter.battle.CreateFighter(fighter.otherInfoRef, fighter.idlePosition, playerSide);

        shape.currentHP = fighter.currentHP;
        shape.maxHP = fighter.maxHP;
        shape.currentFP = fighter.currentFP;
        shape.maxFP = fighter.maxFP;

        List<Fighter> fighters = playerSide ? fighter.battle.playerSprites : fighter.battle.enemySprites;
        int index = fighters.IndexOf(fighter);
        fighters[index] = shape;

        fighter.battle.CreateSmokeBomb(shape);
        Destroy(fighter.gameObject);

        shape.EndAttack();
        shape.turnEnded = endTurn;

        return true;
    }
}
