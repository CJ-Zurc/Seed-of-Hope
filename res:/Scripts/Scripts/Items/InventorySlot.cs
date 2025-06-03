using Godot;
using System;

public class InventorySlot
{
    public ItemData Item { get; private set; }
    public int Quantity { get; private set; }
    public int SlotIndex { get; private set; }
    
    public bool IsEmpty => Item == null;
    public bool IsFull => Item != null && Quantity >= Item.MaxStackSize;
    
    public InventorySlot(int index)
    {
        SlotIndex = index;
        Clear();
    }
    
    public bool CanAddItem(ItemData item, int amount = 1)
    {
        if (IsEmpty) return true;
        if (Item.Name != item.Name) return false;
        return Quantity + amount <= Item.MaxStackSize;
    }
    
    public bool AddItem(ItemData item, int amount = 1)
    {
        if (amount <= 0) return false;
        
        if (IsEmpty)
        {
            Item = item;
            Quantity = amount;
            return true;
        }
        
        if (Item.Name == item.Name && Quantity + amount <= Item.MaxStackSize)
        {
            Quantity += amount;
            return true;
        }
        
        return false;
    }
    
    public bool RemoveItems(int amount)
    {
        if (IsEmpty || amount > Quantity) return false;
        
        Quantity -= amount;
        if (Quantity <= 0)
        {
            Clear();
        }
        return true;
    }
    
    public void Clear()
    {
        Item = null;
        Quantity = 0;
    }
    
    public void TransferTo(InventorySlot other, int amount)
    {
        if (IsEmpty || amount > Quantity) return;
        
        int transferAmount = Math.Min(amount, Quantity);
        if (other.AddItem(Item, transferAmount))
        {
            RemoveItems(transferAmount);
        }
    }
    
    public float GetDurability()
    {
        return Item?.Durability ?? 0f;
    }
    
    public void UseDurability(float amount)
    {
        if (Item != null && Item.Type == ItemType.Tool)
        {
            Item.Durability = Math.Max(0, Item.Durability - amount);
            if (Item.Durability <= 0)
            {
                Clear(); // Tool breaks
            }
        }
    }
} 