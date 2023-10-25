using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageReward
{
    public EquipmentInfo equipmentReward;
    public int equipmentDropChance;
}

[CreateAssetMenu(fileName = "New Stage", menuName = "TurnBasedSystem/Structure/Stage")]
public class StageInfo : ScriptableObject
{
    public Sprite stageMapGFX;
    public Sprite stageScenery;
    public AudioClip stageBGM;
    public List<MatchInfo> orderedMatches = new List<MatchInfo>();
    public List<StageReward> stageEquipmentRewards = new List<StageReward>();
    public Vector2 stageGoldRewardRange;
    public bool isTown;
}
