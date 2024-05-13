using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FighterButton : StateButton
{
    [SerializeField] TMP_Text fighterName;
    [SerializeField] TMP_Text health;
    [SerializeField] TMP_Text FP;
    [HideInInspector] public Fighter assignedFighter;

    public void DisplayInfo()
    {
        if (fighterName) fighterName.text = assignedFighter.fighterInfo.displayName;

        if (health) health.text = "HP: " + assignedFighter.currentHP + "/" + assignedFighter.fighterInfo.maxHealth;

        if (FP) FP.text = "FP: " + assignedFighter.currentFP + "/" + assignedFighter.fighterInfo.maxFP;

        if (assignedFighter)
        {
            if (assignedFighter.actionState == Fighter.ActionStates.Dead)
            {
                ToggleEnable(false);
            }
            else
            {
                ToggleEnable(!assignedFighter.turnEnded);
            }
        }
    }

    public override void SwitchState()
    {
        battle.selectedFighter = assignedFighter;

        base.SwitchState();
    }

    public override void DisplayDescription()
    {
        battle.SetHoveredButton(button, assignedFighter.fighterInfo.description);
    }
}
