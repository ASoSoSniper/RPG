using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : StateButton
{
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text quantity;
    [SerializeField] Image icon;
    [HideInInspector] public Item assignedItem;
    public override void SwitchState()
    {
        base.SwitchState();
        battle.UseItem(assignedItem);
    }

    public void DisplayInfo()
    {
        if (itemName) itemName.text = assignedItem.name;

        if (quantity && battle) quantity.text = battle.playerItems[assignedItem].ToString();

        if (icon) icon.sprite = assignedItem.icon;
    }

    public override void DisplayDescription()
    {
        battle.SetHoveredButton(button, assignedItem.description);
    }
}
