using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "TurnBasedSystem/Status Effect")]
public class StatusEffectInfo : ScriptableObject
{
    [Header("Basic Info")]
    public string effectName;
    public Sprite effectIcon;
    public EffectType effectType;
    public int afflictionChance;
    public int durationInTurns;

    [Header("Power")]
    public int effectBaseValue;
    public bool affectedByStats;
    public StatEffectOnBaseValue statEffectOnBaseValue;
    public EffectiveStat effectiveStat;

    [Header("Visual Feedback Parameters")]
    public GameObject receiveVFX;
}
