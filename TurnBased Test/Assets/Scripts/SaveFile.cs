using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapNode
{
    public StageInfo stageInfo;
    public bool alreadyUnlocked;
}

[System.Serializable]
public class SaveFile 
{
    [Header("Map Progress")]
    public MapNode currentMapNode;
    public List<MapNode> mapNodeProgress = new List<MapNode>();

    [Header("Player Progress")]
    public List<RecruitedCharacter> recruitedCharacters = new List<RecruitedCharacter>();
    public List<EquipmentInfo> sharedInventory = new List<EquipmentInfo>();
    public int currentMoney;
}
