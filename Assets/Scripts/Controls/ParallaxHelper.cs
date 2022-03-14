using UnityEngine;
using System.Collections;
using MovementEffects;
using System.Collections.Generic;

public class ParallaxHelper : MonoBehaviour {
    [Tooltip("The scroll bar for the object to map movement with background. This scroll bar could be invisible")]
    [SerializeField]
    private UIProgressBar scrollBarForObject;

    private bool shouldUpdate = false;

    void OnDisable () {
        //Debug.Log("no mapping");
        shouldUpdate = false;
    }

    void OnEnable () {
        //Debug.Log("yes mapping");
        Timing.RunCoroutine(C_MapParallaxBackground(0.8f));
    }

    IEnumerator<float> C_MapParallaxBackground (float delay) {
        ParallaxBackground.Instance.MapMovement(scrollBarForObject.value, true, delay);
        yield return Timing.WaitForSeconds(delay);
        shouldUpdate = true;
    }
    
    void Update () {
        if (shouldUpdate) {
            ParallaxBackground.Instance.MapMovement(scrollBarForObject.value);
        }
    }
}
