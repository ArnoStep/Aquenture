using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoxController : MonoBehaviour
{
    public KeyBoxData data;
    [HideInInspector]
    public int minigame;
    [HideInInspector]
    public string keyColor;
    [HideInInspector]
    public int difficulty;
    [HideInInspector]
    public bool completed = false;

    private void Awake()
    {
        minigame = data.minigame;
        keyColor = data.keyColor;
        difficulty = data.difficulty;
    }
}
