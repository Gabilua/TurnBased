using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterStats
{
    enum CharacterStatTier { Minion, Creature, Hero, Boss}

    [Header("How Strong is This Character?")]
    [SerializeField] CharacterStatTier characterStatTier;

    [Header("Main Stats")]
    [Range(1, 5)]
    public int strenght;
    [Range(1, 5)]
    public int vitality;
    [Range(1, 5)]
    public int dexterity;
    [Range(1, 5)]
    public int agility;
    [Range(1, 5)]
    public int intelligence;


    public int maxHP { get; private set; }
    public int maxMP { get; private set; }
    public int dodge { get; private set; }
    public int accuracy { get; private set; }

    public void CopyStats(CharacterStats referenceStats)
    {
        characterStatTier = referenceStats.characterStatTier;

        strenght = referenceStats.strenght;
        vitality = referenceStats.vitality;
        dexterity = referenceStats.dexterity;
        agility = referenceStats.agility;
        intelligence = referenceStats.intelligence;
    }

    public void CalculateSecondaryStats()
    {
        int tierMultiplier = (int)characterStatTier + 1;

        maxHP = Mathf.CeilToInt((35 * tierMultiplier) + (vitality * 60 * tierMultiplier));
        maxMP = Mathf.CeilToInt((35 * tierMultiplier) + (intelligence * 50 * tierMultiplier));
        dodge = Mathf.CeilToInt(5 + (agility * tierMultiplier));
        accuracy = Mathf.CeilToInt(65 + (dexterity * tierMultiplier));
    }
}


[CreateAssetMenu(fileName = "New Character", menuName = "TurnBasedSystem/Character")]
public class CharacterInfo : ScriptableObject
{
    public GameObject inGameGFX;
    public Sprite faceSprite;
    public CharacterStats characterStats;
    public List<SkillInfo> learnableSkills = new List<SkillInfo>();
}
