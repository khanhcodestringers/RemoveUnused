using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
[System.Serializable]
public class CrossPromotion  {
	public string name;
	public string sub;
	public string socical;
	public string textCallBtn;
	public string linkAndroid;
	public string linkIOS;
	public string linkIcon;
	public bool showAndroid;
	public bool showIOS;
	public Sprite icon;

	public void Apply(string s)
	{
		string[] listString = s.Split ('#');
		if (listString.Length >= 9) {
			this.name = listString [0];
			this.sub = listString [1];
			this.socical = listString [2];
			this.textCallBtn = listString [3];
			this.linkAndroid = listString [4];
			this.linkIOS = listString [5];
			this.linkIcon = listString [6];
			if (listString [7] == "0")
				this.showAndroid = true;
			else
				this.showAndroid = false;
			if (listString [8] == "0")
				this.showIOS = true;
			else
				this.showIOS = false;
			
		}
	}
	public string GetLink(){
		string link = this.linkAndroid;
		#if UNITY_IOS
		link = this.linkIOS;
		#endif
		return link;
	}

	public void LoadIcon(Action<Sprite> onCompleted)
	{
		if (string.IsNullOrEmpty (this.linkIcon))
			return;
		//ResourceLoaderManager.Instance.DownloadSprite(this.linkIcon, res =>
		//	{
		//		if (res == null)
		//		{
		//			//load default
		//		}
		//		else
		//		{
		//			onCompleted(res);
		//		}
		//	});
	}
}
public class AllCrossPromotion 
{
	public List<CrossPromotion> data;

	public void InitData(List<string> stringData)
	{
		data = new List<CrossPromotion> (stringData.Count);
		for (int i = 0; i < stringData.Count; i++) {
			CrossPromotion c = new CrossPromotion ();
			c.Apply (stringData [i]);
			data.Add (c);
		}
	}
	public void Remove(CrossPromotion c)
	{
		if (this.data.Contains (c)) {
			this.data.Remove (c);
		}
	}

	public void RemoveWithPlatform(){
		#if UNITY_ANDROID
		for (int i = 0;i  < this.data.Count;i++) {
			if(!this.data[i].showAndroid)
			{
				data.Remove(this.data[i]);
				i--;
			}
		}
		#elif UNITY_IOS
		for (int i = 0;i  < this.data.Count;i++) {
		if(!this.data[i].showIOS)
		{
		data.Remove(this.data[i]);
		i--;
		}
		}
		#endif
	}

}