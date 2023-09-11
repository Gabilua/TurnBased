using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VFXManager : VisualFeedbackManager
{
    [SerializeField] Animator _damagedFX;
    [SerializeField] Animator _healedFX;

    [SerializeField] Transform _displaysHolder;
    [SerializeField] TextMeshProUGUI _damageValueDisplay;
    [SerializeField] TextMeshProUGUI _healValueDisplay;
    [SerializeField] TextMeshProUGUI _skillDodgedDisplay;

    Animator _damageValueAnimator;
    Animator _healValueAnimator;
    Animator _skillDodgedAnimator;    

    protected override void Start()
    {
        base.Start();

        AccountForCombatantInversion();

        _damageValueAnimator = _damageValueDisplay.GetComponent<Animator>();
        _healValueAnimator = _healValueDisplay.GetComponent<Animator>();
        _skillDodgedAnimator = _skillDodgedDisplay.GetComponent<Animator>();
    }

    void AccountForCombatantInversion()
    {
        Transform invertibleParent = transform.parent.parent.parent;

        _displaysHolder.localScale =
           new Vector3(_displaysHolder.localScale.x * invertibleParent.localScale.x,
             _displaysHolder.localScale.y, _displaysHolder.localScale.y);
    }

    protected override void Damaged(int value)
    {
        _damagedFX.SetTrigger("Action");

        if (value == 0)
            return;

        _damageValueDisplay.transform.SetAsLastSibling();
        _damageValueDisplay.text = value.ToString();
        _damageValueAnimator.SetTrigger("Action");
    }

    protected override void Healed(int value)
    {
        _healedFX.SetTrigger("Action");

        if (value == 0)
            return;

        _healValueDisplay.transform.SetAsLastSibling();
        _healValueDisplay.text = value.ToString();
        _healValueAnimator.SetTrigger("Action");
    }

    protected override void ReleasedSkill(SkillInfo skill)
    {
        if (skill.useVFX == null)
            return;

        GameObject fx = Instantiate(skill.useVFX, _displaysHolder);
        fx.transform.SetAsFirstSibling();
        Destroy(fx, 5f);
    }

    protected override void DodgedSkill()
    {
        _skillDodgedAnimator.transform.SetAsLastSibling();
        _skillDodgedAnimator.SetTrigger("Action");
    }

    protected override void ReceivedSkill(SkillInfo skill)
    {
        if (skill.receiveVFX == null)
            return;

        GameObject fx = Instantiate(skill.receiveVFX, _displaysHolder);
        fx.transform.SetAsFirstSibling();
        Destroy(fx, 5f);
    }
}
