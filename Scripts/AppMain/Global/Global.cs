using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.Global
{
    public static class Global
    {
        public static string version = "2017.2";
        public static string photonAppKey = "52ce8009-c83d-4bc5-90ea-61f5bb39d10a";
        public static string mainSceneName = "MainGame";
        public static int playerPerRoom = 10;
        public static int instructionListeningPort = 5054;
        public static int instructionListeningInterval = 16;
        public static byte creditByte = 0x1f;
        public static int UdpBytesLength = 16;
    }

    public enum GameMode
    {
        unconnected,
        inLobby,
        inRoom,
        isServer,
        inGame,
        inOfflineGame,
        asOfflineAIServer,
        isPlayBack,
    }

    public enum GameStage
    {
        inRoom,
        preparing,
        main,
        end
    }

    public static class Singletons
    {
        public static WholeGame.WholeGameController wholeGameController;
        public static WholeGame.GameRoutineController gameRoutineController = new WholeGame.GameRoutineController();
        public static WholeGame.GameInteractionManager gameInteractionManager = new WholeGame.GameInteractionManager();
    }

    public static class GameState
    {
        public static bool isOnline = false;
        public static GameMode mode;
        public static GameStage stage;
        public static bool isRecording;
        public static string saveFilename;
        public static int playerID;
        public static int nPlayer;
        public static int[] AllPlayerIDs;
        public static MainGame.Stats.GameStatManager gameStatManager;
    }

    public static class GameSettings
    {
        public static string serverIPAddress;
        private static float mouseSensitivity = 0.5f;
        public static float MouseSensitivty
        {
            get
            {
                return mouseSensitivity;
            }
            set
            {
                mouseSensitivity = value;
                if(mouseSensitivity > 1f)
                {
                    mouseSensitivity = 1f;
                }
                else if(mouseSensitivity < 0.3f)
                {
                    mouseSensitivity = 0.3f;
                }
            }
        }
        public static float maxRayDistance = 400.0f;

        public const int n_AmmoKind = 1;
        public const int n_TankBodyPart = 4;
    }

}
