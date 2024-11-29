using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDiamond : MonoBehaviour
{
    [SerializeField] private Image m_ImageSale;
    [SerializeField] private TextMeshProUGUI m_TextSale, m_TextDiscount, m_TextChip, m_TextPrice;
    [SerializeField] private Button m_ButtonBuy;
    void Start()
    {
        m_ButtonBuy.onClick.AddListener(ClickButtonBuy);
    }

    void Update()
    {
        
    }
    private void ClickButtonBuy()
    {
        Debug.LogError($"Click Button Buy Item Diamond");
    }
}
