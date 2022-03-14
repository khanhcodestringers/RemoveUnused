using UnityEngine;
using System.Collections;
using Mio.Utils.MessageBus;

public class ChooseLanguageItemView : MonoBehaviour {

    [SerializeField]
    private UILabel lbLanguage;
    [SerializeField]
    private UISprite selected;
    [SerializeField]
    private GameObject chooseLanuage;
    [SerializeField]
    private UIButton btnChoose;
    [SerializeField]
    private string language;

	public void InitUI (string currLanguage) {
        lbLanguage.text = language;
        if (currLanguage.Equals(language, System.StringComparison.OrdinalIgnoreCase))
        {
            LanguageSelected(true);
        }
        else {
            LanguageSelected(false);
        }
	}
    private void LanguageSelected(bool isSelected) {
        selected.cachedGameObject.SetActive(isSelected);
        chooseLanuage.SetActive(!isSelected);
    }
    public void ChooseLanguageClick(){
        Localization.language = language;
        MessageBus.Annouce(new Message(MessageBusType.LanguageChanged, Localization.language));
   //     ChooseLanguagePopUp.Insta

    }
}
