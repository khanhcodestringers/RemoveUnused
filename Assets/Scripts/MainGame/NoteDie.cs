using UnityEngine;
using System.Collections;

public class NoteDie : MonoBehaviour {
	public SpriteRenderer sprite;
	public void Setup(float height){
		Vector3 scale = sprite.transform.localScale;
		scale.y = (int)(height/480.0f*100);
		sprite.transform.localScale = scale;

	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
