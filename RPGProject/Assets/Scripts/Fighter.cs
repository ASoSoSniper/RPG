using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class AbilityEffectDuration
{
    public AbilityEffect effect;
    public int turns;
}

public class Fighter : MonoBehaviour
{
    public FighterInfo fighterInfo;
    public int currentHP;
    public int currentFP;
    public Vector2 idlePosition;
    public Vector2 spectaclePosition;
    protected Vector2 initReturnPos;
    [HideInInspector] public Animator animator;
    [SerializeField] Image healthBar;
    [HideInInspector] public Battle battle;
    [HideInInspector] public int axis = 1;

    Rigidbody2D rigidBody;
    BoxCollider2D boxCollider;

    [HideInInspector] public bool move;
    public Ability activeAbility;
    public Dictionary<AbilityEffect, int> activeEffects = new Dictionary<AbilityEffect, int>();
    public List<AbilityEffectDuration> effectDurations = new List<AbilityEffectDuration>();

    public bool turnEnded;
    public List<Fighter> targets = new List<Fighter>();
    public int targetIndex = 0;
    [SerializeField]protected float returnTime = 1f;
    public float projectileYOffset = 1f;
    public int projectilesToSpawn;
    protected float attackDelay;
    protected float returnDelay;

    public enum ActionStates
    {
        Idle,
        Move,
        Attack,
        Return,
        Dead
    }
    public ActionStates actionState = ActionStates.Idle;

    [SerializeField] GameObject hitStatusPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        battle = FindObjectOfType<Battle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fighterInfo && healthBar)
        {
            healthBar.fillAmount = (float)currentHP / (float)fighterInfo.maxHealth;
        }

        switch (actionState)
        {
            case ActionStates.Idle:
                break;
            case ActionStates.Move:
                MoveToTarget(spectaclePosition, activeAbility.spectacleMoveType, activeAbility.spectacleMoveDuration);
                if (!move)
                {
                    attackDelay -= Time.deltaTime;
                    if (attackDelay <= 0f) BeginAttack();
                }
                break;
            case ActionStates.Attack:
                if (targets.Count == 0) return;
                MoveToTarget(targets[targetIndex].transform.position - new Vector3( axis * activeAbility.stopDistance, 0f), activeAbility.movementType, activeAbility.moveDuration);
                break;
            case ActionStates.Return:
                if (!move)
                {
                    returnDelay -= Time.deltaTime;
                    if (returnDelay <= 0f) move = true;
                }
                else MoveToIdle();
                break;
        }
    }

    public void SetupFighter(FighterInfo fighter)
    {
        fighterInfo = fighter;
        currentHP = fighterInfo.currentHealth;
        currentFP = fighterInfo.currentFP;
        if (fighterInfo.model)
        {
            GameObject sprite = Instantiate(fighterInfo.model, this.transform);
            animator = sprite.GetComponent<Animator>();
        }
    }

    public void SetupAbility(Ability ability)
    {
        activeAbility = ability;
    }

    void MoveToTarget(Vector2 destination, Ability.MovementType type, float duration)
    {
        if (!move) return;

        Vector3 target = destination;
        Vector3 direction = target - transform.position;
        switch (type)
        {
            case Ability.MovementType.Constant:
                float distance = Vector3.Distance(idlePosition, target);
                float speed = distance / duration;
                transform.position += direction.normalized * speed * Time.deltaTime;
                break;

            case Ability.MovementType.Lerp:
                transform.position = Vector3.Lerp(transform.position, target, duration);
                break;
        }

        if (Vector3.Distance(target, transform.position) < 0.01f)
        {
            move = false;
        }
    }

    protected virtual void MoveToIdle()
    {
        if (!move) return;

        Vector3 target = idlePosition;
        Vector3 direction = target - transform.position;

        float distance = Vector3.Distance(initReturnPos, target);
        float speed = distance / returnTime;
        transform.position += direction.normalized * speed * Time.deltaTime;

        if (Vector3.Distance(target, transform.position) < 0.01f)
        {
            move = false;
            transform.position = target;
            actionState = ActionStates.Idle;
            battle.TurnCheck();
        }
    }

    public void AddTarget(Fighter target, bool bypassTargetMods = false)
    {
        Fighter targetToAdd = target;

        if (!bypassTargetMods && target.activeEffects.Count > 0)
        {
            foreach (KeyValuePair<AbilityEffect, int> effect in target.activeEffects)
            {
                Fighter result = effect.Key.TargetMod(target);
                if (result != targetToAdd)
                {
                    targetToAdd = result;
                    break;
                }
            }
        }

        targets.Add(targetToAdd);
        if (activeAbility.targetSelection == Ability.TargetSelection.Individual && targets.Count == activeAbility.numberOfTargets)
        {
            BeginAttack();
        }
    }

    public virtual void BeginAttack()
    {
        battle.attackerAttacking = true;

        if (actionState == ActionStates.Idle && activeAbility.moveToSpectaclePos)
        {
            move = true;
            actionState = ActionStates.Move;
            attackDelay = activeAbility.spectacleAttackDelay;
            return;
        }

        turnEnded = true;
        currentFP -= activeAbility.cost;
        returnDelay = activeAbility.attackReturnDelay;
        actionState = ActionStates.Attack;
        animator.SetTrigger(activeAbility.animation);
        if (activeEffects.Count > 0)
        {
            foreach (KeyValuePair<AbilityEffect, int> effect in activeEffects)
            {
                effect.Key.AttackMod(this);
            }
        }
        projectilesToSpawn = (activeAbility.castsPerTarget * targets.Count);
    }

    public void EndAttack()
    {
        targetIndex = 0;
        initReturnPos = transform.position;
        returnTime = activeAbility.attackReturnDuration;
        actionState = ActionStates.Return;
        targets.Clear();
    }

    public virtual void Damage(Fighter target)
    {
        int randomDamage = Random.Range(activeAbility.damageMin, activeAbility.damageMax + 1);
        if (activeEffects.Count > 0)
        {
            foreach (KeyValuePair<AbilityEffect, int> effect in activeEffects)
            {
                randomDamage += effect.Key.StatMod(this, AbilityEffect.BuffType.Damage) * effect.Value;
            }
        }

        if (!WillHitTarget(target, this)) return;

        target.RecieveDamage(randomDamage);
    }
    public virtual void RecieveDamage(int damage, bool noHitStatus = false)
    {
        if (actionState == ActionStates.Dead) return;

        if (activeEffects.Count > 0)
        {
            foreach (KeyValuePair<AbilityEffect, int> effect in activeEffects)
            {
                damage -= damage * effect.Key.StatMod(this, AbilityEffect.BuffType.Defense) * effect.Value;
            }
        }

        if (!noHitStatus)
        {
            GameObject hitStatus = Instantiate(hitStatusPrefab, battle.hitStatusCanvas.transform);
            hitStatus.transform.position = transform.position + Vector3.up * 2f;
            HitStatus component = hitStatus.GetComponent<HitStatus>();
            if (component)
            {
                component.InputValues(damage.ToString());
                component.Launch();
            }
        }

        currentHP = Mathf.Clamp(currentHP - damage, 0, currentHP);
        if (currentHP <= 0)
        {
            animator.SetTrigger("Death");
            //actionState = ActionStates.Dead;
            turnEnded = true;
        }
    }

    public bool WillHitTarget(Fighter fighter, Fighter attacker)
    {
        if (attacker.activeAbility.cannotMiss) return true;

        int hitChance = Mathf.Max(attacker.activeAbility.accuracy - fighter.fighterInfo.agility, 0);

        int rand = Random.Range(1, 101);
        if (rand <= hitChance) return true;

        //If miss, create miss message
        GameObject hitStatus = Instantiate(hitStatusPrefab, battle.hitStatusCanvas.transform);
        hitStatus.transform.position = fighter.transform.position + Vector3.up * 2f;
        HitStatus component = hitStatus.GetComponent<HitStatus>();
        if (component)
        {
            component.InputValues("Missed!");
            component.Launch();
        }

        return false;
    }

    void OnMouseOver()
    {
        //Debug.Log("Hovered");
    }

    void OnMouseDown()
    {
        if (actionState != ActionStates.Dead)
            battle.CollectTarget(this);
    }
}
