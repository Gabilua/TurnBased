using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentIconDisplay : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public Action<EquipmentIconDisplay> EquipmentIconDragging;
    public Action<EquipmentIconDisplay> EquipmentIconDragStart;
    public Action<EquipmentIconDisplay> EquipmentIconDragEnd;
    public Action<TavernInventorySlot> InventorySlotDroppedOnEquipment;

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

    #region Drag Events

    public void OnDrag(PointerEventData eventData)
    {
        if (_equipmentInfo == null)
            return;

        EquipmentIconDragging?.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_equipmentInfo == null)
            return;

        _equipmentIcon.gameObject.SetActive(false);

        EquipmentIconDragStart?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_equipmentInfo == null)
            return;

        _equipmentIcon.gameObject.SetActive(true);

        EquipmentIconDragEnd?.Invoke(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        if (!eventData.pointerDrag.GetComponent<TavernInventorySlot>())
            return;

        TavernInventorySlot inventorySlot = eventData.pointerDrag.GetComponent<TavernInventorySlot>();

        InventorySlotDroppedOnEquipment?.Invoke(inventorySlot);
    }

    #endregion
}
