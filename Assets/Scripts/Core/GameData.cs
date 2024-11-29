using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public static int levelChoosing = 0;
    public static int CountUnlockNextLevel
    {
        get => PlayerPrefs.GetInt(StringHelper.CountUnlockNextLevel, 3);
        set => PlayerPrefs.SetInt(StringHelper.CountUnlockNextLevel, value);
    }

    public static int CurrentLevel
    {
        get => PlayerPrefs.GetInt(StringHelper.CurrentLevel, 1);
        set => PlayerPrefs.SetInt(StringHelper.CurrentLevel, value);
    }

    public static bool StateSound
    {
        get => GetBool(StringHelper.StateSound, true);
        set => SetBool(StringHelper.StateSound, value);
    }

    public static bool StateMusic
    {
        get => GetBool(StringHelper.StateMusic, true);
        set => SetBool(StringHelper.StateMusic, value);
    }

    public static bool StateVibrate
    {
        get => GetBool(StringHelper.StateVibration, true);
        set => SetBool(StringHelper.StateVibration, value);
    }

    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool GetBool(string key, bool defaultValue = true)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }

    public struct StringHelper
    {
        public const string StateSound = "StateSound";
        public const string StateMusic = "StateMusic";
        public const string StateVibration = "StateVibration";
        public const string CurrentLevel = "CurrentLevel";
        public const string CountUnlockNextLevel = "CountUnlockNextLevel";
    }
}