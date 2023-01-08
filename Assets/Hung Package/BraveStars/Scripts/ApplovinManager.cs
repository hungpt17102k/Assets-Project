using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ADS
using AppsFlyerSDK;
#endif

public class ApplovinManager : MonoBehaviour
{
#if ADS

	// AppLovin Max SDK Key
	private const string MAX_SdkKey =
		"7yuxQQv3-BNp4sB7KaiNkH-xtLw_TFb87GJ-xZZbU2WIawMF8Se3a5y9PC6rC9iwEuTkiOriUlvTFtx6gOJd30";
#if PRODUCTION
#if UNITY_IOS
		// prod ios
		private const string MAX_BannerAdUnitId = ""; // Sample
		private const string MAX_InterstitialAdUnitId = ""; // Sample
		private const string MAX_RewardedAdUnitId = ""; // Sample
#else
		// prod android
		private const string MAX_BannerAdUnitId = ""; // Sample
		private const string MAX_InterstitialAdUnitId = ""; // Sample
		private const string MAX_RewardedAdUnitId = ""; // Sample
#endif
#else
#if UNITY_IOS
		// dev ios
		private const string MAX_BannerAdUnitId = ""; // Sample
		private const string MAX_InterstitialAdUnitId = ""; // Sample
		private const string MAX_RewardedAdUnitId = ""; // Sample
#else
	// dev android
	private const string MAX_BannerAdUnitId = "dae0eba9d10ebf30";
	private const string MAX_InterstitialAdUnitId = "701e81c8553f86ab";
	private const string MAX_RewardedAdUnitId = "bf898582789cecc5";
#endif
#endif
	private bool MAX_Initialized;
	private bool MAX_isBannerShowing = false;
	public static float LastTimeShowAds { get; private set; }
	private int interstitialRetryAttempt;
	private int rewardedRetryAttempt;

	private Action InterstitialCallback;

	private Action<bool> RewardedAdCallback;
	private Action<bool> RewardedAdListenCallback;

	public static bool RewardAdTestMode;

	private bool mIsRewardLoaded;
	[HideInInspector] public bool MAX_RewardedAdLoaded;

	// Output text to the debug log text field, as well as the console.
	public void DebugLog(string s)
	{
		Debug.Log("-------------------------------" + s);
	}

	// Start is called before the first frame update
	void Start()
	{
		InitializeAppLovin();

	}

	public string GetAdUnitId()
	{
		var text = "Bundle ID: " + Application.identifier;
		text += "\n MAX_BannerAdUnitId: " + MAX_BannerAdUnitId;
		text += "\n MAX_InterstitialAdUnitId: " + MAX_InterstitialAdUnitId;
		text += "\n MAX_RewardedAdUnitId: " + MAX_RewardedAdUnitId;
		text += "\n systemLanguage: " + Application.systemLanguage;
		text += "\n CurrentRegion: " + System.Globalization.RegionInfo.CurrentRegion.Name;
		return text;
	}

	// 3rd party SDK AppLovin ****************************************************************
	void InitializeAppLovin()
	{
		MAX_Initialized = true;
		mIsRewardLoaded = false;
		MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
		{
			DebugLog("AppLovin SDK is initialized.");
			MAX_InitializeInterstitialAds();
			MAX_InitializeRewardedAds();
			MAX_InitializeBannerAds();
			// MaxSdk.ShowMediationDebugger();
		};
        MaxSdk.SetSdkKey(MAX_SdkKey);
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
		MaxSdk.InitializeSdk();
	}

	#region Interstitial Ad Methods

	void MAX_InitializeInterstitialAds()
	{
		// Attach callbacks
		MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
		MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
		MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
		MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
		MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
		MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

		// Load the first interstitial
		MAX_LoadInterstitial();
	}

	void MAX_LoadInterstitial()
	{
		DebugLog("MAX Interstitial Ad Loading...");
		MaxSdk.LoadInterstitial(MAX_InterstitialAdUnitId);
	}

	void MAX_ShowInterstitial()
	{
		if (!MAX_Initialized) return;
		if(InterstitialCallback != null) {
			InterstitialCallback();
		}

		if (MaxSdk.IsInterstitialReady(MAX_InterstitialAdUnitId))
		{
			Time.timeScale = 0;
			DebugLog("MAX Interstitial Ad Showing");
			MaxSdk.ShowInterstitial(MAX_InterstitialAdUnitId);
		}
		else
		{
			DebugLog("MAX Interstitial Ad not ready");
			MAX_LoadInterstitial();
		}
	}

	private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		// Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
		DebugLog("MAX Interstitial Ad loaded");
		// AdjustEvent adj = new AdjustEvent("inters_api_called");
		// Adjust.trackEvent(adj);

		AppsFlyer.sendEvent("inters_api_called", null);

		// Reset retry attempt
		interstitialRetryAttempt = 0;
	}

	private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
	{
		DebugLog("MAX Interstitial failed to load with error code: " + errorInfo);
		// Interstitial ad failed to load. We recommend retrying with exponentially higher delays.
		interstitialRetryAttempt++;
		double retryDelay = Math.Pow(2, interstitialRetryAttempt);
		Invoke("MAX_LoadInterstitial", (float)retryDelay);
	}

	private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
	{
		// Interstitial ad failed to display. We recommend loading the next ad
		DebugLog("MAX Interstitial failed to display with error code: " + errorInfo);
		MAX_LoadInterstitial();
	}

	private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
		
	}

	private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {

	}

	private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		// Interstitial ad is hidden. Pre-load the next ad
		DebugLog("MAX Interstitial dismissed");
		// AdjustEvent adj = new AdjustEvent("inters_displayed");
		// Adjust.trackEvent(adj);

		AppsFlyer.sendEvent("inters_displayed", null);
		
		// Store the time of ads Inter
		BravestarsAdsManager.Instance.StoreSecondOfAdsInterTime();

		LastTimeShowAds = Time.time;
		Time.timeScale = 1;
		MAX_LoadInterstitial();

	}

	#endregion


	#region Rewarded Ad Methods

	private void MAX_InitializeRewardedAds()
	{
		// Attach callbacks
		MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
		MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
		MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
		MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
		MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
		MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
		MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
		MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

		// Load the first RewardedAd
		LoadRewardedAd();
	}

	//firebase measue revenue
    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        double revenue = impressionData.Revenue;
        var impressionParameters = new[] {
            new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
            new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
            new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
            new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
            new Firebase.Analytics.Parameter("value", revenue),
            new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }

	private void LoadRewardedAd()
	{
		DebugLog("MAX Rewarded Ad Loading...");
		MaxSdk.LoadRewardedAd(MAX_RewardedAdUnitId);
	}

	private void MAX_ShowRewardedAd()
	{
		MAX_RewardedAdLoaded = false;
		if (RewardedAdListenCallback != null) RewardedAdListenCallback(MAX_RewardedAdLoaded);
		if (MaxSdk.IsRewardedAdReady(MAX_RewardedAdUnitId))
		{
			Time.timeScale = 0;
			DebugLog("MAX Rewarded Ad Showing");
			MaxSdk.ShowRewardedAd(MAX_RewardedAdUnitId);
		}
		else
		{
			DebugLog("MAX Rewarded Ad not ready");
			LoadRewardedAd();
			if (RewardedAdCallback != null) RewardedAdCallback(false);
			RewardedAdCallback = null;
		}
	}

	public bool IsRewardReady()
	{
		return MaxSdk.IsRewardedAdReady(MAX_RewardedAdUnitId);
	}
	
	private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		// Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(MAX_RewardedAdUnitId) will now return 'true'
		DebugLog("MAX Rewarded Ad loaded");
		// AdjustEvent adj = new AdjustEvent("rewarded_api_called");
		// Adjust.trackEvent(adj);

		AppsFlyer.sendEvent("rewarded_api_called", null);

		// Reset retry attempt
		rewardedRetryAttempt = 0;
		MAX_RewardedAdLoaded = true;
		if (RewardedAdListenCallback != null) RewardedAdListenCallback(MAX_RewardedAdLoaded);
	}

	private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
	{
		DebugLog("MAX Rewarded Ad failed to load with error code: " + errorInfo);
		// Rewarded ad failed to load. We recommend retrying with exponentially higher delays.
		rewardedRetryAttempt++;
		double retryDelay = Math.Pow(2, rewardedRetryAttempt);
		Invoke("LoadRewardedAd", (float)retryDelay);
	}

	private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
	{
		// Rewarded ad failed to display. We recommend loading the next ad
		DebugLog("MAX Rewarded Ad failed to display with error code: " + errorInfo);
		LoadRewardedAd();
		if (RewardedAdCallback != null) RewardedAdCallback(false);
		RewardedAdCallback = null;
	}

	private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		DebugLog("MAX Rewarded Ad displayed: " + adInfo);
		// AdjustEvent adj = new AdjustEvent("rewarded_ad_displayed");
		// Adjust.trackEvent(adj);

		AppsFlyer.sendEvent("rewarded_ad_displayed", null);
	}

	private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		DebugLog("MAX Rewarded Ad clicked");
	}

	private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		// Rewarded ad is hidden. Pre-load the next ad
		DebugLog("MAX Rewarded Ad dismissed");
		Time.timeScale = 1;
		LoadRewardedAd();
	}

	private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
	{
		// Rewarded ad was displayed and user should receive the reward
		DebugLog("MAX Rewarded Ad received reward");
        if (RewardedAdCallback != null)
        {
            RewardedAdCallback(true);
        }
		RewardedAdCallback = null;
	}

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }

    #endregion


    #region Banner Ad Methods

    private void MAX_InitializeBannerAds()
    {
        if (!MAX_Initialized) return;
        DebugLog("MAX Initialized banner ad.");
        // Banners are automatically sized to 320x50 on phones and 728x90 on tablets
        // You may use the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(MAX_BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerExtraParameter(MAX_BannerAdUnitId, "adative_banner", "true");
        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(MAX_BannerAdUnitId, new Color(0, 0, 0, 0));
    }

	private void MAX_ToggleBannerVisibility()
	{
		if (!MAX_Initialized) return;
		if (!MAX_isBannerShowing)
		{
			MaxSdk.ShowBanner(MAX_BannerAdUnitId);
			MAX_isBannerShowing = true;
		}
		else
		{
			MaxSdk.HideBanner(MAX_BannerAdUnitId);
			MAX_isBannerShowing = false;
		}
	}

	// MAX_ShowBanner
	private void MAX_ShowBanner()
	{
		if (!MAX_Initialized) return;
		DebugLog("MAX Showing banner ad.");
		MaxSdk.ShowBanner(MAX_BannerAdUnitId);
		MAX_isBannerShowing = true;
	}

	#endregion

	// End AppLovin ****************************************************************

	/// <summary>
	/// Global.Delay
	/// </summary>
	/// <param name="seconds"></param>
	/// <param name="Callback"></param>
	/// <returns></returns>
	public IEnumerator Delay(float seconds, System.Action Callback)
	{
		yield return new WaitForSeconds(seconds);
		Callback();
	}


	public void ShowBannerDelay(float seconds)
	{
		StartCoroutine(Delay(seconds, () => { MAX_ShowBanner(); }));
	}

	public void ToggleBannerVisibility()
	{
		DebugLog("ToggleBannerVisibility");
		MAX_ToggleBannerVisibility();
	}

	public void ShowInterstitialDelay(Action callback, float seconds)
	{
		DebugLog("ShowInterstitialDelay");
        // MAX_ShowInterstitial();
		StartCoroutine(Delay(seconds, () =>
		{
			InterstitialCallback = callback;
            MAX_ShowInterstitial();
		}));
	}


	public void ShowRewardedAd(Action<bool> callback)
	{
		DebugLog("ShowRewardedAd");
		if (RewardAdTestMode)
		{
			callback(true);
			return;
		}

		RewardedAdCallback = callback;
		MAX_ShowRewardedAd();
	}

	public void RewardedAdListen(Action<bool> callback)
	{
		callback(RewardAdTestMode ? true : MAX_RewardedAdLoaded);
		RewardedAdListenCallback = callback;
	}

#endif
}
