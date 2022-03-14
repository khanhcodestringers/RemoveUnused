using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System;
using System.IO;

public class BuildScripts {
    //private const string AndroidBundleName = ;
    //private const string iOSBundleName = "com.game.pianochallenge2";

    static string[] GetScenes() {
        return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
    }

    static void CreateDirAsNeeded(string dir) {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    static string GetBuildNumber() {
        DateTime dt = DateTime.Now;
        return string.Format("{0}{1}{2}{3}{4}",
            dt.Year.ToString().Substring(2),
            dt.Month.ToString("00"),
            dt.Day.ToString("00"),
            dt.Hour.ToString("00"),
            dt.Minute.ToString("00"));
    }

    static void BuildAndroid() {
        string buildNumber = GetBuildNumber();
        //Debug.Log("Building android...");
        PlayerSettings.productName = "Piano Challenges 2";
        PlayerSettings.applicationIdentifier = "com.musicheroesrevenge.pianochallenge2";
        //PlayerSettings = buildNumber;
        PlayerSettings.Android.keyaliasName = "piano challenge";
        PlayerSettings.Android.keyaliasPass = PlayerSettings.Android.keystorePass = "PianoChallenge";
        PlayerSettings.Android.keystoreName = Path.GetFullPath(@"pc.jks").Replace('\\', '/');
        BuildPipeline.BuildPlayer(GetScenes(), string.Format("Builds/PC2-Release-{0}.apk", buildNumber), BuildTarget.Android, BuildOptions.None);
    }

    static void BuildiOS() {
        string buildNumber = GetBuildNumber();
        CreateDirAsNeeded("Builds/iOS");
        PlayerSettings.productName = "Piano Challenge 2";
        PlayerSettings.applicationIdentifier = "com.game.pianochallenge2";
        PlayerSettings.bundleVersion = buildNumber;
        BuildPipeline.BuildPlayer(GetScenes(), "Builds/iOS", BuildTarget.iOS, BuildOptions.None);
    }
}
