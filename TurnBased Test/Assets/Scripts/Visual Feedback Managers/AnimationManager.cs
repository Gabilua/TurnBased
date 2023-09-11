using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : VisualFeedbackManager
{
    Animator _animator;

    protected override void Start()
    {
        base.Start();

        _animator = GetComponent<Animator>();
    }

    protected override void Damaged(int value, TargetStat stat)
    {
        _animator.SetTrigger("Hurt");
    }

    protected override void Healed(int value, TargetStat stat)
    {
        _animator.SetTrigger("Healed");
    }

    protected override void AimedSkill(RealtimeCombatant user, SkillInfo skill)
    {
        _animator.SetBool("Aiming", true);
    }

    protected override void ReleasedSkill(SkillInfo skill)
    {
        _animator.SetBool("Aiming", false);

        if (skill.usesSpecialAnimation)
            _animator.SetTrigger("Skill");
        else
            _animator.SetTrigger("Attack");
    }

    protected override void Died()
    {
        _animator.SetBool("Dead", true);
    }

    protected override void Clicked(RealtimeCombatant combatant)
    {
        if (_thisCombatant.currentTurnState == CombatantTurnState.PreGame || _thisCombatant.currentTurnState == CombatantTurnState.PostGame)
            return;

        _animator.SetTrigger("Clicked");
    }
}
