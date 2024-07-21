using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum MoveStates
    {
        Idle,
        Walk,
        Run
    }
    //MoveStates moveStates = MoveStates.Idle;

    [SerializeField] float moveSpeed = 10f;
    [HideInInspector] public Rigidbody2D rigidBody;
    BoxCollider2D boxCollider;
    [HideInInspector] public GameManager gameManager;
    ExploreSpriteAnims anims;

    public List<FighterInfo> playerCharacters;
    public Dictionary<Item, int> items = new Dictionary<Item, int>();
    [SerializeField] Item testItem;
    public List<PlayerMoveCompanion> companions;
    [SerializeField] GameObject companionPrefab;

    public List<Vector2> footsteps;

    int maxFootsteps = 5;
    public static float footStepInterval = 0.8f;
    [SerializeField] int companionDistance = 3;

    public List<ItemChest> chestsInRange;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anims = GetComponentInChildren<ExploreSpriteAnims>();
        gameManager = FindObjectOfType<GameManager>();

        for (int i = 1; i < maxFootsteps * companionDistance; i++)
        {
            Vector2 footstep = transform.position + Vector3.up * -footStepInterval * i;
            footsteps.Add(footstep);
        }

        int index = 0;
        GameObject following = gameObject;
        foreach (FighterInfo companion in playerCharacters)
        {
            GameObject spawn = Instantiate(companionPrefab);
            PlayerMoveCompanion component = spawn.GetComponent<PlayerMoveCompanion>();

            component.following = following;
            following = spawn;

            component.destination = footsteps[index];
            companions.Add(component);
            index += companionDistance;
        }

        if (testItem)
        {
            items.Add(testItem, 5);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CreateFootstep();
    }

    public void Move(Vector2 direction)
    {
        rigidBody.velocity = direction.normalized * moveSpeed;
        anims.UpdateDirection(direction != Vector2.zero);
    }

    public void Interact()
    {
        if (chestsInRange.Count == 0) return;

        ItemChest closestChest = null;
        float closest = Mathf.Infinity;
        foreach (ItemChest chest in chestsInRange)
        {
            if (chest.Opened()) continue;

            float dist = Vector3.Distance(chest.transform.position, transform.position);
            if (dist < closest)
            {
                closest = dist;
                closestChest = chest;
            }
        }

        if (!closestChest) return;

        List<Item> itemsInRange = closestChest.GetItemsInChest();
        foreach (Item item in itemsInRange)
        {
            AddItem(item);
        }
    }

    public void AddItem(Item item)
    {
        if (!items.ContainsKey(item))
        {
            items.Add(item, 0);
        }

        items[item] += 1;
        Debug.Log("Item acquired!");
    }

    public void StartBattle(WorldEnemy enemy)
    {
        if (!enemy) return;
        if (playerCharacters.Count == 0 || enemy.enemies.Count == 0) return;

        gameManager.StateTransition(GameManager.GameStates.Battle);
        gameManager.CreateBattle(playerCharacters, enemy.enemies, items, enemy.items);
    }
    void CreateFootstep()
    {
        if (Vector2.Distance(transform.position, footsteps[0]) < footStepInterval) return;

        Vector2 footStep = transform.position;
        for (int i = 1; i < footsteps.Count; i++)
        {
            footsteps[i] = footsteps[i - 1];
        }
        footsteps[0] = footStep;
        
        int index = 0;
        foreach (PlayerMoveCompanion companion in companions)
        {
            companion.destination = footsteps[index];
            index += companionDistance;
        }
    }
}
