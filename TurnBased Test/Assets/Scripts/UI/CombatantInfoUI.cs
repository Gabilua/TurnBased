using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CombatantInfoUI : MonoBehaviour
{
    public Action<SkillInfo> SkillSelected;

    [SerializeField] Image _characterFaceIcon;

    [SerializeField] TextMeshProUGUI _characterName;
    [SerializeField] TextMeshProUGUI _characterHealthValueDisplay;
    [SerializeField] TextMeshProUGUI _characterManaValueDisplay;

    [SerializeField] Image _characterHealthBar;
    [SerializeField] Image _characterManaBar;

    [SerializeField] Transform _skillIconHolder;

    [SerializeField] GameObject _skillUIPrefab;

    public RealtimeCombatant _respectiveCombatant { get; private set; }
    List<SkillInfo> _availableSkills = new List<SkillInfo>();
    List<Button> _activeSkillIcons = new List<Button>();

    public void SetupInfo(RealtimeCombatant combatant)
    {
        _respectiveCombatant = combatant;
        _respectiveCombatant.CombatantTurnStateChanged += UpdateCombatantInfo;

        _characterFaceIcon.sprite = combatant._characterInfo.faceSprite;
        _characterName.text = combatant._characterInfo.name;

        _characterHealthValueDisplay.text = combatant._healthPoints.currentResource.ToString("F0");
        _characterHealthBar.fillAmount = (float)combatant._healthPoints.currentResource / (float)combatant._healthPoints.maxResource;

        _characterManaValueDisplay.text = combatant._manaPoints.currentResource.ToString("F0");
        _characterManaBar.fillAmount = (float)combatant._manaPoints.currentResource / (float)combatant._manaPoints.maxResource;
        
        _availableSkills.AddRange(_respectiveCombatant.GetSkillList);

        foreach (var skill in _availableSkills)
            SetupSkillIcon(skill);

        gameObject.name = combatant._characterInfo.name + " Info";
        gameObject.SetActive(false);
    }

    void UpdateCombatantInfo(RealtimeCombatant combatant, CombatantTurnState turnState)
    {
        if (turnState != CombatantTurnState.WaitingForInput)
            return;

        _characterHealthValueDisplay.text = _respectiveCombatant._healthPoints.currentResource.ToString("F0");
        _characterHealthBar.fillAmount = (float)_respectiveCombatant._healthPoints.currentResource / (float)_respectiveCombatant._healthPoints.maxResource;

        _characterManaValueDisplay.text = _respectiveCombatant._manaPoints.currentResource.ToString("F0");
        _characterManaBar.fillAmount = (float)_respectiveCombatant._manaPoints.currentResource / (float)_respectiveCombatant._manaPoints.maxResource;

        UpdateSkillAvailablity();
    }

    void SetupSkillIcon(SkillInfo skill)
    {
        GameObject icon = Instantiate(_skillUIPrefab, _skillIconHolder);

        SkillUI selectable = icon.GetComponent<SkillUI>();
        selectable.Setup(skill);

       _activeSkillIcons.Add(selectable.GetComponent<Button>());

        selectable.SkillClicked += SkillClicked;
    }

    void UpdateSkillAvailablity()
    {
        for (int i = 0; i < _availableSkills.Count; i++)
        {
            if (_availableSkills[i].costStat == CostStat.MP)
            {
                if (_respectiveCombatant._manaPoints.currentResource >= _availableSkills[i].costAmount)
                    _activeSkillIcons[i].interactable = true;
                else
                    _activeSkillIcons[i].interactable = false;
            }
            else if (_availableSkills[i].costStat == CostStat.HP)
            {
                if (_respectiveCombatant._healthPoints.currentResource >= _availableSkills[i].costAmount)
                    _activeSkillIcons[i].interactable = true;
                else
                    _activeSkillIcons[i].interactable = false;
            }
        }
    }

    void SkillClicked(SkillInfo skill)
    {
        SkillSelected?.Invoke(skill);
    }
}
