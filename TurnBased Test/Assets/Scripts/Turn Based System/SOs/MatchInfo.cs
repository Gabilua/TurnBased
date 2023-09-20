using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Match", menuName = "TurnBasedSystem/Structure/Match")]
public class MatchInfo : ScriptableObject
{
    public List<CharacterInfo> _matchOpponents = new List<CharacterInfo>();
}
