using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SkillUI : MonoBehaviour
{
    public Action<SkillInfo> SkillClicked;

    [SerializeField] Image _skillIconDisplay;
    [SerializeField] TextMeshProUGUI _skillNameDisplay;
    SkillInfo _skillInfo;

    public void Setup(SkillInfo skill)
    {
        _skillInfo = skill;
        _skillIconDisplay.sprite = _skillInfo.skillIcon;
        _skillNameDisplay.text = _skillInfo.skillName;
    }

    public void SkillIconClicked()
    {
        SkillClicked?.Invoke(_skillInfo);
    }
}
