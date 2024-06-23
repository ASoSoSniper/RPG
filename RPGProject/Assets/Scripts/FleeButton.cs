using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeButton : StateButton
{
    public override void SwitchState()
    {
        FindObjectOfType<LineupLogic>().GetComponent<Animator>().SetTrigger("Flee");
    }
}
