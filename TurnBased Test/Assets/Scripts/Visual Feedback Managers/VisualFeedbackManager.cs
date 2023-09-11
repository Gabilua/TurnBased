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
        _thisCombatant.CombatantReceivedDamage += Damaged;
        _thisCombatant.CombatantReceivedHealing += Healed;
        _thisCombatant.CombatantDied += Died;
        _thisCombatant.CombatantClicked += Clicked;
    }

    protected virtual void Damaged(int value)
    {

    }

    protected virtual void Healed(int value)
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
