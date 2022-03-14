using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using MovementEffects;
using System;
using Mio.Utils.MessageBus;


namespace Mio.Utils {
    public class GeoLocationManager : MonoSingleton<GeoLocationManager> {
        public const string GEOLOCATION_KEY = "geolocation";

        private CountryCodeModel countryCode;

        public CountryCodeModel CountryCode {
            get { return countryCode; }
        }

        //void Start()
        //{
        //    //TextAsset bindata = Resources.Load("geolocation") as TextAsset;
        //    //Debug.Log(bindata.text);
        //    //fsData jsonData = fsJsonParser.Parse(bindata.text);
        //    //fsResult result = FileUtilities.JSONSerializer.TryDeserialize<CountryCodeModel>(jsonData, ref countryCode);
        //    //Debug.Log(countryCode.city);
        //    Initialize();
        //}

        public void Initialize () {
            if (!IsInitialize()) {
                StartCoroutine(IEGetGeoLocation(OnCompletedRequest));
            }
        }

        public bool IsInitialize () {
            if (countryCode == null)
                return false;
            else
                return true;
        }

        // get geolocation from http://ip-api.com/json
        IEnumerator IEGetGeoLocation (Action<bool, string> result, float timeOut = 5f) {
            float timer = Time.realtimeSinceStartup;
            using (WWW www = new WWW("http://ip-api.com/json")) {
                while (!www.isDone) {
                    if (Time.realtimeSinceStartup - timer >= timeOut) {
                        if (result != null) {
                            result(false, "Request timed out");
                        }
                        yield break;
                    }
                    yield return new WaitForSeconds(0.1f);
                }
                yield return www;

                if (www.error != null) {
                    if (result != null)
                        result(false, www.error);
                }
                else {
                    if (result != null)
                        result(true, www.text);
                }

            }

        }

        private void OnCompletedRequest (bool onsucced, string jsonString) {

            countryCode = new CountryCodeModel();
            //fsData jsonData;
            //fsResult result;
            //if (onsucced) {


            //    jsonData = fsJsonParser.Parse(jsonString);
            //    result = FileUtilities.JSONSerializer.TryDeserialize<CountryCodeModel>(jsonData, ref countryCode);
            //    if (!result.Failed) {
            //        //Debug.Log("SAVE LOCATION");
            //        PlayerPrefs.SetString(GEOLOCATION_KEY, jsonString);
            //        PlayerPrefs.Save();
            //    }
            //    else {
            //        string jsonLocation = GetLocalLocation();
            //        jsonData = fsJsonParser.Parse(jsonLocation);
            //        result = FileUtilities.JSONSerializer.TryDeserialize<CountryCodeModel>(jsonData, ref countryCode);
            //    }
            //}
            //else {
            //    string jsonLocation = GetLocalLocation();
            //    jsonData = fsJsonParser.Parse(jsonLocation);
            //    result = FileUtilities.JSONSerializer.TryDeserialize<CountryCodeModel>(jsonData, ref countryCode);
            //}
            //Debug.Log(countryCode.countryCode);
            //MessageBus.MessageBus.Annouce(new Message(MessageBusType.OnCompletedGetGeoLocation));
        }

        private string GetLocalLocation () {
            if (string.IsNullOrEmpty(PlayerPrefs.GetString(GEOLOCATION_KEY))) {
                TextAsset bindata = Resources.Load("geolocation") as TextAsset;
                return bindata.text;

            }
            else {
                return PlayerPrefs.GetString(GEOLOCATION_KEY);

            }
        }

    }
}
