using UnityEngine;
using System.Collections;

public class RateItemView : MonoBehaviour {
    public GameObject starActive;
    private int index;
    public System.Action<int> onStarClick;
    public void RateActive (bool isActive = false) {
        if (isActive)
            starActive.SetActive(true);
        else
            starActive.SetActive(false);
    }

    public void SetIndex (int _index) {
        index = _index;
    }
    public void OnClickStar () {
        if (onStarClick != null)
            onStarClick(index);
    }
}
