using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stage", menuName = "TurnBasedSystem/Structure/Stage")]
public class StageInfo : ScriptableObject
{
    public Sprite stageMapGFX;
    public Sprite stageScenery;
    public List<MatchInfo> orderedMatches = new List<MatchInfo>();
    public bool isTown;
}
