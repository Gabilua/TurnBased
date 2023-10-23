using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopStockSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Action<ShopStockSlot> ShopSlotDragging;
    public Action<ShopStockSlot> ShopSlotDragStart;
    public Action<ShopStockSlot> ShopSlotDragEnd;

    public Action<ShopStockSlot> ShopSlotClicked;

    [SerializeField] GameObject _selectionBracket;
    [SerializeField] Image _equipmentIcon;
    [SerializeField] TextMeshProUGUI _equipmentValue;

    public EquipmentInfo _equipmentInfo { get; private set; }

    public void SetupShopStockSlot(EquipmentInfo equipment)
    {
        _equipmentInfo = equipment;

        _equipmentIcon.sprite = _equipmentInfo.equipmentIcon;
        _equipmentValue.text = _equipmentInfo.equipmentValue.ToString();
    }

    public void SlotClicked()
    {
        if (_equipmentInfo == null)
            return;

        ShopSlotClicked.Invoke(this);
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

        ShopSlotDragging?.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_equipmentInfo == null)
            return;

        _equipmentIcon.gameObject.SetActive(false);

        ShopSlotDragStart?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_equipmentInfo == null)
            return;

        _equipmentIcon.gameObject.SetActive(true);

        ShopSlotDragEnd?.Invoke(this);
    }

    #endregion
}
