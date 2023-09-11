using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MatchTurnState { PreBattle, PlayerTurnWaitForInput, EnemyTurnWait, TurnExecution, PostBattle }

[System.Serializable]
class StoredTurnAction
{
    public RealtimeCombatant performer;
    public List<RealtimeCombatant> receivers = new List<RealtimeCombatant>();
    public List<bool> skillActuallyHit = new List<bool>();
    public SkillInfo skillUsed;
    public int finalEffectValue;
}

public class CombatManager : MonoBehaviour
{
    public Action<RealtimeCombatant> CombatantSelected;
    public Action<RealtimeCombatant, SkillInfo, List<RealtimeCombatant>> CombatantAimedSkillAtValidTargets;

    [SerializeField] MatchManager _matchManager;

    [SerializeField] float _timeDelayBetweenCombatantActions;
    public float timeDelayForAIToDecideOnInput;

    [SerializeField] GameObject _statusEffectControllerPrefab;
    [SerializeField] CombatManagerUI _combatManagerUI;

    List<StoredTurnAction> _storedTurnActions = new List<StoredTurnAction>();
    List<StatusEffectController> _activeStatusEffects = new List<StatusEffectController>();

    #region Unity

    private void Awake()
    {
        _matchManager.CombatantCreated += OnCombatantCreation;
        _matchManager.TurnStateChange += ExecutionturnStarted;
    }

    #endregion 

    #region Combat


    public List<CombatantInfoUI> GetAllCharacterInfoUI { get; } = new List<CombatantInfoUI>();

    void AssignCharacterInfoUI(RealtimeCombatant combatant)
    {
        GetAllCharacterInfoUI.Add(_combatManagerUI.NewCombatantInfoUIHolder(combatant));
    }

    void CombatantTurnStateChanged(RealtimeCombatant combatant, CombatantTurnState combatantTurnState)
    {
        if ((_matchManager.GetMatchTurnState == MatchTurnState.PlayerTurnWaitForInput || _matchManager.GetMatchTurnState == MatchTurnState.EnemyTurnWait) && combatantTurnState == CombatantTurnState.DoneForTheTurn)
            CombatantFinishedTheirInput(combatant);
    }  

    IEnumerator ExecuteStoredCombatActions()
    {

        //at the start of the execution turn, before conventional skill effects are applied, we start by applying status effects to their targets
        foreach (var activeStatusEffect in _activeStatusEffects)
        {
            yield return new WaitForSeconds(_timeDelayBetweenCombatantActions);

            activeStatusEffect.ApplyStatusEffectToTarget();
        }

        //then, we run through each stored action, remebering that each is the result of a single character skill affecting n targets.
        for (int i = 0; i < _storedTurnActions.Count; i++)
        {
            // dead combatants dont execute actions
            if (_storedTurnActions[i].performer.currentTurnState == CombatantTurnState.Dead)
                continue;

            bool allTargetsAlreadyDead = true;

            foreach (var receiver in _storedTurnActions[i].receivers)
            {
                if(receiver.currentTurnState != CombatantTurnState.Dead)
                {
                    allTargetsAlreadyDead = false;
                    break;
                }
            }

            // neither do actions get executed if all their targets are dead
            if (allTargetsAlreadyDead)
                continue;

            yield return new WaitForSeconds(_timeDelayBetweenCombatantActions);

            // regardless of the outcome, the single performer of the skill needs to do their thing now for visual feedback purposes.
            _storedTurnActions[i].performer.ReleaseSkill(_storedTurnActions[i].skillUsed);

            yield return new WaitForSeconds(_storedTurnActions[i].skillUsed.targetReactionDelay);

            //here we need to look at each of the ones targeted by that skill
            for (int j = 0; j < _storedTurnActions[i].receivers.Count; j++)
            {
                //if it hit, this receiver will react accordingly, if not theyll show visual feedback of dodge
                if (_storedTurnActions[i].skillActuallyHit[j])
                {
                    _storedTurnActions[i].receivers[j].ReceiveSkillEffect(_storedTurnActions[i].finalEffectValue, _storedTurnActions[i].skillUsed);

                    CheckForStatusEffectAppliedBySkill(_storedTurnActions[i].receivers[j], _storedTurnActions[i].performer, _storedTurnActions[i].skillUsed);
                }
                else
                {
                    _storedTurnActions[i].receivers[j].DodgeSkill();
                }
            }
        }

        _storedTurnActions.Clear();

        _matchManager.ReadyToAdvanceTurn();
    }

    void CheckForStatusEffectAppliedBySkill(RealtimeCombatant target, RealtimeCombatant user, SkillInfo skill)
    {
        if (!skill.appliesStatusEffect)
            return;

        if (UnityEngine.Random.value * 100 >= skill.statusEffect.afflictionChance)
            return;

        StatusEffectController controller = Instantiate(_statusEffectControllerPrefab, target.transform).GetComponent<StatusEffectController>();
        controller.SetupStatusEffect(target, user, skill.statusEffect);

        _activeStatusEffects.Add(controller);

        controller.EffectDurationEnded += StatusEffectDurationEnded;
    }

    void EnterTurnActionIntoStorage(RealtimeCombatant performer, List<RealtimeCombatant> receivers, SkillInfo skillUsed, int finalEffectValue, List<bool> skillActuallyHit)
    {
        StoredTurnAction newAction = new StoredTurnAction();

        newAction.performer = performer;
        newAction.receivers.AddRange(receivers);
        newAction.skillUsed = skillUsed;
        newAction.finalEffectValue = finalEffectValue;
        newAction.skillActuallyHit.AddRange(skillActuallyHit);

        _storedTurnActions.Add(newAction);
    }

    void ToggleTargetPotentialDisplayOnValidTargets(List<RealtimeCombatant> validTargets, bool state)
    {
        foreach (var target in validTargets)
            target.ToggleTargetPotentialDisplay(state);
    }

    void ToggleTargetLockedDisplayOnAffectedTargets(List<RealtimeCombatant> affectedTargets, bool state)
    {
        foreach (var target in affectedTargets)
            target.ToggleTargetLockedDisplay(state);
    }

    #endregion

    #region Events

    void OnCombatantCreation(RealtimeCombatant combatant, bool isPlayerCombatant)
    {
        combatant.CombatantTurnStateChanged += CombatantTurnStateChanged;
        combatant.CombatantClicked += CombatantGFXClicked;
        combatant.CombatantAimedSkill += CombatantAimingSkill;
        combatant.CombatantUsedSkill += CombatantUsingSkill;

        AssignCharacterInfoUI(combatant);

        if (!isPlayerCombatant)
        {
            AICombatantController controller = combatant.gameObject.AddComponent<AICombatantController>();
            controller.SetupController();
            CombatantAimedSkillAtValidTargets += controller.SkillAimedAtValidTargets;
        }

        _combatManagerUI.NewGeneralCombatantHealthBar(combatant);
    }

    void ExecutionturnStarted(MatchTurnState turnState)
    {
        if (turnState == MatchTurnState.TurnExecution)
            StartCoroutine("ExecuteStoredCombatActions");
    }

    void CombatantGFXClicked(RealtimeCombatant combatant)
    {
        CombatantSelected?.Invoke(combatant);
    }

    void CombatantAimingSkill(RealtimeCombatant user, SkillInfo skill)
    {
        ToggleTargetPotentialDisplayOnValidTargets(_matchManager.GetAllCombatants, false);

        #region Manage allegiances based on user
        bool playerCombatant = _matchManager.GetPlayerTeam.Contains(user);

        List<RealtimeCombatant> _allyTeam = new List<RealtimeCombatant>();
        List<RealtimeCombatant> _adversaryTeam = new List<RealtimeCombatant>();
        List<RealtimeCombatant> _bothTeams = new List<RealtimeCombatant>();

        foreach (var member in _matchManager.GetAllCombatants)
        {
            if (member.currentTurnState != CombatantTurnState.Dead)
                _bothTeams.Add(member);
        }

        if (playerCombatant)
        {
            foreach (var member in _matchManager.GetPlayerTeam)
            {
                if (member.currentTurnState != CombatantTurnState.Dead)
                    _allyTeam.Add(member);
            }

            foreach (var member in _matchManager.GetEnemyTeam)
            {
                if (member.currentTurnState != CombatantTurnState.Dead)
                    _adversaryTeam.Add(member);
            }
        }
        else
        {
            foreach (var member in _matchManager.GetPlayerTeam)
            {
                if (member.currentTurnState != CombatantTurnState.Dead)
                    _adversaryTeam.Add(member);
            }

            foreach (var member in _matchManager.GetEnemyTeam)
            {
                if (member.currentTurnState != CombatantTurnState.Dead)
                    _allyTeam.Add(member);
            }
        }
        #endregion

        List<RealtimeCombatant> _validTargets = new List<RealtimeCombatant>();

        if (skill.targetRange == TargetRange.AnySingleTarget || skill.targetRange == TargetRange.Everyone)
        {
            foreach (var combatant in _bothTeams)
            {
                if (combatant == user && !skill.selfTargetingAllowed)
                    continue;           

                _validTargets.Add(combatant);
            }
        }
        else if(skill.targetRange == TargetRange.EnemySide || skill.targetRange == TargetRange.AnySingleEnemy)
            _validTargets.AddRange(_adversaryTeam);
        else if (skill.targetRange == TargetRange.AllySide || skill.targetRange == TargetRange.AnySingleAlly)
            _validTargets.AddRange(_allyTeam);

        ToggleTargetPotentialDisplayOnValidTargets(_validTargets, true);

        CombatantAimedSkillAtValidTargets?.Invoke(user, skill, _validTargets);
    }

    void CombatantUsingSkill(RealtimeCombatant user, SkillInfo skill, List<RealtimeCombatant> targets)
    {
        ToggleTargetPotentialDisplayOnValidTargets(_matchManager.GetAllCombatants, false);

        #region Manage allegiances based on user
        bool playerCombatant = _matchManager.GetPlayerTeam.Contains(user);

        List<RealtimeCombatant> _allyTeam = new List<RealtimeCombatant>();
        List<RealtimeCombatant> _adversaryTeam = new List<RealtimeCombatant>();

        if (playerCombatant)
        {
            foreach (var member in _matchManager.GetPlayerTeam)
            {
                if (member.currentTurnState != CombatantTurnState.Dead)
                    _allyTeam.Add(member);
            }

            foreach (var member in _matchManager.GetEnemyTeam)
            {
                if (member.currentTurnState != CombatantTurnState.Dead)
                    _adversaryTeam.Add(member);
            }
        }
        else
        {
            foreach (var member in _matchManager.GetPlayerTeam)
            {
                if (member.currentTurnState != CombatantTurnState.Dead)
                    _adversaryTeam.Add(member);
            }

            foreach (var member in _matchManager.GetEnemyTeam)
            {
                if (member.currentTurnState != CombatantTurnState.Dead)
                    _allyTeam.Add(member);
            }
        }
        #endregion

        List<RealtimeCombatant> affectedTargets = new List<RealtimeCombatant>();
        List<bool> _skillsThatActuallyHit = new List<bool>();

        switch (skill.targetRange)
        {
            case TargetRange.AnySingleTarget:
                affectedTargets.AddRange(targets);
                break;
            case TargetRange.AnySingleEnemy:
                affectedTargets.AddRange(targets);
                break;
            case TargetRange.AnySingleAlly:
                affectedTargets.AddRange(targets);
                break;
            case TargetRange.EnemySide:
                affectedTargets.AddRange(_adversaryTeam);
                break;
            case TargetRange.AllySide:
                affectedTargets.AddRange(_allyTeam);
                break;
            case TargetRange.Everyone:
                {
                    foreach (var combatant in _matchManager.GetAllCombatants)
                    {
                        if (combatant == user && !skill.selfTargetingAllowed)
                            continue;

                        affectedTargets.Add(combatant);
                    }
                }
                break;
        }

        foreach (var target in affectedTargets)
        {
            float skillEffectiveHitChance = skill.hitChance + user._runtimeStats.hit - target._runtimeStats.dodge;

            bool skillActuallyHit = true;

            if (UnityEngine.Random.value * 100 >= skillEffectiveHitChance)
                skillActuallyHit = false;

            _skillsThatActuallyHit.Add(skillActuallyHit);
        }

        int leveragedStatValue = 0;
        int finalEffectValue = 0;

        switch (skill.effectiveStat)
        {
            case EffectiveStat.Strenght:
                leveragedStatValue = user._runtimeStats.strenght;
                break;
            case EffectiveStat.Vitality:
                leveragedStatValue = user._runtimeStats.vitality;
                break;
            case EffectiveStat.Dexterity:
                leveragedStatValue = user._runtimeStats.dexterity;
                break;
            case EffectiveStat.Agility:
                leveragedStatValue = user._runtimeStats.agility;
                break;
            case EffectiveStat.Intelligence:
                leveragedStatValue = user._runtimeStats.intelligence;
                break;
        }

        switch (skill.statEffectOnBaseValue)
        {
            case StatEffectOnBaseValue.Additive:
                finalEffectValue = skill.effectBaseValue + leveragedStatValue;
                break;
            case StatEffectOnBaseValue.Multiplicative:
                finalEffectValue = skill.effectBaseValue * leveragedStatValue;
                break;
        }

        ToggleTargetLockedDisplayOnAffectedTargets(affectedTargets, true);

        Run.After(0.25f, () => ToggleTargetLockedDisplayOnAffectedTargets(affectedTargets, false));

        EnterTurnActionIntoStorage(user, affectedTargets, skill, finalEffectValue, _skillsThatActuallyHit);
    }

    void CombatantFinishedTheirInput(RealtimeCombatant doneCombatant)
    {
        if (_matchManager.AllCombatantsDoneForTheTurn() && _matchManager.GetMatchTurnState != MatchTurnState.TurnExecution)
            _matchManager.ReadyToAdvanceTurn();
    }

    void StatusEffectDurationEnded(StatusEffectController statusEffect)
    {
        _activeStatusEffects.Remove(statusEffect);

        if (statusEffect.gameObject != null)
            Destroy(statusEffect.gameObject);
    }

    #endregion


}
