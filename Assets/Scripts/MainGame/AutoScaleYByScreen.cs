using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class AutoScaleYByScreen : MonoBehaviour {
	public bool forceX=false;
	void Awake(){
		float ratioDefault = 9.0f / 16;
		float ratio = Screen.width * 1.0f / Screen.height;
		float scale =  ratioDefault/ratio;
		if(scale<0.99f){
			Vector3 vec = transform.localScale;
			if (forceX) {
				vec.x = scale;
			}
			vec.y = scale;
			transform.localScale=vec;
		}
	}
}
