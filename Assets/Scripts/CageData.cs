using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cage Data", menuName = "Cage Data", order = 51)]
public class CageData : ScriptableObject
{
	[SerializeField]
	public int redKeys, greenKeys, blueKeys;
}
