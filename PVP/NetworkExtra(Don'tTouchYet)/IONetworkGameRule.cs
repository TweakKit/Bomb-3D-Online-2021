﻿//using UnityEngine;
//using UnityEngine.Networking;

//public class IONetworkGameRule : BaseNetworkGameRule
//{
//    public UIGameplay uiGameplayPrefab;

//    public override bool HasOptionBotCount { get { return true; } }
//    public override bool HasOptionMatchTime { get { return false; } }
//    public override bool HasOptionMatchKill { get { return false; } }
//    public override bool HasOptionMatchScore { get { return false; } }
//    public override bool ShowZeroScoreWhenDead { get { return true; } }
//    public override bool ShowZeroKillCountWhenDead { get { return true; } }
//    public override bool ShowZeroAssistCountWhenDead { get { return true; } }
//    public override bool ShowZeroDieCountWhenDead { get { return true; } }

//    protected override BaseNetworkGameCharacter NewBot()
//    {
//        var gameInstance = GameInstance.Singleton;
//        var botList = gameInstance.bots;
//        var bot = botList[Random.Range(0, botList.Length)];
//        var botEntity = Instantiate(gameInstance.botPrefab);
//        botEntity.playerName = bot.name;
//        botEntity.selectHead = bot.GetSelectHead();
//        botEntity.selectCharacter = bot.GetSelectCharacter();
//        botEntity.selectBomb = bot.GetSelectBomb();
//        return botEntity;
//    }

//    protected override void EndMatch()
//    {
//    }

//    public override bool CanCharacterRespawn(BaseNetworkGameCharacter character, params object[] extraParams)
//    {
//        var gameplayManager = GameplayPerformManager.Singleton;
//        var targetCharacter = character as CharacterEntity;
//        return Time.unscaledTime - targetCharacter.deathTime >= gameplayManager.respawnDuration;
//    }

//    public override bool RespawnCharacter(BaseNetworkGameCharacter character, params object[] extraParams)
//    {
//        var isWatchedAds = false;
//        if (extraParams.Length > 0 && extraParams[0] is bool)
//            isWatchedAds = (bool)extraParams[0];

//        var targetCharacter = character as CharacterEntity;
//        var gameplayManager = GameplayPerformManager.Singleton;
//        // For IO Modes, character stats will be reset when dead
//        if (!isWatchedAds || targetCharacter.watchAdsCount >= gameplayManager.watchAdsRespawnAvailable)
//            targetCharacter.Reset();
//        else
//            ++targetCharacter.watchAdsCount;

//        return true;
//    }
    
//    public override void InitialClientObjects()
//    {
//        var ui = FindObjectOfType<UIGameplay>();
//        if (ui == null && uiGameplayPrefab != null)
//            ui = Instantiate(uiGameplayPrefab);
//        if (ui != null)
//            ui.gameObject.SetActive(true);
//    }
//}
