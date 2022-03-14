using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AchievementBanner : MonoBehaviour {
    public Text lbTitle;
    public Text lbDescription;

	public void ShowAchievement(string title, string description) {
        lbDescription.text = description;
        lbTitle.text = title;
        gameObject.SetActive(true);
        Invoke("Hide", 2);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
