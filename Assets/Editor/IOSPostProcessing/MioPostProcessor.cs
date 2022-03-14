using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.IO;
using System.Diagnostics;


public class MioPostProcessor : MonoBehaviour
{
	[UnityEditor.MenuItem( "Tools/Manually run Xcode post processor...", false, 21 )]
	static void manuallyRunPostProcessor()
	{
		var path = EditorUtility.OpenFilePanel( "Locate your Xcode project file", Application.dataPath.Replace( "/Assets", string.Empty ), "xcodeproj" );
		if( path != null && path.Length > 10 )
		{
#if UNITY_4_6 || UNITY_4_7 || UNITY_4_8
			onPostProcessBuildPlayer( BuildTarget.iPhone, Directory.GetParent( path ).FullName );
#else
			onPostProcessBuildPlayer( BuildTarget.iOS, Directory.GetParent( path ).FullName );
#endif
		}
	}


	[PostProcessBuild( 200 )]
	private static void onPostProcessBuildPlayer( BuildTarget target, string pathToBuiltProject )
	{
		if( target == BuildTarget.iOS )
		{
			// grab the path to the postProcessor.py file
			var scriptPath = Path.Combine( Application.dataPath, "Editor/IOSPostProcessing/MioPostProcessing.py");

			// sanity check
			if( !File.Exists( scriptPath ) )
			{
				UnityEngine.Debug.LogError("Mio post processor could not find the MioPostProcessing.py file at path " + scriptPath + ". Did you accidentally delete it?");
				return;
			}

			var pathToMoPubFolder = Path.Combine( Application.dataPath, "Editor/IOSPostProcessing");

			var args = string.Format( "\"{0}\" \"{1}\" \"{2}\"", scriptPath, pathToBuiltProject, pathToMoPubFolder );
			var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "python2.6",
					Arguments = args,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				}
			};

			proc.Start();
		}
	}
}
