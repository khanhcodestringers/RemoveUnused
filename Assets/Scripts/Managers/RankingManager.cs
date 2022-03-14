using UnityEngine;
using System.Collections;
//using Parse;
//using UnityParseHelpers;
using System.Collections.Generic;
//using Facebook.Unity;
using Mio.Utils;
using Mio.Utils.MessageBus;
using MovementEffects;
using System;

namespace Mio.TileMaster {
    public class RankingManager : MonoSingleton<RankingManager> {
        private List<string> m_listFriends;
        private Dictionary<string, List<PlayerRankingModel>> dic_player_rank;

        public void Initialize () {

        }

        private IEnumerator<float> C_PushCurrentHighScore (PlayerRankingModel pModel) {
            yield return 0;
            //ParseQuery<ParseObject> query = ParseObject.GetQuery(GameConsts.PT_LEVEL_RANKING).WhereEqualTo(GameConsts.PK_USER_ID, pModel.user_id);
            //query.Select(GameConsts.PK_USER_ID);
            //query.Select(GameConsts.PK_LEVELRANKING_CONTENT);

            ////get first record for level ranking
            //var q = query.FirstAsync();

            //while (!q.IsCanceled && !q.IsCompleted && !q.IsFaulted) {
            //    Debug.Log("Querying...");
            //    yield return Timing.WaitForSeconds(0.25f);
            //}

            //Debug.Log("isCancelled: " + q.IsCanceled);
            //if (q.IsFaulted) {
            //    //Debug.LogError("Error querying level ranking: " + q.Exception);
            //    foreach (var ex in q.Exception.InnerExceptions) {
            //        //Debug.LogError("Error querying level ranking: " + ex);
            //        if (ex.ToString().Contains("No results matched")) {
            //            List<PlayerRankingModel> listplayer;
            //            //Debug.Log("insert du lieu:");
            //            // khong co du lieu ghi moi vao
            //            ParseObject rankLevel = new ParseObject(GameConsts.PT_LEVEL_RANKING);
            //            rankLevel[GameConsts.PK_USER_ID] = pModel.user_id;
            //            listplayer = new List<PlayerRankingModel>();
            //            listplayer.Add(pModel);
            //            fsData dataPlayer = null;
            //            //string json = "";
            //            FileUtilities.JSONSerializer.TrySerialize<List<PlayerRankingModel>>(listplayer, out dataPlayer);
            //            rankLevel[GameConsts.PK_LEVELRANKING_CONTENT] = dataPlayer.ToString();
            //            rankLevel.SaveAsync();
            //            yield break;
            //        }
            //    }
            //}
            ////List<ParseObject> listParse = new List<ParseObject>();

            //if (q.IsCompleted) {
            //    Debug.Log("Query completed");
            //    ParseObject obj = q.Result;
            //    List<PlayerRankingModel> listplayer;
            //    if (obj == null)// khong co -> ghi du lieu moi len parse
            //    {
            //        Debug.Log("insert du lieu:");
            //        // khong co du lieu ghi moi vao
            //        ParseObject rankLevel = new ParseObject(GameConsts.PT_LEVEL_RANKING);
            //        rankLevel[GameConsts.PK_USER_ID] = pModel.user_id;
            //        listplayer = new List<PlayerRankingModel>();
            //        listplayer.Add(pModel);
            //        fsData dataPlayer = null;
            //        //string json = "";
            //        FileUtilities.JSONSerializer.TrySerialize<List<PlayerRankingModel>>(listplayer, out dataPlayer);
            //        rankLevel[GameConsts.PK_LEVELRANKING_CONTENT] = dataPlayer.ToString();
            //        rankLevel.SaveAsync();
            //    }
            //    else {
            //        Debug.Log("update du lieu:");
            //        ParseObject rankLevel = obj;
            //        //string contentOld = "";
            //        string oldName = "";
            //        if (rankLevel.ContainsKey(GameConsts.PK_LEVELRANKING_CONTENT)) {
            //            //rankLevel.TryGetValue("content", out contentOld);
            //            string jsonUserRanking = (string)rankLevel[GameConsts.PK_LEVELRANKING_CONTENT];
            //            listplayer = new List<PlayerRankingModel>();
            //            fsData jsonData = fsJsonParser.Parse(jsonUserRanking);
            //            fsResult dresult = FileUtilities.JSONSerializer.TryDeserialize<List<PlayerRankingModel>>(jsonData, ref listplayer);
            //            if (dresult.Failed) {
            //                Debug.LogError("Failed to parse level ranking data, creating new one");
            //                //listplayer.Add(pModel);
            //                //fsData dataPlayer = null;
            //                ////string json = "";
            //                //FileUtilities.JSONSerializer.TrySerialize<List<PlayerRankingModel>>(listplayer, out dataPlayer);
            //                //rankLevel[GameConsts.PK_LEVELRANKING_CONTENT] = dataPlayer.ToString();
            //            }

            //            bool isExitedSongLevelRanking = false;
            //            for (int i = 0; i < listplayer.Count; i++) {
            //                if (listplayer[i].song_id.Equals(pModel.song_id)) {
            //                    listplayer[i] = pModel;
            //                    isExitedSongLevelRanking = true;
            //                    break;
            //                }
            //            }
            //            if (!isExitedSongLevelRanking)
            //                listplayer.Add(pModel);
            //            fsData dataPlayer = null;
            //            //string json = "";
            //            FileUtilities.JSONSerializer.TrySerialize<List<PlayerRankingModel>>(listplayer, out dataPlayer);
            //            rankLevel[GameConsts.PK_LEVELRANKING_CONTENT] = dataPlayer.ToString();
            //        }
            //        else {
            //            Debug.LogWarning("khong co content parse");
            //        }


            //        if (rankLevel.ContainsKey("user_name"))
            //            rankLevel.TryGetValue("user_name", out oldName);
            //        if (oldName.Contains((string)ParseUser.CurrentUser[GameConsts.PK_USERNAME])) {
            //            rankLevel["user_name"] = ParseUser.CurrentUser[GameConsts.PK_USERNAME];
            //        }
            //        else {
            //            //Debug.LogWarning("khong co username parse");
            //            rankLevel["user_name"] = ParseUser.CurrentUser[GameConsts.PK_USERNAME];
            //        }

            //        rankLevel.SaveAsync();
            //    }
            //}
        }

        public void PushCurrentHighscore (PlayerRankingModel pModel) {
            //if (ParseUser.CurrentUser == null)
            //    return;

            //fsData data = null;
            //string json = "";

            ////serialize ranking data into json
            //var result = FileUtilities.JSONSerializer.TrySerialize<PlayerRankingModel>(pModel, out data);
            //if (result.Failed) {
            //    Debug.LogError("Can't serialize PlayerRankingModel, skipping");
            //    return;
            //}
            ////Loom.Instance.CheckInitial();
            //json = data.ToString();
            //Debug.Log("Level ranking json:" + json);
            //if (!string.IsNullOrEmpty(json)) {
            //    Timing.RunCoroutine(C_PushCurrentHighScore(pModel));
            //    return;

            //    #region Old implement
            //    ////first, try to query a record of current user
            //    //ParseQuery<ParseObject> query = ParseObject.GetQuery(GameConsts.PT_LEVEL_RANKING).WhereEqualTo(GameConsts.PK_USER_ID, pModel.user_id);
            //    //query.Select(GameConsts.PK_USER_ID);
            //    //query.Select(GameConsts.PK_LEVELRANKING_CONTENT);
            //    //Debug.Log("Querying...");
            //    //query.FirstAsync().ContinueWith(t => {
            //    //    Debug.Log(t.Result);
            //    //    Loom.Instance.QueueOnMainThread(() => {
            //    //        Debug.Log("isCancelled: " + t.IsCanceled);
            //    //        if (t.IsFaulted) {
            //    //            Debug.LogError("Loi Query Parse save ranking user");
            //    //            return;
            //    //        }
            //    //        //List<ParseObject> listParse = new List<ParseObject>();

            //    //        if (t.IsCompleted) {
            //    //            //IEnumerable<ParseObject> results = t.Result;
            //    //            //IEnumerator<ParseObject> iterator = results.GetEnumerator();

            //    //            //while (iterator.MoveNext()) {
            //    //            //    ParseObject currentObj = iterator.Current;
            //    //            //    if (currentObj != null) {
            //    //            //        listParse.Add(currentObj);
            //    //            //    }
            //    //            //}

            //    //            //ParseObject obj = null;
            //    //            //if (listParse.Count > 0) {
            //    //            //    obj = listParse[0];
            //    //            //}
            //    //            Debug.Log("Query completed");
            //    //            ParseObject obj = t.Result;
            //    //            List<PlayerRankingModel> listplayer;
            //    //            if (obj == null)// khong co -> ghi du lieu moi len parse
            //    //            {
            //    //                Debug.Log("insert du lieu:");
            //    //                // khong co du lieu ghi moi vao
            //    //                ParseObject rankLevel = new ParseObject(GameConsts.PT_LEVEL_RANKING);
            //    //                rankLevel[GameConsts.PK_USER_ID] = pModel.user_id;
            //    //                listplayer = new List<PlayerRankingModel>();
            //    //                listplayer.Add(pModel);
            //    //                fsData dataPlayer = null;
            //    //                //string json = "";
            //    //                FileUtilities.JSONSerializer.TrySerialize<List<PlayerRankingModel>>(listplayer, out dataPlayer);
            //    //                rankLevel[GameConsts.PK_LEVELRANKING_CONTENT] = dataPlayer.ToString();
            //    //                rankLevel.SaveAsync();
            //    //            }
            //    //            else {
            //    //                Debug.Log("update du lieu:");
            //    //                ParseObject rankLevel = obj;
            //    //                //string contentOld = "";
            //    //                string oldName = "";
            //    //                if (rankLevel.ContainsKey(GameConsts.PK_LEVELRANKING_CONTENT)) {
            //    //                    //rankLevel.TryGetValue("content", out contentOld);
            //    //                    string jsonUserRanking = (string)rankLevel[GameConsts.PK_LEVELRANKING_CONTENT];
            //    //                    listplayer = new List<PlayerRankingModel>();
            //    //                    fsData jsonData = fsJsonParser.Parse(jsonUserRanking);
            //    //                    fsResult dresult = FileUtilities.JSONSerializer.TryDeserialize<List<PlayerRankingModel>>(jsonData, ref listplayer);
            //    //                    if (dresult.Failed) {
            //    //                        Debug.LogError("Failed to parse level ranking data, creating new one");
            //    //                        //listplayer.Add(pModel);
            //    //                        //fsData dataPlayer = null;
            //    //                        ////string json = "";
            //    //                        //FileUtilities.JSONSerializer.TrySerialize<List<PlayerRankingModel>>(listplayer, out dataPlayer);
            //    //                        //rankLevel[GameConsts.PK_LEVELRANKING_CONTENT] = dataPlayer.ToString();
            //    //                    }

            //    //                    bool isExitedSongLevelRanking = false;
            //    //                    for (int i = 0; i < listplayer.Count; i++) {
            //    //                        if (listplayer[i].song_id.Equals(pModel.song_id)) {
            //    //                            listplayer[i] = pModel;
            //    //                            isExitedSongLevelRanking = true;
            //    //                            break;
            //    //                        }
            //    //                    }
            //    //                    if (!isExitedSongLevelRanking)
            //    //                        listplayer.Add(pModel);
            //    //                    fsData dataPlayer = null;
            //    //                    //string json = "";
            //    //                    FileUtilities.JSONSerializer.TrySerialize<List<PlayerRankingModel>>(listplayer, out dataPlayer);
            //    //                    rankLevel[GameConsts.PK_LEVELRANKING_CONTENT] = dataPlayer.ToString();
            //    //                }
            //    //                else {
            //    //                    Debug.LogWarning("khong co content parse");
            //    //                }


            //    //                if (rankLevel.ContainsKey("user_name"))
            //    //                    rankLevel.TryGetValue("user_name", out oldName);
            //    //                if (oldName.Contains((string)ParseUser.CurrentUser[GameConsts.PK_USERNAME])) {
            //    //                    rankLevel["user_name"] = ParseUser.CurrentUser[GameConsts.PK_USERNAME];
            //    //                }
            //    //                else {
            //    //                    //Debug.LogWarning("khong co username parse");
            //    //                    rankLevel["user_name"] = ParseUser.CurrentUser[GameConsts.PK_USERNAME];
            //    //                }

            //    //                rankLevel.SaveAsync();
            //    //            }
            //    //        }
            //    //    });
            //    //}); 
            //    #endregion

            //}
        }

        public List<PlayerRankingModel> GetRankingListForLevel (string storeID) {
            if (dic_player_rank != null && dic_player_rank.ContainsKey(storeID)) {
                return dic_player_rank[storeID];
            }
            else {
                return null;
            }
        }

        public void InitializeFriendData () {
            //REMOVE FB
            //if (AccessToken.CurrentAccessToken == null) return;
            //Debug.Log(AccessToken.CurrentAccessToken.UserId);
            //FB.API("me/friends?fields=id,name", HttpMethod.GET, resGetInfo => {
            //    //			Debug.Log("Friends:"+resGetInfo.RawResult);
            //    //  List<string> listFriend = new List<string>();
            //    if (!resGetInfo.Cancelled && string.IsNullOrEmpty(resGetInfo.Error)) {
            //        //Debug.LogError("Exception qk:");
            //        try {
            //            m_listFriends = new List<string>();
            //            string json = resGetInfo.RawResult;
            //            fsData jsonData = fsJsonParser.Parse(json);
            //            //FBFriendQuery query = Pathfinding.Serialization.JsonFx.JsonReader.Deserialize(json, typeof(FBFriendQuery)) as FBFriendQuery;
            //            FBFriendQuery query = new FBFriendQuery();
            //            fsResult r = FileUtilities.JSONSerializer.TryDeserialize<FBFriendQuery>(jsonData, ref query);
            //            if (!r.Failed) {
            //                for (int i = 0; i < query.data.Count; i++) {
            //                    m_listFriends.Add(query.data[i].id);
            //                }
            //            }
            //            else {
            //                m_listFriends.Add(AccessToken.CurrentAccessToken.UserId);
            //            }
            //        }
            //        catch (System.Exception ex) {
            //            Debug.LogError("Exception query friend facebook:" + ex.Message);
            //        }
            //    }
            //    else {
            //        Debug.LogError("Error:" + resGetInfo.Error.ToString());
            //    }
            //    QueryLevelSongInParse();
            //});

            //OneSignalPushManager.PushNotificationToAllFriend();

        }

        private IEnumerator<float> C_QueryLevelRankingData () {
            yield return 0;

            //if (m_listFriends == null) {
            //    yield break;
            //}

            //var query = ParseObject.GetQuery(GameConsts.PT_LEVEL_RANKING).WhereContainedIn(GameConsts.PK_USER_ID, m_listFriends).Limit(10);
            //query.Select(GameConsts.PK_USER_ID);
            //query.Select(GameConsts.PK_LEVELRANKING_CONTENT);
            //var q = query.FindAsync();

            //while (!q.IsCanceled && !q.IsCompleted && !q.IsFaulted) {
            //   // Debug.Log("Querying level ranking...");
            //    yield return Timing.WaitForSeconds(0.25f);
            //}

            //if (q.IsFaulted) {
            //    foreach (var ex in q.Exception.InnerExceptions) {
            //        Debug.LogError("Error querying level ranking data: " + ex);
            //        //if (ex.ToString().Contains("No results matched")) {
            //        //}
            //    }
            //}
            //else {
            //    if (q.IsCompleted) {
            //        IEnumerable<ParseObject> results = q.Result;
            //        IEnumerator<ParseObject> iterator = results.GetEnumerator();
            //        List<PlayerRankingModel> listplayer;

            //        //do not eat all the cpu
            //        float lastRelease = Time.realtimeSinceStartup;

            //        dic_player_rank = new Dictionary<string, List<PlayerRankingModel>>(20);
            //        while (iterator.MoveNext()) {
            //            if(Time.realtimeSinceStartup - lastRelease >= 0.07) {
            //                yield return 0;
            //                lastRelease = Time.realtimeSinceStartup;
            //            }

            //            listplayer = new List<PlayerRankingModel>();
            //            //Debug.Log("next");
            //            ParseObject currentObj = iterator.Current;
            //            //string user_id = (string)currentObj["user_id"];
            //            if (currentObj.ContainsKey(GameConsts.PK_LEVELRANKING_CONTENT)) {
            //                string json = (string)currentObj[GameConsts.PK_LEVELRANKING_CONTENT];

            //                fsData jsonData = fsJsonParser.Parse(json);
            //                fsResult result = FileUtilities.JSONSerializer.TryDeserialize<List<PlayerRankingModel>>(jsonData, ref listplayer);
            //                //Debug.Log(json);
            //                if (!result.Failed) {
            //                    //listplayer.Sort(new SortInSong());

            //                    for (int i = 0; i < listplayer.Count; i++) {
            //                        if (!dic_player_rank.ContainsKey(listplayer[i].song_id))
            //                            dic_player_rank[listplayer[i].song_id] = new List<PlayerRankingModel>();
            //                        dic_player_rank[listplayer[i].song_id].Add(listplayer[i]);
            //                    }
            //                }
            //            }
            //            else {
            //                Debug.LogWarning("Data receive doesn't contain column name " + GameConsts.PK_LEVELRANKING_CONTENT);
            //            }
            //        }
            //        if (dic_player_rank != null) {
            //            foreach (KeyValuePair<string, List<PlayerRankingModel>> pair in dic_player_rank) {
            //                //Debug.Log("aaaa" + pair.Key);
            //                pair.Value.Sort(new PlayerRankingComparer());
            //            }
            //            MessageBus.Annouce(new Message(MessageBusType.CompletedLevelRanking));
            //        }
            //    }
            //}
        }

        private void QueryLevelSongInParse () {
            Timing.RunCoroutine(C_QueryLevelRankingData());

            #region Old code
            ////Debug.LogError("Go Parse");
            //if (m_listFriends == null)
            //    m_listFriends = new List<string>();

            ////first record is current user
            //m_listFriends.Add(AccessToken.CurrentAccessToken.UserId);

            ////query in another thread
            //Loom.Instance.CheckInitial();

            //var query = ParseObject.GetQuery("LevelRanking").WhereContainedIn("user_id", m_listFriends).Limit(10);
            //query.Select("user_id");
            //query.Select("content");
            //query.FindAsync().ContinueWith(t => {
            //    if (t.IsFaulted) {
            //        Debug.LogError("Error querying level ranking: " + t.Result.ToString());
            //    }
            //    else {
            //        if (t.IsCompleted) {
            //            // switch to main thread
            //            Loom.Instance.QueueOnMainThread(() => {
            //                IEnumerable<ParseObject> results = t.Result;
            //                IEnumerator<ParseObject> iterator = results.GetEnumerator();
            //                List<PlayerRankingModel> listplayer;

            //                dic_player_rank = new Dictionary<string, List<PlayerRankingModel>>();
            //                while (iterator.MoveNext()) {
            //                    listplayer = new List<PlayerRankingModel>();
            //                    //Debug.Log("next");
            //                    ParseObject currentObj = iterator.Current;
            //                    //string user_id = (string)currentObj["user_id"];
            //                    if (currentObj.ContainsKey("content")) {
            //                        string json = (string)currentObj["content"];

            //                        fsData jsonData = fsJsonParser.Parse(json);
            //                        fsResult result = FileUtilities.JSONSerializer.TryDeserialize<List<PlayerRankingModel>>(jsonData, ref listplayer);
            //                        //Debug.Log(json);
            //                        if (!result.Failed) {
            //                            //listplayer.Sort(new SortInSong());

            //                            for (int i = 0; i < listplayer.Count; i++) {
            //                                if (!dic_player_rank.ContainsKey(listplayer[i].song_id))
            //                                    dic_player_rank[listplayer[i].song_id] = new List<PlayerRankingModel>();
            //                                dic_player_rank[listplayer[i].song_id].Add(listplayer[i]);
            //                            }
            //                        }
            //                    }
            //                    else {
            //                        Debug.LogError("Data receive not contain <content>");
            //                    }
            //                }
            //                if (dic_player_rank != null) {
            //                    foreach (KeyValuePair<string, List<PlayerRankingModel>> pair in dic_player_rank) {
            //                        //Debug.Log("aaaa" + pair.Key);
            //                        pair.Value.Sort(new PlayerRankingComparer());
            //                    }
            //                    MessageBus.Annouce(new Message(MessageBusType.CompletedLevelRanking));
            //                }
            //                //InitData(true);
            //            });
            //        }
            //    }
            //}); 
            #endregion

        }

        public int GetIndexLevelRankingCurrentUser (string song_id) {
            if (string.IsNullOrEmpty(song_id))
                return 0;
            else {
                int index = 0;
                if (dic_player_rank != null) {
                    List<PlayerRankingModel> pModel;

                    dic_player_rank.TryGetValue(song_id, out pModel);
                    if (pModel != null && pModel.Count > 0) {
                        for (int i = 0; i < pModel.Count; i++) {
                            //REMOVE FB
                            //if (pModel[i].user_id == AccessToken.CurrentAccessToken.UserId) {
                            //    index = i + 1;
                            //    break;
                            //}
                        }
                    }
                }
                return index;
            }
        }

        public List<PlayerRankingModel> GetListFriendsPlayInSong (string songId) {
            List<PlayerRankingModel> listFriends = null;
            if (dic_player_rank != null) {
                if (dic_player_rank.ContainsKey(songId))
                    listFriends = dic_player_rank[songId];
            }
            return listFriends;

        }

        /// <summary>
        /// Update local current ranking for this user after playing any level/song
        /// </summary>
        /// <param name="pModel"></param>
        public void UpdateLevelRanking (PlayerRankingModel pModel) {
            bool isNewPlayThisSong = true;

            if (dic_player_rank == null) dic_player_rank = new Dictionary<string, List<PlayerRankingModel>>();

            List<PlayerRankingModel> listpModel;

            //try to get level ranking for specified song
            if (dic_player_rank.TryGetValue(pModel.song_id, out listpModel)) {
                //if success, browse all level ranking record for that song
                for (int i = 0; i < listpModel.Count; i++) {
                    //and update record of this user
                    //REMOVE FB
                    //if (listpModel[i].user_id == AccessToken.CurrentAccessToken.UserId) {
                    //    isNewPlayThisSong = false;
                    //    if (listpModel[i].score < pModel.score)
                    //        listpModel[i] = pModel;

                    //    break;
                    //}
                }
                //if there is no record for this user, add a new one
                if (isNewPlayThisSong) listpModel.Add(pModel);

                //finally, rank them
                listpModel.Sort(new PlayerRankingComparer());
            }
            else {
                //if there is no ranking for this song, create a new one
                listpModel = new List<PlayerRankingModel>();
                listpModel.Add(pModel);
                dic_player_rank[pModel.song_id] = listpModel;
            }

            //Debug.Log("refect : " + listpModel);
            //if (listpModel == null) {
            //    //Debug.Log("refacr null : ");
            //    listpModel = new List<PlayerRankingModel>();
            //    listpModel.Add(pModel);
            //    dic_player_rank[pModel.song_id] = listpModel;
            //}
            //else {
            //    for (int i = 0; i < listpModel.Count; i++) {
            //        if (listpModel[i].user_id == AccessToken.CurrentAccessToken.UserId ) {
            //            //Debug.Log("add: " + pModel.user_name);
            //            isNewPlayThisSong = false;
            //            if(listpModel[i].score < pModel.score)
            //                listpModel[i] = pModel;
            //        }
            //    }
            //    if (isNewPlayThisSong)
            //        listpModel.Add(pModel);
            //    listpModel.Sort(new SortInSong());
            //}
            //MessageBus.Annouce(new Message(MessageBusType.CompletedLevelRanking));
        }
    }
}