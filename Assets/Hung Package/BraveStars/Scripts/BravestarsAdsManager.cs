using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if ADS
using AppsFlyerSDK;
#endif

#if ADS
[RequireComponent(typeof(ApplovinManager))]
#endif
public class BravestarsAdsManager : MonoBehaviour
{
    public static BravestarsAdsManager Instance;

    [HideInInspector]
    public bool isAdEnabled;


    private float _timeBtwShowAdInter = 25f;

    private bool _hasAdsResume = false;

    private const string FIRST_TIME_USER_PLAYED = "First_Time_Play";
    private bool _hadPlayOneTime;

    public bool HadPlayOneTime => _hadPlayOneTime;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

#if ADS
        AppsFlyer.setHost("","appsflyersdk.com");
#endif

        GetDataInFireBase();

        StoreSecondOfAdsInterTime();

        CheckUserPlayedFirstTime();
    }

    private void Start() {
        ShowBanner();
    }

    private void GetDataInFireBase() {
        // Get time btw ads interval from server
        _timeBtwShowAdInter = PlayerPrefs.GetInt(DataRemoteFirebase.Ads_Time_Btw_Interval, 25);

        // _timeBtwShowAdInter = 0;
        
        // Get is has ads resume from server
        int checkAdInt = PlayerPrefs.GetInt(DataRemoteFirebase.Ads_Resume, 0);

        switch(checkAdInt) {
            case 0:
                _hasAdsResume = false;
                break;
            case 1:
                _hasAdsResume = true;
                break;
        }
    }

    public void CheckUserPlayedFirstTime() {
        // Get the first time user play
        int timePlayed = PlayerPrefs.GetInt(FIRST_TIME_USER_PLAYED, 0);

        if(timePlayed <= 0) {
            _hadPlayOneTime = false;
            PlayerPrefs.SetInt(FIRST_TIME_USER_PLAYED, 1);
        }
        else {
            _hadPlayOneTime = true;
        }
    }

    public void UserPlayedOneTime() {
        _hadPlayOneTime = true;
    }

    public void ShowReward(Action<bool> callback)
    {
#if ADS
        AppsFlyer.sendEvent("rewarded_ad_eligible", null);
        GetComponent<ApplovinManager>().ShowRewardedAd(callback);
#endif

    }

    public void ShowBanner()
    {
#if ADS
        GetComponent<ApplovinManager>().ShowBannerDelay(0f);
#endif
    }

    public void ShowInterstitial(Action callback = null)
    {
#if ADS
        AppsFlyer.sendEvent("inters_ad_eligible", null);
#endif

        string timeAdsInter = PlayerPrefs.GetString("Ads_Inter_Time");

        if(!IsTimeHadPassEnough(timeAdsInter, _timeBtwShowAdInter) || !_hadPlayOneTime) {
            callback?.Invoke();
            return;
        }

#if ADS
        GetComponent<ApplovinManager>().ShowInterstitialDelay(callback, 0);
#endif
    }

    public void ShowInterstitial() {
        ShowInterstitial(null);
    }

#if ADS
    public bool IsRewardReady()
    {
        return GetComponent<ApplovinManager>().IsRewardReady();
    }
#endif

    public bool CheckIsCountDownEnd(string passedInGmtAsString, float timeCountDown) {
        bool result = false;
        DateTime gmt;

        if (DateTime.TryParse(passedInGmtAsString, out gmt))
        {
            DateTime utcNow = DateTime.UtcNow;

            var minutes = utcNow.Subtract(gmt).TotalMinutes;

            if(minutes > timeCountDown) {
                result  = true;
            }
            else {
                result = false;
            }
            
        }
        
        return result;
    }

    public bool IsTimeHadPassEnough(string passedInPreviousTimeAsString, float timeBtw) {
        bool result = false;

        DateTime previousTime;

        if(DateTime.TryParse(passedInPreviousTimeAsString, out previousTime)) {
            DateTime utcNow = DateTime.UtcNow;
            
            var seconds = utcNow.Subtract(previousTime).TotalSeconds;

            // print("=======================Time Inter: " + seconds);

            if(seconds >= timeBtw) {
                result = true;
            }
            else {
                result = false;
            }
        }

        return result;
    }

    public void StoreSecondOfAdsInterTime() {
        PlayerPrefs.SetString("Ads_Inter_Time", DateTime.UtcNow.ToString());
    }

    public void StoreDateTimeAtLeaderBoard() {
        PlayerPrefs.SetString("TimeLeaderBoard", DateTime.UtcNow.ToString());
    }

    private void OnApplicationFocus(bool focusStatus) {
        if(focusStatus && _hasAdsResume) {
            ShowInterstitial();
        }
    }
    
}
