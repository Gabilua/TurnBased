using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapNodeProgressState
{
    public MapNodeController mapNode;
    public MapNodeState mapNodeState;
}

[System.Serializable]
public class UnlockableCharacter
{
    public CharacterInfo characterInfo;
    public bool IsUnlocked;
}

[System.Serializable]
public class SaveFile 
{
    [Header("Map Progress")]
    public List<MapNodeProgressState> mapNodeProgress = new List<MapNodeProgressState>();

    [Header("Player Progress")]
    public List<RecruitedCharacter> recruitedCharacters = new List<RecruitedCharacter>();
    public List<EquipmentInfo> equipmentStorage = new List<EquipmentInfo>();
    public int currentMoney;

    [Header("Character Progress")]
    public UnlockableCharacter[] unlockableCharacters;
    public bool startingCharacterRecruited;
}
