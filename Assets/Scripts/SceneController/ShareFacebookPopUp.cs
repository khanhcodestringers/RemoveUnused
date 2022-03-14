using UnityEngine;
using System.Collections;
using Mio.Utils.MessageBus;
using Mio.TileMaster;
using Mio.Utils;

public class ShareFacebookPopUp : SSController {
    [SerializeField]
    private UIInput inputComment;
    [SerializeField]
    private UI2DSprite uiScreenShot;
    //[SerializeField]
    //private UILabel lbStatus;
    public override void OnSet (object data) {
        if (data != null) {
            byte[] screenShot = (byte[])data;
            Texture2D text = new Texture2D(1, 1);
            text.LoadImage(screenShot);

            Sprite sprite = Sprite.Create(text, new Rect(0, 0, text.width, text.height), Vector2.zero);
            uiScreenShot.sprite2D = sprite;
        }
    }

    public void SendUserMessageButtonClick () {
        string status = inputComment.value;
        MessageBus.Annouce(new Message(MessageBusType.CompletedPostStatusShareFacebook, status));
        SceneManager.Instance.CloseScene();

    }

    public void SendCancel () {
        MessageBus.Annouce(new Message(MessageBusType.CancelPostStatusShareFacebook));
        SceneManager.Instance.CloseScene();
    }
}
