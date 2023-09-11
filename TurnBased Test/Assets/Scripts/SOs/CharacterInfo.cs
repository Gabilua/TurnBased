using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterStats
{
    public int strenght;
    public int vitality;
    public int dexterity;
    public int agility;
    public int intelligence;

    public int maxHP { get; private set; }
    public int maxMP { get; private set; }
    public int dodge { get; private set; }
    public int hit { get; private set; }

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
        maxHP = Mathf.CeilToInt(5 + ((float)vitality / 2));
        maxMP = Mathf.CeilToInt(10 + ((float)intelligence / 3));
        dodge = Mathf.CeilToInt(5 + ((float)agility / 3));
        hit = Mathf.CeilToInt(20 + ((float)dexterity / 4));
    }
}


[CreateAssetMenu(fileName = "New Character", menuName = "TurnBasedSystem/Character")]
public class CharacterInfo : ScriptableObject
{
    public string characterName;
    public GameObject inGameGFX;
    public Sprite faceSprite;
    public CharacterStats characterStats;
    public List<SkillInfo> learnableSkills = new List<SkillInfo>();
}
