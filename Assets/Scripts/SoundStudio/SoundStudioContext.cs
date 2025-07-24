// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// SoundStudio.SoundStudioContext
using System;
using Disney.ClubPenguin;
using Disney.ClubPenguin.ForcedUpdate;
using Disney.ClubPenguin.Service.DirectoryService;
using Disney.ClubPenguin.Service.Friends;
using Disney.ClubPenguin.Service.MWS;
using Disney.ClubPenguin.Service.PDR;
using Disney.HTTP.Client;
using Disney.HTTP.Client.HTTPUnity;
using Disney.Xmpp;
using SoundStudio.Command;
using SoundStudio.Command.Analytics.DMO;
using SoundStudio.Command.Friends;
using SoundStudio.Command.MWS;
using SoundStudio.Command.PDR;
using SoundStudio.Controller;
using SoundStudio.Event;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

public class SoundStudioContext : MVCSContext
{
	public SoundStudioContext(MonoBehaviour view)
		: base(view)
	{
		Application.RegisterLogCallback(HandleException);
	}

	public SoundStudioContext(MonoBehaviour view, ContextStartupFlags flags)
		: base(view, flags)
	{
	}

	private static void HandleException(string condition, string stackTrace, LogType type)
	{
		if (type == LogType.Exception)
		{
			throw new ApplicationException("Something went wrong: " + condition + " " + stackTrace);
		}
	}

	protected override void mapBindings()
	{
		injectionBinder.Bind<IDirectoryServiceClient>().ToValue(DirectoryServiceClient.Instance).CrossContext();
		injectionBinder.Bind<IHTTPRequestFactory>().ToValue(HTTPUnityRequestFactory.Instance).CrossContext();
		injectionBinder.Bind<IMWSClient>().ToValue(MWSClient.Instance).CrossContext();
		injectionBinder.Bind<IPDRClient>().ToValue(PDRClient.Instance).CrossContext();
		injectionBinder.Bind<XmppManager>().ToValue(XmppManager.Instance).CrossContext();
		injectionBinder.Bind<IForcedUpdateManager>().ToValue(ForcedUpdateManager.Instance).CrossContext();
		ForcedUpdateManager.Instance.AlertOnUpdateRecommended = true;
		injectionBinder.Bind<AssetBundleDownloadService>().To<AssetBundleDownloadService>().ToSingleton()
			.CrossContext();
		injectionBinder.Bind<IMWSClientService>().To<MWSClientService>().ToSingleton()
			.CrossContext();
		injectionBinder.Bind<IPDRClientService>().To<PDRClientService>().ToSingleton()
			.CrossContext();
		injectionBinder.Bind<RetrySynchronizationService>().To<RetrySynchronizationService>().ToSingleton()
			.CrossContext();
		injectionBinder.Bind<IAnalyticsService>().To<DMOAnalyticsService>().ToSingleton()
			.CrossContext();
		injectionBinder.Bind<FriendsClientService>().To<FriendsClientService>().ToSingleton()
			.CrossContext();
		injectionBinder.Bind<IFriendsManager>().To<FriendsManager>().ToSingleton()
			.CrossContext();
		injectionBinder.Bind<IGameStateManager>().To<GameStateManager>().ToSingleton()
			.CrossContext();
		base.commandBinder.Bind(SongSyncEvent.SONG_SYNC).InSequence().To<CheckOnlineCommand>()
			.To<SongSyncCommand>();
		base.commandBinder.Bind(SoundStudioEvent.RELOGIN_FOR_SAVE).To<ReloginCommandForSyncSongCommand>();
		base.commandBinder.Bind(LoginEvent.LOGIN_SUCCESS).To<InitializeFriendsCommand>();
		base.commandBinder.Bind(LoadLevelEvent.LOAD_LEVEL).To<LoadLevelCommand>().To<CheckAppUpdateCommand>();
		base.commandBinder.Bind(SongPlayEvent.SONG_PLAY).To<SongPlayCommand>();
		base.commandBinder.Bind(SongStopEvent.SONG_STOP).To<SongStopCommand>();
		base.commandBinder.Bind(SongDeleteEvent.SONG_DELETE).To<SongDeleteCommand>();
		base.commandBinder.Bind(SongEditEvent.SONG_EDIT).To<SongEditCommand>();
		base.commandBinder.Bind(SongShareEvent.SONG_SHARE).To<SongShareCommand>();
		base.commandBinder.Bind(SongUnshareEvent.SONG_UNSHARE).To<SongUnshareCommand>();
		base.commandBinder.Bind(SongSaveEvent.SONG_SAVE).To<SongSaveCommand>();
		base.commandBinder.Bind(LoadSongListEvent.LOAD_SONG_LIST).To<LoadSongListCommand>();
		base.commandBinder.Bind(ShowLoginEvent.SHOW_LOGIN).To<ShowLoginModuleCommand>();
		base.commandBinder.Bind(SoundStudioEvent.SHOW_POPUP).To<ShowPopupCommand>();
		base.commandBinder.Bind(SoundStudioEvent.SHOW_APP_USES_DATA).InSequence().To<ShowAppUsesDataCommand>();
		base.commandBinder.Bind(SoundStudioEvent.CHECK_APP_UPDATE).To<CheckAppUpdateCommand>();
		base.commandBinder.Bind(ShowStoreEvent.SHOW_STORE).InSequence().To<ShowParentPermissionCommand>()
			.To<ShowStoreCommand>();
		base.commandBinder.Bind(ShowReferralStoreEvent.SHOW_REFERRAL_STORE).InSequence().To<ShowParentPermissionCommand>()
			.To<ShowReferralStoreCommand>();
		base.commandBinder.Bind(ShowCPAppEvent.SHOW_CP_APP).InSequence().To<ShowParentPermissionCommand>()
			.To<ShowCPAppCommand>();
		base.commandBinder.Bind(ShowProtectedWebPageEvent.SHOW_PROTECTED_WEB_PAGE).InSequence().To<ShowParentPermissionCommand>()
			.To<ShowProtectedWebPageCommand>();
		base.commandBinder.Bind(ShowAboutMembershipEvent.SHOW_ABOUT_MEMBERSHIP).InSequence().To<ShowParentPermissionCommand>()
			.To<ShowAboutMembershipCommand>();
		base.commandBinder.Bind(LoadPlayerPurchasesEvent.LOAD_PLAYER_PURCHASES).To<LoadPlayerPurchasesCommand>();
		base.commandBinder.Bind(RestorePlayerPurchasesEvent.RESTORE_PLAYER_PURCHASES).To<RestorePlayerPurchasesCommand>();
		base.commandBinder.Bind(ShowManageAccountEvent.SHOW_MANAGE_ACCOUNT).InSequence().To<ShowParentPermissionCommand>()
			.To<ShowManageAccountCommand>();
		base.commandBinder.Bind(ShowConfirmQuitEvent.SHOW_CONFIRM_QUIT).To<ShowConfirmQuitCommand>();
		base.commandBinder.Bind(SoundStudioEvent.AUTO_LOGIN).To<AutoLoginCommand>();
		base.commandBinder.Bind(ShowShareConfirmEvent.SHOW_SHARE_CONFIRM).To<ShowShareConfirmCommand>();
		base.commandBinder.Bind(SoundStudioEvent.PERFORM_CACHED_ACTIONS).To<PerformCachedActionsCommand>();
		base.commandBinder.Bind(SoundStudioEvent.CONSOLIDATE_TRACKS).To<ConsolidateTracksCommand>();
		base.commandBinder.Bind(SoundStudioEvent.SHOW_MAX_SONGS_POPUP).To<ShowMaxSongsPopupCommand>();
		base.commandBinder.Bind(SoundStudioEvent.GET_ASSET_BUNDLE).To<GetAssetBundleCommand>();
		base.commandBinder.Bind(SoundStudioEvent.LOAD_PLAYER).InSequence().To<LoadPlayerCommand>()
			.To<LoadPlayerPurchasesCommand>();
		base.commandBinder.Bind(SoundStudioEvent.APPLICATION_PAUSED_STATE_CHANGE).To<ApplicationPauseStateChangeCommand>();
		base.commandBinder.Bind(SoundStudioEvent.CREATE_LOGIN_PROGRESS).To<CreateLoginProgressCommand>();
		base.commandBinder.Bind(SoundStudioEvent.CHANGE_USER).To<ChangeUserCommand>();
		base.commandBinder.Bind(SoundStudioEvent.LOG_OUT).To<LogoutCommand>();
		base.commandBinder.Bind(SoundStudioEvent.LOAD_RADIO_BLOCK).To<LoadRadioBlockCommand>();
		base.commandBinder.Bind(SoundStudioEvent.SHOW_MOPUB_AD).To<ShowMopubAdCommand>();
		base.commandBinder.Bind(SoundStudioEvent.SHOW_NO_SONGS_POPUP).To<ShowNoSongsPopupCommand>();
		base.commandBinder.Bind(SoundStudioEvent.SHOW_RECORD_OOPS_POPUP).To<ShowRecordOopsCommand>();
		base.commandBinder.Bind(SoundStudioEvent.SETUP_COMPLETE).To<DMOAnalyticsCommand>().To<LoadGenreConfigCommand>()
			.To<ScheduleNotificationsCommand>();
		base.commandBinder.Bind(SoundStudioEvent.SHOW_MEMBERSHIP_PROMPT).To<ShowMembershipPromptCommand>();
		base.commandBinder.Bind(MWSEvent.GET_MY_TRACKS).To<GetMySoundStudioTracksCommand>();
		base.commandBinder.Bind(MWSEvent.GET_MY_TRACKS_LISTING).To<GetMySoundStudioTracksListingCommand>();
		base.commandBinder.Bind(MWSEvent.GET_SHARED_TRACKS_LISTING).To<GetSharedTracksListingCommand>();
		base.commandBinder.Bind(MWSEvent.GET_TRACK).To<GetSongDataCommand>();
		base.commandBinder.Bind(MWSEvent.GET_ACCOUNT).To<GetAccountCommand>();
		base.commandBinder.Bind(MWSEvent.GET_RADIO_LIST).To<GetRadioListCommand>();
		base.commandBinder.Bind(FriendsEvent.FRIENDS_REFRESH_SUCCESS).To<FriendsRefreshCommand>();
		base.commandBinder.Bind(PDREvent.GET_PAPER_DOLL).To<GetPaperDollCommand>();
		base.commandBinder.Bind(LoadGenreConfigEvent.LOAD_GENRE_CONFIG).To<LoadGenreConfigCommand>();
		base.commandBinder.Bind(SelectGenreEvent.SELECT_GENRE).To<SelectGenreCommand>();
		base.commandBinder.Bind(SFXEvent.SFX_CLICK_BACK).To<SFXPlayCommand>();
		base.commandBinder.Bind(SFXEvent.SFX_CLICK_ERROR).To<SFXPlayCommand>();
		base.commandBinder.Bind(SFXEvent.SFX_CLICK_TOGGLE).To<SFXPlayCommand>();
		base.commandBinder.Bind(SFXEvent.SFX_CLICK_SELECT).To<SFXPlayCommand>();
		base.commandBinder.Bind(SFXEvent.SFX_WITH_PAYLOAD_IN_EVENT).To<SFXPlayCommand>();
		base.mediationBinder.Bind<StartScreenView>().To<StartScreenMediator>();
		base.mediationBinder.Bind<GenreSelectView>().To<GenreSelectMediator>();
		base.mediationBinder.Bind<SettingsView>().To<SettingsMediator>();
		base.mediationBinder.Bind<SongListView>().To<SongListMediator>();
		base.mediationBinder.Bind<SongListItemView>().To<SongListItemMediator>();
		base.mediationBinder.Bind<EditSongView>().To<EditSongMediator>();
		base.mediationBinder.Bind<MixingBoardView>().To<MixingBoardMediator>();
		base.mediationBinder.Bind<RadioView>().To<RadioMediator>();
		base.mediationBinder.Bind<RadioSongView>().To<RadioSongMediator>();
		base.mediationBinder.Bind<SongProgressView>().To<SongProgressMediator>();
		base.mediationBinder.Bind<LoginProgressView>().To<LoginProgressMediator>();
		base.mediationBinder.Bind<RadioRandomView>().To<RadioRandomMediator>();
		base.mediationBinder.Bind<RadioNewView>().To<RadioNewMediator>();
		base.mediationBinder.Bind<RadioFriendsView>().To<RadioFriendsMediator>();
		base.mediationBinder.Bind<ImageLocalizerView>().To<ImageLocalizerMediator>();
		SongPlayer o = (base.contextView as GameObject).AddComponent<SongPlayer>();
		injectionBinder.Bind<SongPlayer>().ToValue(o).ToSingleton()
			.CrossContext();
		StrikeModules component = (base.contextView as GameObject).GetComponent<StrikeModules>();
		component.Init();
		injectionBinder.Bind<StrikeModules>().ToValue(component).ToSingleton()
			.CrossContext();
		injectionBinder.Bind<ApplicationState>().To<ApplicationState>().ToSingleton();
		injectionBinder.Bind<SongRecorder>().To<SongRecorder>().ToSingleton();
		injectionBinder.Bind<GenreAudioManager>().To<GenreAudioManager>().ToSingleton();
		injectionBinder.Bind<SongVO>().To<SongVO>();
		InjectMopubManager();
		InjectLocalNotifications();
		base.commandBinder.Bind(ContextEvent.START).To<InitialSetupCommand>();
	}

	private void InjectMopubManager()
	{
	}

	private void InjectLocalNotifications()
	{
		LocalNotificationPlugin localNotificationPlugin = new LocalNotificationPluginAndroid();
		localNotificationPlugin.Initialize();
		injectionBinder.Bind<LocalNotificationPlugin>().ToValue(localNotificationPlugin).ToSingleton()
			.CrossContext();
	}

	protected override void postBindings()
	{
		IDirectoryServiceClient instance = injectionBinder.GetInstance<IDirectoryServiceClient>();
		instance.AppID = "SoundStudio";
		instance.AppVersion = "1.2";
		instance.Environment = CPEnvironment.PROD;
		IHTTPRequestFactory instance2 = injectionBinder.GetInstance<IHTTPRequestFactory>();
		instance2.LogAllRequests = Debug.isDebugBuild;
		instance2.VerboseLogging = Debug.isDebugBuild;
		XmppManager.Instance.directoryServiceClient = instance;
		injectionBinder.GetInstance<IForcedUpdateManager>().DirectoryServiceClient = instance;
		injectionBinder.GetInstance<IForcedUpdateManager>().AlertOnUpdateRecommended = true;
		base.postBindings();
	}
}
