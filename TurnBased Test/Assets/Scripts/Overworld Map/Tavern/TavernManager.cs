using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[System.Serializable]
public class GeneratedTeamsByTavern
{
    public StageInfo tavernTown;
    public List<RecruitedCharacter> tavernTeam = new List<RecruitedCharacter>();

    public void SetupTeam(StageInfo town, List<RecruitedCharacter> team)
    {
        tavernTown = town;
        tavernTeam.AddRange(team);
    }
}

public class TavernManager : MonoBehaviour
{
    public Action<RecruitedCharacter> TeamMemberRecruited;
    public Action<RecruitedCharacter> TeamMemberDismissed;
    public Action<bool> OnTavernSceneExitSelected;
    public Action OnTavernExited;

    [SerializeField] PlayerTeamController _playerTeamController;
    [SerializeField] StorageManager _storageManager;
    [SerializeField] ShopManager _shopManager;

    [SerializeField] GameObject _tavernScene;
    [SerializeField] Animator _exitButtonAnimator;

    [SerializeField] List<Transform> _characterSpots = new List<Transform>();
    [SerializeField] GameObject _tavernCharacterPrefab;

    [SerializeField] CharacterInfoDisplay _characterInfoDisplay;
    [SerializeField] Animator _characterInfoDisplayAnimator;

    [SerializeField] Transform _teamMemberDisplayHolder;
    [SerializeField] GameObject _teamMemberDisplayPrefab;
    List<TavernCharacter> _generatedTavernCharacters = new List<TavernCharacter>();

    [SerializeField] List<RecruitedCharacter> _startingCharacterOptions = new List<RecruitedCharacter>();

    RecruitedCharacter _currentlySelectedCharacter;
    public StageInfo _currentTown { get; private set; }

    List<GeneratedTeamsByTavern> _generatedTavernTeams = new List<GeneratedTeamsByTavern>();

    [SerializeField] Image _draggedSlotGFX;

    private void Start()
    {
        _characterInfoDisplay.CharacteEquipmentIconDragStart += CharacterEquipmentDragStart;
        _characterInfoDisplay.CharacteEquipmentIconDragEnd += CharacterEquipmentDragEnd;
        _characterInfoDisplay.CharacterEquipmentDragging += CharacterEquipmentDragging;
        _characterInfoDisplay.OnStorageSlotDroppedOnEquipment += AttemptToMoveEquipmentFromStorageToCharacter;

        _storageManager.StorageWindowStateChange += StorageWindowStageChange;
        _storageManager.OnCharacterEquipmentDroppedOnSlot += AttemptToMoveEquipmentFromCharacterToStorage;

        _storageManager.OnShopStockDroppedOnSlot += AttemptToBuyEquipmentFromShop;
        _storageManager.SetupStorageWindow();

        _shopManager.OnStorageSlotDroppedOnBuyingArea += AttemptToSellEquipmentToShop;
        _shopManager.ShopWindowStateChanged += ShopWindowStateChange;
        _shopManager.ShopSlotDragStart += ShopStockDragStart;
        _shopManager.ShopSlotDragEnd += ShopStockDragEnd;
        _shopManager.ShopSlotDragging += ShopStockDragging;

        _tavernScene.SetActive(false);
    }

    #region Tavern Scene Management

    bool FirstTimeThisSessionVisitingThisTavern(StageInfo tavernTown)
    {
        bool result = true;

        foreach (var tavernTeam in _generatedTavernTeams)
        {
            if(tavernTeam.tavernTown == tavernTown)
            {
                result = false;
                break;
            }
        }

        return result;
    }

    List<RecruitedCharacter> GetTavernCharactersForThisTavern(StageInfo tavern)
    {
        List<RecruitedCharacter> team = new List<RecruitedCharacter>();

        foreach (var generatedTeam in _generatedTavernTeams)
        {
            if(generatedTeam.tavernTown == tavern)
            {
                team.AddRange(generatedTeam.tavernTeam);
                break;
            }
        }

        return team;
    }

    public void SetupTavernScene(List<RecruitedCharacter> savedPlayerTeam, StageInfo currentTownTavern)
    {
        _currentTown = currentTownTavern;

        _tavernScene.SetActive(true);

        _storageManager.UpdateStorageWindow(_currentlySelectedCharacter);

        if (GameManager.instance.IsStartingCharacterAlreadyRecruited())
        {
            if (FirstTimeThisSessionVisitingThisTavern(_currentTown))
            {
                GeneratedTeamsByTavern newTeam = new GeneratedTeamsByTavern();

                newTeam.SetupTeam(_currentTown, GenerateNewTavernTeam());

                _generatedTavernTeams.Add(newTeam);
            }

            SetupTavernCharacters(GetTavernCharactersForThisTavern(_currentTown));
        }
        else
            SetupTavernCharacters(_startingCharacterOptions);

        SetupTeamCharacters(savedPlayerTeam);
    }

    List<RecruitedCharacter> GenerateNewTavernTeam()
    {
        List<RecruitedCharacter> newTeam = new List<RecruitedCharacter>();

        foreach (var unlockedCharacter in GameManager.instance.GetUnlockedCharacters())
        {
            if(UnityEngine.Random.value*100 <= unlockedCharacter.tavernAppearanceChance)
            {
                RecruitedCharacter newCharacter = new RecruitedCharacter(unlockedCharacter, unlockedCharacter.nativeEquipmentSet.equipments);

                newTeam.Add(newCharacter);
            }
        }

        return newTeam;
    }

    public void ExitTavernScene()
    {
        _currentTown = null;

        if (_storageManager.IsStorageOpen())
            _storageManager.ChestIconClicked();

        if (_shopManager.IsShopOpen())
            _shopManager.NPCIconClicked();

        _tavernScene.SetActive(false);

        OnTavernExited?.Invoke();
    }

    public void TavernSceneExitSelected()
    {
        _exitButtonAnimator.SetTrigger("Action");

        OnTavernSceneExitSelected?.Invoke(true);
    }

    void SetupTavernCharacters(List<RecruitedCharacter> availableCharacters)
    {
        ResetTavernCharacters();

        _characterSpots.Shuffle();

        for (int i = 0; i < availableCharacters.Count; i++)
        {
            TavernCharacter tavernCharacter = Instantiate(_tavernCharacterPrefab, _characterSpots[i]).GetComponent<TavernCharacter>();
            tavernCharacter.SetupCharacter(availableCharacters[i]);

            tavernCharacter.gameObject.name = "Tavern "+availableCharacters[i].characterInfo.name;

            tavernCharacter.TavernCharacterSelected += TavernCharacterSelected;

            _generatedTavernCharacters.Add(tavernCharacter);
        }
    }

    void SetupTeamCharacters(List<RecruitedCharacter> teamCharacters)
    {
        ResetTeamCharacters();

        for (int i = 0; i < teamCharacters.Count; i++)
        {
            TavernTeamMemberDisplay display = Instantiate(_teamMemberDisplayPrefab, _teamMemberDisplayHolder).GetComponent<TavernTeamMemberDisplay>();
            display.SetupTeamMemberDisplay(teamCharacters[i]);
            display.TeamMemberClicked += TeamMemberSelected;
        }
    }

    void ResetTavernCharacters()
    {
        foreach (var tavernCharacter in _generatedTavernCharacters)
            Destroy(tavernCharacter.gameObject);

        _generatedTavernCharacters.Clear();
    }

    void ResetTeamCharacters()
    {
        foreach (Transform child in _teamMemberDisplayHolder)
            Destroy(child.gameObject);
    }

    #endregion

    #region Team Management

    void SetupDismissedCharacterOnTavern(RecruitedCharacter dismissedCharacter)
    {
        foreach (Transform child in _teamMemberDisplayHolder)
        {
            if(child.gameObject.GetComponent<TavernTeamMemberDisplay>()._characterInfo == dismissedCharacter)
            {
                child.gameObject.GetComponent<TavernTeamMemberDisplay>().TeamMemberClicked -= TeamMemberSelected;
                Destroy(child.gameObject);
                break;
            }
        }

        foreach (var spot in _characterSpots)
        {
            if(spot.childCount == 0)
            {
                TavernCharacter tavernCharacter = Instantiate(_tavernCharacterPrefab, spot).GetComponent<TavernCharacter>();
                tavernCharacter.SetupCharacter(dismissedCharacter);

                tavernCharacter.gameObject.name = "Tavern " + dismissedCharacter.characterInfo.name;

                tavernCharacter.TavernCharacterSelected += TavernCharacterSelected;

                _generatedTavernCharacters.Add(tavernCharacter);

                break;
            }
        }
    }

    void RemoveRecruitedCharacterFromTavern(RecruitedCharacter recruitedCharacter)
    {
        TavernCharacter toBeRemoved = null;

        foreach (var tavernCharacter in _generatedTavernCharacters)
        {
            if(recruitedCharacter == tavernCharacter._characterInfo)
            {
                toBeRemoved = tavernCharacter;
              
                break;
            }
        }

        _generatedTavernCharacters.Remove(toBeRemoved);

        toBeRemoved.TavernCharacterSelected -= TavernCharacterSelected;
        Destroy(toBeRemoved.gameObject);

        TavernTeamMemberDisplay display = Instantiate(_teamMemberDisplayPrefab, _teamMemberDisplayHolder).GetComponent<TavernTeamMemberDisplay>();
        display.SetupTeamMemberDisplay(recruitedCharacter);
        display.TeamMemberClicked += TeamMemberSelected;
    }

    void TeamMemberSelected(TavernTeamMemberDisplay character)
    {
        SelectCharacter(character._characterInfo, true);
    }

    void TavernCharacterSelected(TavernCharacter character)
    {
        SelectCharacter(character._characterInfo, false);
    }

    void SelectCharacter(RecruitedCharacter character, bool teamMember)
    {
        if (_shopManager.IsShopOpen())
            return;

        _currentlySelectedCharacter = character;

        _characterInfoDisplay.gameObject.SetActive(true);
        _characterInfoDisplayAnimator.SetTrigger("Action");

        _characterInfoDisplay.UpdateDisplay(character, teamMember);
        _storageManager.UpdateStorageWindow(_currentlySelectedCharacter);
    }

    public void DeselectCharacter()
    {
        _currentlySelectedCharacter = null;

        _characterInfoDisplay.gameObject.SetActive(false);
    }

    public void RecruitCharacter()
    {
        if (_playerTeamController.GetSavedPlayerTeam().Count == 6)
            return;

        TeamMemberRecruited?.Invoke(_currentlySelectedCharacter);
        RemoveRecruitedCharacterFromTavern(_currentlySelectedCharacter);
        DeselectCharacter();

        if (!GameManager.instance.IsStartingCharacterAlreadyRecruited())
        {
            GameManager.instance.StartingCharacterRecruited();

            ResetTavernCharacters();
        }
    }

    public void DismissCharacter()
    {
        if (_playerTeamController.GetSavedPlayerTeam().Count == 1)
            return;

        TeamMemberDismissed?.Invoke(_currentlySelectedCharacter);
        SetupDismissedCharacterOnTavern(_currentlySelectedCharacter);
        DeselectCharacter();
    }

    #endregion

    #region Equipment Management

    public void UpdateDraggedGFX(bool state, Sprite icon = null)
    {
        if (!_storageManager.IsStorageOpen())
            return;

        _draggedSlotGFX.gameObject.SetActive(state);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        _draggedSlotGFX.transform.position = new Vector3(mousePos.x, mousePos.y, _draggedSlotGFX.transform.position.z);

        _draggedSlotGFX.sprite = icon;
    }

    void CharacterEquipmentDragStart(EquipmentIconDisplay equipment)
    {
        _storageManager.CharacterEquipmentStartDrag(equipment);
    }

    void CharacterEquipmentDragEnd(EquipmentIconDisplay equipment)
    {
        _storageManager.CharacterEquipmentEndDrag(equipment);
    }

    void CharacterEquipmentDragging(EquipmentIconDisplay equipment)
    {
        _storageManager.CharacterEquipmentDragging(equipment);
    }

    void StorageWindowStageChange(bool state)
    {
        if (state)
            DeselectCharacter();
    }

    void ShopWindowStateChange(bool state)
    {
        if (state)
        {
            _storageManager.ChestIconClicked();

            DeselectCharacter();
        }

        _storageManager.ToggleSellingValueOnStorage(state);
    }

    void ShopStockDragStart(ShopStockSlot shopSlot)
    {
        UpdateDraggedGFX(true);
    }

    void ShopStockDragEnd(ShopStockSlot shopSlot)
    {
        UpdateDraggedGFX(false);
    }

    void ShopStockDragging(ShopStockSlot shopSlot)
    {
        UpdateDraggedGFX(true, shopSlot._equipmentInfo.equipmentIcon);
    }

    void AttemptToMoveEquipmentFromStorageToCharacter(StorageSlot slot)
    {
        UpdateDraggedGFX(false);

        if (_currentlySelectedCharacter.IsCharacterInventoryFull())
            return;

        _currentlySelectedCharacter.AddEquipmentToCharacter(slot._equipmentInfo);

        GameManager.instance.RemoveEquipmentFromPlayerStorage(slot._equipmentInfo);

        _characterInfoDisplay.UpdateEquipmentDisplay();
        _storageManager.UpdateStorageWindow(_currentlySelectedCharacter);
    }

    void AttemptToMoveEquipmentFromCharacterToStorage(EquipmentIconDisplay equipment)
    {
        UpdateDraggedGFX(false);

        if (!_currentlySelectedCharacter.DoesCharacterInventoryContainEquipment(equipment._equipmentInfo))
            return;

        if (_storageManager.IsStorageFull())
            return;

        _currentlySelectedCharacter.RemoveEquipmentFromCharacter(equipment._equipmentInfo);

        GameManager.instance.AddEquipmentToPlayerStorage(equipment._equipmentInfo);

        _characterInfoDisplay.UpdateEquipmentDisplay();
        _storageManager.UpdateStorageWindow(_currentlySelectedCharacter);
    }

    void AttemptToBuyEquipmentFromShop(ShopStockSlot stock)
    {
        UpdateDraggedGFX(false);

        if (_storageManager.IsStorageFull())
            return;

        if (GameManager.instance.GetCurrentPlayerMoney() < stock._equipmentInfo.equipmentValue)
            return;

        GameManager.instance.AddEquipmentToPlayerStorage(stock._equipmentInfo);
        GameManager.instance.SpendPlayerMoney(stock._equipmentInfo.equipmentValue);

        _shopManager.RemoveStockFromSale(stock);
        _shopManager.UpdateShopWindow();

        _storageManager.UpdateStorageWindow(_currentlySelectedCharacter);
        _storageManager.ToggleSellingValueOnStorage(true);
    }

    void AttemptToSellEquipmentToShop(StorageSlot storageSlot)
    {
        UpdateDraggedGFX(false);

        GameManager.instance.RemoveEquipmentFromPlayerStorage(storageSlot._equipmentInfo);
        GameManager.instance.EarnPlayerMoney(storageSlot._equipmentInfo.equipmentValue);

        _storageManager.UpdateStorageWindow(_currentlySelectedCharacter);
        _storageManager.ToggleSellingValueOnStorage(true);
    }

    #endregion
}
