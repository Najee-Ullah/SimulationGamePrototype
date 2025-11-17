using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int inventorySlotsAllowed = 4;

    public event EventHandler<OnItemsChangedArgs> OnItemAdded;
    public event EventHandler<OnItemsChangedArgs> OnItemHold;
    public event EventHandler<OnItemsChangedArgs> OnItemRemoved;
    public event EventHandler<OnItemsReplacedArgs> OnItemsReplaced;
    public class OnItemsChangedArgs : EventArgs
    {
        public ItemDataSO changedItem;
    }
    public class OnItemsReplacedArgs : EventArgs
    {
        public ItemDataSO replacingItem;
        public ItemDataSO replacedItem1;
        public ItemDataSO replacedItem2;
    }
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();

    private int itemIdNo = 0;

    public void AddItem(ItemDataSO item)
    {
            if (inventorySlots.Count < inventorySlotsAllowed)
            {
                item.itemId = itemIdNo++;
                inventorySlots.Add(new InventorySlot(item));
                OnItemAdded?.Invoke(this,new OnItemsChangedArgs {changedItem = item });
            }
    }

    public void RemoveItem(ItemDataSO item)
    {
        InventorySlot existingSlot = inventorySlots.Find(x => x.itemData == item);
        if (existingSlot != null)
        {
            inventorySlots.Remove(existingSlot);
            OnItemRemoved?.Invoke(this, new OnItemsChangedArgs { changedItem = item });
        }

    }

    public void ReplaceItem(ItemDataSO itemDataA, ItemDataSO itemDataB,ItemDataSO itemDataC)
    {
        InventorySlot existingSlotA = inventorySlots.Find(x => x.itemData == itemDataA);
        InventorySlot existingSlotB = inventorySlots.Find(x => x.itemData == itemDataB);
        if (existingSlotA != null && existingSlotB != null)
        {
            inventorySlots.Remove(existingSlotA);
            inventorySlots.Remove(existingSlotB);
            inventorySlots.Add(new InventorySlot(itemDataC));
            OnItemsReplaced.Invoke(this, new OnItemsReplacedArgs { replacedItem1 = itemDataA,replacedItem2 = itemDataB,replacingItem = itemDataC });
        }

    }

    public bool HasItem(ItemDataSO item)
    {
        InventorySlot existingSlot = inventorySlots.Find(x => x.itemData == item);
        return existingSlot != null;
    }

    public int GetInventorySlotsAmount()
    {
        return inventorySlotsAllowed;
    }

    public void OnHoldClicked(ItemDataSO item)
    {
        OnItemHold?.Invoke(this, new OnItemsChangedArgs { changedItem = item });
    }

    public void OnDropClicked(ItemDataSO item)
    {
        RemoveItem(item);
    }
    public void OnCombineClicked(ItemDataSO itemA)
    {
        if (ItemCombineManager.Instance.IsActive())
        {
            ItemDataSO itemB = ItemCombineManager.Instance.GetFirstItem();
            UpdateItemDataSO(itemA,itemB);
        }
        else
        {
            ItemCombineManager.Instance.CombineStart(itemA);
        }
    }
    private void UpdateItemDataSO(ItemDataSO itemA,ItemDataSO itemB)
    {
        ReplaceItem(itemA,itemB, ItemCombineManager.Instance.CombineEnd(itemA));
    }
}