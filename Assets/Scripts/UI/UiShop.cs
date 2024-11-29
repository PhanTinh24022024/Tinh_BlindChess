using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShop : MonoBehaviour
{
    [SerializeField] private Button m_ButtonBack, m_ButtonChip, m_ButtonDiamond, m_ButtonChessBoard, m_ButtonChess;
    [SerializeField] private GameObject m_TotalItemChip, m_TotalItemDiamond;
    [SerializeField] private Sprite m_TabChip,m_TabDiamond;
    [SerializeField] private Image m_Tab;

    private void OnEnable()
    {
        if (BlindChessController.instance.isClickButtonIncreaseChip)
        {
            ClickButtonChip();
        }
        else if (BlindChessController.instance.isClickButtonIncreaseDiamond)
        {
            ClickButtonDiamond();
        }
        else
        {
            ClickButtonChip();
        }
    }

    void Start()
    {
        m_ButtonChip.onClick.AddListener(ClickButtonChip);
        m_ButtonDiamond.onClick.AddListener(ClickButtonDiamond);
        m_ButtonBack.onClick.AddListener(ClickButtonBack);
    }

    void Update()
    {
    }

    private void ClickButtonChip()
    {
        m_Tab.sprite = m_TabChip;
        m_TotalItemChip.SetActive(true);
        m_TotalItemDiamond.SetActive(false);
    }
    private void ClickButtonDiamond()
    {
        m_Tab.sprite = m_TabDiamond;
        m_TotalItemChip.SetActive(false);
        m_TotalItemDiamond.SetActive(true);
    }

    private void ClickButtonBack()
    {
        gameObject.SetActive(false);
        BlindChessController.instance.UiHome.SetActive(true);
        BlindChessController.instance.isClickButtonIncreaseChip = false;
        BlindChessController.instance.isClickButtonIncreaseDiamond = false;
    }
}