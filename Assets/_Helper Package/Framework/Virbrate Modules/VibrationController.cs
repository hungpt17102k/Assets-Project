using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationController : MonoBehaviour
{
    private static VibrationController vibrationController;

    // Key for save load value
    private const string VIBRATE_SETTING = "Vibrate_Setting";

    [Header("Value of millisecond for vibration")]
    [Min(1)][Tooltip("This is vibration in milliseconds")]
    [SerializeField] private int shortVibration;

    [Min(1)][Tooltip("This is vibration in milliseconds")]
    [SerializeField] private int longVibration;

    private bool _vibrateState;

    //---------------------------Unity Functions----------------------------------
    private void Awake() {
        vibrationController = this;

        LoadVibrateSetting();
    }

    //-----------------------------Vibration Controller Functions--------------------------------
    private void SaveVibrateSetting()
    {
        PrefsSettings.SetBool(VIBRATE_SETTING, _vibrateState);
    }

    private void LoadVibrateSetting()
    {
        if(PlayerPrefs.HasKey(VIBRATE_SETTING))
        {
            _vibrateState = PrefsSettings.GetBool(VIBRATE_SETTING);
        }
        else
        {
            _vibrateState = true;
            PrefsSettings.SetBool(VIBRATE_SETTING, _vibrateState);
        }
    }
    
    public static void VibrateShort()
    {
        if(!vibrationController._vibrateState) return;

        Vibration.Vibrate(vibrationController.shortVibration);
    }

    public static void VibrateLong()
    {
        if(!vibrationController._vibrateState) return;

        Vibration.Vibrate(vibrationController.longVibration);
    }

    public static void TurnOnVibrate()
    {
        vibrationController._vibrateState = true;
        vibrationController.SaveVibrateSetting();
    }

    public static void TurnOffVibrate()
    {
        vibrationController._vibrateState = false;
        vibrationController.SaveVibrateSetting();
    }
}
