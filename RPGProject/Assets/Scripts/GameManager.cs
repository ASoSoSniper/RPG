using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameStates
    {
        Explore,
        Talk,
        Battle,
        Pause
    }
    public GameStates gameState;

    PlayerMovement player;
    public Battle activeBattle;
    [SerializeField] GameObject battlePrefab;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameStates.Explore:
                player.Move(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
                break;

            case GameStates.Talk:
                break;

            case GameStates.Battle:
                break;

            case GameStates.Pause:
                break;
        }
    }

    public void StateTransition(GameStates state)
    {
        switch (state)
        {
            case GameStates.Explore:
                if (activeBattle) Destroy(activeBattle.gameObject);
                break;
            case GameStates.Battle:
                
                break;
        }
    }

    public void CreateBattle(List<FighterInfo> playerFighters, List<FighterInfo> enemyFighters, Dictionary<Item, int> playerItems, Dictionary<Item, int> enemyItems)
    {
        if (!activeBattle)
        {
            activeBattle = Instantiate(battlePrefab).GetComponent<Battle>();
            activeBattle.SetupBattle(playerFighters, enemyFighters, playerItems, enemyItems);
        }
    }
}
