using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "TurnBasedSystem/Status Effect")]
public class StatusEffectInfo : ScriptableObject
{
    [Header("Basic Info")]
    public Sprite effectIcon;
    public EffectType effectType;
    public TargetStat targetStat;
    public int durationInTurns;

    [Header("Power")]
    public int effectBaseValue;
    public bool affectedByStats;
    public StatEffectOnBaseValue statEffectOnBaseValue;
    public EffectiveStat effectiveStat;
    public bool effectStacksDuration;

    [Header("Visual Feedback Parameters")]
    public GameObject receiveVFX;
}
