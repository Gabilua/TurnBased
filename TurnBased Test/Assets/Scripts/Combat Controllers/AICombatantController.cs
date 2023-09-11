using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICombatantController : MonoBehaviour
{
    RealtimeCombatant _thisCombatant;

    List<RealtimeCombatant> enemies = new List<RealtimeCombatant>();
    List<RealtimeCombatant> allies = new List<RealtimeCombatant>();

    public void SetupController()
    {
        _thisCombatant = GetComponent<RealtimeCombatant>();
        _thisCombatant.CombatantTurnStateChanged += CharacterTurnStateChanged;

        IdentifyFactions();
    }

    void IdentifyFactions()
    {
        foreach (var combatant in _thisCombatant.GetCombatantList)
        {
            if (combatant.GetComponent<AICombatantController>())
                allies.Add(combatant);
            else
                enemies.Add(combatant);
        }
    }

    void CharacterTurnStateChanged(RealtimeCombatant character, CombatantTurnState turnState)
    {
        if (allies.Count == 0 || enemies.Count == 0)
            IdentifyFactions();

        if (_thisCombatant.currentTurnState == CombatantTurnState.Dead)
            return;

        if (turnState == CombatantTurnState.WaitingForInput)
           ChooseSkillToUse();
    }

    void ChooseSkillToUse()
    {
        #region Validate Available Skills
        List<SkillInfo> availableSkills = new List<SkillInfo>();

        foreach (var skill in _thisCombatant.GetSkillList)
        {
            if (skill.costStat == CostStat.MP)
            {
                if (_thisCombatant._manaPoints.currentResource >= skill.costAmount)
                    availableSkills.Add(skill);
            }
            else if (skill.costStat == CostStat.HP)
            {
                if (_thisCombatant._healthPoints.currentResource >= skill.costAmount)
                    availableSkills.Add(skill);
            }
        }

        SkillInfo chosenSkill = availableSkills[Random.Range(0, availableSkills.Count)];
        #endregion           

        _thisCombatant.AimSkill(chosenSkill);
    }

    public void SkillAimedAtValidTargets(RealtimeCombatant combatant, SkillInfo skill, List<RealtimeCombatant> validTargets)
    {
        if (combatant != _thisCombatant)
            return;

        ChooseTargetsForSkill(combatant, skill, validTargets);
    }

    void ChooseTargetsForSkill(RealtimeCombatant combatant, SkillInfo skill, List<RealtimeCombatant> validTargets)
    {
        #region Validate Available Targets
        List<RealtimeCombatant> availableEnemies = new List<RealtimeCombatant>();
        List<RealtimeCombatant> availableAllies = new List<RealtimeCombatant>();

        foreach (var target in enemies)
        {
            if (target.currentTurnState != CombatantTurnState.Dead)
                availableEnemies.Add(target);
        }

        foreach (var target in allies)
        {
            if (target.currentTurnState != CombatantTurnState.Dead)
                availableAllies.Add(target);
        }
        #endregion

        #region Skill Effect Type Priorization
        List<RealtimeCombatant> chosenTargets = new List<RealtimeCombatant>();

        switch (skill.targetRange)
        {
            case TargetRange.AnySingleTarget:
                {
                    if (skill.effectType == EffectType.Damaging)
                        chosenTargets.Add(availableEnemies[Random.Range(0, availableEnemies.Count)]);
                    else if (skill.effectType == EffectType.Healing)
                        chosenTargets.Add(availableAllies[Random.Range(0, availableAllies.Count)]);
                }
                break;
            case TargetRange.AnySingleAlly:
                {
                    chosenTargets.Add(availableAllies[Random.Range(0, availableAllies.Count)]);
                }
                break;
            case TargetRange.AnySingleEnemy:
                {
                    chosenTargets.Add(availableEnemies[Random.Range(0, availableEnemies.Count)]);
                }
                break;
            case TargetRange.OnlyUser:
                {
                    chosenTargets.Add(_thisCombatant);
                }
                break;
        }
        #endregion

        _thisCombatant.UseSkill(skill, chosenTargets);
    }
}
