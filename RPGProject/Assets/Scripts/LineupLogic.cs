using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineupLogic : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> playerFighters = new List<SpriteRenderer>();
    [SerializeField] List<SpriteRenderer> enemyFighters = new List<SpriteRenderer>();
    [SerializeField] GameObject lineUpObject;
    Battle battle;

    private void Start()
    {
        battle = FindObjectOfType<Battle>();
    }

    public void SetSprites()
    {
        lineUpObject.SetActive(true);

        for (int i = 0; i < battle.playerSprites.Count; i++)
        {
            if (i > playerFighters.Count - 1) break;

            GameObject sprite = Instantiate(battle.playerSprites[i].fighterInfo.model, playerFighters[i].transform);
            SpriteRenderer renderer = sprite.GetComponent<SpriteRenderer>();
            renderer.color = playerFighters[i].color;
            renderer.sortingOrder = playerFighters[i].sortingOrder;
        }

        for (int i = 0; i < battle.enemySprites.Count; i++)
        {
            if (i > enemyFighters.Count - 1) break;

            GameObject sprite = Instantiate(battle.enemySprites[i].fighterInfo.model, enemyFighters[i].transform);
            SpriteRenderer renderer = sprite.GetComponent<SpriteRenderer>();
            renderer.color = enemyFighters[i].color;
            renderer.sortingOrder = enemyFighters[i].sortingOrder;
        }
    }

    public void StartBattle()
    {
        lineUpObject.SetActive(false);
        battle.StartCoroutine(battle.IntroSequence());
    }

    public void HideUI()
    {
        battle.battleUI.SetActive(false);
    }
    public void HideBattle()
    {
        battle.topLayer.SetActive(false);
    }

    public void EndBattle()
    {
        FindObjectOfType<GameManager>().StateTransition(GameManager.GameStates.Explore);
    }
}
