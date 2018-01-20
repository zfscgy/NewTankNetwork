using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.Global
{
    public static class Global
    {
        public static string version = "201701";
        public static string photonAppKey = "52ce8009-c83d-4bc5-90ea-61f5bb39d10a";
        public static string mainSceneName = "MainGame";
        public static int playerPerRoom = 10;
    }

    public enum GameMode
    {
        unconnected,
        inLobby,
        inRoom,
        isServer,
        inGame,
        inOfflineGame,
    }

    public static class GameState
    {
        public static bool isOnline = false;
        public static GameMode mode;
        public static int playerID;
    }

    public static class GameSettings
    {
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

    }
}
