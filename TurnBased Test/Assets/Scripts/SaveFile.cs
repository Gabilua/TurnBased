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
public class SaveFile 
{
    [Header("Map Progress")]
    public MapNodeController currentMapNode;
    public List<MapNodeProgressState> mapNodeProgress = new List<MapNodeProgressState>();

    [Header("Player Progress")]
    public List<RecruitedCharacter> recruitedCharacters = new List<RecruitedCharacter>();
    public List<EquipmentInfo> sharedInventory = new List<EquipmentInfo>();
    public int currentMoney;
}
