using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Action<StageInfo> StageLoaded;
    public Action GameSaved;
    public Action GameLoaded;

    [SerializeField] MapManager _mapManager;
    [SerializeField] PlayerTeamController _playerTeamController;

   [SerializeField] List<EquipmentInfo> _playerStorage = new List<EquipmentInfo>();
    [SerializeField] int _playerCurrentMoney;
    [SerializeField] SaveFile _templateStartingFile;
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

        saveFile.equipmentStorage.Clear();
        saveFile.equipmentStorage.AddRange(_playerStorage);

        saveFile.currentMoney = _playerCurrentMoney;

        SaveToFile(saveFile);
    } 

    public List<EquipmentInfo> GetCurrentPlayerStorage()
    {
        return _playerStorage;
    }

    public void AddEquipmentToPlayerStorage(EquipmentInfo equipment)
    {
        _playerStorage.Add(equipment);

        RemoveEmptyStorageSpace();

        SaveCurrentGameProgress();
    }

    public void RemoveEquipmentFromPlayerStorage(EquipmentInfo equipment)
    {
        _playerStorage.Remove(equipment);

        RemoveEmptyStorageSpace();

        SaveCurrentGameProgress();
    }

    void RemoveEmptyStorageSpace()
    {
        List<EquipmentInfo> emptySlots = new List<EquipmentInfo>();

        foreach (var slot in _playerStorage)
        {
            if (slot == null)
                emptySlots.Add(slot);
        }

        foreach (var slot in emptySlots)
            _playerStorage.Remove(slot);
    }

    public List<CharacterInfo> GetUnlockedCharacters()
    {
        List<CharacterInfo> alreadyUnlockedCharacters = new List<CharacterInfo>();

        foreach (var unlockableCharacter in saveFile.unlockableCharacters)
        {
            if (unlockableCharacter.IsUnlocked)
                alreadyUnlockedCharacters.Add(unlockableCharacter.characterInfo);
        }

        return alreadyUnlockedCharacters;
    }

    public List<CharacterInfo> GetUnlockableCharacters()
    {
        List<CharacterInfo> list = new List<CharacterInfo>();

        foreach (var unlockableCharacter in saveFile.unlockableCharacters)
            list.Add(unlockableCharacter.characterInfo);

        return list;
    }

    public void UnlockableCharactersDefeatedInMatch(List<CharacterInfo> defeatedCharacters)
    {
        foreach (var unlockableCharacter in saveFile.unlockableCharacters)
        {
            foreach (var defeatedCharacter in defeatedCharacters)
            {
                if (defeatedCharacter == unlockableCharacter.characterInfo)
                    unlockableCharacter.IsUnlocked = true;
            }
        }

        SaveCurrentGameProgress();
    }

    public bool IsStartingCharacterAlreadyRecruited()
    {
        return saveFile.startingCharacterRecruited;
    }

    public void StartingCharacterRecruited()
    {
        saveFile.startingCharacterRecruited = true;

        SaveCurrentGameProgress();
    }

    public int GetCurrentPlayerMoney()
    {
        return _playerCurrentMoney;
    }

    public void SpendPlayerMoney(int amount)
    {
        if (GetCurrentPlayerMoney() == 0 || GetCurrentPlayerMoney() < amount)
            return;

        _playerCurrentMoney -= amount;

        SaveCurrentGameProgress();
    }

    public void EarnPlayerMoney(int amount)
    {
        _playerCurrentMoney += amount;

        SaveCurrentGameProgress();
    }

    #region Data Management

    [ContextMenu("Use Test Save File")]
    public void UseTestSaveFile()
    {
        ResetSaveFile();

        saveFile = _testFile;

        SaveToFile(saveFile);
    }

    [ContextMenu("Reset Save File")]
    public void ResetSaveFile()
    {
        saveFile = _templateStartingFile;

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

        _playerStorage.Clear();
        _playerStorage.AddRange(saveFile.equipmentStorage);

        _playerCurrentMoney = saveFile.currentMoney;        

        GameLoaded?.Invoke();
    }

    #endregion
}
