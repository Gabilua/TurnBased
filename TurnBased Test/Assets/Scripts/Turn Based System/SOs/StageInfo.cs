using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stage", menuName = "TurnBasedSystem/Structure/Stage")]
public class StageInfo : ScriptableObject
{
    public Sprite stageScenery;
    public List<MatchInfo> orderedMatches = new List<MatchInfo>();
    //public MatchInfo bossMatch { get; private set; }
    //public MatchInfo secretMatch { get; private set; }
}
