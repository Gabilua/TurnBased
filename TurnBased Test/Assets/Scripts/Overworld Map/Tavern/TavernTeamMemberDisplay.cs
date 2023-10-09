using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class TavernTeamMemberDisplay : MonoBehaviour
{
    public Action<TavernTeamMemberDisplay> TeamMemberClicked;

    [SerializeField] GameObject _selectionBracket;
    [SerializeField] Image _characterPortrait;

    public RecruitedCharacter _characterInfo { get; private set; }

    public void SetupTeamMemberDisplay(RecruitedCharacter character)
    {
        _characterInfo = character;

        _characterPortrait.sprite = _characterInfo.characterInfo.faceSprite;
    }

    public void IconClicked()
    {
        if (_characterInfo == null)
            return;

        TeamMemberClicked.Invoke(this);
    }

    public void ToggleSelectionBracket(bool state)
    {
        _selectionBracket.SetActive(state);
    }
}
