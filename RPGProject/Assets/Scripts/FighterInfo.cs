using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter_features", menuName = "ScriptableObject/Fighter")]
public class FighterInfo : ScriptableObject
{
    public string displayName = "";
    public string description = "";
    public int maxHealth = 1;
    public int currentHealth = 1;
    public int defense = 5;
    public int maxFP = 5;
    public int currentFP = 5;
    public int agility = 0;
    public List<Ability> abilities;
    public GameObject model;
}
