using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisualFeedbackManager : MonoBehaviour
{
    protected RealtimeCombatant _thisCombatant;

    protected virtual void Start()
    {
        _thisCombatant = GetComponentInParent<RealtimeCombatant>();

        _thisCombatant.CombatantAimedSkill += AimedSkill;
        _thisCombatant.CombatantReleasedSkill += ReleasedSkill;
        _thisCombatant.CombatantReceivedSkillEffect += ReceivedSkill;
        _thisCombatant.CombatantDodgedSkill += DodgedSkill;
        _thisCombatant.CombatantStatusEffectAfflictionChange += StatusEffectChange;
        _thisCombatant.CombatantAffectedByStatusEffect += StatusEffectHit;
        _thisCombatant.CombatantInterruptedByStatusEffect += StatusEffectInterrupt;
        _thisCombatant.CombatantReceivedDamage += Damaged;
        _thisCombatant.CombatantReceivedHealing += Healed;
        _thisCombatant.CombatantDied += Died;
        _thisCombatant.CombatantClicked += Clicked;

        _thisCombatant.CombatantTurnStateChanged += TurnStateChange;
        _thisCombatant.CombatantFinishedSetup += SetupFinished;
    }

    protected virtual void SetupFinished()
    {

    }

    protected virtual void TurnStateChange(RealtimeCombatant combatant, CombatantTurnState turnState)
    {

    }

    protected virtual void Damaged(int value, TargetStat stat)
    {

    }

    protected virtual void Healed(int value, TargetStat stat)
    {

    }

    protected virtual void AimedSkill(RealtimeCombatant user, SkillInfo skill)
    {

    }

    protected virtual void ReleasedSkill(SkillInfo skill)
    {

    }

    protected virtual void ReceivedSkill(SkillInfo skill)
    {

    }

    protected virtual void StatusEffectChange(StatusEffectInfo statusEffect, bool state)
    {
   
    }

    protected virtual void StatusEffectHit(StatusEffectInfo statusEffect)
    {

    }

    protected virtual void StatusEffectInterrupt(StatusEffectInfo statusEffect)
    {

    }

    protected virtual void DodgedSkill()
    {

    }

    protected virtual void Died()
    {

    }

    protected virtual void Clicked(RealtimeCombatant combatant)
    {

    }
}
