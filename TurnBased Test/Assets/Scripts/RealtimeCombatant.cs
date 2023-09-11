using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ManagedResource
{
    public float currentResource;
    public float maxResource;

    public void SetupResource(float maxAmount)
    {
        maxResource = maxAmount;
        currentResource = maxResource;
    }

    void ChangeResourceValue(float _changeAmount)
    {
        currentResource += _changeAmount;
        currentResource = Mathf.CeilToInt(currentResource);

        if (currentResource <= 0)
            currentResource = 0;
        else if (currentResource > maxResource)
            currentResource = maxResource;
    }

    public void Deplete(float _amount)
    {
        if (_amount == 0)
            return;

        float cleanAmount = Mathf.CeilToInt(Mathf.Abs(_amount));

        ChangeResourceValue(-cleanAmount);
    }

    public void Replenish(float _amount)
    {
        if (_amount == 0)
            return;

        float cleanAmount = Mathf.CeilToInt(Mathf.Abs(_amount));

        ChangeResourceValue(cleanAmount);
    }

    public bool IsResourceFullyDepleted()
    {
        return currentResource <= 0;
    }
}

public enum CombatantTurnState { PreGame, NotMyTeamsTurn, WaitingForInput, DoneForTheTurn, Dead, PostGame }

public class RealtimeCombatant : MonoBehaviour
{
    public Action<RealtimeCombatant,SkillInfo> CombatantAimedSkill;
    public Action<RealtimeCombatant, SkillInfo, List<RealtimeCombatant>> CombatantUsedSkill;
    public Action<int> CombatantReceivedHealing;
    public Action<int> CombatantReceivedDamage;
    public Action<SkillInfo> CombatantReceivedSkillEffect;
    public Action<StatusEffectInfo> CombatantAffectedByStatusEffect;
    public Action<SkillInfo> CombatantReleasedSkill;
    public Action CombatantDodgedSkill;
    public Action CombatantDied;
    public Action<RealtimeCombatant> CombatantClicked;
    public Action<RealtimeCombatant, CombatantTurnState> CombatantTurnStateChanged;

    public CombatantTurnState currentTurnState { get; private set; }

    public CharacterInfo _characterInfo { get; private set; }
    List<SkillInfo> _learnedSkills = new List<SkillInfo>();

    [SerializeField] ToggleableElement _selectionDisplay;
    [SerializeField] ToggleableElement _targetPotentialDisplay;
    [SerializeField] ToggleableElement _targetLockedDisplay;

    List<RealtimeCombatant> _allCombatants = new List<RealtimeCombatant>();

    public ManagedResource _healthPoints { get; private set; }
    public ManagedResource _manaPoints { get; private set; }

    public CharacterStats _runtimeStats { get; private set; }

    GameObject _characterGFX;

    public bool IsValidTarget { get; private set; }

    #region Basic Setup

    public void SetupCharacter(CharacterInfo info)
    {
        _characterInfo = info;
        _learnedSkills.AddRange(_characterInfo.learnableSkills);

        _runtimeStats = new CharacterStats();
        _runtimeStats.CopyStats(_characterInfo.characterStats);
        _runtimeStats.CalculateSecondaryStats();

        _healthPoints = new ManagedResource();
        _healthPoints.SetupResource(_runtimeStats.maxHP);

        _manaPoints = new ManagedResource();
        _manaPoints.SetupResource(_runtimeStats.maxMP);

        _characterGFX = Instantiate(_characterInfo.inGameGFX, transform);

        _targetPotentialDisplay.transform.SetAsLastSibling();
        _targetLockedDisplay.transform.SetAsLastSibling();
    }

    public void SetCombatantList(List<RealtimeCombatant> combatants)
    {
        _allCombatants.AddRange(combatants);
    }

    #endregion

    #region Turn State Control

    public void SetTurnState(CombatantTurnState turnState)
    {
        currentTurnState = turnState;

        CombatantTurnStateChanged?.Invoke(this, currentTurnState);
    }

    #endregion

    #region Getters

    public List<SkillInfo> GetSkillList { get { return _learnedSkills; } }

    public List<RealtimeCombatant> GetCombatantList { get { return _allCombatants; } }

    #endregion

    #region Combat 

    public void AimSkill(SkillInfo skill)
    {
        CombatantAimedSkill?.Invoke(this, skill);

        if (skill.costStat == CostStat.MP)
            _manaPoints.Deplete(skill.costAmount);
        else if (skill.costStat == CostStat.HP)
            _healthPoints.Deplete(skill.costAmount);
    }

    public void UseSkill(SkillInfo skill, List<RealtimeCombatant> targets)
    {
        if (!_learnedSkills.Contains(skill))
            return;

        CombatantUsedSkill?.Invoke(this, skill, targets);

        SetTurnState(CombatantTurnState.DoneForTheTurn);
    }

    public void ReleaseSkill(SkillInfo skill)
    {
        CombatantReleasedSkill?.Invoke(skill);
    }

    public void AffectedByStatusEffect(int finalEffectValue, StatusEffectInfo statusEffect)
    {
        if (currentTurnState == CombatantTurnState.Dead)
            return;

        if (statusEffect.effectType == EffectType.Damaging)
        {
            _healthPoints.Deplete(finalEffectValue);

            if (_healthPoints.IsResourceFullyDepleted())
                Death();

            CombatantReceivedDamage?.Invoke(finalEffectValue);
        }
        else if (statusEffect.effectType == EffectType.Healing)
        {
            _healthPoints.Replenish(finalEffectValue);

            CombatantReceivedHealing?.Invoke(finalEffectValue);
        }

        CombatantAffectedByStatusEffect?.Invoke(statusEffect);
    }

    public void ReceiveSkillEffect(int finalEffectValue, SkillInfo skill)
    {
        if (currentTurnState == CombatantTurnState.Dead)
            return;

        if (skill.effectType == EffectType.Damaging)
        {
            _healthPoints.Deplete(finalEffectValue);

            if (_healthPoints.IsResourceFullyDepleted())
                Death();

            CombatantReceivedDamage?.Invoke(finalEffectValue);
        }
        else if(skill.effectType == EffectType.Healing)
        {
            _healthPoints.Replenish(finalEffectValue);

            CombatantReceivedHealing?.Invoke(finalEffectValue);
        }

        CombatantReceivedSkillEffect?.Invoke(skill);
    }

    public void DodgeSkill()
    {
        CombatantDodgedSkill?.Invoke();
    }

    void Death()
    {
        if (currentTurnState != CombatantTurnState.Dead)
        {
            SetTurnState(CombatantTurnState.Dead);

            CombatantDied?.Invoke();
        }
    }

    #endregion

    #region Events

    public void GFXClicked()
    {        
        CombatantClicked?.Invoke(this);
    }

    #endregion

    #region UI Displays Management

    public void ToggleCombatantSelection(bool state)
    {
        _selectionDisplay.ToggleElementState(state);
    }

    public void ToggleTargetPotentialDisplay(bool state)
    {
        IsValidTarget = state;
        _targetPotentialDisplay.ToggleElementState(IsValidTarget);
    }

    public void ToggleTargetLockedDisplay(bool state)
    {
        _targetLockedDisplay.ToggleElementState(state);
    }

    #endregion
}
