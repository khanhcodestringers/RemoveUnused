using UnityEngine;
using System.Collections;

public class HideObjectInPlatform : MonoBehaviour {
    [SerializeField]
    private RuntimePlatform[] platformsToHide;
    [SerializeField]
    private GameObject[] objectsToHide;

    // Use this for initialization
    void Awake () {
        if (objectsToHide != null && platformsToHide != null) {
            for (int i = 0; i < objectsToHide.Length; i++) {
                for(int j = 0; j < platformsToHide.Length; j++) {
                    if(Application.platform == platformsToHide[j]) {
                        objectsToHide[i].SetActive(false);
                        break;
                    }
                }
            }
        }
    }

}
