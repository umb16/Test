using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemTemplate", menuName = "ItemTemplate", order = 1)]
public class ItemTemplate :  ScriptableObject
{
    [SerializeField] public string _name;
    [SerializeField] private Sprite icon;
    [SerializeField] public int _cost;

    public string Name => _name;
    public Sprite Icon => icon;
    public int Cost => _cost;

}
