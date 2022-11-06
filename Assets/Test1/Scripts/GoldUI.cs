using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _goldValueText;
    public void SetGold(SpecialProperty<int> gold)
    {
        _goldValueText.text = gold.Value.ToString();
        gold.Changed += GoldChanged;
    }

    private void GoldChanged(int value)
    {
        _goldValueText.text = value.ToString();
    }
}
