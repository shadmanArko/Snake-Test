using UnityEngine;

public class PlayerPrefsManager
{
    
    private static class PrefsKeys
    {
        public const string GLOBAL_MONEY = "GlobalMoney";
        public const string CURRENT_MINE = "CurrentMine";
    }

    private const float DEFAULT_MONEY = 500f;
    private const string DEFAULT_MINE = "mine01";

    
    public PlayerPrefsManager() 
    {
        InitializeIfNotExists();
    }
    

    private void InitializeIfNotExists()
    {
        if (!PlayerPrefs.HasKey(PrefsKeys.GLOBAL_MONEY))
        {
            PlayerPrefs.SetFloat(PrefsKeys.GLOBAL_MONEY, DEFAULT_MONEY);
        }

        if (!PlayerPrefs.HasKey(PrefsKeys.CURRENT_MINE))
        {
            PlayerPrefs.SetString(PrefsKeys.CURRENT_MINE, DEFAULT_MINE);
        }

        PlayerPrefs.Save();
    }

    public static float Money
    {
        get => PlayerPrefs.GetFloat(PrefsKeys.GLOBAL_MONEY);
        set => PlayerPrefs.SetFloat(PrefsKeys.GLOBAL_MONEY, value);
    }

    public static string CurrentMineId
    {
        get => PlayerPrefs.GetString(PrefsKeys.CURRENT_MINE);
        set => PlayerPrefs.SetString(PrefsKeys.CURRENT_MINE, value);
    }

    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
