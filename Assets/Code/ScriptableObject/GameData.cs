﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Zomz/Data/New GameData", fileName = "GameData_New")]
public class GameData : ScriptableObject  
{
    public bool IsPaused = false;

    public LevelData CurrentLevelData;

    public CurrentConversation CurrentConversation;

	private void OnDisable()
	{
        IsPaused = false;
	}
}
