//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using Facebook.Unity;
//using Facebook.MiniJSON;
////using UnityEngine.UI;
//namespace Amanotes.Utils {

//    public enum SocialUIStatus {
//        NoneLogin = 1,
//        ProcessLogin = 2,
//        Logined = 3
//    }

//    public class FBFriend {
//        public string id;
//        public string name;
//        public FBFriend () {
//        }
//        public FBFriend (string _id, string _name) {
//            this.id = _id;
//            this.name = _name;
//        }
//    }
//    public class FBFriendQuery {
//        public List<FBFriend> data;
//        public FBFriendQuery () {

//        }
//    }
//    public class UserScoreInfo {
//        public string name;
//        public long id;
//        public UserScoreInfo () {

//        }
//    }

//    public class UserScoreElement {
//        public UserScoreInfo user;
//        public int score;
//        public UserScoreElement () {

//        }
//    }
//    public class AllUserScore {
//        public List<UserScoreElement> data;
//        public AllUserScore () {

//        }
//    }
//    public class CustomSortScore : IComparer<UserScoreElement> {
//        public int Compare (UserScoreElement x, UserScoreElement y) {
//            if (x.score > y.score) {
//                return -1;
//            }
//            else if (x.score < y.score) {
//                return 1;
//            }
//            return 0;
//        }
//    }
//    public enum FacebookCallbackType {
//        NullResponse = 0,
//        ErrorResponse = 1,
//        CancelResponse = 2,
//        EmptyResponse = 3,
//        NotEnoughPermissions = 4,
//        SuccessResponse = 5
//    }

//    public class CallbackFromFacebook {
//        public int result_code;
//        public IResult result;
//        public CallbackFromFacebook () {

//        }
//        public CallbackFromFacebook (int _result_code, IResult _result) {
//            result_code = _result_code;
//            result = _result;
//        }
//    }

//    public class FacebookManager : MonoSingleton<FacebookManager> {
//        public Action onSuccedInitFB;

//        public void Init () {
//            if (!FB.IsInitialized) {
//                FB.Init(OnInitComplete, OnHideUnity);
//            }
//            else {

//                Debug.LogError("oplab Facebook Activate App1");
//                FB.ActivateApp();
//            }
//        }

//        public bool IsLogin () {
//            if (!FB.IsInitialized) {
//                Init();
//            }
//            return FB.IsLoggedIn;
//        }

//        public Action<CallbackFromFacebook> callbackLogin = null;

//        public void FBLogin (Action<CallbackFromFacebook> callback = null) {
//            callbackLogin = callback;
//            if (FB.IsInitialized) {
//                FB.LogInWithReadPermissions(new List<string>() { "email", "public_profile", "user_friends" }, CallbackFBLogin);
//            }
//        }
//        private void CallbackFBLogin (IResult result) {
//            if (result == null) {
//                Debug.Log("#101 Null Response");
//                //GUIMessageDialog.Show("an error occurred (#101), please try again later !","Error",true);
//                if (callbackLogin != null) {
//                    callbackLogin(new CallbackFromFacebook(-101, result));
//                }
//                return;
//            }
//            // Some platforms return the empty string instead of null.
//            if (!string.IsNullOrEmpty(result.Error)) {
//                Debug.Log("#102 Error Response: " + result.Error);
//                if (callbackLogin != null) {
//                    callbackLogin(new CallbackFromFacebook(-101, result));
//                }
//                //GUIMessageDialog.Show("an error occurred (#102), please try again later !","Error",true);
//            }
//            else if (result.Cancelled) {
//                Debug.Log("#100 Cancelled Response: " + result.RawResult);
//                //Xu ly khi user bam cancel
//                if (callbackLogin != null) {
//                    callbackLogin(new CallbackFromFacebook(-100, result));
//                }

//            }
//            else if (!string.IsNullOrEmpty(result.RawResult)) {
//                //				Debug.Log("Success Response: " + result.RawResult);
//                #region Xin them quyen post permistion
//                /*
//				var dic = Json.Deserialize(result.RawResult) as Dictionary<string,object>;
//				//FB.API("/me?fields=first_name", HttpMethod.GET, CallbackGetInformation);
//				InitInformationFacebook();
//				if(!dic["permissions"].ToString().Contains("publish_actions"))
//				{
//					//Xin cap quyen publish_actions
//					RequestPublishAction();
//				}
//				else
//				{
//					Debug.LogError("OPLab AccessToken:"+Facebook.Unity.AccessToken.CurrentAccessToken.TokenString);
//					if(callbackLogin!=null)
//					{
//						callbackLogin(new CallbackFromFacebook(1,result));
//					}
//				}*/

//                //request additional data
//                InitInformationFacebook();
//                #endregion
//                if (callbackLogin != null) {
//                    callbackLogin(new CallbackFromFacebook(1, result));
//                }
//            }
//            else {
//                if (callbackLogin != null) {
//                    callbackLogin(new CallbackFromFacebook(-1, result));
//                }
//            }
//        }


//        public void InitInformationFacebook (Action<IGraphResult> callback = null) {
//            //AccessToken.CurrentAccessToken.
//            // Get Information User
//            FB.API("/me?fields=name,picture,email,cover", HttpMethod.GET, resGetInfo => {
//                var res = Json.Deserialize(resGetInfo.RawResult) as Dictionary<string, object>;
//                //				Debug.LogError("CallbackGetInformation:"+resGetInfo.RawResult);
//                if (res.ContainsKey("name")) {
//                    UserDisplayName = res["name"] as string;
//                    IsUserInfoAvailable = true;
//                }

//                if (res.ContainsKey("email")) {
//                    UserEmail = res["email"] as string;
//                    IsUserInfoAvailable = true;
//                }

//                if (res.ContainsKey("picture")) {
//                    Dictionary<string, object> pictureData = res["picture"] as Dictionary<string, object>;
//                    Dictionary<string, object> actualPictureData = pictureData["data"] as Dictionary<string, object>;
//                    AvatarURL = actualPictureData["url"] as string;
//                    IsUserInfoAvailable = true;
//                }

//                if (res.ContainsKey("cover")) {
//                    Dictionary<string, object> pictureData = res["cover"] as Dictionary<string, object>;
//                    CoverURL = pictureData["source"] as string;
//                    IsUserInfoAvailable = true;
//                }

//                if (IsUserInfoAvailable) {
//                    MessageBus.MessageBus.Annouce(new MessageBus.Message(MessageBusType.FacebookUserDataReceived));
//                }

//                if (callback != null) {
//                    callback(resGetInfo);
//                }

//            });

//        }

//        public bool CheckHavePublishActionPermision () {
//            string json = Facebook.Unity.AccessToken.CurrentAccessToken.ToJson();
//            if (json.Contains("publish_actions,") || json.Contains(",publish_actions")) {
//                if (!PianoChallenge.GameConsts.BUILD_LIVE) {
//                    //Debug.Log("Have publish_actions permission");
//                    return true;
//                }
//            }
//            if (!PianoChallenge.GameConsts.BUILD_LIVE) {
//                Debug.LogError("Don't have publish_actions permission");
//            }
//            return false;
//        }
//        public void RequestPublishAction (Action<CallbackFromFacebook> publishActionCallback = null) {
//            if (publishActionCallback != null) {
//                callbackLogin = publishActionCallback;
//            }
//            //Debug.Log("Request Publish Action");
//            FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, CallbackPublishAction);
//        }
//        public void CallbackPublishAction (IResult result) {
//            //Debug.Log("Response Publish Action");
//            if (result == null) {
//                Debug.Log("#103 Null Response");
//                if (callbackLogin != null) {
//                    callbackLogin(new CallbackFromFacebook(-103, result));
//                }
//                return;
//            }
//            // Some platforms return the empty string instead of null.
//            if (!string.IsNullOrEmpty(result.Error)) {
//                Debug.LogError("#104 Error Response: " + result.Error);
//                if (callbackLogin != null) {
//                    callbackLogin(new CallbackFromFacebook(-104, result));
//                }
//            }
//            else if (result.Cancelled) {
//                Debug.Log("Cancelled Response: " + result.RawResult);
//                //Xu ly khi user bam cancel
//                if (callbackLogin != null) {
//                    callbackLogin(new CallbackFromFacebook(-100, result));
//                }
//            }
//            else if (!string.IsNullOrEmpty(result.RawResult)) {
//                //Debug.Log("OPLab AccessToken Post Permision:" + Facebook.Unity.AccessToken.CurrentAccessToken.TokenString);
//                //finish
//                if (callbackLogin != null) {
//                    callbackLogin(new CallbackFromFacebook(1, result));
//                }
//                /*
//				Debug.Log("Success Response: " + result.RawResult);
//				//Submit score
//				int highScore = ControlData.GetHighScore();
//				//FacebookManager.SubmitScore(highScore);
//				if(typeRequest == 0){
//					//Show LeaderBoard
//					LeaderBoardManager.Instance.Init();
//				}else if(typeRequest == 1){
//					// Share FB
//					SystemCandy.Instance.ClickShare();
//				}*/
//            }
//            else {
//                Debug.Log("Empty Response");
//                if (callbackLogin != null) {
//                    callbackLogin(new CallbackFromFacebook(-1, result));
//                }
//            }
//        }

//        public void SubmitScore (int _score, Action<bool> callback) {
//            int score = _score;
//            var ScoreData = new Dictionary<string, string>() { { "score", score.ToString() } };
//            if (FB.IsLoggedIn) {
//                FB.API("/me/scores", HttpMethod.POST, delegate (IGraphResult r) {
//                    //Debug.LogError ("Submit Score Result: " + r.RawResult);
//                    //Debug.Log("Submit Score Result: " + r.RawResult);
//                    if (callback != null) {
//                        callback(true);
//                    }
//                }, ScoreData);
//            }
//        }

//        public void RequestGetScore () {
//            FB.API("/" + FB.AppId + "/scores?fields=score,user.limit(50)", HttpMethod.GET, CallbackGetScore);
//        }

//        private void CallbackGetScore (IGraphResult r) {
//            //var dict = r.ResultDictionary as IDictionary;
//            //LeaderBoardManager.Instance.SetScoreBoard (r.RawResult);
//        }

//        public void ShowFBInviteDialog (string url, string imageurl = null) {
//            FB.Mobile.AppInvite(new Uri(url), new Uri(imageurl));
//        }

//        /*
//public void GetProfilePicture (string userId, Action<string,Texture2D> callback) {
//   StartCoroutine (Helper.GetFile (userId, (tex)=>{
//       callback (userId, tex);

//       bool isHaveLocal = false;
//       if (tex != null) {
//           isHaveLocal = true;
//       }

//       bool isUpdateAvatar = true;

//       if (isUpdateAvatar) {
//           Dictionary<string,string> formData = new Dictionary<string, string> ();
//           formData.Add ("type", "large");

//           FB.API ("/" + userId + "/picture", HttpMethod.GET, delegate(IGraphResult result) {
//               if (string.IsNullOrEmpty (result.Error)) {
//                   Helper.SaveToFile (result.Texture, userId);
//                   if (callback != null && isUpdateAvatar) {
//                       callback (userId, result.Texture);
//                   }
//               }
//           }, formData);
//       }
//   }));
//}*/

//        public void Share (string toId, string link, string linkName, string linkCaption, string linkDescription, string picture, Action<bool> callback) {
//            FB.FeedShare(
//                toId,
//                new Uri(link),
//                linkName,
//                linkCaption,
//                linkDescription,
//                new Uri(picture),
//                "",
//                delegate (IShareResult result) {
//                    if (string.IsNullOrEmpty(result.Error)) {
//                        if (callback != null) {
//                            callback(true);
//                        }
//                    }
//                    else {
//                        if (callback != null) {
//                            callback(false);
//                        }
//                    }
//                }
//            );
//        }


//        public void Invite (List<string> idList, System.Action<bool> callback) {
//            if (idList == null || idList.Count == 0 || !FB.IsLoggedIn) return;

//            FB.AppRequest("Play fish hunt with me!", idList.ToArray(), null, null, null, "INVITE", "Fish hunt", result => {
//                if (callback != null)
//                    callback(result.Error == null);
//            });
//        }

//        public void FBLogout () {
//            FB.LogOut();
//        }
//        public bool FBInitComplete () {
//            return FB.IsInitialized;
//        }

//        public void OnInitComplete () {
//            //Debug.LogError("oplab Facebook Activate App2");
//            FB.ActivateApp();
//            if (onSuccedInitFB != null)
//                onSuccedInitFB();
//        }

//        public void OnHideUnity (bool isGameShown) {

//        }

//        #region Share ScreenShot
//        public void ShareScreen (Texture2D texture, string message, System.Action<bool> callback) {
//            StartCoroutine(ShareScreenRoutine(texture, message, callback));
//        }
//        public IEnumerator ShareScreenRoutine (Texture2D texture, string message, System.Action<bool> callback) {
//            yield return new WaitForSeconds(0.05f);
//            byte[] screenshot = texture.EncodeToPNG();

//            var wwwForm = new WWWForm();
//            wwwForm.AddBinaryData("image", screenshot, "amanotes.png");
//            wwwForm.AddField("message", message);

//            FB.API("me/photos", Facebook.Unity.HttpMethod.POST, obj => {
//                if (callback != null)
//                    callback(obj.Error == null);
//                Debug.LogError("Upload Photo Result:" + obj.RawResult);
//            }, wwwForm);
//        }

//        public void ShareFeedWithScreenShot (string message, System.Action<bool> callback, float time = 1) {
//            StartCoroutine(ShareFeedWithScreenShotWait(message, callback, time));
//        }

//        IEnumerator ShareFeedWithScreenShotWait (string message, System.Action<bool> callback, float time) {
//            yield return new WaitForSeconds(time);
//            StartCoroutine(TakeScreenshotToFeed(message, callback));
//        }

//        private IEnumerator TakeScreenshotToFeed (string message, System.Action<bool> callback) {
//            //AvUIManager.instance.ShowDialog(DialogName.LoadingTransparent);
//            yield return new WaitForEndOfFrame();

//            var width = Screen.width;
//            var height = Screen.height;
//            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
//            // Read screen contents into the texture
//            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
//            tex.Apply();
//            byte[] screenshot = tex.EncodeToPNG();

//            var wwwForm = new WWWForm();
//            wwwForm.AddBinaryData("image", screenshot, "amanotes.png");
//            wwwForm.AddField("message", message);
//            //Debug.LogError(message);
//            //AvUIManager.instance.HideDialog(DialogName.LoadingTransparent);
//            //AvUIManager.instance.ShowDialog(DialogName.Loading);

//            //chup hinh post

//            FB.API("me/photos", Facebook.Unity.HttpMethod.POST, obj => {

//                //AvUIManager.instance.HideDialog(DialogName.Loading);
//                if (callback != null)
//                    callback(obj.Error == null);
//                Debug.Log("Upload Photo Result:" + obj.RawResult);
//                //Dictionary<string, object> dic = (Dictionary<string, object>)MiniJSON.Json.Deserialize(fb.Text);
//                //string imageUrl ="http://graph.facebook.com/"+profile.id+"/photos/"+dic["id"].ToString();//"https://www.facebook.com/photo.php?fbid="+dic["id"].ToString();


//            }, wwwForm);
//        }
//        public void FeedShare (string url_link, string title, string caption, string description, string url_image, Action<IShareResult> callback = null) {
//            FB.FeedShare(
//                string.Empty,
//                new Uri(url_link),
//                title,
//                caption,
//                description,
//                new Uri(url_image),
//                string.Empty,
//                res => {
//                    if (!PianoChallenge.GameConsts.BUILD_LIVE) {
//                        Debug.LogError("FeedShare result:" + res.RawResult);
//                    }
//                });
//        }

//        public void ShareFeedWithScreenShot(string message, byte[] screenshot, System.Action<bool> callback, float time = 1) {
//            StartCoroutine(ShareFeedWithScreenShotWait(message,screenshot, callback, 1));
//        }

//        private IEnumerator ShareFeedWithScreenShotWait(string message, byte[] screenshot,  System.Action<bool> callback, float time) {
//            yield return new WaitForSeconds(time); 

//            var wwwForm = new WWWForm();
//            wwwForm.AddBinaryData("image", screenshot, "amanotes.png");
//            wwwForm.AddField("message", message);
//            //Debug.LogError(message);
//            //AvUIManager.instance.HideDialog(DialogName.LoadingTransparent);
//            //AvUIManager.instance.ShowDialog(DialogName.Loading);

//            //chup hinh post

//            FB.API("me/photos", Facebook.Unity.HttpMethod.POST, obj => {

//                //AvUIManager.instance.HideDialog(DialogName.Loading);
//                if(callback != null)
//                    callback(obj.Error == null);
//                //Debug.Log("Upload Photo Result:" + obj.RawResult);
//                //Dictionary<string, object> dic = (Dictionary<string, object>)MiniJSON.Json.Deserialize(fb.Text);
//                //string imageUrl ="http://graph.facebook.com/"+profile.id+"/photos/"+dic["id"].ToString();//"https://www.facebook.com/photo.php?fbid="+dic["id"].ToString();


//            }, wwwForm);
//        }


//        #endregion


//        #region Init Friend Facebook
//        public void InitFriendFBList (Action<FBFriendQuery> callback = null) {
//            FB.API("me/friends?fields=id,name&limit=100", HttpMethod.GET, resGetInfo => {
//                Debug.LogError("Friends:" + resGetInfo.RawResult);
//                //List<string> listFriend=new List<string>();
//                if (!resGetInfo.Cancelled && string.IsNullOrEmpty(resGetInfo.Error)) {
//                    try {
//                        string json = resGetInfo.RawResult;
//                        //FBFriendQuery query = Pathfinding.Serialization.JsonFx.JsonReader.Deserialize(json, typeof(FBFriendQuery)) as FBFriendQuery;
//                        FBFriendQuery query = null;
//                        FullSerializer.fsData jsonData = new FullSerializer.fsData(json);
//                        var res = FileUtilities.JSONSerializer.TryDeserialize(jsonData, ref query);
//                        if (res.Succeeded) {
//                            if (callback != null) {
//                                callback(query);
//                            }
//                        }
//                    }
//                    catch (System.Exception ex) {
//                        Debug.LogError("Exception query friend facebook:" + ex.Message);
//                    }
//                }
//                else {
//                    Debug.LogWarning("Get friend list is NULL or EMPTY");
//                }
//            });
//        }
//        #endregion

//        #region Get Avatar By User ID
//        /// <summary>
//        /// Change the avatar FB with User ID
//        /// </summary>
//        /// <param name="UserId">User ID of FB.</param>
//        /// <param name="targetImage">Target image that you use</param>
//        //public void ChangeAvatarFBWithUserID (string UserId, Image targetImage) {
//        //    if (UserId.Length > 0) {
//        //        //if (ResourceLoaderManager.Instance.CheckExistCacheFile(UserId)) {
//        //        //    // co cache
//        //        //    string path = ResourceLoaderManager.Instance.GetPathFileCache(UserId);
//        //        //    ResourceLoaderManager.Instance.DownloadSprite(path, res => {
//        //        //        if (res != null) {
//        //        //            targetImage.sprite = res;
//        //        //        }
//        //        //    });
//        //        //}
//        //        //else {
//        //        //    string path = "http://graph.facebook.com/" + UserId + "/picture?type=square";
//        //        //    ResourceLoaderManager.Instance.DownloadSprite(path, res => {
//        //        //        if (res != null) {
//        //        //            targetImage.sprite = res;
//        //        //            //save cache
//        //        //            ResourceLoaderManager.Instance.SaveToLocalCacheFile(res.texture, UserId);
//        //        //        }
//        //        //    });
//        //        //}
//        //    }
//        //}
//        #endregion

//        #region Invite Facebook Friend

//        public void OnInviteFriends (Action<IAppInviteResult> callback = null) {
//            if (FB.IsLoggedIn) {
//                //Debug.LogError("oplab OnInviteFriends Facebook da login, call show OpenDialogInviteFacebookFriend");
//                OpenDialogInviteFacebookFriend(callback);
//            }
//        }
//        private void OpenDialogInviteFacebookFriend (Action<IAppInviteResult> callback = null) {

//            Debug.LogError("App Invite");
//            string linkApp = PianoChallenge.GameConsts.LINK_FACEBOOK_APP;

//            FB.Mobile.FetchDeferredAppLinkData(res => {
//                //Debug.LogError("oplab FetchDeferredAppLinkData:" + Pathfinding.Serialization.JsonFx.JsonWriter.Serialize(res));
//                FB.Mobile.AppInvite(
//                    new System.Uri(linkApp),
//                    new System.Uri(PianoChallenge.GameConsts.APP_ICON_URL),
//                    resInvite => {
//                        //Debug.LogError("oplab FB.Mobile.AppInvite:" + Pathfinding.Serialization.JsonFx.JsonWriter.Serialize(res));
//                        if (callback != null) {
//                            callback(resInvite);
//                            if (string.IsNullOrEmpty(resInvite.Error) && resInvite.Cancelled == false) {
//                                callback(resInvite);
//                            }
//                        }
//                        //Debug.LogError("oplab OpenDialogInviteFacebookFriend  invite result:" + Pathfinding.Serialization.JsonFx.JsonWriter.Serialize(resInvite));
//                    });
//            });

//        }
//        #endregion

//        #region Gift From Friend
//        public void CheckGiftFromFriend (Action<IGraphResult> callback = null) {
//            /*
//			string apiQueryGift="me/apprequests?access_token="+Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
//			FB.API(apiQueryGift, Facebook.Unity.HttpMethod.GET,result=>{
//				if (!String.IsNullOrEmpty (result.Error))
//				{
//					Debug.LogError("oplab CallBackFriendRequest Error:"+result.Error);
//				}
//				else
//				{
//					//ThuanTQ : gift
//					string json=result.RawResult;
//					if(!GameConst.BUILD_LIVE)
//					{
//						Debug.LogError("oplab CallBackFriendRequest:"+json);
//					}
//					SocialGiftData giftData=null;
//					try
//					{
//						giftData = Pathfinding.Serialization.JsonFx.JsonReader.Deserialize(json, typeof(SocialGiftData)) as SocialGiftData;
//					}
//					catch(Exception ex)
//					{
//						Debug.LogError("oplab CallBackFriendRequest exception:"+ex.Message);
//						giftData=null;
//					}
//					if(!GameConst.BUILD_LIVE)
//					{
//						Debug.LogWarning("oplab CallBackFriendRequest json:"+Pathfinding.Serialization.JsonFx.JsonWriter.Serialize(giftData));
//					}
//					if(giftData!=null)
//					{
//						GiftService.Instance.RefreshGiftData(giftData);
//						for(int i =0;i<giftData.data.Count;i++)
//						{
//							string apiQuery="/"+giftData.data[i].id+"?access_token="+Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
//							FB.API(apiQuery, Facebook.Unity.HttpMethod.DELETE, DeleteRequestCallback);
//						}
//						if(!GameConst.BUILD_LIVE)
//						{
//							Debug.LogError("oplab Gift From Friend Count:"+GiftService.Instance.GetAllModelGift().Count);
//						}
//						if(callback!=null)
//						{
//							callback(result);
//						}
//					}
					
//				}
//			});*/
//        }
//        //private void DeleteRequestCallback (IGraphResult result) {
//        //    if (!String.IsNullOrEmpty(result.Error)) {
//        //        if (!GameConst.BUILD_LIVE) {
//        //            Debug.LogError("oplab Error DeleteRequestCallback:" + result.Error);
//        //        }
//        //    }
//        //    else {
//        //        // delete request
//        //        string json = result.RawResult;
//        //        if (!GameConst.BUILD_LIVE) {
//        //            Debug.LogError("oplab DeleteRequestCallback result:" + json);
//        //        }
//        //    }
//        //}
//        #endregion


//        public void GetInvitatbleFriend (string afterToken, int limit = 25, Action<IGraphResult> callback = null) {

//            if (FB.IsLoggedIn) {
//                FB.API("me/invitable_friends?fields=id,name,picture&limit="
//                    + limit.ToString() + "&after=" + afterToken, HttpMethod.GET, delegate (IGraphResult r) {
//                        //Debug.LogError ("Submit Score Result: " + r.RawResult);
//                        //Debug.Log("GetInvitatbleFriend: " + Pathfinding.Serialization.JsonFx.JsonWriter.Serialize(r));
//                        if (callback != null) {
//                            callback(r);
//                        }
//                    });
//            }
//        }
//        public void SendInviteBeta (Action<IAppRequestResult> callback, List<string> friends) {
//            int max = friends.Count;
//            FB.AppRequest(
//                "Send Invite Beta",
//                friends,
//                null,
//                null,
//                max,
//                "invite Data",
//                "invite friend",
//                delegate (IAppRequestResult result) {
//                    Debug.Log("SendInviteBeta" + result.RawResult);
//                    if (callback != null) {
//                        callback(result);
//                    }
//                }
//            );
//        }

//        public string UserID {
//            get {
//                if (IsUserInfoAvailable) {
//                    return AccessToken.CurrentAccessToken.UserId;
//                }
//                else {
//                    return "";
//                }
//            }
//        }
//        //private bool isUserDataAvailable = false;
//        public bool IsUserInfoAvailable {
//            get; set;
//        }

//        //private string avatarURL = string.Empty;
//        public string AvatarURL {
//            get; set;
//        }

//        public string UserDisplayName {
//            get; set;
//        }

//        public string UserEmail {
//            get; set;
//        }

//        public string CoverURL {
//            get; set;
//        }
//    }

//}