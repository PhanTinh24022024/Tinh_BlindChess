using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Day : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI m_TextDay, m_TextChipBonus, m_TextInButton;
    [SerializeField] private Button m_ButtonClaim;
    public int chipBonus;
    void Start()
    {
        m_ButtonClaim.onClick.AddListener(ClickButtonClaim);
    }

    void Update()
    {
        
    }

    private void ClickButtonClaim()
    {
        Debug.LogError($"Click Button Claim");
        BlindChessController.instance.ClaimAndShowMoneyWithFx(BlindChessController.instance.totalChip,chipBonus,m_ButtonClaim);
    }
    
    
}
