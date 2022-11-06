using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Transform _dragZone;
    private bool _isShop;
    private SpecialProperty<int> _gold;

    public event Action<PointerEventData, ItemUI> BeginDrag;
    public event Action<PointerEventData, ItemUI> Drag;
    public event Action<PointerEventData, ItemUI> EndDrag;

    public Item Item { get; private set; }
    public Transform DragZone => _dragZone;

    public void SetItem(Item item, bool isShop, SpecialProperty<int> gold = null)
    {
        Item = item;
        _image.sprite = item.Icon;
        _nameText.text = item.Name;
        _isShop = isShop;
        _gold = gold;
        if (!isShop)
        {
            _costText.text = Item.SellCost.ToString();
        }
        else
        {
            SetCostInShop(gold.Value);
            gold.Changed += SetCostInShop;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isShop && _gold.Value < Item.Cost)
            return;
        BeginDrag?.Invoke(eventData, this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isShop && _gold.Value < Item.Cost)
            return;
        Drag?.Invoke(eventData, this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isShop && _gold.Value < Item.Cost)
            return;
        EndDrag?.Invoke(eventData, this);
    }

    private void SetCostInShop(int currentGold)
    {
        int buyCost = Item.Cost;
        if (buyCost > currentGold)
            _costText.text = "<color=red>" + buyCost + "</color>";
        else
            _costText.text = Item.Cost.ToString();
    }

    private void OnDestroy()
    {
        if (_isShop)
            _gold.Changed -= SetCostInShop;
    }
}
