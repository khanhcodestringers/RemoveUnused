//using UnityEngine;
//using System.Collections;
//using System;
//#if MICROSOFT_ADS
//using CI.WSANative.Advertising;
//#endif

//namespace Mio.Monentize {
//    public class AdsManager : MonoSingleton<AdsManager> {
//        public const string ADS_SYMBOL = "ENABLE_ADS";
//        public const string MOPUB_ADS_SYMBOL = "MOPUB_ADS";
//        public const string MICROSOFT_ADS_SYMBOL = "MICROSOFT_ADS";

//        [Header("Index for Android is 0, iOS is 1")]
//        public string[] idMopubInterstitialAds;
//        public string[] idMopubRewardVideoAds;
//        public int secondBeforeRetryCachingAds = 5;
//        //private int indexAdsID = 0;
       

//        [Header("Microsoft Ads ID")]
//        public string idMicrosoftAdsApp;
//        public string idMicrosoftAdUnit;
//        //private bool isMicrosoftInterstitialAdsReady = false;
//        //private bool isCachingMicrosoftInterstitialAds = false;

//        //private bool isInterstitialAdsReady = false;
//        private bool isCachingInterstitialAds = false;


//        private bool isInitialized = false;
        
//        [System.Diagnostics.Conditional(ADS_SYMBOL)]
//        public void Initialize (bool autoCacheAds = true) {
//            if (!isInitialized) {
//                SetupPlatformDependencies();
//                InitializeMopub();
//                InitializeMicrosoftAds();
//                Logger.Log("Initialized ads");
//                isInitialized = true;
//                if (autoCacheAds) {
//                    CacheInterstitialAds();
//                }
//            }
//        }

//        private void SetupPlatformDependencies () {
//            //if(Application.platform == RuntimePlatform.Android) {
//            //    //indexAdsID = 0;
//            //}else if (Application.platform == RuntimePlatform.IPhonePlayer) {
//            //    //indexAdsID = 1;
//            //}
//        }

//        [System.Diagnostics.Conditional(MICROSOFT_ADS_SYMBOL)]
//        private void InitializeMicrosoftAds () {
//#if MICROSOFT_ADS
//            AddMicrosoftAdsCallbacks();
//            WSANativeInterstitialAd.Initialise(WSAInterstitialAdType.Microsoft, idMicrosoftAdsApp, idMicrosoftAdUnit);
//#endif
//            //microsoftAds = new AdsPlugin();
//            //microsoftAds.InitializeMicrosoftInterstitialAds();
            
//        }

//#if MICROSOFT_ADS
//        private void AddMicrosoftAdsCallbacks () {
//            WSANativeInterstitialAd.AdReady += OnMicrosoftInterstitialAdsReady;
//            WSANativeInterstitialAd.ErrorOccurred += OnMicrosoftInterstitialAdsError;
//            WSANativeInterstitialAd.Completed += OnMicrosoftInterstitialAdsCompleted;
//            WSANativeInterstitialAd.Cancelled += OnMicrosoftInterstitialAdsCancelled;
//        }

//        private void RemoveMicrosoftAdsCallbacks () {
//            WSANativeInterstitialAd.AdReady -= OnMicrosoftInterstitialAdsReady;
//            WSANativeInterstitialAd.ErrorOccurred -= OnMicrosoftInterstitialAdsError;
//            WSANativeInterstitialAd.Completed -= OnMicrosoftInterstitialAdsCompleted;
//            WSANativeInterstitialAd.Cancelled -= OnMicrosoftInterstitialAdsCancelled;
//        }

//        private void OnMicrosoftInterstitialAdsReady (WSAInterstitialAdType type) {
//            isInterstitialAdsReady = true;
//            isCachingInterstitialAds = false;
//        }

//        private void OnMicrosoftInterstitialAdsError (WSAInterstitialAdType type, string error) {
//            isInterstitialAdsReady = false;
//            isCachingInterstitialAds = false;
//            StartCoroutine(C_WaitThenCacheInterstitialAdsAfter(secondBeforeRetryCachingAds));
//        }

//        private void OnMicrosoftInterstitialAdsCompleted (WSAInterstitialAdType type) {
//            isInterstitialAdsReady = false;
//            isCachingInterstitialAds = false;
//            StartCoroutine(C_WaitThenCacheInterstitialAdsAfter(secondBeforeRetryCachingAds));
//        }

//        private void OnMicrosoftInterstitialAdsCancelled (WSAInterstitialAdType type) {
//            isInterstitialAdsReady = false;
//            isCachingInterstitialAds = false;
//            StartCoroutine(C_WaitThenCacheInterstitialAdsAfter(secondBeforeRetryCachingAds));
//        }
//#endif

//        [System.Diagnostics.Conditional(MOPUB_ADS_SYMBOL)]
//        private void InitializeMopub () {
//#if MOPUB_ADS
//            AddMopubCallbacks();
//            string[] interstitialIDs = new string[] { idMopubInterstitialAds[indexAdsID] };
//			#if !UNITY_IOS
//            MoPub.loadInterstitialPluginsForAdUnits(interstitialIDs);
//			#else
//			MoPub.loadPluginsForAdUnits(interstitialIDs);
//			#endif
//#endif
//        }

//        [System.Diagnostics.Conditional(ADS_SYMBOL)]
//        public void CacheInterstitialAds () {
//            RequestMopubInterstitialAds();
//            RequestMicrosoftInterstitialAds();
//            Logger.Log("Requesting interstitial ads"); 
//        }

//        [System.Diagnostics.Conditional(MOPUB_ADS_SYMBOL)]
//        private void RequestMopubInterstitialAds () {
//#if MOPUB_ADS
//            Logger.Log("Requesting ads for unit: " + idMopubInterstitialAds[indexAdsID]);
//            MoPub.requestInterstitialAd(idMopubInterstitialAds[indexAdsID]);
//#endif
//        }

//        [System.Diagnostics.Conditional(MICROSOFT_ADS_SYMBOL)]
//        private void RequestMicrosoftInterstitialAds () {
//#if MICROSOFT_ADS
//            WSANativeInterstitialAd.RequestAd(WSAInterstitialAdType.Microsoft);
//#endif
//        }

//        [System.Diagnostics.Conditional(ADS_SYMBOL)]
//        public void ShowInterstitialAds () {
//            ShowMopubInterstitialAds();
//            ShowMicrosoftInterstitialAds();
//        }

//        [System.Diagnostics.Conditional(MOPUB_ADS_SYMBOL)]
//        private void ShowMopubInterstitialAds () {
//#if MOPUB_ADS
//            if (isInterstitialAdsReady) {                
//                Logger.Log("Calling mopub to show ads");
//                MoPub.showInterstitialAd(idMopubInterstitialAds[indexAdsID]);
//            }else if (!isCachingInterstitialAds) {
//                Logger.Log("No cached ad, calling mopub to cache one");
//                StartCoroutine(C_WaitThenCacheInterstitialAdsAfter(secondBeforeRetryCachingAds));
//            }
//#endif
//        }

//        [System.Diagnostics.Conditional(MICROSOFT_ADS_SYMBOL)]
//        private void ShowMicrosoftInterstitialAds () {
//#if MICROSOFT_ADS
//            if (isInterstitialAdsReady) {                
//                Logger.Log("Calling to show ads");
//                WSANativeInterstitialAd.ShowAd(WSAInterstitialAdType.Microsoft);
//            }
//            else if (!isCachingInterstitialAds) {
//                Logger.Log("No cached ad, calling Microsoft to cache one");
//                StartCoroutine(C_WaitThenCacheInterstitialAdsAfter(secondBeforeRetryCachingAds));
//            }
//#endif
//        }

//        private IEnumerator C_WaitThenCacheInterstitialAdsAfter(float duration) {
//            if (isCachingInterstitialAds) yield break;

//            isCachingInterstitialAds = true;
//            yield return new WaitForSeconds(duration);
//            RequestMopubInterstitialAds();
//            RequestMicrosoftInterstitialAds();
//        }

//#region Mopub ads
//        [System.Diagnostics.Conditional(MOPUB_ADS_SYMBOL)]
//        void AddMopubCallbacks () {
//#if MOPUB_ADS
//            MoPubManager.onAdLoadedEvent += onAdLoadedEvent;
//            MoPubManager.onAdFailedEvent += onAdFailedEvent;
//            MoPubManager.onAdClickedEvent += onAdClickedEvent;
//            MoPubManager.onAdExpandedEvent += onAdExpandedEvent;
//            MoPubManager.onAdCollapsedEvent += onAdCollapsedEvent;

//            MoPubManager.onInterstitialLoadedEvent += onInterstitialLoadedEvent;
//            MoPubManager.onInterstitialFailedEvent += onInterstitialFailedEvent;
//            MoPubManager.onInterstitialShownEvent += onInterstitialShownEvent;
//            MoPubManager.onInterstitialClickedEvent += onInterstitialClickedEvent;
//            MoPubManager.onInterstitialDismissedEvent += onInterstitialDismissedEvent;
//            MoPubManager.onInterstitialExpiredEvent += onInterstitialExpiredEvent;

//            MoPubManager.onRewardedVideoLoadedEvent += onRewardedVideoLoadedEvent;
//            MoPubManager.onRewardedVideoFailedEvent += onRewardedVideoFailedEvent;
//            MoPubManager.onRewardedVideoExpiredEvent += onRewardedVideoExpiredEvent;
//            MoPubManager.onRewardedVideoShownEvent += onRewardedVideoShownEvent;
//            MoPubManager.onRewardedVideoFailedToPlayEvent += onRewardedVideoFailedToPlayEvent;
//            //MoPubManager.onRewardedVideoReceivedRewardEvent += onRewardedVideoReceivedRewardEvent;
//            MoPubManager.onRewardedVideoClosedEvent += onRewardedVideoClosedEvent;
//            MoPubManager.onRewardedVideoLeavingApplicationEvent += onRewardedVideoLeavingApplicationEvent; 
//#endif
//        }

//        [System.Diagnostics.Conditional(MOPUB_ADS_SYMBOL)]
//        void RemoveMopubCallbacks () {
//#if MOPUB_ADS
//            // Remove all event handlers
//            MoPubManager.onAdLoadedEvent -= onAdLoadedEvent;
//            MoPubManager.onAdFailedEvent -= onAdFailedEvent;
//            MoPubManager.onAdClickedEvent -= onAdClickedEvent;
//            MoPubManager.onAdExpandedEvent -= onAdExpandedEvent;
//            MoPubManager.onAdCollapsedEvent -= onAdCollapsedEvent;

//            MoPubManager.onInterstitialLoadedEvent -= onInterstitialLoadedEvent;
//            MoPubManager.onInterstitialFailedEvent -= onInterstitialFailedEvent;
//            MoPubManager.onInterstitialShownEvent -= onInterstitialShownEvent;
//            MoPubManager.onInterstitialClickedEvent -= onInterstitialClickedEvent;
//            MoPubManager.onInterstitialDismissedEvent -= onInterstitialDismissedEvent;
//            MoPubManager.onInterstitialExpiredEvent -= onInterstitialExpiredEvent;

//            MoPubManager.onRewardedVideoLoadedEvent -= onRewardedVideoLoadedEvent;
//            MoPubManager.onRewardedVideoFailedEvent -= onRewardedVideoFailedEvent;
//            MoPubManager.onRewardedVideoExpiredEvent -= onRewardedVideoExpiredEvent;
//            MoPubManager.onRewardedVideoShownEvent -= onRewardedVideoShownEvent;
//            MoPubManager.onRewardedVideoFailedToPlayEvent -= onRewardedVideoFailedToPlayEvent;
//            //MoPubManager.onRewardedVideoReceivedRewardEvent -= onRewardedVideoReceivedRewardEvent;
//            MoPubManager.onRewardedVideoClosedEvent -= onRewardedVideoClosedEvent;
//            MoPubManager.onRewardedVideoLeavingApplicationEvent -= onRewardedVideoLeavingApplicationEvent; 
//#endif
//        }
        
//#region Banner events
//        void onAdLoadedEvent (float height) {
//            Logger.Log("onAdLoadedEvent. height: " + height);
//        }

//        void onAdFailedEvent (string errorMsg) {
//            Logger.Log("onAdFailedEvent: " + errorMsg);
//        }

//        void onAdClickedEvent (string adUnitId) {
//            Logger.Log("onAdClickedEvent: " + adUnitId);
//        }

//        void onAdExpandedEvent (string adUnitId) {
//            Logger.Log("onAdExpandedEvent: " + adUnitId);
//        }

//        void onAdCollapsedEvent (string adUnitId) {
//            Logger.Log("onAdCollapsedEvent: " + adUnitId);
//        }
//#endregion

//#region Interstitial Events
//        void onInterstitialLoadedEvent (string adUnitId) {
//            isInterstitialAdsReady = true;
//            isCachingInterstitialAds = false;
//            Logger.Log("onInterstitialLoadedEvent: " + adUnitId);
//        }

//        void onInterstitialFailedEvent (string errorMsg) {
//            isInterstitialAdsReady = false;
//            isCachingInterstitialAds = false;
//            StartCoroutine(C_WaitThenCacheInterstitialAdsAfter(secondBeforeRetryCachingAds));
//            Logger.Log("onInterstitialFailedEvent: " + errorMsg);
//        }

//        void onInterstitialShownEvent (string adUnitId) {
//            Logger.Log("onInterstitialShownEvent: " + adUnitId);
//        }

//        void onInterstitialClickedEvent (string adUnitId) {
//            Logger.Log("onInterstitialClickedEvent: " + adUnitId);
//        }

//        void onInterstitialDismissedEvent (string adUnitId) {
//            isInterstitialAdsReady = false;
//            StartCoroutine(C_WaitThenCacheInterstitialAdsAfter(secondBeforeRetryCachingAds));
//            Logger.Log("onInterstitialDismissedEvent: " + adUnitId);
//        }

//        void onInterstitialExpiredEvent (string adUnitId) {
//            isInterstitialAdsReady = false;
//            StartCoroutine(C_WaitThenCacheInterstitialAdsAfter(secondBeforeRetryCachingAds));
//            Logger.Log("onInterstitialExpiredEvent: " + adUnitId);
//        }
//#endregion
        
//#region Rewarded Video Events
//        void onRewardedVideoLoadedEvent (string adUnitId) {
//            Logger.Log("onRewardedVideoLoadedEvent: " + adUnitId);
//        }

//        void onRewardedVideoFailedEvent (string errorMsg) {
//            Logger.Log("onRewardedVideoFailedEvent: " + errorMsg);
//        }

//        void onRewardedVideoExpiredEvent (string adUnitId) {
//            Logger.Log("onRewardedVideoExpiredEvent: " + adUnitId);
//        }

//        void onRewardedVideoShownEvent (string adUnitId) {
//            Logger.Log("onRewardedVideoShownEvent: " + adUnitId);
//        }

//        void onRewardedVideoFailedToPlayEvent (string errorMsg) {
//            Logger.Log("onRewardedVideoFailedToPlayEvent: " + errorMsg);
//        }

        
//        //void onRewardedVideoReceivedRewardEvent (MoPubManager.RewardedVideoData rewardedVideoData) {
//        //    Logger.Log("onRewardedVideoReceivedRewardEvent: " + rewardedVideoData);
//        //}

//        void onRewardedVideoClosedEvent (string adUnitId) {
//            Logger.Log("onRewardedVideoClosedEvent: " + adUnitId);
//        }

//        void onRewardedVideoLeavingApplicationEvent (string adUnitId) {
//            Logger.Log("onRewardedVideoLeavingApplicationEvent: " + adUnitId);
//        } 
//#endregion

//#endregion
//    }
//}