using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManagerUI : MonoBehaviour
{
    [SerializeField] Transform _combatantInfoUIHolder;
    [SerializeField] GameObject _combatantInfoUIPrefab;

    [SerializeField] Transform _combatantHealthbarHolder;
    [SerializeField] GameObject _combatantGeneralHealthBarPrefab;

    public void NewGeneralCombatantHealthBar(RealtimeCombatant combatant)
    {
        GeneralCombatantHealthBar healthBar = Instantiate(_combatantGeneralHealthBarPrefab, _combatantHealthbarHolder).GetComponent<GeneralCombatantHealthBar>();
        healthBar.Setup(combatant);
    }

    public CombatantInfoUI NewCombatantInfoUIHolder(RealtimeCombatant combatant)
    {
        CombatantInfoUI characterInfoUI = Instantiate(_combatantInfoUIPrefab, _combatantInfoUIHolder).GetComponent<CombatantInfoUI>();
        characterInfoUI.SetupInfo(combatant);

        return characterInfoUI;
    }

    public void ResetCombatManagerUI()
    {
        foreach (Transform child in _combatantHealthbarHolder)
            Destroy(child.gameObject);

        foreach (Transform child in _combatantInfoUIHolder)
            Destroy(child.gameObject);
    }
}
