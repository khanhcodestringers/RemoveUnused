using UnityEngine;
using System.Collections;

public class AutoScroll : MonoBehaviour {
    public UIScrollView scrollView;
    [Tooltip("Unit per seconds")]
    public float scrollSpeed = 0.05f;    
	
	// Update is called once per frame
	void Update () {
        scrollView.Scroll(scrollSpeed);
	}
}
