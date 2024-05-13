using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleAI : MonoBehaviour
{
    Battle battle;

    // Start is called before the first frame update
    void Start()
    {
        battle = FindObjectOfType<Battle>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfCanAttack();
    }

    void CheckIfCanAttack()
    {
        if (battle.currentTurn != Battle.Turns.EnemyTurn) return;
        if (battle.attackerAttacking) return;

        AttackRandom();
    }

    void AttackRandom()
    {
        List<Fighter> selectableFighters = new List<Fighter>();
        for (int i = 0; i < battle.enemySprites.Count; i++)
        {
            if (!battle.enemySprites[i].turnEnded) selectableFighters.Add(battle.enemySprites[i]);
        }

        if (selectableFighters.Count == 0) return;

        Fighter selectedFighter = selectableFighters[Random.Range(0, selectableFighters.Count)];

        selectedFighter.activeAbility = selectedFighter.fighterInfo.abilities[Random.Range(0, selectedFighter.fighterInfo.abilities.Count)];

        for (int i = 0; i < selectedFighter.activeAbility.numberOfTargets; i++)
        {
            selectedFighter.AddTarget(battle.playerSprites[Random.Range(0, battle.playerSprites.Count)]);
        }
    }
}
