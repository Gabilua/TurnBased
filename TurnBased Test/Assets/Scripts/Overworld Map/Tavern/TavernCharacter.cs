using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TavernCharacter : MonoBehaviour
{
    public Action<TavernCharacter> TavernCharacterSelected;

    AnimationManager _animationManager;

    public RecruitedCharacter _characterInfo { get; private set; }

    public void SetupCharacter(RecruitedCharacter character)
    {
        _characterInfo = character;

        Instantiate(character.characterInfo.inGameGFX, transform);

        _animationManager = GetComponentInChildren<AnimationManager>();
    }

    public void CharacterSelected()
    {
        _animationManager.TavernClicked();

        TavernCharacterSelected?.Invoke(this);
    }
}
