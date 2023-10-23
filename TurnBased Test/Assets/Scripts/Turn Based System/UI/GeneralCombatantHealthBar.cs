using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GeneralCombatantHealthBar : MonoBehaviour
{
    [SerializeField] Image _healthBar;
    [SerializeField] CanvasGroup _canvasGroup;
    RealtimeCombatant _respectiveCombatant;

    public void Setup(RealtimeCombatant combatant)
    {
        _respectiveCombatant = combatant;
        _respectiveCombatant.CombatantReceivedSkillEffect += CombatantHealthChanged;
        _respectiveCombatant.CombatantDied += CombatantDied;

        transform.position = _respectiveCombatant.transform.position;
    }

    void CombatantHealthChanged(SkillInfo skill)
    {
        float currentFill = _healthBar.fillAmount;
        float updatedFill = _respectiveCombatant._healthPoints.currentResource / _respectiveCombatant._healthPoints.maxResource;

        DOTween.To(() => currentFill, x => currentFill = x, updatedFill, 0.25f)
            .OnUpdate(() => 
            {
                _healthBar.fillAmount = currentFill;
            });


        if (_respectiveCombatant.currentTurnState == CombatantTurnState.Dead)
            _canvasGroup.alpha = 0;
    }

    void CombatantDied()
    {
        _canvasGroup.alpha = 0;
    }
}
