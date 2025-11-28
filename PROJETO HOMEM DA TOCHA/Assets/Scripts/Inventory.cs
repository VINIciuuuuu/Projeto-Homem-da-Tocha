using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public ItemData item;
    public int count;
}

public class Inventory : MonoBehaviour
{
    [Header("Configuração")]
    [Min(1)] public int capacity = 20;

    [Header("Estado (debug/inspeção)")]
    public List<InventorySlot> slots = new List<InventorySlot>();

    public event Action OnInventoryChanged;

    public int AddItem(ItemData data, int amount)
    {
        if (data == null || amount <= 0) return amount;

        int remaining = amount;

        if (data.stackable)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].item == data && slots[i].count < data.maxStack)
                {
                    int canFill = data.maxStack - slots[i].count;
                    int toAdd = Mathf.Min(canFill, remaining);
                    slots[i].count += toAdd;
                    remaining -= toAdd;
                    if (remaining <= 0) { OnInventoryChanged?.Invoke(); return 0; }
                }
            }
        }


        while (remaining > 0 && slots.Count < capacity)
        {
            int toAdd = data.stackable ? Mathf.Min(data.maxStack, remaining) : 1;
            slots.Add(new InventorySlot { item = data, count = toAdd });
            remaining -= toAdd;

            if (!data.stackable) break;
        }

        OnInventoryChanged?.Invoke();
        return remaining;
    }

    public bool RemoveItem(ItemData data, int amount)
    {
        if (data == null || amount <= 0) return false;

        int need = amount;


        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == data)
            {
                int take = Mathf.Min(slots[i].count, need);
                slots[i].count -= take;
                need -= take;

                if (slots[i].count <= 0)
                {
                    slots.RemoveAt(i);
                    i--;
                }

                if (need <= 0) { OnInventoryChanged?.Invoke(); return true; }
            }
        }

        OnInventoryChanged?.Invoke();
        return false;
    }

    public int GetCount(ItemData data)
    {
        int total = 0;
        foreach (var s in slots) if (s.item == data) total += s.count;
        return total;
    }
}


