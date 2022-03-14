using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SSApplication
{
	public delegate void OnLoadedDelegate(GameObject root);

	private static Dictionary<string, OnLoadedDelegate> m_OnLoaded = new Dictionary<string, OnLoadedDelegate>();

    public static void LoadLevel(string sceneName, bool isAsync = false, bool isAdditive = false, OnLoadedDelegate onLoaded = null)
	{
		if (m_OnLoaded.ContainsKey (sceneName)) 
		{
			Debug.LogWarning ("Loaded this scene before. Please check again.");
			return;
		}

		m_OnLoaded.Add(sceneName, onLoaded);

		if (!isAsync)
		{
            if (isAdditive)
                //Application.LoadLevelAdditive (sceneName);
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            else
                //Application.LoadLevel(sceneName);
                SceneManager.LoadScene(sceneName);
        }
		else
		{
            if (isAdditive)
                //Application.LoadLevelAdditiveAsync (sceneName);
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            else
                //Application.LoadLevelAsync (sceneName);
                SceneManager.LoadSceneAsync(sceneName);
        }
	}

	public static void OnLoaded(GameObject root)
	{
       //Debug.Log("Onloaded: " + root.name);
        //Debug.Log("onload: " + m_OnLoaded);
		if (m_OnLoaded[root.name] != null)
		{
			m_OnLoaded[root.name] (root);
		}
        //Debug.Log("Onloaded 2: " + root.name);
    }

	public static void OnUnloaded(GameObject root)
	{
		if (m_OnLoaded.ContainsKey(root.name))
		{
			m_OnLoaded.Remove (root.name);
		}
	}
}
