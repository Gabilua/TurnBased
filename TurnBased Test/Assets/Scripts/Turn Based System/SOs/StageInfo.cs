using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stage", menuName = "TurnBasedSystem/Structure/Stage")]
public class StageInfo : ScriptableObject
{
    public Sprite stageMapGFX;
    public Sprite stageScenery;
    public AudioClip stageBGM;
    public List<MatchInfo> orderedMatches = new List<MatchInfo>();
    public List<EquipmentInfo> stageEquipmentRewards = new List<EquipmentInfo>();
    public Vector2 stageGoldRewardRange;
    public bool isTown;
}
