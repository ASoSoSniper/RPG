using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_features", menuName = "ScriptableObject/Item")]
public class Item : ScriptableObject
{
    public string title;
    public string description;
    public Ability ability;
    public Sprite icon;
}
