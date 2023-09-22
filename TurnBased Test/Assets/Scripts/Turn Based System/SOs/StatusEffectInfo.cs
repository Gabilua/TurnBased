using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TemporaryStatChanges
{
    public TargetStat targetStat;
    public int effectBaseValue;
    public StatEffectOnBaseValue statEffectOnBaseValue;
    public StatusEffectInfo sourceOfChanges;
}

[CreateAssetMenu(fileName = "New Status Effect", menuName = "TurnBasedSystem/Combat/StatusEffect")]
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
    [Range(0, 100)]
    public int chanceToInterruptActions;

    [Header("Visual Feedback Parameters")]
    public GameObject receiveVFX;
}
