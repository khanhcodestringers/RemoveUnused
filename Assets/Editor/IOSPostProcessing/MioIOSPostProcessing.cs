using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;


public static class MioIOSPostProcessing
{
	static string[] FRAMEWORKS_PATH = new string[]{"$(PROJECT_DIR)/Frameworks","$(PROJECT_DIR)"};
	 
	[PostProcessBuild(99999)]
	public static void OnPostProcessBuild( BuildTarget buildTarget, string path )
	{
		if(buildTarget == BuildTarget.iOS)
		{
			string projectPath = path+"/Unity-iPhone.xcodeproj/project.pbxproj";

			// Create a new project object from build target
			PBXProject project = new PBXProject(  );
			var file = File.ReadAllText(projectPath);
			project.ReadFromString(file);

			string target = project.TargetGuidByName("Unity-iPhone");

//			for (int i = 0; i < FRAMEWORKS_PATH.Length; i++) {
//				project.AddBuildProperty (target, "FRAMEWORK_SEARCH_PATHS", FRAMEWORKS_PATH [i]);
//			}
			project.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
			//project.SetBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");


			// Finally save the xcode project
			File.WriteAllText(projectPath, project.WriteToString());
		}
	}
}
#endif