using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SaveData {
    public int highscore;
    public int gems;
    public bool tutorialCompleted;
    // Options
    public bool accelEnabled;
    public bool volumeOn;
    public int curGemChallenge;
    public int curDistanceChallenge;

    // Unlocks
    public int color;
    public List<int> colorsUnlocked;
    public int skin;
    public List<int> skinsUnlocked;
}
