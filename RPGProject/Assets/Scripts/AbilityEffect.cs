using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class AbilityEffect : ScriptableObject
{
    public enum EffectType
    {
        StatusEffect
    }

    public enum Element
    {
        Normal,
        Steel,
        Fighting,
        Ground,
        Water,
        Fire,
        Grass,
        Electric,
        Dark,
        Psychic,
        Fairy,
        Dragon
    }

    public enum BuffType
    {
        None,
        Defense,
        Damage
    }

    public enum DurationType
    {
        Infinite,
        Countdown
    }

    public BuffType buffType = BuffType.None;

    //public EffectType effectType;
    //public Element element;
    public string effectName;
    public int stackCap = 1;
    public bool duplicatesCanCast = true;

    public DurationType durationType = DurationType.Infinite;
    public int turnsTillRemoval = 1;
    public bool castOnTurnStart = false;

    public bool hitStatusOnCast = false;
    public string triggerMessage = "Effect Applied!";
    public bool hitStatusOnTurnStart = false;
    public string turnStartMessage = "";
    public Color textColor = Color.white;
    public float textSize = 0.8f;
    public GameObject hitStatusPrefab;

    public virtual bool Trigger(Fighter fighter)
    {
        //If not on the fighter's active effects list, add to list
        if (!fighter.activeEffects.ContainsKey(this))
        {
            fighter.activeEffects.Add(this, 0);
        }
        
        if (fighter.activeEffects[this] < stackCap)
        {
            fighter.activeEffects[this]++;
            if (durationType == DurationType.Countdown)
            {
                AbilityEffectDuration effectDuration = new AbilityEffectDuration();
                effectDuration.effect = this;
                effectDuration.turns = turnsTillRemoval;

                fighter.effectDurations.Add(effectDuration);
            }

            if (hitStatusOnCast) SpawnHitStatus(fighter, triggerMessage);
            return true;
        }
        else
        {
            Debug.Log("At capacity, cannot cast");
            return false;
        }
    }

    public virtual void TriggerOnTurnStart(Fighter fighter)
    {
        if (!castOnTurnStart) return;
    }

    public virtual void RemoveEffect(Fighter fighter, int stacks)
    {
        if (fighter.activeEffects.ContainsKey(this))
        {
            fighter.activeEffects[this] -= stacks;
            if (fighter.activeEffects[this] <= 0)
            {
                fighter.activeEffects.Remove(this);
            }
        }
    }

    public virtual Fighter TargetMod(Fighter fighter)
    {
        return fighter;
    }

    public virtual void AttackMod(Fighter fighter)
    {

    }

    public virtual int StatMod(Fighter fighter, BuffType type)
    {
        return 0;
    }

    public virtual GameObject SpawnHitStatus(Fighter fighter, string message)
    {
        if (!hitStatusPrefab) return null;

        Debug.Log("Hit Status Spawned");

        GameObject hitStatus = Instantiate(hitStatusPrefab, fighter.battle.hitStatusCanvas.transform);
        hitStatus.transform.position = fighter.transform.position + Vector3.up * 2f;
        HitStatus component = hitStatus.GetComponent<HitStatus>();
        if (component)
        {
            component.InputValues(message);
            component.textComponent.color = textColor;
            component.textComponent.fontSize = textSize;
            component.Launch();
        }

        return hitStatus;
    }

    public void DamageTarget(Fighter fighter, int damage)
    {
        fighter.RecieveDamage(damage, true);
    }
}
