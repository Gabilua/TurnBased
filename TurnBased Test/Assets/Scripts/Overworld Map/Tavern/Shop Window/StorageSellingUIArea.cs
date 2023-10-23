using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class StorageSellingUIArea : MonoBehaviour, IDropHandler
{
    public Action<StorageSlot> StorageSlotDroppedOnBuyingArea;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        if (!eventData.pointerDrag.GetComponent<StorageSlot>())
            return;

        StorageSlot storageSlot = eventData.pointerDrag.GetComponent<StorageSlot>();

        StorageSlotDroppedOnBuyingArea?.Invoke(storageSlot);
    }
}
