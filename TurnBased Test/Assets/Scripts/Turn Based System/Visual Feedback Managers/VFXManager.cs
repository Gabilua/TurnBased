using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VFXManager : VisualFeedbackManager
{
    [SerializeField] Animator _damagedFX;
    [SerializeField] Animator _healedFX;

    [SerializeField] Transform _displaysHolder;
    [SerializeField] TextMeshProUGUI _damageValueDisplay;
    [SerializeField] TextMeshProUGUI _healValueDisplay;
    [SerializeField] TextMeshProUGUI _skillDodgedDisplay;

    [SerializeField] GameObject _statusEffectDisplayPrefab;
    [SerializeField] Transform _statusEffectDisplayHolder;
    List<Image> _activeStatusEffectDisplays = new List<Image>();

    Animator _shadow;

    Animator _damageValueAnimator;
    Animator _healValueAnimator;
    Animator _skillDodgedAnimator;

    protected override void Start()
    {
        base.Start();

        _shadow = _thisCombatant.transform.GetChild(0).GetComponent<Animator>();

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

    protected override void Damaged(int value, TargetStat stat)
    {
        if (value == 0)
            return;

        _damagedFX.SetTrigger("Action");

        _damageValueDisplay.transform.SetAsLastSibling();
        _damageValueDisplay.text = value.ToString();
        _damageValueAnimator.SetTrigger("Action");
    }

    protected override void Healed(int value, TargetStat stat)
    {
        if (value == 0)
            return;

        _healedFX.SetTrigger("Action");

        _healValueDisplay.transform.SetAsLastSibling();
        _healValueDisplay.text = value.ToString();
        _healValueAnimator.SetTrigger("Action");
    }

    protected override void ReleasedSkill(SkillInfo skill)
    {
        if (skill.useVFX == null)
            return;

        GameObject fx = Instantiate(skill.useVFX, _thisCombatant.transform);
        fx.transform.SetAsFirstSibling();
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

        GameObject fx = Instantiate(skill.receiveVFX, _thisCombatant.transform);
        fx.transform.SetAsFirstSibling();
    }

    protected override void Died()
    {
        foreach (Transform child in _statusEffectDisplayHolder)
            Destroy(child.gameObject);

        _shadow.SetTrigger("Action");

        _activeStatusEffectDisplays.Clear();
    }

    protected override void StatusEffectChange(StatusEffectInfo statusEffect, bool state)
    {
        if (state)
        {
           Image display = Instantiate(_statusEffectDisplayPrefab, _statusEffectDisplayHolder).GetComponent<Image>();
            display.sprite = statusEffect.effectIcon;
            display.gameObject.name = statusEffect.name;

            _activeStatusEffectDisplays.Add(display);

            ShowStatusEffectVFX(statusEffect);
        }
        else
        {
            Image doomedDisplay = null;

            foreach (var display in _activeStatusEffectDisplays)
            {
                if (display.gameObject.name == statusEffect.name)
                {
                    doomedDisplay = display;
                    break;
                }
            }

            RemoveStatusEffectDisplay(doomedDisplay);
        }
    }

    void RemoveStatusEffectDisplay(Image display)
    {
        _activeStatusEffectDisplays.Remove(display);

        if(display != null)
        Destroy(display.gameObject);
    }

    protected override void StatusEffectHit(StatusEffectInfo statusEffect)
    {
        if (statusEffect.effectBaseValue == 0 && statusEffect.affectedByStats == false)
            return;

        ShowStatusEffectVFX(statusEffect);
    }

    protected override void StatusEffectInterrupt(StatusEffectInfo statusEffect)
    {
        ShowStatusEffectVFX(statusEffect);
    }

    void ShowStatusEffectVFX(StatusEffectInfo statusEffect)
    {
        if (statusEffect.receiveVFX == null)
            return;

        GameObject fx = Instantiate(statusEffect.receiveVFX, _thisCombatant.transform);
        fx.transform.SetAsFirstSibling();
    }
}
