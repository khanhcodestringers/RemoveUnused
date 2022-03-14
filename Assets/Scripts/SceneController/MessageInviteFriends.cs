using UnityEngine;
using System.Collections;
using System;

namespace Mio.TileMaster
{
	public class MessageInviteFriends : SSController
	{
		public UILabel lbtitle;
		MessageBoxDataModel mess;
		//public Action<string> onAction;
		public GameObject okBtn;
		public GameObject yesBtn;
		public GameObject noBtn;
		public UILabel yesLb;
		public UILabel noLb;


		public override void OnSet(object data)
		{
			if (data != null)
			{
				mess = (MessageBoxDataModel)data;
				lbtitle.text = mess.message;
				if (mess.style == PopUpStyle.YES_NO) {
					okBtn.SetActive (false);
					yesBtn.SetActive (true);
					noBtn.SetActive (true);
					yesLb.text = mess.messageYes;
					noLb.text = mess.messageNo;

				} else {
					okBtn.SetActive (true);
					yesBtn.SetActive (false);
					noBtn.SetActive (false);
				}
			}           
		}

		public void Close()
		{
			Helpers.Callback(mess.OnYesButtonClicked);
			SceneManager.Instance.CloseScene();
		}

		public void OnYesClick(){
			Helpers.Callback(mess.OnYesButtonClicked);
		}
		public void OnNoClick(){
			Helpers.Callback(mess.OnNoButtonClicked);
		}
	}
}
