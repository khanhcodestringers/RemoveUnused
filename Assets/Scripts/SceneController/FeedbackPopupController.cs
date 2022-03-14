using UnityEngine;
using System.Collections;

namespace Mio.TileMaster {
    public class FeedbackPopupController : SSController {
        [SerializeField]
        private UILabel title;
        [SerializeField]
        private UILabel subTitle;
        [SerializeField]
        private UIInput txtMessage;
        [SerializeField]
        private UIInput txtEmail;
        public Animation anim;

        //[Header("Button Submit")]
        [SerializeField]
        private UILabel lbSubmitButton;
        [SerializeField]
        private UIButton btnSubmit;

        private Transform tf;
        private FeedbackDataModel model;

        private WaitForSeconds waitBeforeDone = new WaitForSeconds(1f);
        private WaitForSeconds waitBeforeClose = new WaitForSeconds(0.25f);

        public override void OnSet (object data) {
            base.OnSet(data);
            if (data != null) {
                model = (FeedbackDataModel)data;
                if (model != null) {
                    lbSubmitButton.enabled = true;
                    title.text = model.title;
                    subTitle.text = model.subtitle;
                    lbSubmitButton.text = model.buttonLabel;
                    txtMessage.value = "";
                }
            }
        }

        public void OnSubmitButtonClicked () {
            if (ValidationMessage()) {
                SubmitMessage();
                StartCoroutine(CoroutineCloseScene());
            }
        }

        private IEnumerator CoroutineCloseScene () {
            btnSubmit.enabled = false;
            //lbSubmitButton.text = "Sending message...";
            lbSubmitButton.text = Localization.Get("pu_requestsong_btn_submit");
            yield return waitBeforeDone;

            //lbSubmitButton.text = "Done";
            lbSubmitButton.text = Localization.Get("pu_requestsong_btn_done");
            yield return waitBeforeClose;

            SceneManager.Instance.CloseScene();
        }

        private void SubmitMessage () {
            string message = txtMessage.value;
            string email = txtEmail.value;

            //var parse = new Parse.ParseObject(model.type);
            //parse["message"] = message;
            //parse["email"] = email;
            //parse["deviceID"] = GameManager.Instance.DeviceID;
            //parse["deviceModel"] = SystemInfo.deviceModel;
            //parse["deviceName"] = SystemInfo.deviceName;
            //if (Parse.ParseUser.CurrentUser != null) {
            //    parse["userID"] = Parse.ParseUser.CurrentUser.ObjectId;
            //}

            //parse.SaveAsync();
        }

        private bool ValidationMessage () {
            bool res = true;

            if (txtMessage.value.Length < 8) {
                //txtMessage.defaultText = "Message must be longer than 8 characters";
                txtMessage.defaultText = Localization.Get("pu_requestsong_megss");
                txtMessage.defaultColor = Color.red;

                res = false;
            }

            if (txtEmail.value != null && txtEmail.value.Length > 3) {
                if (!txtEmail.value.Contains("@") || !txtEmail.value.Contains(".")) {
                    //txtEmail.defaultText = "Email is invalid";
                    txtEmail.defaultText = Localization.Get("pu_requestsong_email_megss"); ;
                    txtEmail.defaultColor = Color.red;

                    res = false;
                }
            }

            return res;
        }

        public void OnCloseButtonClicked () {
            StopAllCoroutines();
            SceneManager.Instance.CloseScene();
        }
    }
}