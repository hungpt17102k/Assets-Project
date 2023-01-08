using UnityEngine;
using System;

public static class PrefsSettings
{

    //---------------------------Manager Methods--------------------------
    public static void ClearAll()
    {
        PlayerPrefs.DeleteAll();
    }

    //-----------------------------Set Methods----------------------------
    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetString(key, value.ToString());
    }

    public static void SetInt(string key, int value) 
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public static void SetLong(string key, long value)
    {
        PlayerPrefs.SetString(key, value.ToString());
    }

    public static void SetDouble(string key, double value)
    {
        PlayerPrefs.SetString(key, value.ToString());
    }

    //-----------------------------Get Methods----------------------------
    public static bool GetBool(string key)
    {
        bool value = Convert.ToBoolean(PlayerPrefs.GetString(key));

        return value;
    }

    public static int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public static float GetFloat(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }

    public static long GetLong(string key)
    {
        long value = long.Parse(PlayerPrefs.GetString(key));

        return value;
    }

    public static double GetDouble(string key)
    {
        double value = Convert.ToDouble(PlayerPrefs.GetString(key));

        return value;
    }
}
