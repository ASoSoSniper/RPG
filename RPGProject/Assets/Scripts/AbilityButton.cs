using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityButton : StateButton
{
    [SerializeField] TMP_Text abilityName;
    [SerializeField] TMP_Text cost;
    [HideInInspector] public Ability assignedAbility;

    public void DisplayInfo()
    {
        if (abilityName) abilityName.text = assignedAbility.abilityName;

        if (cost) cost.text = "FP: " + assignedAbility.cost;

        if (battle) ToggleEnable(battle.selectedFighter.currentFP >= assignedAbility.cost);
    }

    public override void SwitchState()
    {
        base.SwitchState();
        battle.UseAbility(assignedAbility);
    }

    public override void DisplayDescription()
    {
        battle.SetHoveredButton(button, assignedAbility.description);
    }
}
