using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TavernInventorySlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public Action<TavernInventorySlot> InventorySlotDragging;
    public Action<TavernInventorySlot> InventorySlotDragStart;
    public Action<TavernInventorySlot> InventorySlotDragEnd;
    public Action<EquipmentIconDisplay> CharacterEquipmentDroppedOnSlot;

    [SerializeField] Image _equipmentIcon;
    [SerializeField] TextMeshProUGUI _sellingPriceDisplay;
    [SerializeField] Sprite _emptySprite;

    [SerializeField] Color[] _availabilityColors;

    RecruitedCharacter _currentlySelectedCharacter;

    public EquipmentInfo _equipmentInfo { get; private set; }

    public bool IsSlotOccupied()
    {
        return _equipmentInfo != null;
    }

    public void SetupInventorySlotEquipment(EquipmentInfo equipment)
    {
        _equipmentInfo = equipment;

        _equipmentIcon.sprite = _equipmentInfo.equipmentIcon;

        _sellingPriceDisplay.text = _equipmentInfo.sellingPrice + "$";

        if (_currentlySelectedCharacter == null)
            return;

        if (EquipmentUsableByCurrentlySelectedCharacter())
            _equipmentIcon.color = _availabilityColors[1];
        else
            _equipmentIcon.color = _availabilityColors[0];
    }

    public void SetupInventorySlotInfo(RecruitedCharacter currentlySelectedCharacter)
    {
        _currentlySelectedCharacter = currentlySelectedCharacter;
    }

    public void ResetInventorySlot()
    {
        _equipmentInfo = null;
        _equipmentIcon.sprite = _emptySprite;
        _sellingPriceDisplay.text = string.Empty;
        _sellingPriceDisplay.gameObject.SetActive(false);
    }

    public bool EquipmentUsableByCurrentlySelectedCharacter()
    {
        return _equipmentInfo.allowedUsers.Contains(_currentlySelectedCharacter.characterInfo);
    }

    #region Events

    public void OnDrag(PointerEventData eventData)
    {
        if (_equipmentInfo == null)
            return;

        if (!EquipmentUsableByCurrentlySelectedCharacter())
            return;

        InventorySlotDragging?.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_equipmentInfo == null)
            return;

        if (!EquipmentUsableByCurrentlySelectedCharacter())
            return;

        _equipmentIcon.gameObject.SetActive(false);

        InventorySlotDragStart?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_equipmentInfo == null)
            return;

        if (!EquipmentUsableByCurrentlySelectedCharacter())
            return;

        _equipmentIcon.gameObject.SetActive(true);

        InventorySlotDragEnd?.Invoke(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        if (!eventData.pointerDrag.GetComponent<EquipmentIconDisplay>())
            return;

        EquipmentIconDisplay characterEquipment = eventData.pointerDrag.GetComponent<EquipmentIconDisplay>();

        CharacterEquipmentDroppedOnSlot?.Invoke(characterEquipment);    
    }

    #endregion
}
