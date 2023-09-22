using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatusEffectController : MonoBehaviour
{
    public Action<StatusEffectController> EffectDurationEnded;

    public RealtimeCombatant _target { get; private set; }
    public RealtimeCombatant _skillUser { get; private set; }
    public StatusEffectInfo _effectApplied { get; private set; }
    public int _remainingAffectedTurns { get; private set; }

    public void SetupStatusEffect(RealtimeCombatant target, RealtimeCombatant user, StatusEffectInfo effectInfo)
    {
        target.CombatantDied += TargetDied;

        _target = target;
        _skillUser = user;
        _effectApplied = effectInfo;
        _remainingAffectedTurns = _effectApplied.durationInTurns;

        if (_effectApplied.effectType == EffectType.Altering)
            _target._runtimeStats.AddTemporaryChange(_effectApplied);
    }

    public void StackDuration()
    {
        _remainingAffectedTurns += _effectApplied.durationInTurns;
    }

    void TurnCountdown()
    {
        _remainingAffectedTurns--;

        if (_remainingAffectedTurns == 0)
            RemoveStatusEffectFromTarget();
    }

    void TargetDied()
    {
        Destroy(this);
    }

    public void ApplyStatusEffectToTarget()
    {
        int finalEffectValue = 0;

        if (_effectApplied.affectedByStats)
        {
            int leveragedStatValue = 0;

            switch (_effectApplied.effectiveStat)
            {
                case EffectiveStat.Strenght:
                    leveragedStatValue = _skillUser._runtimeStats.GetFinalStat(TargetStat.STR);
                    break;
                case EffectiveStat.Vitality:
                    leveragedStatValue = _skillUser._runtimeStats.GetFinalStat(TargetStat.VIT);
                    break;
                case EffectiveStat.Dexterity:
                    leveragedStatValue = _skillUser._runtimeStats.GetFinalStat(TargetStat.DEX);
                    break;
                case EffectiveStat.Agility:
                    leveragedStatValue = _skillUser._runtimeStats.GetFinalStat(TargetStat.AGI);
                    break;
                case EffectiveStat.Intelligence:
                    leveragedStatValue = _skillUser._runtimeStats.GetFinalStat(TargetStat.INT);
                    break;
            }

            switch (_effectApplied.statEffectOnBaseValue)
            {
                case StatEffectOnBaseValue.Additive:
                    finalEffectValue = _effectApplied.effectBaseValue + leveragedStatValue;
                    break;
                case StatEffectOnBaseValue.Multiplicative:
                    finalEffectValue = _effectApplied.effectBaseValue * leveragedStatValue;
                    break;
            }
        }
        else
            finalEffectValue = _effectApplied.effectBaseValue;

        _target.AffectedByStatusEffect(finalEffectValue, _effectApplied);

        TurnCountdown();
    }    

    void RemoveStatusEffectFromTarget()
    {
        _target._runtimeStats.RemoveTemporaryChange(_effectApplied);

        EffectDurationEnded?.Invoke(this);
    }
}
