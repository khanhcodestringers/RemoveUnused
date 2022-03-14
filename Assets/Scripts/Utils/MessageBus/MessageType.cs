

public enum MessageBusType
{
    NONE,
    LevelStart,
    LevelEnd,
    PlayerPosition,
    PointAdded,
    LoggedInFacebook,
    LoggedOutFacebook,
    UserDataConflicted,
    UserDataConflictSolved,

    UserDataChanged,
    GameDataChanged,
    AchievementDataChanged,
    XPChanged,
    LifeChanged,
    DiamondChanged,
    UserLevelChanged,
    UserPlayRecordChanged,
    TimeLifeCountDown,
    FacebookUserDataReceived,
    IAPManagerInitializeFailed,
    PurchaseFailed,
    ProductPurchased,
    NativeAdItemLoaded,
    LanguageChanged,
    NativeAdItemFailedToLoad,
    CompletedIAPLocalization,
    OnSelectedOrUnselectedInviteFriend,
    OnUserDataInitialized,
    CompletedLevelRanking,
    OnCompletedGetGeoLocation,
    CompletedPostStatusShareFacebook,
    CancelPostStatusShareFacebook,
    OnCompletedRestoreApple,
    PlaySongAction,
    RewardVideoDiamond,
    RewardVideoPostponeAds,
    Evt_Room_Join_Status = 201,// when waiting another person in room
    Evt_Room_MoveTo_Gameplay = 202,// start room
    Evt_Room_Sync_Status = 203,// when waiting another download this song
    Evt_Room_Error_To_Play = 204,// when room can not start due to error
    Evt_Room_Ready_To_Play = 205,// when room ready to start(all person ready or time to go)
    Evt_Room_Finish = 206,// when room end
    Evt_SyncBehaviorToRoom = 207,// when user sync behavior
    Evt_Room_Chat = 208,// when a person chat in waiting start room
    Evt_Kick_out = 209,// kick user violate (hack, cheat, timeout...)      

    Evt_HasError = 1000,
    Evt_Online_Disconnect = 2000,
    OnMeDieOnline
}