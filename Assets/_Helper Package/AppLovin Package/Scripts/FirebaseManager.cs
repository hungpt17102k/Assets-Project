using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if ADS
using Firebase;
using Firebase.Extensions;
#endif

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    #if ADS
    private FirebaseApp app;
    #endif

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    #if ADS
    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
                InitFirebase();
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    void InitFirebase()
    {
        Dictionary<string, object> defaults = new Dictionary<string, object>();

        defaults.Add("draw_theme", 1);

        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
        Firebase.Analytics.FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
        FetchRemoteData();
    }

    public void FetchRemoteData()
    {
        FetchDataAsync();
    }

    public Task FetchDataAsync()
    {
        Debug.Log("Retching data...");
        Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {

        }
        else if (fetchTask.IsFaulted)
        {

        }
        else if (fetchTask.IsCompleted)
        {

        }

        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync().
                    ContinueWithOnMainThread(task =>
                    {
                        Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).", info.FetchTime));
                    });
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");
                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }

                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }

        // Fetch data config on server
        FetchDataConfig();
    }

    private void FetchDataConfig() {
        // Fetch data config on server
        long timeBtwAd = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(DataRemoteFirebase.Ads_Time_Btw_Interval).LongValue;
        PlayerPrefs.SetInt(DataRemoteFirebase.Ads_Time_Btw_Interval, Int32.Parse(timeBtwAd.ToString()));

        bool hasResumeAd = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(DataRemoteFirebase.Ads_Resume).BooleanValue;
        if(hasResumeAd) {
            PlayerPrefs.SetInt(DataRemoteFirebase.Ads_Resume, 1);
        }
        else {
            PlayerPrefs.SetInt(DataRemoteFirebase.Ads_Resume, 0);
        }
    }

    public void LogEvent(string parameter, string message, int value = 0)
    {
        Debug.Log("-------------" + parameter + "---" + message + "---" + value);
        Firebase.Analytics.FirebaseAnalytics.LogEvent(parameter, "event", value);
    }

    public void LogEvent(string parameter, string message, string value = "")
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent(parameter, message, value);
    }

    #endif
}
