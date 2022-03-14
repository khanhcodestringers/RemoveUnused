using System.Collections.Generic;
using Mio.TileMaster;
//using Parse;
[System.Serializable]
public class UserDataModel {
    //public int xp;
    ////level is calculated from xp
    //public int level;
    public int life;
    public int diamond;
    public List<ScoreItemModel> listHighscore;
    public List<string> listBoughtSongs;
    public bool isLoggedInFacebook = false;
    //public bool isGuest = true;
    //public string userName;
    public double timeStartCountdown;
    public long timeToResumeAds;

    //achievement 
    public string achievementPropertiesValueCSV;
    public string achievementUnlockedAndClaimedCSV;

    //used to sync data between devices
    public string lastsyncDevice;
    public int dataVersion;
}

//[ParseClassName("UserData")]
//public class CloudUserDataModel : ParseObject
//{

//    [ParseFieldName("linkedFacebook")]
//    public bool IsLinkedFacebook
//    {
//        get { return GetProperty<bool>("IsLinkedFacebook"); }
//        set { SetProperty<bool>(value, "IsLinkedFacebook"); }
//    }

//    [ParseFieldName("life")]
//    public int Life
//    {
//        get { return GetProperty<int>("Life"); }
//        set { SetProperty<int>(value, "Life"); }
//    }

//    [ParseFieldName("diamond")]
//    public int Diamond
//    {
//        get { return GetProperty<int>("Diamond"); }
//        set { SetProperty<int>(value, "Diamond"); }
//    }

//    [ParseFieldName("totalStars")]
//    public int TotalStars {
//        get { return GetProperty<int>("TotalStars"); }
//        set { SetProperty<int>(value, "TotalStars"); }
//    }

//    [ParseFieldName("totalCrowns")]
//    public int TotalCrowns {
//        get { return GetProperty<int>("TotalCrowns"); }
//        set { SetProperty<int>(value, "TotalCrowns"); }
//    }

//    [ParseFieldName("rev")]
//    public int Revision
//    {
//        get { return GetProperty<int>("Revision"); }
//        set { SetProperty<int>(value, "Revision"); }
//    }

//    [ParseFieldName("timeStartCountDown")]
//    public double TimeStartCountDown
//    {
//        get { return GetProperty<double>("TimeStartCountDown"); }
//        set { SetProperty<double>(value, "TimeStartCountDown"); }
//    }

//    [ParseFieldName("highscores")]
//    public List<ScoreItemModel> Highscores
//    {
//        get { return GetProperty<List<ScoreItemModel>>("Highscores"); }
//        set { SetProperty<List<ScoreItemModel>>(value, "Highscores"); }
//    }

//    [ParseFieldName("boughtSongs")]
//    public List<string> ListBoughtSongs {
//        get { return GetProperty<List<string>>("ListBoughtSongs"); }
//        set { SetProperty<List<string>>(value, "ListBoughtSongs"); }
//    }

//    [ParseFieldName("userID")]
//    public string UserID {
//        get { return GetProperty<string>("UserID"); }
//        set { SetProperty<string>(value, "UserID"); }
//    }

//    [ParseFieldName("lastSyncDevice")]
//    public string LastSyncDevice
//    {
//        get { return GetProperty<string>("LastSyncDevice"); }
//        set { SetProperty<string>(value, "LastSyncDevice"); }
//    }

//    [ParseFieldName("achPropertyCSV")]
//    public string AchivementPropertyCSV
//    {
//        get { return GetProperty<string>("AchivementPropertyCSV"); }
//        set { SetProperty<string>(value, "AchivementPropertyCSV"); }
//    }

//    [ParseFieldName("achStatusCSV")]
//    public string AchievementStatusCSV
//    {
//        get { return GetProperty<string>("AchievementStatusCSV"); }
//        set { SetProperty<string>(value, "AchievementStatusCSV"); }
//    }
//}

