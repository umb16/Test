using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private ItemUI _itemUIPrefab;
    [SerializeField] private Transform _itemsRoot;
    private Inventory _inventory;
    private Dictionary<Item, ItemUI> _items = new Dictionary<Item, ItemUI>();
    private bool _isShop;
    private SpecialProperty<int> _gold;

    public void SetInventory(Inventory inventory, bool isShop, SpecialProperty<int> gold)
    {
        _isShop = isShop;
        _gold = gold;
        if (_inventory != null)
        {
            ClearUI();
            Unsubscribe(_inventory);
        }
        Subscribe(inventory);
        AddItemsUI(inventory);
        _inventory = inventory;
    }

    public void AddRealItem(Item item)
    {
        _inventory.Add(item);
    }
    public void RemoveRealItem(Item item)
    {
        _inventory.Remove(item);
    }
    private void OnBeginDrag(PointerEventData eventData, ItemUI itemUI)
    {
        itemUI.DragZone.SetParent(transform.parent, true);
    }
    private void OnDrag(PointerEventData eventData, ItemUI itemUI)
    {
        itemUI.DragZone.position = eventData.position;
    }
    private void OnEndDrag(PointerEventData eventData, ItemUI itemUI)
    {
        var result = eventData.pointerCurrentRaycast;

        var inventory = result.gameObject?.GetComponentInParent<InventoryUI>();

        itemUI.DragZone.SetParent(itemUI.transform);
        itemUI.DragZone.localPosition = Vector3.zero;
        /*Debug.Log(inventory.name);
        Debug.Log(result.gameObject.name);*/


        if (inventory != null && inventory != this)
        {
            inventory.AddRealItem(itemUI.Item);
            RemoveRealItem(itemUI.Item);
        }
    }

    private void ClearUI()
    {
        foreach (var item in _items)
        {
            Destroy(item.Value.gameObject);
        }
        _items.Clear();
    }
    private void Subscribe(Inventory inventory)
    {
        inventory.ItemRemoved += RemoveItemUI;
        inventory.ItemAdded += AddItemUI;
    }
    private void Unsubscribe(Inventory inventory)
    {
        inventory.ItemRemoved -= RemoveItemUI;
        inventory.ItemAdded -= AddItemUI;
    }

    private void AddItemsUI(Inventory inventory)
    {
        foreach (var item in inventory.GetItems())
        {
            AddItemUI(item);
        }
    }

    private void AddItemUI(Item item)
    {
        var itemUI = Instantiate(_itemUIPrefab, _itemsRoot);
        itemUI.BeginDrag += OnBeginDrag;
        itemUI.Drag += OnDrag;
        itemUI.EndDrag += OnEndDrag;
        itemUI.SetItem(item, _isShop, _gold);
        _items.Add(item, itemUI);
    }
    private void RemoveItemUI(Item item)
    {
        Destroy(_items[item].gameObject);
        _items.Remove(item);
    }
}
