using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_features", menuName = "ScriptableObject/Ability")]
public class Ability : ScriptableObject
{
    public enum AbilityType
    {
        Damage,
        Heal,
        Buff,
        Other
    }    
    public enum Duration
    {
        Instant,
        OverTime
    }
    public enum TargetSelection
    {
        Individual,
        All,
        Self
    }
    public enum FightersToTarget
    {
        Enemies,
        Allies,
        Both
    }
    public enum VisualType
    {
        Reticule,
        Melee,
        Projectile,
        Arc,
        Overhead
    }
    public enum FirePattern
    {
        LoopTargets,
        Simultaneous
    }
    public enum MovementType
    {
        Constant,
        Lerp
    }

    [Header("Base Settings")]
    public string abilityName = "";
    public string description = "";
    public int cost = 1;
    public string animation = "";

    [Header("Spectacle Position")]
    public bool moveToSpectaclePos = false;
    public MovementType spectacleMoveType = MovementType.Constant;
    public float spectacleMoveDuration = 1f;
    public float spectacleAttackDelay = 0f;

    [Header("Burst Cast")]
    #region BurstCast
    public bool burstCast = false;
    public int burstCount = 1;
    public float burstRate = 0.5f;
    #endregion

    [Header("Ability Type")]
    public AbilityType type;
    #region AbilityType

    //Damage
    public int damageMin = 1;
    public int damageMax = 1;
    
    //Heal
    public int healMin = 1;
    public int healMax = 1;

    //Buff
    public float damageMultiplier = 1.5f;
    public float defenseMultiplier = 1.5f;

    public AbilityEffect abilityEffect;
    public int accuracy = 100;
    public bool cannotMiss = true;

    #endregion

    [Header("Duration")]
    public Duration duration;
    #region Duration
    //OverTime
    public int turnDuration = 1;
    #endregion

    [Header("Target Selection")]
    public FightersToTarget fightersToTarget;
    public TargetSelection targetSelection;
    #region TargetSelection
    //Individual
    public int numberOfTargets = 1;
    public bool randomizeSelection = false;
    #endregion

    [Header("Visual")]
    public GameObject visualEffect;
    public VisualType visualType;
    public float attackReturnDelay = 0.5f;
    public float attackReturnDuration = 1f;
    #region Visual
    public FirePattern firePattern;
    public int castsPerTarget = 1;
    public float projectileSpeed = 5f;
    public float arcAngle = 45f;
    public float arcGravity = 1f;
    public float visualDuration = 0.5f;
    public MovementType movementType;
    public float moveDuration = 1f;
    public float stopDistance = 5f;
    #endregion
}

[CustomEditor(typeof(Ability))]
public class AbilityEditor : Editor
{
    SerializedProperty abilityName;
    SerializedProperty description;
    SerializedProperty cost;
    SerializedProperty visualEffect;
    SerializedProperty animation;
    SerializedProperty moveToSpectaclePos;
    SerializedProperty spectacleMoveType;
    SerializedProperty spectacleMoveDuration;
    SerializedProperty spectacleAttackDelay;

    SerializedProperty burstCast;
    SerializedProperty burstCount;
    SerializedProperty burstRate;

    SerializedProperty abilityType;
    SerializedProperty damageMin;
    SerializedProperty damageMax;
    SerializedProperty healMin;
    SerializedProperty healMax;
    SerializedProperty damageMultiplier;
    SerializedProperty defenseMultiplier;
    SerializedProperty abilityEffect;
    SerializedProperty accuracy;
    SerializedProperty cannotMiss;

    SerializedProperty duration;
    SerializedProperty turnDuration;

    SerializedProperty targetSelection;
    SerializedProperty fightersToTarget;
    SerializedProperty numberOfTargets;
    SerializedProperty randomizeSelection;

    SerializedProperty visualType;
    SerializedProperty attackReturnDelay;
    SerializedProperty attackReturnDuration;
    SerializedProperty firePattern;
    SerializedProperty castsPerTarget;
    SerializedProperty projectileSpeed;
    SerializedProperty arcAngle;
    SerializedProperty arcGravity;
    SerializedProperty visualDuration;
    SerializedProperty movementType;
    SerializedProperty moveDuration;
    SerializedProperty stopDistance;

    private void OnEnable()
    {
        abilityName = serializedObject.FindProperty("abilityName");
        description = serializedObject.FindProperty("description");
        cost = serializedObject.FindProperty("cost");
        visualEffect = serializedObject.FindProperty("visualEffect");
        animation = serializedObject.FindProperty("animation");
        moveToSpectaclePos = serializedObject.FindProperty("moveToSpectaclePos");
        spectacleMoveType = serializedObject.FindProperty("spectacleMoveType");
        spectacleMoveDuration = serializedObject.FindProperty("spectacleMoveDuration");
        spectacleAttackDelay = serializedObject.FindProperty("spectacleAttackDelay");

        burstCast = serializedObject.FindProperty("burstCast");
        burstCount = serializedObject.FindProperty("burstCount");
        burstRate = serializedObject.FindProperty("burstRate");

        abilityType = serializedObject.FindProperty("type");
        damageMin = serializedObject.FindProperty("damageMin");
        damageMax = serializedObject.FindProperty("damageMax");
        healMin = serializedObject.FindProperty("healMin");
        healMax = serializedObject.FindProperty("healMax");
        damageMultiplier = serializedObject.FindProperty("damageMultiplier");
        defenseMultiplier = serializedObject.FindProperty("defenseMultiplier");
        abilityEffect = serializedObject.FindProperty("abilityEffect");
        accuracy = serializedObject.FindProperty("accuracy");
        cannotMiss = serializedObject.FindProperty("cannotMiss");

        duration = serializedObject.FindProperty("duration");
        turnDuration = serializedObject.FindProperty("turnDuration");

        targetSelection = serializedObject.FindProperty("targetSelection");
        fightersToTarget = serializedObject.FindProperty("fightersToTarget");
        numberOfTargets = serializedObject.FindProperty("numberOfTargets");
        randomizeSelection = serializedObject.FindProperty("randomizeSelection");

        visualType = serializedObject.FindProperty("visualType");
        attackReturnDelay = serializedObject.FindProperty("attackReturnDelay");
        attackReturnDuration = serializedObject.FindProperty("attackReturnDuration");
        firePattern = serializedObject.FindProperty("firePattern");
        castsPerTarget = serializedObject.FindProperty("castsPerTarget");
        projectileSpeed = serializedObject.FindProperty("projectileSpeed");
        arcAngle = serializedObject.FindProperty("arcAngle");
        arcGravity = serializedObject.FindProperty("arcGravity");
        visualDuration = serializedObject.FindProperty("visualDuration");
        movementType = serializedObject.FindProperty("movementType");
        moveDuration = serializedObject.FindProperty("moveDuration");
        stopDistance = serializedObject.FindProperty("stopDistance");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Ability.AbilityType abilityMode = (Ability.AbilityType)abilityType.enumValueIndex;
        Ability.Duration durationMode = (Ability.Duration)duration.enumValueIndex;
        Ability.TargetSelection targetSelectMode = (Ability.TargetSelection)targetSelection.enumValueIndex;
        Ability.VisualType visualMode = (Ability.VisualType)visualType.enumValueIndex;

        //Base Settings
        EditorGUILayout.PropertyField(abilityName);
        EditorGUILayout.PropertyField(description);
        EditorGUILayout.PropertyField(cost);
        EditorGUILayout.PropertyField(animation);
        EditorGUILayout.PropertyField(moveToSpectaclePos);
        if (moveToSpectaclePos.boolValue)
        {
            EditorGUILayout.PropertyField(spectacleMoveType);
            EditorGUILayout.PropertyField(spectacleMoveDuration);
            EditorGUILayout.PropertyField(spectacleAttackDelay);
        }

        //Burst Cast
        EditorGUILayout.PropertyField (burstCast);
        if (burstCast.boolValue)
        {
            EditorGUILayout.PropertyField(burstCount);
            EditorGUILayout.PropertyField(burstRate);
        }

        //Ability Type
        EditorGUILayout.PropertyField(abilityType);
        switch (abilityMode)
        {
            case Ability.AbilityType.Damage:
                EditorGUILayout.PropertyField(damageMin);
                EditorGUILayout.PropertyField(damageMax);
                EditorGUILayout.PropertyField(accuracy);
                EditorGUILayout.PropertyField(cannotMiss);
                break;
            case Ability.AbilityType.Heal:
                EditorGUILayout.PropertyField(healMin);
                EditorGUILayout.PropertyField(healMax);
                EditorGUILayout.PropertyField(abilityEffect);
                EditorGUILayout.PropertyField(castsPerTarget);
                break;
            case Ability.AbilityType.Buff:
                EditorGUILayout.PropertyField(damageMultiplier);
                EditorGUILayout.PropertyField(defenseMultiplier);
                EditorGUILayout.PropertyField(abilityEffect);
                EditorGUILayout.PropertyField(castsPerTarget);
                EditorGUILayout.PropertyField(accuracy);
                EditorGUILayout.PropertyField(cannotMiss);
                break;
        }

        //Ability Duration
        EditorGUILayout.PropertyField(duration);
        switch (durationMode)
        {
            case Ability.Duration.Instant:
                break;
            case Ability.Duration.OverTime:
                EditorGUILayout.PropertyField(turnDuration);
                break;
        }

        //Target Selection        
        EditorGUILayout.PropertyField(fightersToTarget);
        EditorGUILayout.PropertyField(targetSelection);        

        switch (targetSelectMode)
        {
            case Ability.TargetSelection.Individual:
                EditorGUILayout.PropertyField(numberOfTargets);
                EditorGUILayout.PropertyField(randomizeSelection);
                break;
            case Ability.TargetSelection.All:
                break;
            case Ability.TargetSelection.Self:
                break;
        }

        //Visual
        EditorGUILayout.PropertyField(visualEffect);
        EditorGUILayout.PropertyField(visualDuration);
        EditorGUILayout.PropertyField(attackReturnDelay);
        EditorGUILayout.PropertyField(attackReturnDuration);

        if (abilityMode == Ability.AbilityType.Damage)
        {
            EditorGUILayout.PropertyField(visualType);
            switch (visualMode)
            {
                case Ability.VisualType.Reticule:
                    break;
                case Ability.VisualType.Melee:
                    EditorGUILayout.PropertyField(movementType);
                    EditorGUILayout.PropertyField(moveDuration);
                    EditorGUILayout.PropertyField(stopDistance);
                    break;
                case Ability.VisualType.Projectile:
                    EditorGUILayout.PropertyField(firePattern);
                    EditorGUILayout.PropertyField(castsPerTarget);
                    EditorGUILayout.PropertyField(projectileSpeed);
                    break;
                case Ability.VisualType.Arc:
                    EditorGUILayout.PropertyField(firePattern);
                    EditorGUILayout.PropertyField(castsPerTarget);
                    EditorGUILayout.PropertyField(arcAngle);
                    EditorGUILayout.PropertyField(arcGravity);
                    break;
                case Ability.VisualType.Overhead:
                    EditorGUILayout.PropertyField(firePattern);
                    EditorGUILayout.PropertyField(castsPerTarget);
                    EditorGUILayout.PropertyField(projectileSpeed);
                    break;
            }
        }
        

        serializedObject.ApplyModifiedProperties();
    }
}
