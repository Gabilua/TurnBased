using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TavernCharacterInfoDisplay : MonoBehaviour
{
    public Action<EquipmentIconDisplay> CharacterEquipmentDragging;
    public Action<EquipmentIconDisplay> CharacteEquipmentIconDragStart;
    public Action<EquipmentIconDisplay> CharacteEquipmentIconDragEnd;
    public Action<TavernInventorySlot> OnInventorySlotDroppedOnEquipment;

    [SerializeField] TextMeshProUGUI _characterName;
    [SerializeField] Image _characterProtrait;
    [SerializeField] TextMeshProUGUI[] _characterStatValues;

    [SerializeField] GameObject _equipmentIconDisplayPrefab;
    [SerializeField] Transform _characterEquipmentIconHolder;
    [SerializeField] TextMeshProUGUI _equipmentName;
    [SerializeField] TextMeshProUGUI _equipmentDescription;

    [SerializeField] Button _recruitButton;
    [SerializeField] Button _dismissButton;

    EquipmentIconDisplay _selectedEquipment;
    RecruitedCharacter _selectedCharacter;

    List<EquipmentIconDisplay> _activeEquipmentIcons = new List<EquipmentIconDisplay>();

    void ResetEquipmentDisplays()
    {
        foreach (var icon in _activeEquipmentIcons)
        {
            icon.EquipmentIconClicked -= EquipmentIconClicked;

            icon.EquipmentIconDragStart -= EquipmentStartDrag;
            icon.EquipmentIconDragEnd -= EquipmentEndDrag;
            icon.EquipmentIconDragging -= EquipmentDragging;
        }

        _activeEquipmentIcons.Clear();

        foreach (Transform child in _characterEquipmentIconHolder)
            Destroy(child.gameObject);
    }

    void SetupEquipmentDisplays(RecruitedCharacter selectedCharacter)
    {
        for (int i = 0; i < 4; i++)
        {
            EquipmentIconDisplay display = Instantiate(_equipmentIconDisplayPrefab, _characterEquipmentIconHolder).GetComponent<EquipmentIconDisplay>();

            if (i < selectedCharacter.heldEquipment.Count)
                display.SetupEquipmentIconDisplay(selectedCharacter.heldEquipment[i]);

            display.EquipmentIconClicked += EquipmentIconClicked;

            display.EquipmentIconDragStart += EquipmentStartDrag;
            display.EquipmentIconDragEnd += EquipmentEndDrag;
            display.EquipmentIconDragging += EquipmentDragging;
            display.InventorySlotDroppedOnEquipment += InventorySlotDroppedOnEquipment;

            _activeEquipmentIcons.Add(display);
        }

        DeselectEquipment();
        UpdateEquipmentInfo();
    }

    public void UpdateDisplay(RecruitedCharacter selectedCharacter, bool alreadyRecruited)
    {
        _selectedCharacter = selectedCharacter;

        _characterName.text = _selectedCharacter.characterInfo.name;
        _characterProtrait.sprite = _selectedCharacter.characterInfo.faceSprite;

        _characterStatValues[0].text = _selectedCharacter.characterInfo.characterStats.GetFinalStat(TargetStat.HP).ToString();
        _characterStatValues[1].text = _selectedCharacter.characterInfo.characterStats.GetFinalStat(TargetStat.MP).ToString();
        _characterStatValues[2].text = _selectedCharacter.characterInfo.characterStats.GetFinalStat(TargetStat.STR).ToString();
        _characterStatValues[3].text = _selectedCharacter.characterInfo.characterStats.GetFinalStat(TargetStat.VIT).ToString();
        _characterStatValues[4].text = _selectedCharacter.characterInfo.characterStats.GetFinalStat(TargetStat.INT).ToString();
        _characterStatValues[5].text = _selectedCharacter.characterInfo.characterStats.GetFinalStat(TargetStat.DEX).ToString();
        _characterStatValues[6].text = _selectedCharacter.characterInfo.characterStats.GetFinalStat(TargetStat.AGI).ToString();

        UpdateEquipmentDisplay();

        _recruitButton.interactable = !alreadyRecruited;
        _dismissButton.interactable = alreadyRecruited;

        if (alreadyRecruited)
        {
            _recruitButton.GetComponentInChildren<TextMeshProUGUI>().color = _recruitButton.colors.disabledColor;
            _dismissButton.GetComponentInChildren<TextMeshProUGUI>().color = _recruitButton.colors.normalColor;
        }
        else
        {
            _recruitButton.GetComponentInChildren<TextMeshProUGUI>().color = _recruitButton.colors.normalColor;
            _dismissButton.GetComponentInChildren<TextMeshProUGUI>().color = _recruitButton.colors.disabledColor;
        }
    }

    public void UpdateEquipmentDisplay()
    {
        ResetEquipmentDisplays();

        SetupEquipmentDisplays(_selectedCharacter);
    }

    void SelectEquipment(EquipmentIconDisplay equipment)
    {
        _selectedEquipment = equipment;

        _selectedEquipment.ToggleSelectionBracket(true);
    }

    void DeselectEquipment()
    {
        if (_selectedEquipment == null)
            return;

        _selectedEquipment.ToggleSelectionBracket(false);
        _selectedEquipment = null;
    }

    void UpdateEquipmentInfo()
    {
        if(_selectedEquipment == null)
        {
            _equipmentName.text = string.Empty;
            _equipmentDescription.text = string.Empty;
        }
        else
        {
            _equipmentName.text = _selectedEquipment._equipmentInfo.name;
            _equipmentDescription.text =
                _selectedEquipment._equipmentInfo.equipmentDescription +
                "\n Grants the skill: \n\n"
                + _selectedEquipment._equipmentInfo.awardedSkill.name + "\n\n"
                + _selectedEquipment._equipmentInfo.awardedSkill.skillDescription;
        }
    }

    void EquipmentIconClicked(EquipmentIconDisplay selectedIcon)
    {
        DeselectEquipment();

        if (selectedIcon._equipmentInfo != null)
            SelectEquipment(selectedIcon);

        UpdateEquipmentInfo();
    }

    #region Equipment Drag Events

    void EquipmentStartDrag(EquipmentIconDisplay equipment)
    {
        CharacteEquipmentIconDragStart?.Invoke(equipment);
    }

    void EquipmentEndDrag(EquipmentIconDisplay equipment)
    {
        CharacteEquipmentIconDragEnd?.Invoke(equipment);
    }

    void EquipmentDragging(EquipmentIconDisplay equipment)
    {
        CharacterEquipmentDragging?.Invoke(equipment);
    }

    void InventorySlotDroppedOnEquipment(TavernInventorySlot slot)
    {
        OnInventorySlotDroppedOnEquipment?.Invoke(slot);
    }

    #endregion
}
