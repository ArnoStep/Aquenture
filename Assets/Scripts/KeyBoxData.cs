using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New KeyBox Data", menuName = "KeyBox Data", order = 52)]
public class KeyBoxData : ScriptableObject
{
    [HideInInspector]
    [SerializeField]
    public string keyColor;
    [HideInInspector]
    [SerializeField]
    public int keyColorSelection;
    [HideInInspector]
    public string[] keyColors = new string[] { "Red", "Green", "Blue" };
    [HideInInspector]
    [SerializeField]
    public int minigame;
    [HideInInspector]
    public string[] minigames = new string[] { "Reaction", "Memory" };
    [HideInInspector]
    [SerializeField]
    public int difficulty;
    [HideInInspector]
    public string[] difficulties = new string[] { "Easy", "Medium", "Hard" };

	private void OnEnable()
	{
        switch (keyColorSelection)
        {
            case 0:
                keyColor = "red";
                break;
            case 1:
                keyColor = "green";
                break;
            case 2:
                keyColor = "blue";
                break;
        }
    }
}
