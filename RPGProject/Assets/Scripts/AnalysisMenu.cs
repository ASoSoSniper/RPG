using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnalysisMenu : MonoBehaviour
{
    [SerializeField] GameObject listElementPrefab;

    [SerializeField] Transform abilityContentBox;
    [SerializeField] Transform effectContentBox;

    [SerializeField] TMP_Text fighterName;
    [SerializeField] TMP_Text fighterHP;
    [SerializeField] TMP_Text fighterFP;
    [SerializeField] TMP_Text fighterDef;
    [SerializeField] TMP_Text fighterAgil;

    List<GameObject> abilities = new List<GameObject>();
    List<GameObject> effects = new List<GameObject>();

    public void DisplayFighter(Fighter fighter)
    {
        fighterName.text = fighter.fighterInfo.displayName;
        fighterHP.text = "HP: " + fighter.currentHP.ToString() + "/" + fighter.fighterInfo.maxHealth.ToString();
        fighterFP.text = "FP: " + fighter.currentFP.ToString() + "/" + fighter.fighterInfo.maxFP.ToString();
        fighterDef.text = "Defense: " + fighter.fighterInfo.defense.ToString();
        fighterAgil.text = "Agility: " + fighter.fighterInfo.agility.ToString();

        foreach (GameObject element in abilities)
        {
            Destroy(element);
        }
        foreach (GameObject element in effects)
        {
            Destroy(element);
        }

        int abilityIndex = 0;
        foreach (Ability ability in fighter.fighterInfo.abilities)
        {
            GameObject spawn = Instantiate(listElementPrefab, abilityContentBox);
            spawn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50 * abilityIndex);

            spawn.GetComponent<AnalysisListElement>().SetDisplayValues(ability.abilityName, ability.cost.ToString());
            abilities.Add(spawn);
            abilityIndex++;
        }

        int effectIndex = 0;
        foreach (KeyValuePair<AbilityEffect, int> effect in fighter.activeEffects)
        {
            GameObject spawn = Instantiate(listElementPrefab, effectContentBox);
            spawn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50 * effectIndex);

            spawn.GetComponent<AnalysisListElement>().SetDisplayValues(effect.Key.effectName, effect.Value.ToString());
            effects.Add(spawn);
            effectIndex++;
        }
    }
}
