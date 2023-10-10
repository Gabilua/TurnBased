using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Action GameSaved;
    public Action GameLoaded;

    [SerializeField] MapManager _mapManager;
    [SerializeField] PlayerTeamController _playerTeamController;

   [SerializeField] List<EquipmentInfo> _playerInventory = new List<EquipmentInfo>();
    [SerializeField] int _playerCurrentMoney;
    [SerializeField] SaveFile _testFile;

    public SaveFile saveFile { get; private set; }

    #region Unity

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        LoadFromFile();
    }

    #endregion

    public void SaveCurrentGameProgress()
    {
        saveFile.currentMapNode = _mapManager._currentMapNode;

        saveFile.mapNodeProgress.Clear();

        List<MapNodeProgressState> nodeProgress = new List<MapNodeProgressState>();

        foreach (var node in _mapManager.GetAllMapNodes())
        {
            MapNodeProgressState progress = new MapNodeProgressState();

            progress.mapNode = node;
            progress.mapNodeState = node.currentNodeState;

            nodeProgress.Add(progress);
        }

        saveFile.mapNodeProgress = nodeProgress;

        saveFile.recruitedCharacters = _playerTeamController.GetSavedPlayerTeam();

        saveFile.sharedInventory.Clear();
        saveFile.sharedInventory.AddRange(_playerInventory);

        saveFile.currentMoney = _playerCurrentMoney;

        SaveToFile(saveFile);
    }

    [ContextMenu("Use Test Save File")]
    public void UseTestSaveFile()
    {
        saveFile = _testFile;

        SaveToFile(saveFile);
    }

    public List<EquipmentInfo> GetCurrentPlayerInventory()
    {
        return _playerInventory;
    }

    public void AddEquipmentToPlayerInventory(EquipmentInfo equipment)
    {
        _playerInventory.Add(equipment);

        RemoveEmptyInventorySpace();

        SaveCurrentGameProgress();
    }

    public void RemoveEquipmentFromPlayerInventory(EquipmentInfo equipment)
    {
        _playerInventory.Remove(equipment);

        RemoveEmptyInventorySpace();

        SaveCurrentGameProgress();
    }

    void RemoveEmptyInventorySpace()
    {
        List<EquipmentInfo> emptySlots = new List<EquipmentInfo>();

        foreach (var slot in _playerInventory)
        {
            if (slot == null)
                emptySlots.Add(slot);
        }

        foreach (var slot in emptySlots)
            _playerInventory.Remove(slot);
    }

    #region Data Management

    [ContextMenu("Reset Save File")]
    public void ResetSaveFile()
    {
        SaveFile emptyFile = new SaveFile();

        emptyFile.currentMapNode = _mapManager._currentMapNode;

        List<MapNodeProgressState> nodeProgress = new List<MapNodeProgressState>();

        foreach (var node in _mapManager.GetAllMapNodes())
        {
            MapNodeProgressState progress = new MapNodeProgressState();

            progress.mapNode = node;

            if (_mapManager._startingMapNode == node)
                progress.mapNodeState = MapNodeState.Unlocked;
            else
                progress.mapNodeState = MapNodeState.Locked;

            nodeProgress.Add(progress);
        }

        emptyFile.mapNodeProgress.AddRange(nodeProgress);

        emptyFile.recruitedCharacters.Clear();
        emptyFile.sharedInventory.Clear();
        emptyFile.currentMoney = 0;

        saveFile = emptyFile;

        SaveToFile(saveFile);
    }

    public void SaveToFile(SaveFile file)
    {
        string json = JsonUtility.ToJson(file);

        using (StreamWriter writer = new StreamWriter(Application.dataPath + Path.AltDirectorySeparatorChar + "SaveFile.json"))
            writer.Write(json);

        GameSaved?.Invoke();
    }

    public void LoadFromFile()
    {
        if (!File.Exists(Application.dataPath + Path.AltDirectorySeparatorChar + "SaveFile.json"))
            ResetSaveFile();

        string json = string.Empty;

        using (StreamReader reader = new StreamReader(Application.dataPath + Path.AltDirectorySeparatorChar + "SaveFile.json"))
            json = reader.ReadToEnd();

        SaveFile file = JsonUtility.FromJson<SaveFile>(json);
        saveFile = file;

        _mapManager.DownloadMapNodeProgress(saveFile.mapNodeProgress);
        _playerTeamController.SetSavedPlayerTeam(saveFile.recruitedCharacters);

        _playerInventory.Clear();
        _playerInventory.AddRange(saveFile.sharedInventory);

        _playerCurrentMoney = saveFile.currentMoney;

        GameLoaded?.Invoke();
    }

    #endregion
}
