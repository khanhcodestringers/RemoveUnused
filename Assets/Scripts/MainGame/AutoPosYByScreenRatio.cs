using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class AutoPosYByScreenRatio : MonoBehaviour {
	void Awake(){
		float ratioDefault = 9.0f / 16;
		float ratio = Screen.width * 1.0f / Screen.height;
		float scale =  ratioDefault/ratio;
		if(scale<0.99f){
			Vector3 vec = transform.localPosition;
			vec.y = scale*vec.y;
			transform.localPosition=vec;
		}
	}
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
}