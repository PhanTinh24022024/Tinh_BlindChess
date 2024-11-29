using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiSetting : MonoBehaviour
{
    [SerializeField] Button m_ButtonClose,
        m_ButtonSound,
        m_ButtonMusic,
        m_ButtonVibrate,
        m_ButtonRule,
        m_ButtonFaceBook,
        m_ButtonLogout;

    [SerializeField] Sprite m_OnSprite, m_OffSprite;

    private void OnEnable()
    {
        SoundManager.Instance.StartState();
        _StateSound();
        _StateMusic();
        _StateVibrate();
    }

    private void Start()
    {
        m_ButtonSound.onClick.AddListener(CLickButtonSound);
        m_ButtonMusic.onClick.AddListener(CLickButtonMusic);
        m_ButtonVibrate.onClick.AddListener(CLickButtonVibrate);
        m_ButtonClose.onClick.AddListener(ClickButtonClose);
    }

    private void CLickButtonSound()
    {
        GameData.StateSound = !GameData.StateSound;
        _StateSound();
    }

    private void CLickButtonMusic()
    {
        GameData.StateMusic = !GameData.StateMusic;
        _StateMusic();
    }

    private void CLickButtonVibrate()
    {
        GameData.StateVibrate = !GameData.StateVibrate;
        _StateVibrate();
    }

    private void _StateSound()
    {
        if (GameData.StateSound)
        {
            m_ButtonSound.image.sprite = m_OnSprite;
        }
        else
        {
            m_ButtonSound.image.sprite = m_OffSprite;
        }
    }

    private void _StateMusic()
    {
        if (GameData.StateMusic)
        {
            m_ButtonMusic.image.sprite = m_OnSprite;
        }
        else
        {
            m_ButtonMusic.image.sprite = m_OffSprite;
        }
    }

    private void _StateVibrate()
    {
        if (GameData.StateVibrate)
        {
            m_ButtonVibrate.image.sprite = m_OnSprite;
        }
        else
        {
            m_ButtonVibrate.image.sprite = m_OffSprite;
        }
    }

    private void ClickButtonClose()
    {
        gameObject.SetActive(false);
    }
}