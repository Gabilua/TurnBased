using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterStats
{
    [Range(0, 25)]
    public int strenght;
    [Range(0, 25)]
    public int vitality;
    [Range(0, 25)]
    public int dexterity;
    [Range(0, 25)]
    public int agility;
    [Range(0, 25)]
    public int intelligence;

    public int maxHP { get; private set; }
    public int maxMP { get; private set; }
    public int dodge { get; private set; }
    public int accuracy { get; private set; }

    public void CopyStats(CharacterStats referenceStats)
    {
        strenght = referenceStats.strenght;
        vitality = referenceStats.vitality;
        dexterity = referenceStats.dexterity;
        agility = referenceStats.agility;
        intelligence = referenceStats.intelligence;
    }

    public void CalculateSecondaryStats()
    {
        maxHP = Mathf.CeilToInt(100 + ((float)vitality * 100));
        maxMP = Mathf.CeilToInt(100 + ((float)intelligence * 25));
        dodge = Mathf.CeilToInt(5 + ((float)agility / 3));
        accuracy = Mathf.CeilToInt(20 + ((float)dexterity / 4));
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
