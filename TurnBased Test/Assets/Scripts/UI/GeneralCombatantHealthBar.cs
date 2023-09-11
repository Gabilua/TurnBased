using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralCombatantHealthBar : MonoBehaviour
{
    [SerializeField] Image _healthBar;
    [SerializeField] CanvasGroup _canvasGroup;
    RealtimeCombatant _respectiveCombatant;

    public void Setup(RealtimeCombatant combatant)
    {
        _respectiveCombatant = combatant;
        _respectiveCombatant.CombatantReceivedSkillEffect += CombatantHealthChanged;

        transform.position = _respectiveCombatant.transform.position;
    }

    void CombatantHealthChanged(SkillInfo skill)
    {
        _healthBar.fillAmount = (float)_respectiveCombatant._healthPoints.currentResource / (float)_respectiveCombatant._healthPoints.maxResource;

        if (_respectiveCombatant.currentTurnState == CombatantTurnState.Dead)
            _canvasGroup.alpha = 0;
    }
}
