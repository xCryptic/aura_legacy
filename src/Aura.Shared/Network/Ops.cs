// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder					

namespace Aura.Shared.Network
{
	/// <summary>
	/// List of all op codes.
	/// </summary>
	public static class Op
	{
		// Login Server
		// ------------------------------------------------------------------
		public const uint ClientIdent = 0x0FD1020A;
		public const uint ClientIdentR = 0x1F;
		public const uint Login = 0x0FD12002;
		public const uint LoginR = 0x23;
		public const uint ChannelStatus = 0x26;
		public const uint CharInfoRequest = 0x29;
		public const uint CharInfo = 0x2A;
		public const uint CreateCharacter = 0x2B;
		public const uint CharacterCreated = 0x2C;
		public const uint DeleteCharRequest = 0x2D;
		public const uint DeleteCharRequestR = 0x2E;
		public const uint EnterGame = 0x2F;
		public const uint ChannelInfo = 0x30;
		public const uint DeleteChar = 0x35;
		public const uint DeleteCharR = 0x36;
		public const uint RecoverChar = 0x37;
		public const uint RecoverCharR = 0x38;
		public const uint NameCheck = 0x39;
		public const uint NameCheckR = 0x3A;
		public const uint PetInfoRequest = 0x3B;
		public const uint PetInfo = 0x3C;
		public const uint CreatePet = 0x3D;
		public const uint PetCreated = 0x3E;
		public const uint DeletePetRequest = 0x3F;
		public const uint DeletePetRequestR = 0x40;
		public const uint DeletePet = 0x41;
		public const uint DeletePetR = 0x42;
		public const uint RecoverPet = 0x43;
		public const uint RecoverPetR = 0x44;
		public const uint CreatePartner = 0x45;
		public const uint CreatePartnerR = 0x46;
		public const uint AccountInfoRequest = 0x47;
		public const uint AccountInfoRequestR = 0x48;
		public const uint AcceptGift = 0x49;
		public const uint AcceptGiftR = 0x4A;
		public const uint RefuseGift = 0x4B;
		public const uint RefuseGiftR = 0x4C;
		public const uint Disconnect = 0x4D;
		public const uint EnterPetCreation = 0x50;
		public const uint CreatingPetR = 0x51;
		public const uint EnterPartnerCreation = 0x55;
		public const uint CreatingPartnerR = 0x56;

		// World Server
		// ------------------------------------------------------------------
		public const uint WorldLogin = 0x4E22;
		public const uint WorldLoginR = 0x4E23;
		public const uint WorldDisconnect = 0x4E24;
		public const uint WorldDisconnectR = 0x4E25;
		public const uint RequestClientDisconnect = 0x4E26;
		public const uint Disappear = 0x4E2A;
		//public const uint GoRebirth = 0x4E32;
		public const uint WarpUnk1 = 0x4E39;

		public const uint WorldCharInfoRequest = 0x5208;
		public const uint WorldCharInfoRequestR = 0x5209;
		public const uint EntityAppears = 0x520C;
		public const uint EntityDisappears = 0x520D;
		public const uint ItemAppears = 0x5211;
		public const uint ItemDisappears = 0x5212;
		public const uint AssignChair = 0x5215;
		public const uint Chat = 0x526C;
		public const uint Notice = 0x526D;
		public const uint WarpUnk2 = 0x526E;
		public const uint MsgBox = 0x526F;
		public const uint AcquireInfo = 0x5271;
		public const uint WhisperChat = 0x5273;
		public const uint BeginnerChat = 0x5275;
		public const uint AcquireInfo2 = 0x5278; // ?
		public const uint VisualChat = 0x527A;
		public const uint PropAppears = 0x52D0;
		public const uint PropDisappears = 0x52D1;
		public const uint PropUpdate = 0x52D2; // Doors, MGs?
		public const uint EntitiesAppear = 0x5334;
		public const uint EntitiesDisappear = 0x5335;
		public const uint BackFromTheDead1 = 0x53FD;
		public const uint IsNowDead = 0x53FC;
		public const uint Revive = 0x53FE;
		public const uint Revived = 0x53FF;
		public const uint DeadMenu = 0x5401;
		public const uint DeadMenuR = 0x5402;
		public const uint DeadFeather = 0x5403;
		public const uint UseGesture = 0x540E;
		public const uint UseGestureR = 0x540F;
		public const uint NPCTalkStart = 0x55F0;
		public const uint NPCTalkStartR = 0x55F1;
		public const uint NPCTalkEnd = 0x55F2;
		public const uint NPCTalkEndR = 0x55F3;
		public const uint NPCTalkPartner = 0x55F8;
		public const uint NPCTalkPartnerR = 0x55F9;
		public const uint ItemMove = 0x59D8;
		public const uint ItemMoveR = 0x59D9;
		public const uint ItemPickUp = 0x59DA;
		public const uint ItemPickUpR = 0x59DB;
		public const uint ItemDrop = 0x59DC;
		public const uint ItemDropR = 0x59DD;
		public const uint ItemMoveInfo = 0x59DE;
		public const uint ItemSwitchInfo = 0x59DF;
		public const uint ItemNew = 0x59E0;
		public const uint ItemRemove = 0x59E1;
		public const uint ItemDestroy = 0x59E2;
		public const uint ItemDestroyR = 0x59E4;
		public const uint EquipmentChanged = 0x59E6;
		public const uint EquipmentMoved = 0x59E7;
		public const uint ItemSplit = 0x59E8;
		public const uint ItemSplitR = 0x59E9;
		public const uint ItemAmount = 0x59EA;
		public const uint ItemUse = 0x59EB;
		public const uint UseItemR = 0x59EC;

		// [180300, NA166 (18.09.2013)] 2 new ops
		//public const uint ? = 0x59F8; // Request for that --v ?
		//public const uint ? = 0x59F9; // "Unable to receive [something]."

		public const uint NPCTalkSelectEnd = 0x59FB;

		public const uint SwitchSet = 0x5BCD;
		public const uint SwitchSetR = 0x5BCE;
		public const uint UpdateWeaponSet = 0x5BCF;
		public const uint ItemStateChange = 0x5BD0;
		public const uint ItemStateChangeR = 0x5BD1;
		public const uint ItemUpdate = 0x5BD4;
		public const uint ItemDurabilityUpdate = 0x5BD5;
		public const uint ItemStateChanged = 0x5BD9;
		public const uint ItemExpUpdate = 0x5BDA;
		public const uint ViewEquipment = 0x5BDF;
		public const uint ViewEquipmentR = 0x5BE0;
		public const uint OptionSet = 0x5BE7;
		public const uint OptionSetR = 0x5BE8;
		public const uint NPCTalkKeyword = 0x5DC4;
		public const uint NPCTalkKeywordR = 0x5DC5;

		public const uint SetLocation = 0x6594;
		public const uint TurnTo = 0x6596;
		public const uint EnterRegionPermission = 0x6597;
		public const uint EnterRegion = 0x6598;
		public const uint WarpRegion = 0x6599;
		public const uint RunTo = 0x659A;  // Used for correcting positions?
		public const uint WalkTo = 0x659B; // Used for correcting positions?
		public const uint EnterRegionR = 0x659C;
		public const uint TakeOff = 0x65A8;
		public const uint TakingOff = 0x65A9;
		public const uint TakeOffR = 0x65AA;
		public const uint FlyTo = 0x65AE;
		public const uint FlyingTo = 0x65AF;
		public const uint Land = 0x65AB;
		public const uint Landing = 0x65AC;
		public const uint CanLand = 0x65AD;
		public const uint SkillInfo = 0x6979;
		public const uint SkillTrainingUp = 0x697C;
		public const uint SkillAdvance = 0x697E;
		public const uint SkillRankUp = 0x697F;
		public const uint SkillPrepare = 0x6982;
		public const uint SkillReady = 0x6983;
		public const uint SkillUse = 0x6986;
		public const uint SkillComplete = 0x6987;
		public const uint SkillCancel = 0x6989;
		public const uint SkillStart = 0x698A;
		public const uint SkillStop = 0x698B;
		public const uint SkillSilentCancel = 0x698D;
		public const uint SkillStackSet = 0x6991;
		public const uint SkillStackUpdate = 0x6992;
		public const uint UseMotion = 0x6D62;
		public const uint MotionCancel = 0x6D65;
		public const uint MotionCancel2 = 0x6D66; //Delayed?
		public const uint LevelUp = 0x6D69;
		public const uint RankUp = 0x6D6A;
		public const uint SitDown = 0x6D6C;
		public const uint StandUp = 0x6D6D;
		public const uint ArenaHideOn = 0x6D6F;
		public const uint ArenaHideOff = 0x6D70;
		public const uint ChangeStance = 0x6E28;
		public const uint ChangeStanceR = 0x6E29;
		public const uint ChangesStance = 0x6E2A;

		public const uint BackFromTheDead2 = 0x701D;
		public const uint CharacterLock = 0x701E;
		public const uint CharacterUnlock = 0x701F;
		public const uint OpenUmbrella = 0x7025;
		public const uint CloseUmbrella = 0x7026;
		public const uint SpreadWingsOn = 0x702E;
		public const uint SpreadWingsOff = 0x702F;
		public const uint ShopBuyItem = 0x7150;
		public const uint ShopBuyItemR = 0x7151;
		public const uint ShopSellItem = 0x7152;
		public const uint ShopSellItemR = 0x7153;
		public const uint OpenNPCShop = 0x715E;
		public const uint OpenMail = 0x7242;
		public const uint CloseMail = 0x7243;
		public const uint ConfirmMailRecipent = 0x7244;
		public const uint ConfirmMailRecipentR = 0x7245;
		public const uint SendMail = 0x7246;
		public const uint SendMailR = 0x7247;
		public const uint GetMails = 0x7248;
		public const uint GetMailsR = 0x7249;
		public const uint MarkMailRead = 0x724A;
		public const uint MarkMailReadR = 0x724B;
		public const uint RecieveMailItem = 0x724C;
		public const uint ReceiveMailItemR = 0x724D;
		public const uint ReturnMail = 0x724E;
		public const uint ReturnMailR = 0x724F;
		public const uint DeleteMail = 0x7250;
		public const uint DeleteMailR = 0x7251;
		public const uint RecallMail = 0x7252;
		public const uint RecallMailR = 0x7253;
		public const uint UnreadMailCount = 0x7255;
		public const uint StatUpdatePrivate = 0x7530;
		public const uint StatUpdatePublic = 0x7532;
		public const uint CombatTargetSet = 0x791A;
		public const uint CombatSetAim = 0x791D;
		public const uint CombatSetAimR = 0x791E;
		public const uint CombatSetTarget = 0x7920;
		public const uint CombatSetFinisher = 0x7921;
		public const uint CombatSetFinisher2 = 0x7922;
		public const uint CombatAction = 0x7924;
		public const uint CombatActionEnd = 0x7925;
		public const uint CombatActionBundle = 0x7926;
		public const uint CombatUsedSkill = 0x7927;
		public const uint CombatAttackR = 0x7D01;

		public const uint AreaChange = 0x88B8; // More like "event triggered"?
		public const uint QuestNew = 0x8CA0;
		public const uint QuestClear = 0x8CA1;
		public const uint QuestUpdate = 0x8CA2;
		public const uint QuestComplete = 0x8CA3;
		public const uint QuestCompleteR = 0x8CA4;
		public const uint QuestGiveUp = 0x8CA5;
		public const uint QuestGiveUpR = 0x8CA6;
		public const uint QuestStartPTJ = 0x8D68; // ?
		public const uint QuestEndPTJ = 0x8D69; // ?
		public const uint QuestUpdatePTJ = 0x8D6A;
		public const uint PartyCreate = 0x8E94;
		public const uint PartyCreateR = 0x8E95;
		public const uint PartyCreateUpdate = 0x8E96;
		public const uint PartyJoin = 0x8E97;
		public const uint PartyJoinR = 0x8E98;
		public const uint PartyJoinUpdate = 0x8E99;
		public const uint PartyLeave = 0x8E9A;
		public const uint PartyLeaveR = 0x8E9B;
		public const uint PartyLeaveUpdate = 0x8E9C;
		public const uint PartyRemove = 0x8E9D;
		public const uint PartyRemoveR = 0x8E9E;
		public const uint PartyRemoved = 0x8E9F;
		public const uint PartyChangeSetting = 0x8EA0;
		public const uint PartyChangeSettingR = 0x8EA1;
		public const uint PartySettingUpdate = 0x8EA2;
		public const uint PartyChangePassword = 0x8EA3;
		public const uint PartyChangePasswordR = 0x8EA4;
		public const uint PartyChangeLeader = 0x8EA5;
		public const uint PartyChangeLeaderR = 0x8EA6;
		public const uint PartyChangeLeaderUpdate = 0x8EA7;
		public const uint PartyWantedShow = 0x8EA9;
		public const uint PartyWantedShowR = 0x8EAA;
		public const uint PartyWantedOpened = 0x8EAB;
		public const uint PartyWantedHide = 0x8EAC;
		public const uint PartyWantedHideR = 0x8EAD;
		public const uint PartyWantedClosed = 0x8EAE;
		public const uint PartyChangeFinish = 0x8EB5;
		public const uint PartyChangeFinishR = 0x8EB6;
		public const uint PartyFinishUpdate = 0x8EB7;
		public const uint PartyChangeExp = 0x8EB8;
		public const uint PartyChangeExpR = 0x8EB9;
		public const uint PartyExpUpdate = 0x8EBA;
		public const uint GuildInfoNoGuild = 0x8EFB;
		public const uint OpenGuildPanel = 0x8EFC;
		public const uint GuildInfo = 0x8EFD;
		public const uint GuildApply = 0x8EFF;
		public const uint GuildApplyR = 0x8F00;
		public const uint GuildMembershipChanged = 0x8F01;
		public const uint GuildstoneLocation = 0x8F02;
		public const uint ConvertGp = 0x8F03;
		public const uint ConvertGpR = 0x8F04;
		public const uint ConvertGpConfirm = 0x8F05;
		public const uint ConvertGpConfirmR = 0x8F06;
		public const uint GuildDonate = 0x8F07;
		public const uint GuildDonateR = 0x8F08;
		public const uint GuildMessage = 0x8F0F;
		public const uint AddTitleKnowledge = 0x8FC0;
		public const uint AddTitle = 0x8FC1;
		public const uint ChangeTitle = 0x8FC4;
		public const uint TitleUpdate = 0x8FC5;
		public const uint ChangeTitleR = 0x8FC6;

		public const uint PetRegister = 0x9024;
		public const uint PetUnRegister = 0x9025;
		public const uint PetSummon = 0x902C;
		public const uint PetSummonR = 0x902D;
		public const uint PetUnsummon = 0x9031;
		public const uint PetUnsummonR = 0x9032;
		public const uint HitProp = 0x9088;
		public const uint HitPropR = 0x9089;
		public const uint HittingProp = 0x908A;
		public const uint TouchProp = 0x908B;
		public const uint TouchPropR = 0x908C;
		public const uint PropInteraction = 0x908D; // Doors?
		public const uint PlaySound = 0x908F;
		public const uint Effect = 0x9090;
		public const uint EffectDelayed = 0x9091;
		public const uint QuestOwlComplete = 0x9093;
		public const uint QuestOwlNew = 0x9094;
		public const uint PartyWantedUpdate = 0x9095;
		public const uint PvPInformation = 0x9096;
		public const uint NaoRevivalExit = 0x9098;
		public const uint NaoRevivalEntrance = 0x909C;
		public const uint DungeonInfo = 0x9470;
		public const uint ArenaRoundInfo = 0x9667;
		public const uint ArenaRoundInfoCancel = 0x9668;

		public const uint StatusEffectUpdate = 0xA028;
		public const uint DyePaletteReq = 0xA418;
		public const uint DyePaletteReqR = 0xA419;
		public const uint DyePickColor = 0xA41A;
		public const uint DyePickColorR = 0xA41B;
		public const uint Transformation = 0xA41C;
		//public const uint Pet??? = 0xA41D;
		public const uint SharpMind = 0xA41E;
		public const uint MoonGateRequest = 0xA428;
		public const uint MoonGateRequestR = 0xA429;
		public const uint MoonGateMap = 0xA42D;
		public const uint MoonGateUse = 0xA42E;
		public const uint MoonGateUseR = 0xA42F;
		public const uint ItemShopInfo = 0xA436;
		public const uint PartyWindowUpdate = 0xA43C;
		public const uint PartyTypeUpdate = 0xA44B;
		public const uint OpenItemShop = 0xA44D;
		public const uint MailsRequest = 0xA898;
		public const uint MailsRequestR = 0xA899;
		public const uint WarpUnk3 = 0xA8AF;
		public const uint UmbrellaJump = 0xA8E0;
		public const uint UmbrellaJumpR = 0xA8E1;
		public const uint UmbrellaLand = 0xA8E2;
		public const uint SetBgm = 0xA910;
		public const uint UnsetBgm = 0xA911;
		public const uint EnableRoyalAlchemist = 0xA9A3;
		public const uint SosButton = 0xA9A9;
		public const uint SosButtonR = 0xA9AA;
		public const uint SubsribeStun = 0xAA1C; // ?
		public const uint StunMeter = 0xAA1D;
		//public const uint StunMeter? = 0xAA1E;
		public const uint HomesteadInfoRequest = 0xAA54;
		public const uint HomesteadInfoRequestR = 0xAA55;

		// [180300, NA166 (18.09.2013)] 2 new ops somewhere here

		public const uint CollectionRequest = 0xAA87;
		public const uint CollectionRequestR = 0xAA88;

		public const uint GoBeautyShop = 0xAAF4;
		public const uint GoBeautyShopR = 0xAAF5;
		public const uint LeaveBeautyShop = 0xAAF6;
		public const uint LeaveBeautyShopR = 0xAAF7;
		public const uint OpenBeautyShop = 0xAAF8;
		//public const uint ? = 0xAAF9;	// Buy looks?
		//public const uint ? = 0xAAFA;	// Buy looks R?
		public const uint CancelBeautyShop = 0xAAFB;
		public const uint CancelBeautyShopR = 0xAAFC;

		public const uint TalentInfoUpdate = 0xAB13;
		public const uint TalentTitleChange = 0xAB14;
		public const uint TalentTitleUpdate = 0xAB15;

		public const uint ShamalaTransformationUpdate = 0xAB17;
		public const uint ShamalaTransformationUse = 0xAB18;
		public const uint ShamalaTransformation = 0xAB19;
		public const uint ShamalaTransformationEnd = 0xAB1A;
		public const uint ShamalaTransformationEndR = 0xAB1B;

		public const uint NPCTalk = 0x13882;
		public const uint NPCTalkSelect = 0x13883;

		public const uint SpecialLogin = 0x15F90; // ?
		public const uint EnterSoulStream = 0x15F91;
		//public const uint ? = 0x15F92;
		public const uint LeaveSoulStream = 0x15F93;
		public const uint LeaveSoulStreamR = 0x15F94;

		public const uint CutsceneFinished = 0x186A0;
		public const uint CutsceneStart = 0x186A6;
		public const uint CutsceneEnd = 0x186A7;
		//public const uint ? = 0x186A8;

		public const uint Weather = 0x1ADB0;

		public const uint GMCPOpen = 0x1D589;
		public const uint GMCPClose = 0x1D58A;
		public const uint GMCPSummon = 0x1D58B;
		public const uint GMCPMoveToChar = 0x1D58C;
		public const uint GMCPMove = 0x1D58D;
		public const uint GMCPRevive = 0x1D58E;
		public const uint GMCPInvisibility = 0x1D58F;
		public const uint GMCPInvisibilityR = 0x1D590;
		public const uint GMCPSearch = 0x1D595;
		public const uint GMCPExpel = 0x1D596;
		public const uint GMCPBan = 0x1D597;
		public const uint GMCPNPCList = 0x1D59F;

		public const uint PetMount = 0x1FBD0;
		public const uint PetMountR = 0x1FBD1;
		public const uint PetUnmount = 0x1FBD2;
		public const uint PetUnmountR = 0x1FBD3;
		public const uint VehicleBond = 0x1FBD4;

		public const uint Run = 0x0F213303;
		public const uint Running = 0x0F44BBA3;
		public const uint CombatAttack = 0x0FCC3231;
		public const uint Walking = 0x0FD13021;
		public const uint Walk = 0x0FF23431;

        public readonly static uint ShadowMissionMap = 0x8D6B;
        public readonly static uint ShadowMissionMapR = 0x8D6C;
        //public readonly static uint PartyQuestSomethingR = 0x8EB1; // Sent to client
        //public readonly static uint PartyQuestSomethingR = 0x8EB4; // Sent to client
        public readonly static uint ShadowMissionList = 0x8EBF;
        public readonly static uint ShadowMissionListR = 0x8EC0;
        public readonly static uint ShadowMissionAccept = 0x8EC3;
        public readonly static uint ShadowMissionAcceptR = 0x8EC4;
        public readonly static uint ShadowMissionExit = 0xA90D;
        public readonly static uint ShadowMissionExitR = 0xA90E;
        public readonly static uint ShadowMissionRegionR = 0xA97E;

		// Messenger Server
		// ------------------------------------------------------------------
		public static class Msgr
		{
			public const uint Login = 0xC350;
			public const uint LoginR = 0xC351;
			public const uint FriendInvite = 0xC352;
			public const uint FriendInviteR = 0xC353;
			public const uint FriendConfirm = 0xC354;
			public const uint FriendReply = 0xC355;
			public const uint ChatInvite = 0xC356;
			public const uint FriendList = 0xC358;
			public const uint FriendListR = 0xC359;
			public const uint FriendBlock = 0xC35A;
			public const uint FriendBlockR = 0xC35B;
			public const uint FriendUnblock = 0xC35C;
			public const uint FriendUnblockR = 0xC35D;
			public const uint FriendOnline = 0xC35E;
			public const uint FriendOffline = 0xC35F;

			public const uint ChatBegin = 0xC360;
			public const uint ChatBeginR = 0xC361;
			public const uint ChatEnd = 0xC362;
			public const uint ChatInviteR = 0xC366;
			public const uint ChatLeave = 0xC367;
			public const uint Chat = 0xC368;
			public const uint ChatR = 0xC36A;
			public const uint FriendDelete = 0xC36B;
			public const uint ChatJoin = 0xC36C;
			public const uint GuildChat = 0xC36E;
			public const uint GuildChatR = 0xC36F;

			public const uint ChangeOption = 0xC370;
			public const uint ChangeOptionR = 0xC371;
			public const uint FriendOptionChanged = 0xC372;
			public const uint GroupList = 0xC376;
			public const uint SendNote = 0xC37E;
			public const uint SendNoteR = 0xC37F;

			public const uint NoteList = 0xC380;
			public const uint NoteListR = 0xC381;
			public const uint NoteDelete = 0xC382;
			public const uint Refresh = 0xC384;
			public const uint YouGotNote = 0xC385;
			public const uint ReadNote = 0xC386;
			public const uint ReadNoteR = 0xC387;
			public const uint ChangeChannel = 0xC389;
			public const uint FriendChannelChanged = 0xC38A;
			public const uint GuildMemberList = 0xC38B;
			public const uint GuildMemberListR = 0xC38C;
			public const uint GuildMemberState = 0xC38D;

			public const uint PlayerBlock = 0xC392;
		}

		// Internal communication
		// ------------------------------------------------------------------
		public static class Internal
		{
			public const uint ServerIdentify = 0x42420000;
			public const uint ChannelStatus = 0x42420101;
		}

		// Op History
		// ------------------------------------------------------------------

		// [EU]
		// Known differences between G14 (below) and G15
		// ClientIdent = 0x1E;
		// Login = 0x0F010000;
		// CreatePartner += 0x11110000;  // non-existent
		// CreatePartnerR += 0x11110000; // non-existent
		// AccountInfoRequest = 0x45;
		// AccountInfoRequestR = 0x46;
		// AcceptGift = 0x47;
		// AcceptGiftR = 0x48;
		// RefuseGift = 0x49;
		// RefuseGiftR = 0x4A;
		// Disconnect = 0x4B;
		// 
		// Run = 0x0F010300;
		// Running = 0x0F0400A0;
		// Walk = 0x0F010800;
		// Walking = 0x0FA10020;
		// CombatAttack = 0x0FB10001;

		// [NA170401, TW170300, KR180200, KRT180300]
		// New GMCP ops, old ones:
		// GMCPOpen = 0x4EE9;
		// GMCPClose = 0x4EEA;
		// GMCPSummon = 0x4EEB;
		// GMCPMoveToChar = 0x4EEC;
		// GMCPMove = 0x4EED;
		// GMCPRevive = 0x4EEE;
		// GMCPInvisibility = 0x4EEF;
		// GMCPInvisibilityR = 0x4EF0;
		// GMCPSearch = 0x4EF5; // ?
		// GMCPExpel = 0x4EF6;
		// GMCPBan = 0x4EF7;
		// GMCPNPCList = 0x4EFF;

		// [170401]
		// Values shifted due to inserts?
		// Old ones:
		// GoBeautyShop = 0xAAEC;
		// OpenBeautyShop = 0xAAED;
		// ShamalaTransformationUpdate = 0xAB13;
		// ShamalaTransformationUse = 0xAB14;
		// ShamalaTransformation = 0xAB15;
		// ShamalaTransformationEnd = 0xAB16;
		// ShamalaTransformationEndR = 0xAB17;

		// [180300, NA166 (18.09.2013)]
		// Values shifted due to inserts?
		// Old ones:
		// NPCTalkSelectEnd = 0x59F9;
		// CollectionRequest = 0xAA85;
		// CollectionRequestR = 0xAA86;
		// GoBeautyShop = 0xAAF2;
		// GoBeautyShopR = 0xAAF3;
		// LeaveBeautyShop = 0xAAF4;
		// LeaveBeautyShopR = 0xAAF5;
		// OpenBeautyShop = 0xAAF6;
		// CancelBeautyShop = 0xAAF9;
		// CancelBeautyShopR = 0xAAFA;
		// TalentInfoUpdate = 0xAB11;
		// TalentTitleChange = 0xAB12;
		// TalentTitleUpdate = 0xAB13;
		// ShamalaTransformationUpdate = 0xAB15;
		// ShamalaTransformationUse = 0xAB16;
		// ShamalaTransformation = 0xAB17;
		// ShamalaTransformationEnd = 0xAB18;
		// ShamalaTransformationEndR = 0xAB19;
	}
}
