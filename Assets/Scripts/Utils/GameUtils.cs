using UnityEngine;
using System.Collections;
using System;

public class GameUtils  {
    public static DateTime UnixTimeToDateTime(long javaTimeStamp)
    {
        javaTimeStamp = (long)(javaTimeStamp / 1000);
        DateTime unixYear0 = new DateTime(1970, 1, 1);
        long unixTimeStampInTicks = javaTimeStamp * TimeSpan.TicksPerSecond;
        DateTime dtUnix = new DateTime(unixYear0.Ticks + unixTimeStampInTicks);
        return dtUnix;
    }
	public static System.Random rand=new System.Random();
}
