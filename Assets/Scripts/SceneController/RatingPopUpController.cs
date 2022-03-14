using UnityEngine;
using System.Collections;
//using Parse;
using Mio.TileMaster;
using System.Collections.Generic;
using MovementEffects;
using System;

public class RatingPopUpController : SSController {
    [Header("Stars")]
    //star item
    public RateItemView[] rateItem;
    private int currentRate;

    [Header("Star label")]
    [SerializeField]
    private UILabel starLabel;

    [Header("Check-boxes")]
    public RatingOptionFeedbackItem[] listRatingOption;

    //public RateItemView emptyRate;
    public Animation showFeedbackOption;
    [Header("Button to submit rating")]
    public GameObject btnSendRating;
    public TweenAlpha animButtonSendRating;

    //Parse config
    private const string TABLE_NAME = "RatingFeedbacks";
    private const string STAR_COLUMN = "StarNum";
    private const string DIVICE_NAME = "DiviceName";
    private const string FEEDBACK = "FeedbackUser";
    string linkGame = "";

    public override void OnEnableFS () {
        base.OnEnableFS();

        showFeedbackOption.Play("EnableBtnSend");
        //emptyRate.SetIndex(0);
        //emptyRate.onStarClick += OnStarClick;
        for (int i = 0; i < rateItem.Length; i++) {
            RateItemView rate = rateItem[i];
            rate.SetIndex(i + 1);
            rate.onStarClick -= OnStarClick;
            rate.onStarClick += OnStarClick;
        }

        for (int i = 0; i < listRatingOption.Length; i++) {
            RatingOptionFeedbackItem item = listRatingOption[i];
            item.OnCheckedValueChanged -= OnFeedbackOptionChanged;
            item.OnCheckedValueChanged += OnFeedbackOptionChanged;
        }
    }

    private void OnFeedbackOptionChanged () {
        Timing.RunCoroutine(C_ShowHideSendButton());
    }

    private void OnStarClick (int index) {
        if (currentRate == index)
            return;
        Rating(index);
    }

    public override void OnEnable () {
        base.OnEnable();
        //btnRating.SetActive(false);
        for (int i = 0; i < listRatingOption.Length; i++) {
            listRatingOption[i].UnCheckToggle();
            //listRatingOption[i].SetTextOption("ratingoption_" + i);
        }
        Rating(0);
    }

    // this variable is used only for this coroutine, do not delete
    private bool isProcessingShowHideButton = false;
    /// <summary>
    /// Show 'send rating button' only if there is rating option checked
    /// </summary>
    private IEnumerator<float> C_ShowHideSendButton () {
        if (isProcessingShowHideButton) {
            yield break; 
        }else {
            isProcessingShowHideButton = true;
            yield return 1;
        }

        bool shouldShowRateButton = false;
        for (int i = 0; i < listRatingOption.Length; i++) {
            if (listRatingOption[i].IsChecked() == true) {
                shouldShowRateButton = true;
                break;
            }
        }

        if (shouldShowRateButton) {
            btnSendRating.SetActive(true);
            animButtonSendRating.PlayForward();
        }else {
            animButtonSendRating.PlayReverse();
        }
        yield return Timing.WaitForSeconds(animButtonSendRating.duration);
        btnSendRating.SetActive(shouldShowRateButton);

        yield return 1;
        isProcessingShowHideButton = false;
    }

    //show/hide star for rating
    private void Rating (int rateNum) {
        for (int i = 0; i < rateItem.Length; i++) {
            if (i <= rateNum - 1) {
                rateItem[i].RateActive(true);
            }
            else {
                rateItem[i].RateActive(false);
            }
        }
        currentRate = rateNum;
        if (rateNum > 0) {
            if (currentRate >= 4) {
                Timing.RunCoroutine(C_OpenStoreLinkAndClosePopup());
            }
            else {
                ShowFeedBackOption();
            }

            starLabel.text = Localization.Get("pu_rate_star" + rateNum);
        }else {
            starLabel.text = "";
        }        
    }

    IEnumerator<float> C_OpenStoreLinkAndClosePopup () {
        yield return Timing.WaitForSeconds(0.5f);
        Application.OpenURL(linkGame);
        SceneManager.Instance.CloseScene();
    }

    //click "Rate" btn after choose star
    public void RatingClick () {
        //<=4 star open game in AppStore or GoogleStore.
        if (currentRate >= 4) {
            linkGame = "https://play.google.com/store/apps/details?id=com.mio.ptnw";
#if UNITY_TIZEN
            linkGame = "https://play.google.com/store/apps/details?id=com.mio.tilemaster";
#elif UNITY_WSA
            linkGame = "https://www.microsoft.com/en-us/store/p/piano-tiles-new-waves/9nc9rz2mwls9";
#elif UNITY_IOS
            linkGame = "https://itunes.apple.com/us/app/piano-tiles-new-waves/id1212004383?ls=1&mt=8";
#endif
            Application.OpenURL(linkGame);
            SceneManager.Instance.CloseScene();
        }
        else {
            //>=3 star show feedback
            //btnRating.SetActive(false);
            ShowFeedBackOption();
        }
    }

    public void ShowFeedBackOption () {
        //play Animation show feedback popup
        showFeedbackOption.Play();
    }

    // save feedback in Parse
    public void SendFeedbackClick () {
        showFeedbackOption.Play("WaitForPushToParseAnm");
        animButtonSendRating.PlayReverse();
        Timing.RunCoroutine(C_DelayVisibleRateButton(animButtonSendRating.duration, false));

        string feedBack = "";

        for (int i = 0; i < listRatingOption.Length; i++) {
            if (listRatingOption[i].IsChecked() == true) {
                if (string.IsNullOrEmpty(feedBack))
                    feedBack += listRatingOption[i].GetComment();
                else
                    feedBack += "\n" + listRatingOption[i].GetComment();
            }
        }

        //ParseObject uf = new ParseObject(TABLE_NAME);
        //uf[STAR_COLUMN] = currentRate;
        //uf[DIVICE_NAME] = SystemInfo.deviceModel;
        //uf[FEEDBACK] = feedBack;
        //uf["Email"] = ProfileHelper.Instance.Email;
        //uf["GraphicsDeviceName"] = SystemInfo.graphicsDeviceName;
        //uf["GraphicsVendor"] = SystemInfo.graphicsDeviceVendor;
        //uf["GraphicsMemory"] = SystemInfo.graphicsMemorySize;
        //uf["GraphicsDeviceVersion"] = SystemInfo.graphicsDeviceVersion;
        //uf["OS"] = SystemInfo.operatingSystem;
        //uf["CPUs"] = SystemInfo.processorCount;
        //uf["CPUtype"] = SystemInfo.processorType;
        //uf["CPUspeed"] = SystemInfo.processorFrequency;
        //uf["RAM"] = SystemInfo.systemMemorySize;
        //uf.SaveAsync();
        //Debug.Log("Save feedback");
        Timing.RunCoroutine(IECloseScene());
    }

    //public void SaveTestFeedback () {
    //    ParseObject uf = new ParseObject(TABLE_NAME);
    //    uf[STAR_COLUMN] = 69;
    //    uf[DIVICE_NAME] = SystemInfo.deviceModel;
    //    uf[FEEDBACK] = "Kamasutra";
    //    uf["Email"] = ProfileHelper.Instance.Email;
    //    uf["GraphicsDeviceName"] = SystemInfo.graphicsDeviceName;
    //    uf["GraphicsVendor"] = SystemInfo.graphicsDeviceVendor;
    //    uf["GraphicsMemory"] = SystemInfo.graphicsMemorySize;
    //    uf["GraphicsDeviceVersion"] = SystemInfo.graphicsDeviceVersion;
    //    uf["OS"] = SystemInfo.operatingSystem;
    //    uf["CPUs"] = SystemInfo.processorCount;
    //    uf["CPUtype"] = SystemInfo.processorType;
    //    uf["CPUspeed"] = SystemInfo.processorFrequency;
    //    uf["RAM"] = SystemInfo.systemMemorySize;
    //    uf.SaveAsync();
    //}

    //public void SaveTestParseObject () {
    //    ParseObject po = new ParseObject("SASADA");
    //    po["Name"] = "PO";
    //    po["Type"] = "Panda";
    //    po.SaveAsync();
    //}

    private IEnumerator<float> IECloseScene () {
        yield return Timing.WaitForSeconds(1.5f);
        CloseScene();
    }

    private IEnumerator<float> C_DelayVisibleRateButton (float delay, bool visible) {
        yield return Timing.WaitForSeconds(delay);
        btnSendRating.SetActive(visible);
    }

    public void CloseScene () {
        SceneManager.Instance.CloseScene();

    }
}
