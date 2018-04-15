using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.WholeGame
{
    using Global;
    using MainGame.Base;
    public enum PlayerStatus
    {
        notExisting,
        active,
        dead,
    }
    /*
     * This class is in charge of all the tanks in the game
     * including the birth and death of the tank, and the whole games start and end
     */
    public class GameRoutineController
    {
        private Tank[] AllTanks = new Tank[Global.playerPerRoom];
        private int nTank = 0;
        private PlayerStatus[] AllPlayerStatus = new PlayerStatus[Global.playerPerRoom];
        private int redCount = 0;
        private int blueCount = 0;
        public GameRoutineController()
        {
        }
        public Tank[] GetTanks()
        {
            return AllTanks;
        }
        /// <summary>
        /// Anyone who wants to get a new tank, must register here, to get the new tank's seatID
        /// </summary>
        /// <returns>seatID, -1 means the number of tank already reaches the limit</returns>
        public int RegisterNewTank()
        {
            if(nTank<Global.playerPerRoom)
            {
                for(int i = 0; i< AllTanks.Length; i++)
                {
                    if(AllTanks[i] == null)
                    {
                        GameState.playerNum++;
                        AllPlayerStatus[i] = PlayerStatus.active;
                        if (i >= 5) blueCount++; else redCount++;
                        return i;
                    }
                }
                return -1;
            }
            else
            {
                return -1;
            }
        }

        public int RegisterNewTank(int seatID)
        {
            if (seatID < Global.playerPerRoom && AllTanks[seatID] == null)
            {
                GameState.playerNum++;
                AllPlayerStatus[seatID] = PlayerStatus.active;
                if (seatID >= 5) blueCount++; else redCount++;
                return seatID;
            }
            return -1;
        }
        /// <summary>
        /// The death of any tank must register here.
        /// </summary>
        /// <param name="id">The dead tank's seatID</param>
        public void OnePlayerDie(int id)
        {
            if(AllPlayerStatus[id] == PlayerStatus.dead)
            {
                return;//Already dead
            }
            AllPlayerStatus[id] = PlayerStatus.dead;
            if(id < AllPlayerStatus.Length/2)
            {
                redCount--;
            }
            else
            {
                blueCount--;
            }
            if(redCount == 0)
            {
                GameEnded(0);
            }
            if(blueCount == 0)
            {
                GameEnded(1);
            }
        }
        public void GameStart(int[] playerSeatIDs)
        {
            for (int i = 0; i < AllPlayerStatus.Length; i++)
            {
                AllPlayerStatus[playerSeatIDs[i]] = PlayerStatus.notExisting;
            }
        }
        public void GameEnded(int win)
        {
            Singletons.wholeGameController.WaitToExec(Singletons.wholeGameController.mainGameLoader.Restart, 10);

        }

    }
}
