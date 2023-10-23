using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class StorageManager : MonoBehaviour
{
    public Action<bool> StorageWindowStateChange;
    public Action<EquipmentIconDisplay> OnCharacterEquipmentDroppedOnSlot;
    public Action<ShopStockSlot> OnShopStockDroppedOnSlot;

    [SerializeField] TavernManager _tavernManager;

    [SerializeField] Image _chestIcon;
    [SerializeField] Animator _chestIconAnimator;
    [SerializeField] Sprite[] _chestSprites;

    [SerializeField] GameObject _storageWindow;
    [SerializeField] TextMeshProUGUI _playerMoneyDisplay;
    [SerializeField] Animator _playerMoneyDisplayAnimator;

   [SerializeField] List<StorageSlot> _allStorageSlots = new List<StorageSlot>();

    GameManager _gameManager;

    bool isStorageOpen;

    private void Awake()
    {
        ToggleStorageWindow(false);
    }

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
            storageSlot.StorageSlotDragStart += SlotStartDrag;
            storageSlot.StorageSlotDragEnd += SlotEndDrag;
            storageSlot.StorageSlotDragging += SlotDragging;
            storageSlot.CharacterEquipmentDroppedOnSlot += CharacterEquipmentDroppedOnSlot;
            storageSlot.ShopStockDroppedOnSlot += ShopStockDroppedOnSlot;
        }
    }

    public void UpdateStorageWindow(RecruitedCharacter selectedCharacter)
    {
        ResetStorageWindow();

        foreach (var slot in _allStorageSlots)
            slot.SetupStorageSlotInfo(selectedCharacter);

        for (int i = 0; i < _gameManager.GetCurrentPlayerStorage().Count; i++)
        {
            if(_gameManager.GetCurrentPlayerStorage()[i] != null)
            _allStorageSlots[i].SetupStorageSlotEquipment(_gameManager.GetCurrentPlayerStorage()[i]);
        }

        if (GameManager.instance.GetCurrentPlayerMoney().ToString() != _playerMoneyDisplay.text)
            _playerMoneyDisplayAnimator.SetTrigger("Action");

        _playerMoneyDisplay.text = GameManager.instance.GetCurrentPlayerMoney().ToString();
    }

    public void ToggleSellingValueOnStorage(bool state)
    {
        foreach (var storageSlot in _allStorageSlots)
            storageSlot.ToggleSellingPrice(state);
    }

    void ResetStorageWindow()
    {
        foreach (var storageSlot in _allStorageSlots)
            storageSlot.ResetStorageSlot();
    }

    #endregion

    #region Storage Slot & Character Equipment Drag Events 

    void SlotStartDrag(StorageSlot slot)
    {
        _tavernManager.UpdateDraggedGFX(true);
    }

    void SlotEndDrag(StorageSlot slot)
    {
        _tavernManager.UpdateDraggedGFX(false);
    }

    void SlotDragging(StorageSlot slot)
    {
        _tavernManager.UpdateDraggedGFX(true, slot._equipmentInfo.equipmentIcon);
    }

    public void CharacterEquipmentStartDrag(EquipmentIconDisplay equipment)
    {
        _tavernManager.UpdateDraggedGFX(true);
    }

    public void CharacterEquipmentEndDrag(EquipmentIconDisplay equipment)
    {
        _tavernManager.UpdateDraggedGFX(false);
    }

    public void CharacterEquipmentDragging(EquipmentIconDisplay equipment)
    {
        _tavernManager.UpdateDraggedGFX(true, equipment._equipmentInfo.equipmentIcon);
    }

    void CharacterEquipmentDroppedOnSlot(EquipmentIconDisplay equipment)
    {
        OnCharacterEquipmentDroppedOnSlot?.Invoke(equipment);
    }

    void ShopStockDroppedOnSlot(ShopStockSlot shopStock)
    {
        OnShopStockDroppedOnSlot?.Invoke(shopStock);
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
