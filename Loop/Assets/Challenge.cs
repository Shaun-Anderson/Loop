using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Challenge {
    public string challengeTitle;
    public int gemsNeeded;
    public int distanceNeeded;
    public int rewardGems;
}

public enum ChallengeType
{
    Gems,
    Distance
}
