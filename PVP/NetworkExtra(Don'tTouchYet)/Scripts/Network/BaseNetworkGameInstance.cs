﻿//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Rendering;

//public abstract class BaseNetworkGameInstance : MonoBehaviour
//{
//    public const string ARG_SERVER_START = "-gameServerStart";
//    public const string ARG_SERVER_PORT = "-gameServerPort";
//    public const string ARG_SERVER_MAX_CONNECTIONS = "-gameMaxConnections";
//    public const string ARG_SERVER_GAME_ONLINE_SCENE = "-gameOnlineScene";
//    public const string ARG_SERVER_GAME_RULE = "-gameRule";
//    public const string ARG_SERVER_BOT_COUNT = "-gameBotCount";
//    public BaseNetworkGameRule[] gameRules;
//    public static Dictionary<string, BaseNetworkGameRule> GameRules = new Dictionary<string, BaseNetworkGameRule>();
//    protected virtual void Awake()
//    {
//        GameRules.Clear();
//        foreach (var gameRule in gameRules)
//        {
//            GameRules[gameRule.name] = gameRule;
//        }
//    }

//    protected virtual void Start()
//    {
//        var args = System.Environment.GetCommandLineArgs();
//        // Android fix
//        if (args == null)
//            args = new string[0];
//        // Set manager instance
//        var manager = SimpleLanNetworkManager.Singleton as BaseNetworkGameManager;
//        // If game running in batch mode, run as server
//        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null || IsArgsProvided(args, ARG_SERVER_START))
//        {
//            Application.targetFrameRate = 30;
//            Debug.Log("Running as server in batch mode");
//            var maxConnections = manager.maxConnections;
//            manager.maxConnections = ReadArgsInt(args, ARG_SERVER_MAX_CONNECTIONS, maxConnections);
//            var onlineScene = manager.onlineScene;
//            manager.onlineScene = ReadArgs(args, ARG_SERVER_GAME_ONLINE_SCENE, onlineScene);

//            var gameRule = manager.gameRule;
//            if (GameRules.Count > 0)
//            {
//                var allGameRules = new List<BaseNetworkGameRule>(GameRules.Values);
//                gameRule = allGameRules[0];
//                var gameRuleName = ReadArgs(args, ARG_SERVER_GAME_RULE);
//                if (!string.IsNullOrEmpty(gameRuleName))
//                    GameRules.TryGetValue(gameRuleName, out gameRule);

//                var botCount = ReadArgsInt(args, ARG_SERVER_BOT_COUNT, 0);
//                if (gameRule != null)
//                    gameRule.botCount = botCount;

//                manager.gameRule = gameRule;
//            }

//            manager.StartDedicateServer();
//        }
//    }

//    private string ReadArgs(string[] args, string argName, string defaultValue = null)
//    {
//        if (args == null)
//            return defaultValue;

//        var argsList = new List<string>(args);
//        if (!argsList.Contains(argName))
//            return defaultValue;

//        var index = argsList.FindIndex(0, a => a.Equals(argName));
//        return args[index + 1];
//    }

//    public int ReadArgsInt(string[] args, string argName, int defaultValue = -1)
//    {
//        var number = ReadArgs(args, argName, defaultValue.ToString());
//        var result = defaultValue;
//        if (int.TryParse(number, out result))
//            return result;
//        return defaultValue;
//    }

//    public bool IsArgsProvided(string[] args, string argName)
//    {
//        if (args == null)
//            return false;

//        var argsList = new List<string>(args);
//        return argsList.Contains(argName);
//    }
//}
