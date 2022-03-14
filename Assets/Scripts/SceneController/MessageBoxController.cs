using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Mio.TileMaster {
    public class MessageBoxController : SSController {

        [Header("Message")]
        [SerializeField]
        private UILabel lbMessage;

        [Header("Button yes")]
        [SerializeField]
        private UIButton btnYes;
        [SerializeField]
        private UILabel lbYesButton;
        
        [Header("Button no")]
        [SerializeField]
        private UIButton btnNo;
        [SerializeField]
        private UILabel lbNoButton;

        [Header("Button Container")]
        [SerializeField]
        private UIGrid buttonContainer;

        private Transform tf, tfButtonContainer;

        private MessageBoxDataModel data;
        public override void OnSet(object data) {
            tfButtonContainer = buttonContainer.transform;
            tf = tfButtonContainer.parent;

            this.data = (MessageBoxDataModel)data;
            //print("On set messsage: " + this.data.message);
            RefreshUI();
        }

        public override void OnEnable() { 
            

            //RefreshUI();

            base.OnEnable();

            //Test purpose, leave it here
            //testModel = new List<MessageBoxDataModel>(3);
            //MessageBoxDataModel d1 = new MessageBoxDataModel();
            //d1.message = "Yes and no";
            //d1.messageNo = "Fcking no";
            //d1.messageYes = "Fcking yes";
            //testModel.Add(d1);

            //MessageBoxDataModel d2 = new MessageBoxDataModel();
            //d2.message = "Yes only";
            //d2.messageNo = null;
            //d2.messageYes = " yes, Yay";
            //testModel.Add(d2);

            //MessageBoxDataModel d3 = new MessageBoxDataModel();
            //d3.message = "No only";
            //d3.messageNo = "Well, no biatch";
            //d3.messageYes = null;
            //testModel.Add(d3);
        }
        //public bool isTest = false;
        //public List<MessageBoxDataModel> testModel;
        //void Update() {
        //    if (isTest) {
        //        isTest = false;
        //        data = testModel[Random.Range(0, testModel.Count)];

        //        RefreshUI();
        //    }
        //}

        public void RefreshUI() {
            if (data != null) {
                if (!string.IsNullOrEmpty(data.message)) {
                    lbMessage.text = data.message;
                }

                if (!string.IsNullOrEmpty(data.messageYes)) {
                    lbYesButton.text = data.messageYes;
                    btnYes.gameObject.SetActive(true);
                    btnYes.transform.SetParent(tfButtonContainer);
                }
                else {
                    btnYes.transform.SetParent(tf);
                    btnYes.gameObject.SetActive(false);
                }

                if (!string.IsNullOrEmpty(data.messageNo)) {
                    lbNoButton.text = data.messageNo;
                    btnNo.gameObject.SetActive(true);
                    btnNo.transform.SetParent(tfButtonContainer);
                }
                else {
                    btnNo.transform.SetParent(tf);
                    btnNo.gameObject.SetActive(false);
                }

                buttonContainer.enabled = true;
            }
        }

        public void OnNoButtonClicked() {
            //print("No clicked");
            if(data!= null) {
                Helpers.Callback(data.OnNoButtonClicked);
            }
            SceneManager.Instance.CloseScene();
        }

        public void OnYesButtonClicked() {
            //print("No clicked");
            if (data != null) {
                Helpers.Callback(data.OnYesButtonClicked);
            }
            SceneManager.Instance.CloseScene();
        }
    }
}