using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiDailyBonus : MonoBehaviour
{
    [SerializeField] private List<Day> m_ListDays;
    [SerializeField] private Button m_ButtonClose;
    private void OnEnable()
    {
        for (int i = 0; i < m_ListDays.Count; i++)
        {
            m_ListDays[i].m_TextDay.text = $"Day {i + 1}";
            m_ListDays[i].chipBonus = 100 * (i + 1);
            m_ListDays[i].m_TextChipBonus.text = $"{m_ListDays[i].chipBonus}";
        }
    }

    void Start()
    {
        m_ButtonClose.onClick.AddListener(ClickButtonClose);
    }
    void Update()
    {
        
    }

    private void ClickButtonClose()
    {
        gameObject.SetActive(false);
    }
}
