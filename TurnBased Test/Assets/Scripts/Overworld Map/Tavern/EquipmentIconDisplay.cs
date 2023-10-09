using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class EquipmentIconDisplay : MonoBehaviour
{
    public Action<EquipmentIconDisplay> EquipmentIconClicked;

    [SerializeField] GameObject _selectionBracket;
    [SerializeField] Image _equipmentIcon;

    public EquipmentInfo _equipmentInfo { get; private set; }

    public void SetupEquipmentIconDisplay(EquipmentInfo equipment)
    {
        _equipmentInfo = equipment;

        _equipmentIcon.sprite = _equipmentInfo.equipmentIcon;
    }

    public void IconClicked()
    {
        if (_equipmentInfo == null)
            return;

        EquipmentIconClicked.Invoke(this);
    }

    public void ToggleSelectionBracket(bool state)
    {
        _selectionBracket.SetActive(state);
    }
}
