using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Inventory
{
    public event Action<Item> ItemAdded;
    public event Action<Item> ItemRemoved;
    private List<Item> _items = new List<Item>();
    public void Add(Item item)
    {
            _items.Add(item);
            ItemAdded?.Invoke(item);
    }
    public void Remove(Item item)
    {
        _items.Remove(item);
        ItemRemoved?.Invoke(item);
    }

    public ReadOnlyCollection<Item> GetItems()
    {
        return new ReadOnlyCollection<Item>(_items);
    }
}
