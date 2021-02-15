using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ___.util
{
    class IAPI
    {
        //client ID's max 4 and starts at 0 ( 0 1 2 3 )
        public static int playerOne = 0;
        public static int playerTwo = 1;
        public static int playerThree = 2;
        public static int playerFour = 3;


        //Sets the players current amount of money ( put in a loop ) 
        public static void SetMoney(int clientID, int moneyamount)
        {
            byte[] _PlayerHP = new byte[16];
            util.Game.WriteProcessMemory(util.Offsets.hProc, util.Offsets.PlayerCompPtr + (util.Offsets.PC_ArraySize_Offset * clientID) + util.Offsets.PC_Points, moneyamount, 8, out _);
        }

        //Returns the players current amount of money
        public static int GetMoney(int clientID)
        {
            byte[] _currentmoney = new byte[8];
            util.Game.ReadProcessMemory(util.Offsets.hProc, util.Offsets.PlayerCompPtr + (util.Offsets.PC_ArraySize_Offset * clientID) + util.Offsets.PC_Points, _currentmoney, 8, out _);
            return Convert.ToInt32(_currentmoney);
        }

        //Returns the players name
        public static string GetName(int clientID)
        {
            byte[] _playerName = new byte[16];
            util.Game.ReadProcessMemory(util.Offsets.hProc, util.Offsets.PlayerCompPtr + (util.Offsets.PC_ArraySize_Offset * clientID) + util.Offsets.PC_Name, _playerName, 16, out _);
            return Encoding.Default.GetString(_playerName);
        }

        //Returns the number of zombies alive
        public static int GetZombieCount()
        {
            byte[] _zombieCount = new byte[4];
            util.Game.ReadProcessMemory(util.Offsets.hProc, util.Offsets.ZMGlobalBase + util.Offsets.ZM_Global_ZMLeftCount, _zombieCount, 4, out _);
            return BitConverter.ToInt32(_zombieCount, 0);
        }
        

        //Freezes the players ammo values to simulate "infinite ammo"
        public static void FreezeAmmo(int clientID)
        {
            for (int i = 1; i < 6; i++)
            {
                util.Game.WriteProcessMemory(util.Offsets.hProc, util.Offsets.PlayerCompPtr + (util.Offsets.PC_ArraySize_Offset * clientID ) + util.Offsets.PC_Ammo + (i * 0x4), 20, 4, out _);
            }
        }

        //Turns Godmode On for the player
        public static void GodModeOn(int clientID)
        {
            util.Game.WriteProcessMemory(util.Offsets.hProc, util.Offsets.PlayerCompPtr + (util.Offsets.PC_ArraySize_Offset * clientID ) +  util.Offsets.PC_GodMode, 0xA0, 1, out _);
        }
        
        //Turns Godmode Off for the player
        public static void GodModeOff(int clientID)
        {
            util.Game.WriteProcessMemory(util.Offsets.hProc, util.Offsets.PlayerCompPtr + (util.Offsets.PC_ArraySize_Offset * clientID) + util.Offsets.PC_GodMode, 0x20, 1, out _);
        }

        //Sets the players current speed.
        public static void SetSpeed(int clientID, int playerSpeed)
        {
            util.Game.WriteProcessMemory(util.Offsets.hProc, util.Offsets.PlayerCompPtr + (util.Offsets.PC_ArraySize_Offset * clientID ) + util.Offsets.PC_RunSpeed, Convert.ToSingle(playerSpeed), 4, out _);
        }
    }
}
