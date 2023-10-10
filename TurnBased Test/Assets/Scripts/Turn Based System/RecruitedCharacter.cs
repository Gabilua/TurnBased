using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecruitedCharacter 
{
    public CharacterInfo characterInfo;
    public List<EquipmentInfo> heldEquipment;

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
