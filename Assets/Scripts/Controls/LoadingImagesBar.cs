using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadingImagesBar : MonoBehaviour {
    [SerializeField]
    private List<UISprite> listImages;
    //[SerializeField]
    //private string offState;
    //[SerializeField]
    //private string onState;

    private float currentProgress = 0;
    public void SetProgress(float p) {
        currentProgress = Mathf.Clamp01(p);
        UpdateProgressBar();
    }

    private void UpdateProgressBar () {
        int numPicture = Mathf.FloorToInt(currentProgress * listImages.Count);
        for(int i = 0; i < numPicture; i++) {
            listImages[i].gameObject.SetActive(true);// = onState;
        }

        for(int i = numPicture; i < listImages.Count; i++) {
            //listImages[i].spriteName = offState;
            listImages[i].gameObject.SetActive(false);
        }
    }

	// Use this for initialization
	void Start () {
        SetProgress(0);
	}
}
