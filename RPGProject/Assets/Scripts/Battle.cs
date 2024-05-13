using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Battle : MonoBehaviour
{
    public enum Turns
    {
        PlayerTurn,
        EnemyTurn
    }
    public Turns currentTurn;

    [SerializeField] List<FighterInfo> playerFighters; //Temporary until combat starts organically
    [SerializeField] List<FighterInfo> enemyFighters;

    public List<Fighter> playerSprites = new List<Fighter>();
    public List<Fighter> enemySprites = new List<Fighter>();

    [SerializeField] GameObject fighterPrefab;

    [SerializeField] GameObject battleUI;
    [SerializeField] Vector2 fighterInitPosition = new Vector2(100f, 0f);
    [SerializeField] Vector2 fighterInterval = new Vector2(40f, 60f);
    public Vector2 spectaclePosOffset = new Vector2(0f, 0f);

    [Header("Fighter Selection")]
    [SerializeField] GameObject fighterWindow;
    [SerializeField] GameObject fighterContentBox;
    [SerializeField] GameObject fighterButtonPrefab;
    List<FighterButton> fighterList = new List<FighterButton>();

    [Header("Ability Selection")]
    [SerializeField] GameObject abilityWindow;
    [SerializeField] GameObject abilityContentBox;
    [SerializeField] GameObject abilityButtonPrefab;
    List<AbilityButton> abilityList = new List<AbilityButton>();

    [Header("Other UI")]
    [SerializeField] float UISpacing = 30f;
    [SerializeField] TMP_Text descriptionText;
    public Fighter selectedFighter;
    public Button hoveredButton;
    public Canvas hitStatusCanvas;

    public enum SelectionModes
    {
        None,
        FighterSelect,
        AbilitySelect,
        ItemSelect,
        TargetSelect,
        InputDisabled
    }
    SelectionModes selectionMode = SelectionModes.None;
    public bool attackerAttacking;

    // Start is called before the first frame update
    void Start()
    {
        hitStatusCanvas.worldCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (selectionMode)
        {
            case SelectionModes.None:
                
                break;
            case SelectionModes.FighterSelect:
                if (fighterList.Count > 0) UpdateFighterDisplay();
                break;
            case SelectionModes.AbilitySelect:
                if (abilityList.Count > 0) UpdateAbilityDisplay();
                break;
            case SelectionModes.ItemSelect:
                break;
        }

        if (!attackerAttacking) BattleStatus();
    }

    public void SetupBattle(List<FighterInfo> playerSide, List<FighterInfo> enemySide)
    {
        playerFighters = playerSide;
        enemyFighters = enemySide;

        List<Vector2> playerPositions = FindFighterPositions(true);
        List<Vector2> enemyPositions = FindFighterPositions(false);

        for (int i = 0; i < playerFighters.Count; i++)
        {
            //Spawn fighter in the world
            GameObject spawn = Instantiate(fighterPrefab, this.gameObject.transform);

            //Set up fighter identity
            Fighter fighter = spawn.GetComponent<Fighter>();
            fighter.SetupFighter(playerFighters[i]);
            fighter.spectaclePosition = new Vector2(-spectaclePosOffset.x, spectaclePosOffset.y);            
            playerSprites.Add(fighter);

            //Position fighter in arena
            if (i < playerPositions.Count)
            {
                spawn.transform.position = playerPositions[i];
                fighter.idlePosition = playerPositions[i];
            }   
        }

        for (int i = 0; i < enemyFighters.Count; i++)
        {
            //Spawn fighter in the world
            GameObject spawn = Instantiate(fighterPrefab, this.gameObject.transform);

            //Set up fighter identity
            Fighter fighter = spawn.GetComponent<Fighter>();
            fighter.SetupFighter(enemyFighters[i]);
            fighter.spectaclePosition = spectaclePosOffset;
            enemySprites.Add(fighter);

            fighter.transform.localScale = new Vector3(-1, 1, 1);
            fighter.axis = -1;

            //Position fighter in arena
            if (i < enemyPositions.Count)
            {
                spawn.transform.position = enemyPositions[i];
                fighter.idlePosition = enemyPositions[i];
            }
        }

        CreateFighterDisplay();
    }

    List<Vector2> FindFighterPositions(bool playerSide)
    {
        List<Vector2> result = new List<Vector2>();

        int anchorMod = playerSide ? -1 : 1;
        List<FighterInfo> fighters = playerSide ? playerFighters : enemyFighters;

        float initX = (fighterInterval.x * (fighters.Count - 1f)) / 2f;
        float initY = (fighterInterval.y * (fighters.Count - 1f)) / 2f;

        float x = anchorMod * (fighterInitPosition.x - initX);
        float y = fighterInitPosition.y + initY;

        for (int i = 0; i < fighters.Count; i++)
        {
            result.Add(new Vector2(x, y));

            x += anchorMod * fighterInterval.x;
            y -= fighterInterval.y;
        }

        return result;
    }

    void UpdateFighterDisplay()
    {
        for (int i = 0; i < fighterList.Count; i++)
        {
            if (fighterList[i].assignedFighter == null)
                fighterList[i].assignedFighter = playerSprites[i];

            fighterList[i].DisplayInfo();
        }
    }

    void CreateFighterDisplay()
    {
        foreach (FighterButton fighter in fighterList)
        {
            Destroy(fighter.gameObject);
        }
        fighterList.Clear();

        for (int i = 0; i < playerFighters.Count; i++)
        {
            GameObject UISpawn = Instantiate(fighterButtonPrefab, fighterContentBox.transform);
            fighterList.Add(UISpawn.GetComponent<FighterButton>());

            UISpawn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, UISpacing * -i);
        }
    }

    void UpdateAbilityDisplay()
    {
        for (int i = 0; i < abilityList.Count; i++)
        {
            if (abilityList[i].assignedAbility == null)
                abilityList[i].assignedAbility = selectedFighter.fighterInfo.abilities[i];

            abilityList[i].DisplayInfo();
        }
    }

    void CreateAbilityDisplay()
    {
        foreach (AbilityButton ability in  abilityList)
        {
            Destroy(ability.gameObject);
        }
        abilityList.Clear();

        for (int i = 0; i < selectedFighter.fighterInfo.abilities.Count; i++)
        {
            GameObject UISpawn = Instantiate(abilityButtonPrefab, abilityContentBox.transform);
            abilityList.Add(UISpawn.GetComponent<AbilityButton>());

            UISpawn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, UISpacing * -i);
        }
    }

    public void SwitchSelectionMode(SelectionModes mode)
    {
        if (selectionMode == mode) return;
        if (attackerAttacking) return;

        selectionMode = mode;

        fighterWindow.SetActive(false);
        abilityWindow.SetActive(false);
        battleUI.SetActive(true);

        switch (selectionMode)
        {
            case SelectionModes.None:
                break;
            case SelectionModes.FighterSelect:
                fighterWindow.SetActive(true);
                break;
            case SelectionModes.AbilitySelect:
                abilityWindow.SetActive(true);
                CreateAbilityDisplay();
                break;
            case SelectionModes.ItemSelect:
                break;
            case SelectionModes.TargetSelect:
                break;
            case SelectionModes.InputDisabled:
                battleUI.SetActive(false);
                break;
        }
    }

    public void SetHoveredButton(Button button, string description = null)
    {
        hoveredButton = button;

        if (!descriptionText) return;

        if (description == null) descriptionText.text = "";
        else descriptionText.text = description;
    }

    public void UseAbility(Ability ability)
    {
        selectedFighter.SetupAbility(ability);

        //Automatically collect fighters in relevant categories if all are affected
        switch (ability.targetSelection)
        {
            case Ability.TargetSelection.All:
                if (ability.fightersToTarget == Ability.FightersToTarget.Allies || ability.fightersToTarget == Ability.FightersToTarget.Both)
                {
                    foreach (Fighter fighter in playerSprites)
                    {
                        CollectTarget(fighter);
                    }
                }

                if (ability.fightersToTarget == Ability.FightersToTarget.Enemies || ability.fightersToTarget == Ability.FightersToTarget.Both)
                {
                    foreach (Fighter fighter in enemySprites)
                    {
                        CollectTarget(fighter);
                    }
                }

                selectedFighter.BeginAttack();
                break;

            case Ability.TargetSelection.Self:
                CollectTarget(selectedFighter);
                selectedFighter.BeginAttack();
                break;
        }
    }

    public void CollectTarget(Fighter target)
    {
        if (selectionMode != SelectionModes.TargetSelect) return;

        bool ally = playerSprites.Contains(target);
        bool bypassTargetMods = false;

        switch (selectedFighter.activeAbility.fightersToTarget)
        {
            case Ability.FightersToTarget.Enemies:
                if (ally) return;
                bypassTargetMods = false;
                break;
            case Ability.FightersToTarget.Allies:
                if (!ally) return;
                bypassTargetMods = true;
                break;
        }

        selectedFighter.AddTarget(target, bypassTargetMods);
    }

    void OrderFightersByDepth()
    {
        List<Fighter> sortedFighters = new List<Fighter>();
        
        foreach (Fighter fighter in playerSprites)
        {
            sortedFighters.Add(fighter);
        }
        foreach (Fighter fighter in enemySprites)
        {
            sortedFighters.Add(fighter);
        }

        sortedFighters = sortedFighters.OrderByDescending(x => x.transform.position.y).ToList();

        int layerDepth = 2;

        for (int i = 0; i < sortedFighters.Count; i++)
        {
            sortedFighters[i].GetComponentInChildren<SpriteRenderer>().sortingOrder = layerDepth;
            layerDepth++;
        }
    }

    public void TurnCheck()
    {
        attackerAttacking = false;

        int inactiveFighters = 0;

        switch (currentTurn)
        {
            case Turns.PlayerTurn:
                for (int i = 0; i < playerSprites.Count; i++)
                {
                    if (playerSprites[i].turnEnded) inactiveFighters++;
                }

                //If all player fighters have moved, transition to enemy turn
                if (inactiveFighters == playerSprites.Count)
                {
                    Debug.Log("Enemy turn");
                    currentTurn = Turns.EnemyTurn;
                    CountdownStatusEffects();
                    for (int i = 0; i < playerSprites.Count; i++)
                    {
                        if (playerSprites[i].actionState != Fighter.ActionStates.Dead)
                            playerSprites[i].turnEnded = false;
                    }
                }
                break;
            case Turns.EnemyTurn:
                for (int i = 0; i < enemySprites.Count; i++)
                {
                    if (enemySprites[i].turnEnded) inactiveFighters++;
                }

                //If all enemy fighters have moved, transition to player turn
                if (inactiveFighters == enemySprites.Count)
                {
                    Debug.Log("Player turn");
                    currentTurn = Turns.PlayerTurn;
                    CountdownStatusEffects();
                    for (int i = 0; i < enemySprites.Count; i++)
                    {
                        if (enemySprites[i].actionState != Fighter.ActionStates.Dead)
                            enemySprites[i].turnEnded = false;
                    }
                }

                break;
        }
    }
    void BattleStatus()
    {
        int killedFighters = 0;

        for (int i = 0; i < enemySprites.Count; i++)
        {
            if (enemySprites[i].actionState == Fighter.ActionStates.Dead) killedFighters++;
        }

        /*for (int i = 0; i < playerSprites.Count; i++)
        {
            if (playerSprites[i].actionState == Fighter.ActionStates.Dead) killedFighters++;
        }*/

        //If all enemy fighters are killed, end battle
        if (killedFighters == enemySprites.Count)
        {
            Debug.Log("Battle ended");
            EndBattle();
        }
    }
    void EndBattle()
    {
        FindObjectOfType<GameManager>().StateTransition(GameManager.GameStates.Explore);
    }

    //For active status effects for each fighter, countdown each effect's turn duration and, if stated, trigger the effect at the start of the turn
    void CountdownStatusEffects()
    {
        //Choose the fighter list depending on whose turn it is
        List<Fighter> fighters = currentTurn == Turns.PlayerTurn ? playerSprites : enemySprites;

        //For each fighter in that group:
        foreach (Fighter fighter in fighters)
        {
            //Create list of effects to remove
            List<AbilityEffectDuration> effectsToRemove = new List<AbilityEffectDuration>();

            //For each effect with a duration:
            foreach (AbilityEffectDuration effectDuration in fighter.effectDurations)
            {
                //Count down the turn duration
                effectDuration.turns--;

                //If effect is set to cast at start of turn, trigger this effect
                if (effectDuration.effect.castOnTurnStart && effectDuration.turns >= 0) 
                    effectDuration.effect.TriggerOnTurnStart(fighter);

                //If the effect has depleted its active turns, add to list of effects to remove
                if (effectDuration.turns < 0)
                {
                    effectsToRemove.Add(effectDuration);
                }
            }

            //For each effect set to be removed, perform remove behaviors
            foreach (AbilityEffectDuration effectDuration in effectsToRemove)
            {
                effectDuration.effect.RemoveEffect(fighter, 1);
                fighter.effectDurations.Remove(effectDuration);
            }
        }
    }
}
