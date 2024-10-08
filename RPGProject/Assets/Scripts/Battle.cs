using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static UnityEditor.Progress;
using UnityEditor.Playables;

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

    public Dictionary<Item, int> playerItems = new Dictionary<Item, int>();
    public Dictionary<Item, int> enemyItems = new Dictionary<Item, int>();
    [HideInInspector] public Item activeItem;
    List<Fighter> itemTargets = new List<Fighter>();

    [SerializeField] GameObject fighterPrefab;

    public GameObject battleUI;
    [SerializeField] Vector2 fighterInitPosition = new Vector2(100f, 0f);
    [SerializeField] Vector2 fighterInterval = new Vector2(40f, 60f);
    [SerializeField] float fighterXAlternate = 40f;
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

    [Header("Item Selection")]
    [SerializeField] GameObject itemWindow;
    [SerializeField] GameObject itemContentBox;
    [SerializeField] GameObject itemButtonPrefab;
    [SerializeField] int itemsPerRow = 5;
    [SerializeField] float iconSpacing = 10f;
    List<ItemButton> itemList = new List<ItemButton>();

    [Header("Analyze")]
    [SerializeField] AnalysisMenu analyzeWindow;

    [Header("Other UI")]
    [SerializeField] float UISpacing = 30f;
    [SerializeField] TMP_Text descriptionText;
    public Fighter selectedFighter;
    public Button hoveredButton;
    public Canvas hitStatusCanvas;
    public GameObject topLayer;
    public GameObject stateButtonsObject;

    BattleMusic music;

    public enum SelectionModes
    {
        None,
        FighterSelect,
        AbilitySelect,
        ItemSelect,
        TargetSelect,
        InputDisabled,
        Analyze
    }
    [SerializeField] SelectionModes selectionMode = SelectionModes.InputDisabled;

    public enum BattleModes
    {
        Intro,
        Fight,
        End
    }
    [SerializeField] BattleModes battleMode = BattleModes.Intro;
    [SerializeField] float introBeginDelay = 1f;
    [SerializeField] float introSpawnSpacing = 0.5f;
    [SerializeField] float introEndSpacing = 0.05f;
    [SerializeField] float introEndDelay = 1f;
    [SerializeField] GameObject spawnEffect;

    public bool attackerAttacking;
    bool ending;

    // Start is called before the first frame update
    void Start()
    {
        hitStatusCanvas.worldCamera = FindObjectOfType<Camera>();

        music = FindObjectOfType<BattleMusic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (battleMode != BattleModes.Fight) return;

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
                if (itemList.Count > 0) UpdateItemDisplay();
                break;
        }

        if (!attackerAttacking) BattleStatus();
    }

    public void SetupBattle(List<FighterInfo> playerSide, List<FighterInfo> enemySide, Dictionary<Item, int> playerSideItems, Dictionary<Item, int> enemySideItems)
    {
        playerFighters = playerSide;
        enemyFighters = enemySide;
        playerItems = playerSideItems;
        enemyItems = enemySideItems;

        List<Vector2> playerPositions = FindFighterPositions(true);
        List<Vector2> enemyPositions = FindFighterPositions(false);

        for (int i = 0; i < playerFighters.Count; i++)
        {
            CreateFighter(playerFighters[i], playerPositions[i], true);
        }

        for (int i = 0; i < enemyFighters.Count; i++)
        {
            CreateFighter(enemyFighters[i], enemyPositions[i], false);
        }
    }

    public Fighter CreateFighter(FighterInfo info, Vector2 position, bool playerSide = true)
    {
        int offset = playerSide ? -1 : 1;

        //Spawn fighter in the world
        GameObject spawn = Instantiate(fighterPrefab, topLayer.gameObject.transform);

        //Set up fighter identity
        Fighter fighter = spawn.GetComponent<Fighter>();
        fighter.SetupFighter(info);
        fighter.spectaclePosition = new Vector2(offset * spectaclePosOffset.x, spectaclePosOffset.y);
        if (offset < 0)
        {
            playerSprites.Add(fighter);
        }
        else
        {
            enemySprites.Add (fighter);
        }

        fighter.transform.localScale = new Vector3(-offset, 1, 1);
        fighter.axis = -offset;

        //Position fighter in arena
        spawn.transform.position = position;
        fighter.idlePosition = position;

        return fighter;
    }

    public GameObject CreateSmokeBomb(Fighter fighter)
    {
        if (!spawnEffect) return null;

        GameObject particle = Instantiate(spawnEffect, transform);
        particle.transform.position = fighter.transform.position + Vector3.up * fighter.projectileYOffset;

        return particle;
    }

    public IEnumerator IntroSequence()
    {
        topLayer.gameObject.SetActive(true);

        for (int i = 0; i < playerSprites.Count; i++)
        {
            playerSprites[i].gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(introBeginDelay);
        music.PlayBattleTheme();

        for (int i = 0; i < playerSprites.Count; i++)
        {
            playerSprites[i].gameObject.SetActive(true);
            playerSprites[i].animator.SetTrigger("Intro");

            CreateSmokeBomb(playerSprites[i]);      

            yield return new WaitForSeconds(introSpawnSpacing);
        }

        yield return new WaitForSeconds(introEndDelay);

        for (int i = 0; i < playerSprites.Count; i++)
        {
            playerSprites[i].animator.SetTrigger("IntroEnd");
            yield return new WaitForSeconds(introEndSpacing);
        }

        battleMode = BattleModes.Fight;
        battleUI.SetActive(true);
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

        float xAlt = fighterXAlternate;

        for (int i = 0; i < fighters.Count; i++)
        {
            result.Add(new Vector2(x, y));

            x += anchorMod * (fighterInterval.x + xAlt);
            y -= fighterInterval.y;

            xAlt *= -1;
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
                abilityList[i].assignedAbility = selectedFighter.specialAbilities[i];

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

        for (int i = 0; i < selectedFighter.specialAbilities.Count; i++)
        {
            GameObject UISpawn = Instantiate(abilityButtonPrefab, abilityContentBox.transform);
            abilityList.Add(UISpawn.GetComponent<AbilityButton>());

            UISpawn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, UISpacing * -i);
        }
    }
    void UpdateItemDisplay()
    {
        List<Item> itemsInInventory = new List<Item>();
        foreach (KeyValuePair<Item, int> item in playerItems)
        {
            itemsInInventory.Add(item.Key);
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].assignedItem == null)
                itemList[i].assignedItem = itemsInInventory[i];

            itemList[i].DisplayInfo();
        }
    }
    void CreateItemDisplay()
    {
        foreach (ItemButton item in itemList)
        {
            Destroy(item.gameObject);
        }
        itemList.Clear();

        if (playerItems.Count == 0) return;

        int itemCount = 0;
        int rows = playerItems.Count / itemsPerRow;
        for (int i = 0; i <= rows; i++)
        {
            for (int j = 0; j < itemsPerRow; j++)
            {
                GameObject UISpawn = Instantiate(itemButtonPrefab, itemContentBox.transform);
                itemList.Add(UISpawn.GetComponent<ItemButton>());

                UISpawn.GetComponent<RectTransform>().anchoredPosition = new Vector2(iconSpacing * j, iconSpacing * -i);

                itemCount++;
                if (itemCount >= playerItems.Count) return;
            }
        }
    }

    public void SwitchSelectionMode(SelectionModes mode)
    {
        if (selectionMode == mode) return;
        if (attackerAttacking) return;

        selectionMode = mode;

        fighterWindow.SetActive(false);
        abilityWindow.SetActive(false);
        itemWindow.SetActive(false);
        analyzeWindow.gameObject.SetActive(false);
        battleUI.SetActive(true);

        itemTargets.Clear();

        switch (selectionMode)
        {
            case SelectionModes.None:
                if (stateButtonsObject)
                {
                    foreach (Transform child in stateButtonsObject.transform)
                    {
                        StateButton button = child.GetComponent<StateButton>();
                        if (button) button.SetButtonColor(false);
                    }
                }
                break;
            case SelectionModes.FighterSelect:
                fighterWindow.SetActive(true);
                CreateFighterDisplay();
                activeItem = null;
                break;
            case SelectionModes.AbilitySelect:
                abilityWindow.SetActive(true);
                CreateAbilityDisplay();
                break;
            case SelectionModes.ItemSelect:
                itemWindow.SetActive(true);
                CreateItemDisplay();
                selectedFighter = null;
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
    public void UseItem(Item item)
    {
        activeItem = item;

        switch (activeItem.ability.targetSelection)
        {
            case Ability.TargetSelection.All:
                if (activeItem.ability.fightersToTarget == Ability.FightersToTarget.Allies || activeItem.ability.fightersToTarget == Ability.FightersToTarget.Both)
                {
                    foreach (Fighter fighter in playerSprites)
                    {
                        CollectTarget(fighter);
                    }
                }

                if (activeItem.ability.fightersToTarget == Ability.FightersToTarget.Enemies || activeItem.ability.fightersToTarget == Ability.FightersToTarget.Both)
                {
                    foreach (Fighter fighter in enemySprites)
                    {
                        CollectTarget(fighter);
                    }
                }

                TriggerItemEffect();
                break;

            case Ability.TargetSelection.Self:
                Debug.Log("No self for items, skipping");
                break;
        }
    }

    void TriggerItemEffect()
    {
        if (!activeItem.ability.abilityEffect)
        {
            Debug.Log("No ability effect found");
            return;
        }

        playerItems[activeItem]--;
        if (playerItems[activeItem] <= 0) playerItems.Remove(activeItem);

        foreach (Fighter target in itemTargets)
        {
            for (int i = 0; i < activeItem.ability.castsPerTarget; i++)
            {
                if (activeItem.ability.abilityEffect.Trigger(target))
                {
                    if (activeItem.ability.visualEffect)
                    {
                        GameObject visual = Instantiate(activeItem.ability.visualEffect.gameObject, transform);
                        visual.transform.position = target.transform.position + Vector3.up * target.projectileYOffset;
                    }
                }
                
            }
        }

        SwitchSelectionMode(SelectionModes.None);
    }

    public void SelectTarget(Fighter target)
    {
        switch (selectionMode)
        {
            case SelectionModes.TargetSelect:
                CollectTarget(target); 
                break;
            case SelectionModes.Analyze:
                analyzeWindow.gameObject.SetActive(true);
                analyzeWindow.DisplayFighter(target);
                break;
        }
    }

    void CollectTarget(Fighter target)
    {
        if (selectionMode != SelectionModes.TargetSelect) return;

        bool ally = playerSprites.Contains(target);
        
        if (selectedFighter)
        {
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
        else
        {
            switch (activeItem.ability.fightersToTarget)
            {
                case Ability.FightersToTarget.Enemies:
                    if (ally) return;
                    break;
                case Ability.FightersToTarget.Allies:
                    if (!ally) return;
                    break;
            }

            itemTargets.Add(target);
            if (itemTargets.Count >= activeItem.ability.numberOfTargets) TriggerItemEffect();
        }
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

        SwitchSelectionMode(SelectionModes.None);

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
        if (!ending)
        {
            StartCoroutine(TransitionTo());
            ending = true;
        }
    }
    IEnumerator TransitionTo()
    {
        yield return new WaitForSeconds(1f);
        music.PlayVictory();
        FindObjectOfType<LineupLogic>().GetComponent<Animator>().SetTrigger("Victory");
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
