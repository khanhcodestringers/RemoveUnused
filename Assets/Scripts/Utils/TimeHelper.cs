using UnityEngine;
using System.Collections;
using System;

namespace Mio.Utils {
    public class TimeHelper{
        public static readonly DateTime dtEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }

        public static double DateTimeToUnixTimeStamp(DateTime dt) {
            var elapsed = dt - dtEpoch;
            return elapsed.TotalSeconds;
        }
    }
}