using UnityEngine;
using System.Collections;

public class RatingOptionFeedbackItem : MonoBehaviour {

    public UIToggle toggle;
    public UILabel lbOption;
    //public string optionValue;

    public UIInput inputText;
    public System.Action OnCheckedValueChanged;

    // Use this for initialization
    void OnEnable () {
        toggle.value = false;
    }

    public void SetTextOption (string key) {
        lbOption.text = Localization.Get(key);
    }

    public void UnCheckToggle () {
        toggle.value = false;
    }

    public void OnToggleValueChanged () {
        //Debug.Log("Toggle changed " + gameObject.name);
        Helpers.Callback(OnCheckedValueChanged);
    }

    public bool IsChecked () {
        return toggle.value;
    }

    public string GetComment () {
        string comment;
        if (inputText != null) {
            comment = inputText.value;
        }
        else {
            comment = lbOption.text;
        }
        return comment;
    }

    public void OnChangeInputText () {
        //Debug.Log("onsubmit");
        if (!toggle.value)
            toggle.value = true;
    }

}
