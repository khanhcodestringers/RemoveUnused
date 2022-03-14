using UnityEngine;
using System.Collections;
//using TuneSDK;
using Mio.TileMaster;

namespace Mio.Utils {
    public class ConversionTrackingManager : MonoSingleton<ConversionTrackingManager> {
        public void Initialize (string advertiserID, string conversionKey) {
            //Tune.Init(advertiserID, conversionKey);

            //// Check if a deferred deep link is available and handle opening of the deep link as appropriate in the success callback.
            //// Uncomment the following line if your MAT account has enabled deferred deep links
            //// Tune.CheckForDeferredDeeplink();

            //// Uncomment the following line to enable auto-measurement of successful in-app-purchase (IAP) transactions as "purchase" events
            //// Tune.AutomateIapEventMeasurement(true);

            //// If you have existing users from before TUNE SDK implementation,
            //// identify those users using this code snippet.
            ////bool isExistingUser = ...
            ////if (isExistingUser) {
            ////    Tune.SetExistingUser(true);
            ////}

            //if (GameManager.Instance.GameData.numStart > 1) {
            //    Tune.SetExistingUser(true);
            //}

            //Tune.MeasureSession();
        }

        void OnApplicationPause (bool pauseStatus) {
            if (!pauseStatus) {
                // Measure app resumes from background
                //Tune.MeasureSession();
            }
        }
    }
}