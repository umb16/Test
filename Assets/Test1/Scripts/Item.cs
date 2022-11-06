using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    private ItemTemplate _itemTemplate;

    public string Name => _itemTemplate.Name;
    public Sprite Icon => _itemTemplate.Icon;
    public int Cost => _itemTemplate.Cost;
    public int SellCost => _itemTemplate.Cost / 3;

    public Item(ItemTemplate itemTemplate)
    {
        _itemTemplate = itemTemplate;
    }
}
