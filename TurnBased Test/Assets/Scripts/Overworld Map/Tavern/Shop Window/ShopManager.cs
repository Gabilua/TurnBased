using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[System.Serializable]
public class GeneratedShopStocksByTown
{
    public StageInfo tavernTown;
    public List<EquipmentInfo> shopStock = new List<EquipmentInfo>();

    public void SetupStock(StageInfo town, List<EquipmentInfo> stock)
    {
        tavernTown = town;
        shopStock.AddRange(stock);
    }

    public void DepleteStock(EquipmentInfo soldItem)
    {
        shopStock.Remove(soldItem);
    }
}
public class ShopManager : MonoBehaviour
{
    [SerializeField] TavernManager _tavernManager;

    [SerializeField] TextMeshProUGUI _pieuvreCommentDisplay;
    [TextArea]
    [SerializeField] string[] _pieuvreCommentList;

    public Action<bool> ShopWindowStateChanged;

    public Action<ShopStockSlot> ShopSlotDragging;
    public Action<ShopStockSlot> ShopSlotDragStart;
    public Action<ShopStockSlot> ShopSlotDragEnd;
    public Action<StorageSlot> OnStorageSlotDroppedOnBuyingArea;

    [SerializeField] GameObject _shopWindow;
    [SerializeField] StorageSellingUIArea _sellingArea;

    [SerializeField] GameObject _shopStockSlotDisplayPrefab;
    [SerializeField] Transform _shopStockSlotDisplayHolder;
    [SerializeField] TextMeshProUGUI _equipmentName;
    [SerializeField] TextMeshProUGUI _equipmentDescription;

    ShopStockSlot _selectedEquipment;

    List<GeneratedShopStocksByTown> _generatedShopStocks = new List<GeneratedShopStocksByTown>();
    List<ShopStockSlot> _activeShopStockSlots = new List<ShopStockSlot>();

    bool FirstTimeThisSessionVisitingThisTavern(StageInfo tavernTown)
    {
        bool result = true;

        foreach (var shopStock in _generatedShopStocks)
        {
            if (shopStock.tavernTown == tavernTown)
            {
                result = false;
                break;
            }
        }

        return result;
    }

    bool isShopOpen;

    private void Awake()
    {
        _sellingArea.StorageSlotDroppedOnBuyingArea += StorageSlotDroppedOnBuyingArea;

        ToggleShopWindow(false);
    }

    public bool IsShopOpen()
    {
        return isShopOpen;
    }

    public void NPCIconClicked()
    {
        ToggleShopWindow(!isShopOpen);
    }

    void ToggleShopWindow(bool state)
    {
        isShopOpen = state;

        _shopWindow.SetActive(isShopOpen);

        if (isShopOpen)
            UpdateShopWindow();
        else
            ResetStockDisplays();

        ShopWindowStateChanged?.Invoke(isShopOpen);
    }

    void ResetStockDisplays()
    {
        foreach (var icon in _activeShopStockSlots)
        {
            icon.ShopSlotClicked -= ShopStockSlotClicked;

            icon.ShopSlotDragStart -= ShopStockSlotStartDrag;
            icon.ShopSlotDragEnd -= ShopStockSlotEndDrag;
            icon.ShopSlotDragging -= ShopStockSlotDragging;
        }

        _activeShopStockSlots.Clear();

        foreach (Transform child in _shopStockSlotDisplayHolder)
            Destroy(child.gameObject);
    }

    void SetupStockDisplays(List<EquipmentInfo> townStock)
    {
        if(townStock.Count > 0)
        {
            for (int i = 0; i < townStock.Count; i++)
            {
                if (i == 7)
                    break;

                ShopStockSlot display = Instantiate(_shopStockSlotDisplayPrefab, _shopStockSlotDisplayHolder).GetComponent<ShopStockSlot>();

                display.SetupShopStockSlot(townStock[i]);

                display.ShopSlotClicked += ShopStockSlotClicked;

                display.ShopSlotDragStart += ShopSlotDragStart;
                display.ShopSlotDragEnd += ShopStockSlotEndDrag;
                display.ShopSlotDragging += ShopSlotDragging;

                _activeShopStockSlots.Add(display);
            }
        }       

        DeselectStockSlot();
        UpdateStockSlotInfo();
    }

    List<EquipmentInfo> GetShopStockForThisTavern(StageInfo tavern)
    {
        List<EquipmentInfo> stock = new List<EquipmentInfo>();

        foreach (var generatedStock in _generatedShopStocks)
        {
            if (generatedStock.tavernTown == tavern)
            {
                stock.AddRange(generatedStock.shopStock);
                break;
            }
        }

        return stock;
    }

    void DepleteFromThisTavernShopStock(EquipmentInfo equipment)
    {
        GeneratedShopStocksByTown stockToDeplete = null;

        foreach (var generatedStock in _generatedShopStocks)
        {
            if (generatedStock.shopStock.Contains(equipment))
            {
                stockToDeplete = generatedStock;
                break;
            }
        }

        stockToDeplete.shopStock.Remove(equipment);
    }

    public void UpdateShopWindow()
    {
        ResetStockDisplays();

        if (FirstTimeThisSessionVisitingThisTavern(_tavernManager._currentTown))
        {
            GeneratedShopStocksByTown newStock = new GeneratedShopStocksByTown();

            newStock.SetupStock(_tavernManager._currentTown, _tavernManager._currentTown.stageEquipmentRewards);

            _generatedShopStocks.Add(newStock);
        }

        SetupStockDisplays(GetShopStockForThisTavern(_tavernManager._currentTown));
    }

    void SelectStockSlot(ShopStockSlot equipment)
    {
        _selectedEquipment = equipment;

        _selectedEquipment.ToggleSelectionBracket(true);
    }

    void DeselectStockSlot()
    {
        if (_selectedEquipment == null)
            return;

        _selectedEquipment.ToggleSelectionBracket(false);
        _selectedEquipment = null;
    }

    void UpdateStockSlotInfo()
    {
        if (_selectedEquipment == null)
        {
            _equipmentName.text = string.Empty;
            _equipmentDescription.text = string.Empty;
        }
        else
        {
            _equipmentName.text = _selectedEquipment._equipmentInfo.name;
            _equipmentDescription.text =
                _selectedEquipment._equipmentInfo.equipmentDescription +
                "\n Grants the skill: \n\n"
                + _selectedEquipment._equipmentInfo.awardedSkill.name + "\n\n"
                + _selectedEquipment._equipmentInfo.awardedSkill.skillDescription;
        }
    }

    void ShopStockSlotClicked(ShopStockSlot selectedStock)
    {
        DeselectStockSlot();

        if (selectedStock._equipmentInfo != null)
            SelectStockSlot(selectedStock);

        UpdateStockSlotInfo();
    }

    public void RemoveStockFromSale(ShopStockSlot soldStock)
    {
        if (!_activeShopStockSlots.Contains(soldStock))
            return;

        DepleteFromThisTavernShopStock(soldStock._equipmentInfo);

        soldStock.ShopSlotClicked -= ShopStockSlotClicked;

        soldStock.ShopSlotDragStart -= ShopStockSlotStartDrag;
        soldStock.ShopSlotDragEnd -= ShopStockSlotEndDrag;
        soldStock.ShopSlotDragging -= ShopStockSlotDragging;

        _activeShopStockSlots.Remove(soldStock);

        Destroy(soldStock.gameObject);
    }

    #region Equipment Drag Events

    void ShopStockSlotStartDrag(ShopStockSlot equipment)
    {
        ShopSlotDragStart?.Invoke(equipment);
    }

    void ShopStockSlotEndDrag(ShopStockSlot equipment)
    {
        ShopSlotDragEnd?.Invoke(equipment);
    }

    void ShopStockSlotDragging(ShopStockSlot equipment)
    {
        ShopSlotDragging?.Invoke(equipment);
    }

    void StorageSlotDroppedOnBuyingArea(StorageSlot slot)
    {
        OnStorageSlotDroppedOnBuyingArea?.Invoke(slot);
    }

    #endregion
}
