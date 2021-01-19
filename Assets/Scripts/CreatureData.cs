using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creature Data", menuName = "Creature Data", order = 53)]
public class CreatureData : ScriptableObject
{
    [SerializeField]
    public string creatureName;
    [SerializeField]
    public Sprite image;
}
