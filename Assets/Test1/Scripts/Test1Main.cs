using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1Main : MonoBehaviour
{
    [SerializeField] private InventoryUI _playerInventoryUI;
    [SerializeField] private InventoryUI _shopInventoryUI;
    [SerializeField] private GoldUI _goldUI;
    [Header("Денег у игрока")]
    [SerializeField] private int _gold;
    [Header("Предметы игрока")]
    [SerializeField] private ItemTemplate[] _playerItems;
    [Header("Предметы в магазине")]
    [SerializeField] private ItemTemplate[] _shopItems;
    private SpecialProperty<int> _playerGold;

    private void Awake()
    {
        _playerGold = new SpecialProperty<int>(_gold);
        _goldUI.SetGold(_playerGold);
        var playerInventory = CreateInventory(_playerItems, _playerInventoryUI, false, null);
        var shopInventory = CreateInventory(_shopItems, _shopInventoryUI, true, _playerGold);
        shopInventory.ItemRemoved += ShopInventoryItemRemoved;
        shopInventory.ItemAdded += ShopInventoryItemAdded;
    }

    private void ShopInventoryItemAdded(Item item)
    {
        _playerGold.Value += item.SellCost;
    }

    private void ShopInventoryItemRemoved(Item item)
    {
        _playerGold.Value -= item.Cost;
    }

    private Inventory CreateInventory(IEnumerable<ItemTemplate> itemTemplates, InventoryUI inventoryUI,
        bool isShop, SpecialProperty<int> gold)
    {
        var inventory = new Inventory();
        foreach (var item in itemTemplates)
        {
            inventory.Add(new Item(item));
        }
        inventoryUI.SetInventory(inventory, isShop, gold);
        return inventory;
    }
}
