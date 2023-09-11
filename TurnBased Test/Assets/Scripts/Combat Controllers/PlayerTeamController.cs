using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeamController : MonoBehaviour
{
    [SerializeField] CombatManager _combatManager;
    [SerializeField] MatchManager _matchManager;

    RealtimeCombatant _currentlySelectedAttackerCombatant;
    SkillInfo _currentlyAimingSkill;

    List<RealtimeCombatant> _team = new List<RealtimeCombatant>();
    List<CombatantInfoUI> _allCharacterInfoUI = new List<CombatantInfoUI>();

    void Awake()
    {
        _matchManager.TurnStateChange += MatchTurnStateChanged;
        _matchManager.MatchSetupDone += MatchSetupDone;
        _combatManager.CombatantSelected += CombatantSelected;
    }

    void MatchSetupDone()
    {
        _team.AddRange(_matchManager.GetPlayerTeam);

        foreach (var teamMember in _team)
            teamMember.CombatantTurnStateChanged += CombatantTurnStateChanged;

        _allCharacterInfoUI.AddRange(_combatManager.GetAllCharacterInfoUI);

        foreach (var characterInfoUI in _allCharacterInfoUI)
            characterInfoUI.SkillSelected += SkillSelected;
    }

    void CombatantSelected(RealtimeCombatant selectedCombatant)
    {
        //interaction only happens if player is supposed to input something now
        if(_matchManager.GetMatchTurnState != MatchTurnState.PlayerTurnWaitForInput)
            return;

        //this catches the first interaction at the beginning of the player input turn and each interaction right after a combatant is set as donefortheturn
        if (_currentlySelectedAttackerCombatant == null)
        {
            //only matters if the selected character is a player controlled one, not currently dead and not already done this turn.
            if (!_matchManager.GetPlayerTeam.Contains(selectedCombatant))
                return;

            if (selectedCombatant.currentTurnState != CombatantTurnState.WaitingForInput)
                return;

            SelectCombatantForInput(selectedCombatant);
        }
        // this happens when I click someone else even tho Im currently in the middle of a single combatant turn decisions
        else
        {
             // this interaction should only happen when that someone is a valid target, as decided by the combat manager
            if (!selectedCombatant.IsValidTarget)
                return;

            // to get here I needed to be on the player input turn, to have selected a character already, which brings up their UI
            // only through clicking a skill there I could advance to telling the combat manager to "aim" it and to mark people as valid targets

            //if this is all met, the selected combatant here needs to be a valid target for the skill I just tried to use so I use it

            // in terms of actually picking a target thats only really possible for single target skills, the other ones either affect a whole team or everyone
            // so the list of chosen targets here can be comprised of simply the combatant I clicked 
            // since the effect range itself will be enforced by the combat manager in every other case

            List<RealtimeCombatant> _chosenTargets = new List<RealtimeCombatant>();
            _chosenTargets.Add(selectedCombatant);

            _currentlySelectedAttackerCombatant.UseSkill(_currentlyAimingSkill, _chosenTargets);
        }
    }

    void MatchTurnStateChanged(MatchTurnState turnState)
    {
        // player input turn always starts clean
        if (turnState == MatchTurnState.PlayerTurnWaitForInput)
        {
            _currentlySelectedAttackerCombatant = null;
            _currentlyAimingSkill = null;
        }
    }

    void CombatantTurnStateChanged(RealtimeCombatant combatant, CombatantTurnState turnState)
    {
        //cleans this here as well, to allow for a new interaction with another character
        if(combatant == _currentlySelectedAttackerCombatant && turnState == CombatantTurnState.DoneForTheTurn)
        {
            _currentlySelectedAttackerCombatant.ToggleCombatantSelection(false);
            ToggleCombatantCharacterInfoUI(_currentlySelectedAttackerCombatant, false);
            _currentlySelectedAttackerCombatant = null;
            _currentlyAimingSkill = null;
        }
    }

    void SelectCombatantForInput(RealtimeCombatant combatant)
    {
        _currentlySelectedAttackerCombatant = combatant;

        foreach (var teamMember in _team)
            teamMember.ToggleCombatantSelection(false);

        _currentlySelectedAttackerCombatant.ToggleCombatantSelection(true);

        ToggleCombatantCharacterInfoUI(combatant, true);
    }

    void ToggleCombatantCharacterInfoUI(RealtimeCombatant combatant, bool state)
    {
        foreach (var characterInfoUI in _allCharacterInfoUI)
        {
            if(characterInfoUI._respectiveCombatant == combatant)
            {
                characterInfoUI.gameObject.SetActive(state);
                break;
            }
        }
    }

    void SkillSelected(SkillInfo skill)
    {
        if (skill.costStat == CostStat.MP && _currentlySelectedAttackerCombatant._manaPoints.currentResource < skill.costAmount)
            return;

        if (skill.costStat == CostStat.HP && _currentlySelectedAttackerCombatant._healthPoints.currentResource < skill.costAmount)
            return;

        _currentlyAimingSkill = skill;
        _currentlySelectedAttackerCombatant.AimSkill(_currentlyAimingSkill);
    }
}
