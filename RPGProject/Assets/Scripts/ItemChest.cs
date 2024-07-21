using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChest : MonoBehaviour
{
    [SerializeField] List<Item> itemsInChest;
    [SerializeField] GameObject statusMessage;
    bool opened = false;

    public List<Item> GetItemsInChest()
    {
        if (opened) return new List<Item>();

        opened = true;
        foreach (Item item in itemsInChest)
        {
            GameObject message = Instantiate(statusMessage, FindObjectOfType<Canvas>().transform);
            message.transform.position = transform.position;

            HitStatus status = message.GetComponent<HitStatus>();
            if (status)
            {
                status.InputValues(item.title);
                status.Launch();
            }
        }

        return itemsInChest;
    }

    public bool Opened()
    {
        return opened;
    }
}
