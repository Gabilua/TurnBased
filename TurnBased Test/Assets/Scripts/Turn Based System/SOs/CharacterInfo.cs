using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EquipmentSet
{
    public List<EquipmentInfo> equipments = new List<EquipmentInfo>();
}

[System.Serializable]
public class CharacterStats
{
    enum CharacterStatTier { Minion, Creature, Hero, Boss}

    [Header("How Strong is This Character?")]
    [SerializeField] CharacterStatTier characterStatTier;

    [Header("Main Stats")]
    [Range(1, 5)]
  [SerializeField]  int strenght;
    [Range(1, 5)]
    [SerializeField] int vitality;
    [Range(1, 5)]
    [SerializeField] int dexterity;
    [Range(1, 5)]
    [SerializeField] int agility;
    [Range(1, 5)]
    [SerializeField] int intelligence;

    int maxHP;
    int maxMP;
    int dodge;
    int accuracy;

    List<TemporaryStatChanges> _temporaryStatChanges = new List<TemporaryStatChanges>();

    public void CopyBaseStats(CharacterStats referenceStats)
    {
        characterStatTier = referenceStats.characterStatTier;

        strenght = referenceStats.strenght;
        vitality = referenceStats.vitality;
        dexterity = referenceStats.dexterity;
        agility = referenceStats.agility;
        intelligence = referenceStats.intelligence;
    }

    public int GetFinalStat(TargetStat stat)
    {
        int finalStat = 0;
        int additiveBonus = 0;
        int multiplicativeBonus = 1;
        int tierMultiplier = (int)characterStatTier + 1;

        switch (stat)
        {
            case TargetStat.HP:
                finalStat = maxHP = Mathf.CeilToInt((35 * tierMultiplier) + (vitality * 60 * tierMultiplier));
                break;
            case TargetStat.MP:
                finalStat = maxMP = Mathf.CeilToInt((35 * tierMultiplier) + (intelligence * 50 * tierMultiplier));
                break;
            case TargetStat.STR:
                finalStat = strenght * tierMultiplier;
                break;
            case TargetStat.VIT:
                finalStat = vitality * tierMultiplier;
                break;
            case TargetStat.DEX:
                finalStat = dexterity * tierMultiplier;
                break;
            case TargetStat.AGI:
                finalStat = agility * tierMultiplier;
                break;
            case TargetStat.INT:
                finalStat = intelligence * tierMultiplier;
                break;
            case TargetStat.ACCURACY:
                finalStat = accuracy = Mathf.CeilToInt(65 + (dexterity * tierMultiplier));
                break;
            case TargetStat.DODGE:
                finalStat = dodge = Mathf.CeilToInt(5 + (agility * tierMultiplier));
                break;
        }

        foreach (var temporaryStatChange in _temporaryStatChanges)
        {          
            if (temporaryStatChange.targetStat == stat)
            {
                switch (temporaryStatChange.statEffectOnBaseValue)
                {
                    case StatEffectOnBaseValue.Additive:
                        additiveBonus += temporaryStatChange.effectBaseValue;
                        break;
                    case StatEffectOnBaseValue.Multiplicative:
                        multiplicativeBonus += temporaryStatChange.effectBaseValue;
                        break;
                }
            }
        }

        finalStat += additiveBonus;
        finalStat *= multiplicativeBonus;
       
        return finalStat;
    }

    public void AddTemporaryChange(StatusEffectInfo statusEffect)
    {
        TemporaryStatChanges newChange = new TemporaryStatChanges();

        newChange.targetStat = statusEffect.targetStat;
        newChange.effectBaseValue = statusEffect.effectBaseValue;
        newChange.statEffectOnBaseValue = statusEffect.statEffectOnBaseValue;
        newChange.sourceOfChanges = statusEffect;

        _temporaryStatChanges.Add(newChange);
    }

    public void RemoveTemporaryChange(StatusEffectInfo statusEffect)
    {
        TemporaryStatChanges changeToBeRemoved = null;

        foreach (var change in _temporaryStatChanges)
        {
            if (change.sourceOfChanges == statusEffect)
                changeToBeRemoved = change;
        }

        if (changeToBeRemoved != null)
            _temporaryStatChanges.Remove(changeToBeRemoved);
    }
}


[CreateAssetMenu(fileName = "New Character", menuName = "TurnBasedSystem/Character")]
public class CharacterInfo : ScriptableObject
{
    public GameObject inGameGFX;
    public Sprite faceSprite;
    public CharacterStats characterStats;
    public EquipmentSet nativeEquipmentSet;
}
