﻿//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;

//public class DeathMatchNetworkGameRule : IONetworkGameRule
//{
//    public int endMatchCountDown = 10;
//    public int EndMatchCountingDown { get; protected set; }
//    public override bool HasOptionBotCount { get { return true; } }
//    public override bool HasOptionMatchTime { get { return true; } }
//    public override bool HasOptionMatchKill { get { return true; } }
//    public override bool HasOptionMatchScore { get { return false; } }
//    public override bool ShowZeroScoreWhenDead { get { return false; } }
//    public override bool ShowZeroKillCountWhenDead { get { return false; } }
//    public override bool ShowZeroAssistCountWhenDead { get { return false; } }
//    public override bool ShowZeroDieCountWhenDead { get { return false; } }

//    protected override void EndMatch()
//    {
//        networkManager.StartCoroutine(EndMatchRoutine());
//    }

//    IEnumerator EndMatchRoutine()
//    {
//        EndMatchCountingDown = endMatchCountDown;
//        while (EndMatchCountingDown > 0)
//        {
//            yield return new WaitForSeconds(1);
//            --EndMatchCountingDown;
//        }
//        networkManager.StopHost();
//    }

//    public override bool RespawnCharacter(BaseNetworkGameCharacter character, params object[] extraParams)
//    {
//        var targetCharacter = character as CharacterEntity;
//        var gameplayManager = GameplayPerformManager.Singleton;
//        // In death match mode will not reset score, kill, assist, death
//        targetCharacter.watchAdsCount = 0;

//        return true;
//    }

//    public override void InitialClientObjects()
//    {
//        base.InitialClientObjects();
//        var gameplayManager = FindObjectOfType<GameplayPerformManager>();
//        if (gameplayManager != null)
//        {
//            gameplayManager.killScore = 1;
//            gameplayManager.suicideScore = 0;
//        }
//    }
//}
