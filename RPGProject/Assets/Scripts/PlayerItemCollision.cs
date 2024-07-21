using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollision : MonoBehaviour
{
    PlayerMovement player;

    private void Start()
    {
        player = GetComponentInParent<PlayerMovement>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Battle"))
        {
            WorldEnemy enemy = collision.gameObject.GetComponent<WorldEnemy>();
            player.StartBattle(enemy);
        }

        if (collision.gameObject.CompareTag("Chest"))
        {
            ItemChest chest = collision.gameObject.GetComponent<ItemChest>();
            player.chestsInRange.Add(chest);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Chest"))
        {
            ItemChest chest = collision.gameObject.GetComponent<ItemChest>();
            if (chest && player.chestsInRange.Contains(chest))
            {
                player.chestsInRange.Remove(chest);
            }
        }
    }
}
