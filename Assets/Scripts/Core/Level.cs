using System;
using Chess.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public int id = 0;
    [SerializeField] private GameManager m_gameManager;
    public Button m_buttonLevel;
    public TextMeshProUGUI m_textLevel;
    public Image m_imageLock;

    private void Start()
    {
        m_buttonLevel.onClick.AddListener(ClickButtonLevel);
    }

    private void ClickButtonLevel()
    {
        GameData.levelChoosing = id+1;
        if (id > GameData.CurrentLevel) return;
        if (id > 5)
        {
            m_gameManager.LoadLevelHard(20 - (2 * id));
            BlindChessController.instance.PlayOffline();
        }
        else
        {
            m_gameManager.LoadLevelNormal(20 - (2 * id));
            BlindChessController.instance.PlayOffline();
        }
    }
}