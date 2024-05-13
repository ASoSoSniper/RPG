using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StateButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Battle battle;
    public Battle.SelectionModes transitionsTo;
    public Button button;
    public string description;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        battle = FindObjectOfType<Battle>();
        button = GetComponent<Button>();
        animator = GetComponent<Animator>();
    }


    public virtual void SwitchState()
    {
        if (!battle) return;

        battle.SwitchSelectionMode(transitionsTo);
    }

    public virtual void DisplayDescription()
    {
        battle.SetHoveredButton(button, description);
    }

    public virtual void ToggleEnable(bool enabled)
    {
        if (button) button.interactable = enabled;
        GetComponent<Image>().color = enabled ? Color.white : Color.gray;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DisplayDescription();
        if (animator) animator.SetBool("Hovering", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animator) animator.SetBool("Hovering", false);
    }
}
