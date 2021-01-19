using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageController : MonoBehaviour
{
	public CageData data;
	[HideInInspector]
	public int redKeys, greenKeys, blueKeys;
	public CreatureButton creatureButton;

	private void Awake()
	{
		redKeys = data.redKeys;
		greenKeys = data.greenKeys;
		blueKeys = data.blueKeys;
	}
}
