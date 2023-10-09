using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "TurnBasedSystem/Combat/Equipment")]
public class EquipmentInfo : ScriptableObject
{
    public Sprite equipmentIcon;
    public SkillInfo awardedSkill;
    public string equipmentDescription;
    public List<CharacterInfo> allowedUsers = new List<CharacterInfo>();
}
