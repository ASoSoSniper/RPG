using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_features", menuName = "ScriptableObject/Ability Effect Duplicated")]
public class AbilityEffectDuplicate : AbilityEffect
{
    [Header("Duplicate")]
    public GameObject duplicatePrefab;
    public GameObject visual;
    public int health = 20;
    public float damagePercent = 0.25f;
    public float animStaggerSeconds = 0.5f;
    public float distanceFromHost = 2f;
    public float initialSpawnAngle = 180f;
    
    private void Awake()
    {
        stackCap = 2;
        duplicatesCanCast = false;
    }

    public override bool Trigger(Fighter fighter)
    {
        if (fighter.gameObject.GetComponent<FighterDuplicate>())
        {
            return false;
        }

        if (!base.Trigger(fighter)) return false;

        GameObject spawn = Instantiate(duplicatePrefab, fighter.transform);

        float angleInterval = 180f / ((float)stackCap - 1);
        float selectedAngle = 0;
        Fighter[] children = fighter.GetComponentsInChildren<FighterDuplicate>();

        for (int i = 0; i < stackCap; i++)
        {
            bool childInLocation = false;
            float angle = initialSpawnAngle - (angleInterval * i);

            foreach (Fighter child in children)
            {
                if ((Vector3)child.idlePosition == fighter.transform.position + (Quaternion.AngleAxis(angle, Vector3.forward) * (Vector3.right * distanceFromHost)))
                {
                    childInLocation = true;
                }
            }

            if (!childInLocation)
            {
                selectedAngle = angle;
                break;
            }
        }

        spawn.transform.position = fighter.transform.position + (Quaternion.AngleAxis(selectedAngle, Vector3.forward) * (Vector3.right * distanceFromHost));

        FighterDuplicate fighterComponent = spawn.GetComponent<FighterDuplicate>();
        fighterComponent.InitializeDuplicate(this, fighter);

        fighterComponent.idlePosition = spawn.transform.position;
        fighterComponent.spectaclePosition = fighter.spectaclePosition + (Vector2)(Quaternion.AngleAxis(selectedAngle, Vector3.forward) * (Vector3.right * distanceFromHost));

        return true;
    }

    public override void RemoveEffect(Fighter fighter, int stacks)
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

    public override Fighter TargetMod(Fighter fighter)
    {
        List<Fighter> targets = new List<Fighter>();
        Fighter[] children = fighter.GetComponentsInChildren<Fighter>();
        
        targets.Add(fighter);
        foreach (Fighter child in children)
        {
            targets.Add(child);
        }

        int result = Random.Range(0, targets.Count);
        return targets[result];
    }

    public override void AttackMod(Fighter fighter)
    {
        Fighter[] children = fighter.GetComponentsInChildren<Fighter>();

        fighter.StartCoroutine(StaggerAnimations(fighter, children));
    }

    IEnumerator StaggerAnimations(Fighter host, Fighter[] children)
    {
        List<Fighter> targets = new List<Fighter>();
        targets.AddRange(host.targets);

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].gameObject == host.gameObject) continue;
            yield return new WaitForSeconds(animStaggerSeconds);

            children[i].activeAbility = host.activeAbility;
            children[i].targets = targets;

            children[i].BeginAttack();
        }
    }
}
