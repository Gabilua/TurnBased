using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType { Damaging, Healing, Altering }
public enum TargetStat { HP, MP, STR, VIT, DEX, AGI, INT, ACCURACY, DODGE }
public enum StatEffectOnBaseValue { Additive, Multiplicative }
public enum EffectiveStat {  Strenght, Vitality, Dexterity, Agility, Intelligence}
public enum CostStat { HP, MP }
public enum TargetRange { AnySingleTarget, AnySingleEnemy, AnySingleAlly, EnemySide, AllySide, Everyone, OnlyUser }

[CreateAssetMenu(fileName = "New Skill", menuName = "TurnBasedSystem/Combat/Skill")]
public class SkillInfo : ScriptableObject
{
    [Header("Basic Info")]
    public Sprite skillIcon;
    public EffectType effectType;
    public TargetStat targetStat;
    public float hitChance;
    public string skillDescription;

    [Header("Power")]
    public int effectBaseValue;
    public bool affectedByStats;
    public StatEffectOnBaseValue statEffectOnBaseValue;
    public EffectiveStat effectiveStat;

    [Header("Cost")]
    public CostStat costStat;
    public int costAmount;

    [Header("Range")]
    public TargetRange targetRange;
    public bool selfTargetingAllowed;

    [Header("Visual Feedback Parameters")]
    public GameObject useVFX;
    public GameObject receiveVFX;
    public bool usesSpecialAnimation;
    public float targetReactionDelay;

    [Header("Status Effect")]
    public bool appliesStatusEffect;
    public StatusEffectInfo statusEffect;
    public int afflictionChance;
}
