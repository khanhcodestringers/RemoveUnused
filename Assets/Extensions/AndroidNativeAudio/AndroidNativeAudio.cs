/*
 *  Android Native Audio
 *
 */


using UnityEngine;
using System;

public class AndroidNativeAudio {
#if UNITY_ANDROID && !UNITY_EDITOR

	// Set DEBUG to "true" to enable activity logging
	private static bool DEBUG = false;
	
	private const int LOAD_PRIORITY = 1;
	private const int SOURCE_QUALITY = 0;
	
	private static AndroidJavaObject assetFileDescriptor;
	private static AndroidJavaObject assets;
	private static AndroidJavaObject soundPool = null;
	private static bool hasOBB;
	private static int streamMusic = new AndroidJavaClass("android.media.AudioManager").GetStatic<int>("STREAM_MUSIC");
	
	static AndroidNativeAudio()
	{
		AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		
		if (Application.streamingAssetsPath.Substring(Application.streamingAssetsPath.Length - 12) == ".obb!/assets")
		{
			hasOBB = true;
			int versionCode = context.Call<AndroidJavaObject>("getPackageManager").Call<AndroidJavaObject>("getPackageInfo", context.Call<string>("getPackageName"), 0).Get<int>("versionCode");
			try
			{
				assets = new AndroidJavaClass("com.android.vending.expansion.zipfile.APKExpansionSupport").CallStatic<AndroidJavaObject>("getAPKExpansionZipFile", context, versionCode, 0);
			}
			catch(Exception e)
			{
				Debug.Log(e.ToString());
			}
		}
		else
		{
			hasOBB = false;
			assets = context.Call<AndroidJavaObject>("getAssets");
		}
	}
	
	public static int load(string audioFile, bool usePersistentDataPath = false)
	{
		audioFile = audioFile.Replace("\\", "/");
		
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: load(\"" + audioFile + "\", " + usePersistentDataPath + ")");
		
		if(soundPool == null)
		{
			Debug.Log("AndroidNativeAudio: Use makePool() before load()!");
			return -1;
		}
		
		try
		{
			if (usePersistentDataPath)
				return soundPool.Call<int>("load", Application.persistentDataPath + "/" + audioFile, LOAD_PRIORITY);
			else if (hasOBB)
				assetFileDescriptor = assets.Call<AndroidJavaObject>("getAssetFileDescriptor", "assets/" + audioFile);
			else
				assetFileDescriptor = assets.Call<AndroidJavaObject>("openFd", audioFile);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
			return -1;
		}
		
		try
		{
			return soundPool.Call<int>("load", assetFileDescriptor, LOAD_PRIORITY);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
			return -1;
		}
	}
	
	public static void makePool(int maxStreams)
	{
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: makePool(" + maxStreams.ToString() + ")");
		
		if(soundPool != null)
			soundPool.Call("release");
		
		soundPool = new AndroidJavaObject("srs.srsoundpool.SRSoundPool", maxStreams);
	}
	
	public static void pause(int streamID)
	{
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: pause(" + streamID.ToString() + ")");
		
		try
		{
			soundPool.Call("pause", streamID);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	
	public static void pauseAll()
	{
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: pauseAll()");
		
		try
		{
			soundPool.Call("autoPause");
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	
	public static int play(int soundID, float leftVolume = 1, float rightVolume = -1, int priority = 1, int loop = 0, float rate = 1)
	{
		if(rightVolume == -1)
			rightVolume = leftVolume;
		
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: play(" + soundID.ToString() + ", " + leftVolume.ToString() + ", " + rightVolume.ToString() + ", " + priority.ToString() + ", " + loop.ToString() + ", " + rate.ToString() + ")");
		
		try
		{
			return soundPool.Call<int>("play", soundID, leftVolume, rightVolume, priority, loop, rate);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
			return -1;
		}
	}

    public static void play(int[] soundID, float leftVolume = 1, float rightVolume = -1, int priority = 1, int loop = 0, float rate = 1)
	{
		if(rightVolume == -1)
			rightVolume = leftVolume;
		
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: play(" + soundID.ToString() + ", " + leftVolume.ToString() + ", " + rightVolume.ToString() + ", " + priority.ToString() + ", " + loop.ToString() + ", " + rate.ToString() + ")");
		
		try
		{
		 soundPool.Call("play", soundID, leftVolume, rightVolume, priority, loop, rate);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
			//return -1;
		}
	}
	
	public static void releasePool()
	{
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: releasePool()");
		
		try
		{
			soundPool.Call("release");
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
			return;
		}
		soundPool = null;
	}
	
	public static void resume(int streamID)
	{
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: resume(" + streamID.ToString() + ")");
		
		try
		{
			soundPool.Call("resume", streamID);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	
	public static void resumeAll()
	{
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: resumeAll()");
		
		try
		{
			soundPool.Call("autoResume");
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	
	public static void setLoop(int streamID, int loop)
	{
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: setLoop(" + streamID.ToString() + ", " + loop.ToString() + ")");
		
		try
		{
			soundPool.Call("setLoop", streamID, loop);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	
	public static void setPriority(int streamID, int priority)
	{
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: setPriority(" + streamID.ToString() + ", " + priority.ToString() + ")");
		
		try
		{
			soundPool.Call("setPriority", streamID, priority);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	
	public static void setRate(int streamID, float rate)
	{
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: setRate(" + streamID.ToString() + ", " + rate.ToString() + ")");
		
		try
		{
			soundPool.Call("setRate", streamID, rate);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	
	public static void setVolume(int streamID, float leftVolume, float rightVolume = -1)
	{
		if(rightVolume == -1)
			rightVolume = leftVolume;
		
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: setVolume(" + streamID.ToString() + ", " + leftVolume.ToString() + ", " + rightVolume.ToString() + ")");
		
		try
		{
			soundPool.Call("setVolume", streamID, leftVolume, rightVolume);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	
	public static void stop(int streamID)
	{
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: stop(" + streamID.ToString() + ")");
		
		try
		{
			soundPool.Call("stop", streamID);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	
	public static bool unload(int soundID)
	{
		if(DEBUG)
			Debug.Log("AndroidNativeAudio: unload(" + soundID.ToString() + ")");
		
		try
		{
			return soundPool.Call<bool>("unload", soundID);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
			return false;
		}
	}
	
#else

    public static int load(string audioFile, bool usePersistentDataPath = false) {
        audioFile = audioFile.Replace("\\", "/");
        Debug.Log("AndroidNativeAudio: load(\"" + audioFile + "\", " + usePersistentDataPath + ")");
        return 0;
    }

    public static void makePool(int maxStreams) {
        Debug.Log("AndroidNativeAudio: makePool(" + maxStreams.ToString() + ")");
    }

    public static void pause(int streamID) {
        Debug.Log("AndroidNativeAudio: pause(" + streamID.ToString() + ")");
    }

    public static void pauseAll() {
        Debug.Log("AndroidNativeAudio: pauseAll()");
    }

    public static int play(int soundID, float leftVolume = 1, float rightVolume = -1, int priority = 1, int loop = 0, float rate = 1) {
        if (rightVolume == -1)
            rightVolume = leftVolume;

        Debug.Log("AndroidNativeAudio: play(" + soundID.ToString() + ", " + leftVolume.ToString() + ", " + rightVolume.ToString() + ", " + priority.ToString() + ", " + loop.ToString() + ", " + rate.ToString() + ")");
        return 0;
    }

    public static int play (int[] soundID, float leftVolume = 1, float rightVolume = -1, int priority = 1, int loop = 0, float rate = 1) {
        if (rightVolume == -1)
            rightVolume = leftVolume;

        Debug.Log("AndroidNativeAudio: play(" + soundID.ToString() + ", " + leftVolume.ToString() + ", " + rightVolume.ToString() + ", " + priority.ToString() + ", " + loop.ToString() + ", " + rate.ToString() + ")");
        return 0;
    }

    public static void releasePool() {
        Debug.Log("AndroidNativeAudio: releasePool()");
    }

    public static void resume(int streamID) {
        Debug.Log("AndroidNativeAudio: resume(" + streamID.ToString() + ")");
    }

    public static void resumeAll() {
        Debug.Log("AndroidNativeAudio: resumeAll()");
    }

    public static void setLoop(int streamID, int loop) {
        Debug.Log("AndroidNativeAudio: setLoop(" + streamID.ToString() + ", " + loop.ToString() + ")");
    }

    public static void setPriority(int streamID, int priority) {
        Debug.Log("AndroidNativeAudio: setPriority(" + streamID.ToString() + ", " + priority.ToString() + ")");
    }

    public static void setRate(int streamID, float rate) {
        Debug.Log("AndroidNativeAudio: setRate(" + streamID.ToString() + ", " + rate.ToString() + ")");
    }

    public static void setVolume(int streamID, float leftVolume, float rightVolume = -1) {
        if (rightVolume == -1)
            rightVolume = leftVolume;

        Debug.Log("AndroidNativeAudio: setVolume(" + streamID.ToString() + ", " + leftVolume.ToString() + ", " + rightVolume.ToString() + ")");
    }

    public static void stop(int streamID) {
        Debug.Log("AndroidNativeAudio: stop(" + streamID.ToString() + ")");
    }

    public static bool unload(int soundID) {
        Debug.Log("AndroidNativeAudio: unload(" + soundID.ToString() + ")");
        return false;
    }

#endif
}
