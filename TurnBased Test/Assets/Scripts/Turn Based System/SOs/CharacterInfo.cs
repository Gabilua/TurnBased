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
    [Header("Main Stats")]
    [Range(1, 25)]
    [SerializeField] int strenght;
    [Range(1, 25)]
    [SerializeField] int vitality;
    [Range(1, 25)]
    [SerializeField] int intelligence;
    [Range(1, 25)]
    [SerializeField] int dexterity;
    [Range(1, 25)]
    [SerializeField] int agility;

    [SerializeField] int maxHP;
    [SerializeField] int maxMP;
    [SerializeField] int defense;
    [SerializeField] int accuracy;
    [SerializeField] int dodge;

    List<TemporaryStatChanges> _temporaryStatChanges = new List<TemporaryStatChanges>();

    public void UpdateSubStats()
    {
        maxHP = GetFinalStat(TargetStat.HP);
        maxMP = GetFinalStat(TargetStat.MP);
        dodge = GetFinalStat(TargetStat.DODGE);
        accuracy = GetFinalStat(TargetStat.ACCURACY);
        defense = GetFinalStat(TargetStat.DEFENSE);
    }

    public void CopyBaseStats(CharacterStats referenceStats)
    {
        strenght = referenceStats.strenght;
        vitality = referenceStats.vitality;
        dexterity = referenceStats.dexterity;
        agility = referenceStats.agility;
        intelligence = referenceStats.intelligence;
        defense = referenceStats.defense;
    }

    public int GetFinalStat(TargetStat stat)
    {
        int finalStat = 0;
        int additiveBonus = 0;
        int multiplicativeBonus = 1;

        switch (stat)
        {
            case TargetStat.HP:
                finalStat = maxHP = Mathf.CeilToInt(vitality * 60);
                break;
            case TargetStat.MP:
                finalStat = maxMP = Mathf.CeilToInt(intelligence * 60);
                break;
            case TargetStat.STR:
                finalStat = strenght;
                break;
            case TargetStat.VIT:
                finalStat = vitality;
                break;
            case TargetStat.DEX:
                finalStat = dexterity;
                break;
            case TargetStat.AGI:
                finalStat = agility;
                break;
            case TargetStat.INT:
                finalStat = intelligence;
                break;
            case TargetStat.ACCURACY:
                finalStat = accuracy = Mathf.CeilToInt(65 + dexterity);
                break;
            case TargetStat.DODGE:
                finalStat = dodge = Mathf.CeilToInt(5 + agility);
                break;
            case TargetStat.DEFENSE:
                finalStat = (vitality+strenght+dexterity+agility+intelligence) / 5;
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
    [Range(0, 100)]
    public int tavernAppearanceChance;

#if UNITY_EDITOR

    private void OnValidate()
    {
        characterStats.UpdateSubStats();
    }

#endif
}
