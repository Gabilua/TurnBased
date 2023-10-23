using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecruitedCharacter 
{
    public CharacterInfo characterInfo;
    public List<EquipmentInfo> heldEquipment = new List<EquipmentInfo>();

    public RecruitedCharacter(CharacterInfo info, List<EquipmentInfo> equipments)
    {
        characterInfo = info;

        foreach (var equipment in equipments)
            AddEquipmentToCharacter(equipment);
    }

    public bool IsCharacterInventoryFull()
    {
        return heldEquipment.Count == 4;
    }

    public bool DoesCharacterInventoryContainEquipment(EquipmentInfo equipment)
    {
        return heldEquipment.Contains(equipment);
    }

    public void AddEquipmentToCharacter(EquipmentInfo equipment)
    {
        heldEquipment.Add(equipment);
    }

    public void RemoveEquipmentFromCharacter(EquipmentInfo equipment)
    {
        heldEquipment.Remove(equipment);
    }
}
