using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class StorageSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public Action<StorageSlot> StorageSlotDragging;
    public Action<StorageSlot> StorageSlotDragStart;
    public Action<StorageSlot> StorageSlotDragEnd;
    public Action<EquipmentIconDisplay> CharacterEquipmentDroppedOnSlot;
    public Action<ShopStockSlot> ShopStockDroppedOnSlot;

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


    public void SetupStorageSlotEquipment(EquipmentInfo equipment)
    {
        _equipmentInfo = equipment;

        _equipmentIcon.sprite = _equipmentInfo.equipmentIcon;

        _sellingPriceDisplay.text = _equipmentInfo.equipmentValue.ToString();

        if (_currentlySelectedCharacter == null)
            return;

        if (EquipmentUsableByCurrentlySelectedCharacter())
            _equipmentIcon.color = _availabilityColors[1];
        else
            _equipmentIcon.color = _availabilityColors[0];
    }

    public void ToggleSellingPrice(bool state)
    {
        if (!IsSlotOccupied())
            return;

        _sellingPriceDisplay.gameObject.SetActive(state);
    }

    public void SetupStorageSlotInfo(RecruitedCharacter currentlySelectedCharacter)
    {
        _currentlySelectedCharacter = currentlySelectedCharacter;
    }

    public void ResetStorageSlot()
    {
        _equipmentInfo = null;
        _equipmentIcon.sprite = _emptySprite;
        _sellingPriceDisplay.text = string.Empty;
        _sellingPriceDisplay.gameObject.SetActive(false);
    }

    public bool EquipmentUsableByCurrentlySelectedCharacter()
    {
        bool result = true;

        if (_currentlySelectedCharacter != null && !_equipmentInfo.allowedUsers.Contains(_currentlySelectedCharacter.characterInfo))
            result = false;

        return result;
    }

    #region Events

    public void OnDrag(PointerEventData eventData)
    {
        if (_equipmentInfo == null)
            return;

        if (!EquipmentUsableByCurrentlySelectedCharacter())
            return;

        StorageSlotDragging?.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_equipmentInfo == null)
            return;

        if (!EquipmentUsableByCurrentlySelectedCharacter())
            return;

        _equipmentIcon.gameObject.SetActive(false);

        StorageSlotDragStart?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_equipmentInfo == null)
            return;

        if (!EquipmentUsableByCurrentlySelectedCharacter())
            return;

        _equipmentIcon.gameObject.SetActive(true);

        StorageSlotDragEnd?.Invoke(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        if (eventData.pointerDrag.GetComponent<EquipmentIconDisplay>())
        {
            EquipmentIconDisplay characterEquipment = eventData.pointerDrag.GetComponent<EquipmentIconDisplay>();

            CharacterEquipmentDroppedOnSlot?.Invoke(characterEquipment);
        }

        if (eventData.pointerDrag.GetComponent<ShopStockSlot>())
        {
            ShopStockSlot shopStock = eventData.pointerDrag.GetComponent<ShopStockSlot>();

            ShopStockDroppedOnSlot?.Invoke(shopStock);
        }
    }

    #endregion
}
