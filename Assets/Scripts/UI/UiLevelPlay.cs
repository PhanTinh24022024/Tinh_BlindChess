using System;
using System.Collections;
using System.Collections.Generic;
using Chess.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiLevelPlay : MonoBehaviour
{
    [SerializeField] private List<Level> m_listLevel;
    [SerializeField] private Sprite m_lockedSprite, m_unlockedSprite;
    [SerializeField] private Button m_buttonPlayAgain;
    [SerializeField] private TextMeshProUGUI m_textDescription;
    [SerializeField] private GameManager m_GameManager;

    private void OnEnable()
    {
        // Debug.LogError($"Current Level: {GameData.CurrentLevel}");
        m_textDescription.text =
            $"You need win {GameData.CountUnlockNextLevel} times in level {GameData.CurrentLevel} to unlock the next level";
        for (int i = 0; i < m_listLevel.Count; i++)
        {
            m_listLevel[i].id = i;
            m_listLevel[i].m_textLevel.text = $"Level {i + 1}";
            if (i < GameData.CurrentLevel)
            {
                m_listLevel[i].m_buttonLevel.image.sprite = m_unlockedSprite;
                m_listLevel[i].m_imageLock.gameObject.SetActive(false);
                m_listLevel[i].m_buttonLevel.interactable = true;
            }
            else
            {
                m_listLevel[i].m_buttonLevel.image.sprite = m_lockedSprite;
                m_listLevel[i].m_imageLock.gameObject.SetActive(true);
                m_listLevel[i].m_buttonLevel.interactable = false;
            }
        }
    }

    private void Start()
    {
        m_buttonPlayAgain.onClick.AddListener(ClickButtonPlayAgain);
    }

    private void ClickButtonPlayAgain()
    {
        GameData.levelChoosing = GameData.CurrentLevel;
        if (GameData.CurrentLevel > 6)
        {
            m_GameManager.LoadLevelHard(20 - (2 * GameData.CurrentLevel));
            BlindChessController.instance.PlayOffline();
        }
        else
        {
            m_GameManager.LoadLevelNormal(20 - (2 * GameData.CurrentLevel));
            BlindChessController.instance.PlayOffline();
        }
    }
}