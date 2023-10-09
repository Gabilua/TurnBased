using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TavernCharacterInfoDisplay : MonoBehaviour
{
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

    public void UpdateDisplay(RecruitedCharacter selectedCharacter, bool alreadyRecruited)
    {
        _characterName.text = selectedCharacter.characterInfo.name;
        _characterProtrait.sprite = selectedCharacter.characterInfo.faceSprite;

        _characterStatValues[0].text = selectedCharacter.characterInfo.characterStats.GetFinalStat(TargetStat.HP).ToString();
        _characterStatValues[1].text = selectedCharacter.characterInfo.characterStats.GetFinalStat(TargetStat.MP).ToString();
        _characterStatValues[2].text = selectedCharacter.characterInfo.characterStats.GetFinalStat(TargetStat.STR).ToString();
        _characterStatValues[3].text = selectedCharacter.characterInfo.characterStats.GetFinalStat(TargetStat.VIT).ToString();
        _characterStatValues[4].text = selectedCharacter.characterInfo.characterStats.GetFinalStat(TargetStat.INT).ToString();
        _characterStatValues[5].text = selectedCharacter.characterInfo.characterStats.GetFinalStat(TargetStat.DEX).ToString();
        _characterStatValues[6].text = selectedCharacter.characterInfo.characterStats.GetFinalStat(TargetStat.AGI).ToString();

        foreach (Transform child in _characterEquipmentIconHolder)
        {
            child.GetComponent<EquipmentIconDisplay>().EquipmentIconClicked -= EquipmentIconClicked;
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 4; i++)
        {
            EquipmentIconDisplay display = Instantiate(_equipmentIconDisplayPrefab, _characterEquipmentIconHolder).GetComponent<EquipmentIconDisplay>();

            if (i < selectedCharacter.heldEquipment.Count)
                display.SetupEquipmentIconDisplay(selectedCharacter.heldEquipment[i]);

            display.EquipmentIconClicked += EquipmentIconClicked;
        }

        DeselectEquipment();
        UpdateEquipmentInfo();

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
}
