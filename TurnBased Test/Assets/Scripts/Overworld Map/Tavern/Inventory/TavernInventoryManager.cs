using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class TavernInventoryManager : MonoBehaviour
{
    public Action<bool> StorageWindowStateChange;
    public Action<EquipmentIconDisplay> OnCharacterEquipmentDroppedOnSlot;

    [SerializeField] Image _chestIcon;
    [SerializeField] Animator _chestIconAnimator;
    [SerializeField] Sprite[] _chestSprites;

    [SerializeField] GameObject _storageWindow;

   [SerializeField] List<TavernInventorySlot> _allStorageSlots = new List<TavernInventorySlot>();

    [SerializeField] Image _draggedSlotGFX;

    GameManager _gameManager;

    bool isStorageOpen;

    #region Storage Window Management

    public bool IsStorageOpen()
    {
        return isStorageOpen;
    }

    public void ChestIconClicked()
    {
        ToggleStorageWindow(!isStorageOpen);
    }

    void ToggleStorageWindow(bool state)
    {
        isStorageOpen = state;

        _storageWindow.SetActive(isStorageOpen);

        _chestIcon.sprite = _chestSprites[Convert.ToInt32(isStorageOpen)];

        _chestIconAnimator.SetTrigger("Action");

        StorageWindowStateChange?.Invoke(isStorageOpen);
    }

    public void SetupStorageWindow()
    {
        _gameManager = GameManager.instance;

        foreach (var storageSlot in _allStorageSlots)
        {
            storageSlot.InventorySlotDragStart += SlotStartDrag;
            storageSlot.InventorySlotDragEnd += SlotEndDrag;
            storageSlot.InventorySlotDragging += SlotDragging;
            storageSlot.CharacterEquipmentDroppedOnSlot += CharacterEquipmentDroppedOnSlot;
        }
    }

    public void UpdateStorageWindow(RecruitedCharacter selectedCharacter)
    {
        ResetStorageWindow();

        foreach (var slot in _allStorageSlots)
            slot.SetupInventorySlotInfo(selectedCharacter);

        for (int i = 0; i < _gameManager.GetCurrentPlayerInventory().Count; i++)
        {
            if(_gameManager.GetCurrentPlayerInventory()[i] != null)
            _allStorageSlots[i].SetupInventorySlotEquipment(_gameManager.GetCurrentPlayerInventory()[i]);
        }
    }

    void ResetStorageWindow()
    {
        foreach (var storageSlot in _allStorageSlots)
            storageSlot.ResetInventorySlot();
    }

    #endregion

    #region Storage Slot & Character Equipment Drag Events

    public void UpdateDraggedGFX(bool state, Sprite icon = null)
    {
        if (!isStorageOpen)
            return;

        _draggedSlotGFX.gameObject.SetActive(state);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        _draggedSlotGFX.transform.position = new Vector3(mousePos.x, mousePos.y, _draggedSlotGFX.transform.position.z);

        _draggedSlotGFX.sprite = icon;
    }

    void SlotStartDrag(TavernInventorySlot slot)
    {      
        UpdateDraggedGFX(true);
    }

    void SlotEndDrag(TavernInventorySlot slot)
    {
        UpdateDraggedGFX(false);
    }

    void SlotDragging(TavernInventorySlot slot)
    { 
        UpdateDraggedGFX(true, slot._equipmentInfo.equipmentIcon);
    }

    public void CharacterEquipmentStartDrag(EquipmentIconDisplay equipment)
    {
        UpdateDraggedGFX(true);
    }

    public void CharacterEquipmentEndDrag(EquipmentIconDisplay equipment)
    {
        UpdateDraggedGFX(false);
    }

    public void CharacterEquipmentDragging(EquipmentIconDisplay equipment)
    {
        UpdateDraggedGFX(true, equipment._equipmentInfo.equipmentIcon);
    }

    void CharacterEquipmentDroppedOnSlot(EquipmentIconDisplay equipment)
    {
        OnCharacterEquipmentDroppedOnSlot?.Invoke(equipment);
    }

    public bool IsStorageFull()
    {
        bool result = true;

        foreach (var slot in _allStorageSlots)
        {
            if (slot.IsSlotOccupied())
            {
                result = false;
                break;
            }
        }

        return result;
    }

    #endregion   
}
