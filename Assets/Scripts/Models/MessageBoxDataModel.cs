using System;

public enum PopUpStyle{
	NONE,
	YES_NO,
	OK
}
namespace Mio.TileMaster {
    public class MessageBoxDataModel {
        public string message;
        public string messageYes;
        public Action OnYesButtonClicked;
        public string messageNo;
        public Action OnNoButtonClicked;
        private string p1;
        private string p2;
        private Action action;
		public PopUpStyle style = PopUpStyle.OK;
        public MessageBoxDataModel(string mess, string msY,Action onY,string meN,Action onN) 
        {
            this.message = mess;
            this.messageYes = msY;
            this.OnYesButtonClicked = onY;
            this.messageNo = meN;
            this.OnNoButtonClicked = onN;
			this.style = PopUpStyle.YES_NO;
        }

        public MessageBoxDataModel()
        {
            // TODO: Complete member initialization
        }
        public MessageBoxDataModel(string mess,Action onyes)
        {
            this.message = mess;
            this.OnYesButtonClicked = onyes;
			this.style = PopUpStyle.OK;
            // TODO: Complete member initialization
        }
    }
}