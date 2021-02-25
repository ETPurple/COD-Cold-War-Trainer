using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ___.util
{
    class IAPI
    {
        /// <summary>
        /// Player ID's ( Client ID starts at 0 and ends at 3 ( 0 1 2 3 ) 
        /// </summary
        public static int playerOne = 0;
        public static int playerTwo = 1;
        public static int playerThree = 2;
        public static int playerFour = 3;


        /// <summary>
        /// Sets players current points value ( put in a loop :) )
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="moneyamount"></param>
        public static void SetMoney(int clientID, int moneyamount)
        {
            byte[] _PlayerHP = new byte[16];
            Game.WriteProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + (Offsets.PC_ArraySize_Offset * clientID) + Offsets.PC_Points, moneyamount, 8, out _);
        }

        /// <summary>
        /// Returns the players current amount of points
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public static int GetMoney(int clientID)
        {
            byte[] _currentmoney = new byte[8];
            Game.ReadProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + (Offsets.PC_ArraySize_Offset * clientID) + Offsets.PC_Points, _currentmoney, 8, out _);
            return Convert.ToInt32(_currentmoney);
        }

        /// <summary>
        /// Returns the players name
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public static string GetName(int clientID)
        {
            byte[] _playerName = new byte[16];
            Game.ReadProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + (Offsets.PC_ArraySize_Offset * clientID) + Offsets.PC_Name, _playerName, 16, out _);
            return Encoding.Default.GetString(_playerName);
        }

        /// <summary>
        /// Returns the number of zombies left alive
        /// </summary>
        /// <returns></returns>
        public static int GetZombieCount()
        {
            byte[] _zombieCount = new byte[4];
            Game.ReadProcessMemory(Offsets.hProc, Offsets.ZMGlobalBase + Offsets.ZM_Global_ZMLeftCount, _zombieCount, 4, out _);
            return BitConverter.ToInt32(_zombieCount, 0);
        }
        

        /// <summary>
        /// Sets the players ammo to the specified amount
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="ammoCount"></param>
        public static void FreezeAmmo(int clientID, int ammoCount)
        {
            for (int i = 1; i < 6; i++)
            {
                Game.WriteProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + (Offsets.PC_ArraySize_Offset * clientID ) + Offsets.PC_Ammo + (i * 0x4), ammoCount, 4, out _);
            }
        }

        /// <summary>
        /// Enables invicibility for the player
        /// </summary>
        /// <param name="clientID"></param>
        public static void GodModeOn(int clientID)
        {
            Game.WriteProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + (Offsets.PC_ArraySize_Offset * clientID ) +  Offsets.PC_GodMode, 0xA0, 1, out _);
        }
        
        /// <summary>
        /// Disables invicibility for the player
        /// </summary>
        /// <param name="clientID"></param>
        public static void GodModeOff(int clientID)
        {
            Game.WriteProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + (Offsets.PC_ArraySize_Offset * clientID) + Offsets.PC_GodMode, 0x20, 1, out _);
        }

        /// <summary>
        /// Sets the players movement speed
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="playerSpeed"></param>
        public static void SetSpeed(int clientID, int playerSpeed)
        {
            Game.WriteProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + (Offsets.PC_ArraySize_Offset * clientID ) + Offsets.PC_RunSpeed, Convert.ToSingle(playerSpeed), 4, out _);
        }


        /// <summary>
        /// Returns the players Position in a vector3
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetPlayerPos()
        {
            // create new byte array for player coordinates, reads them, and then sets the XYZ coordinates accordingly
            byte[] playerCoords = new byte[12];
            Game.ReadProcessMemory(Offsets.hProc, Offsets.PlayerPedPtr + Offsets.PP_Coords, playerCoords, 12, out _);
            var origx = BitConverter.ToSingle(playerCoords, 0);
            var origy = BitConverter.ToSingle(playerCoords, 4);
            var origz = BitConverter.ToSingle(playerCoords, 8);
            // updates the current playerposition with a Vector3 created from the xyz coordinates

            //Math.Round(val,2) round to the 2nd decimal place
            var POS = new Vector3((float)Math.Round(origx, 2), (float)Math.Round(origy, 2), (float)Math.Round(origz, 2));
            return POS;
        }

        /// <summary>
        /// Returns the specific players HP
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public static int GetHP(int clientID)
        {
            byte[] playerHealth = new byte[4];
            Game.ReadProcessMemory(Offsets.hProc, Offsets.PlayerPedPtr + (Offsets.PC_ArraySize_Offset * clientID) + Offsets.PP_Health, playerHealth, 4, out _);
            return BitConverter.ToInt32(playerHealth, 0);
        }


        /// <summary>
        /// Returns the given players Ammo per specified weapon *1-5* ( 1 2 3 4 5 )
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="weaponSlot"></param>
        /// <returns></returns>
        public static int GetAmmo(int clientID, int weaponSlot)
        {
            byte[] playerAmmo = new byte[4];
            Game.WriteProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + (Offsets.PC_ArraySize_Offset * clientID) + Offsets.PC_Ammo + (weaponSlot * 0x4), 20, 4, out _);
            return BitConverter.ToInt32(playerAmmo, 0);
        }

        /// <summary>
        /// N/A
        /// </summary>
        public static void SetPos()
        {
            // _memory.WriteBytes(_zmBotListBase + (Offsets.ZombieBotListBase.BotArraySizeOffset * i) + Offsets.ZombieBotListBase.Coords, enemyPosBuffer);

            var newcords = new Vector3(-49392, -20150, 633);
            var pos2 = new Vector3((float)Math.Round(newcords.X, 2), (float)Math.Round(newcords.Y, 2), (float)Math.Round(newcords.Z, 2));

            Game.WriteProcessMemory(Offsets.hProc, Offsets.PlayerPedPtr + (Offsets.PC_ArraySize_Offset * 0) + Offsets.PP_Coords, pos2, 12, out _);
            // updates the current playerposition with a Vector3 created from the xyz coordinates
        }

        /// <summary>
        /// N/A
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static async Task SendCommand(string command)
        {
            Game.WriteProcessMemory(Offsets.hProc, Offsets.CMDBufferBase, command + "\0", (uint)command.Length + 2, out _); // Write the command
            Game.WriteProcessMemory(Offsets.hProc, Offsets.CMDBufferBase - 0x1B, null, 4, out _); // Enter the command
            await Task.Delay(15);
            Game.WriteProcessMemory(Offsets.hProc, Offsets.CMDBufferBase, command + "\0", (uint)command.Length + 2, out _); // Clear Input

        }

        public static void EnableThermal(int clientID)
        {
            Game.WriteProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + (Offsets.PC_ArraySize_Offset * clientID) + Offsets.PC_InfraredVision, 0x10, 4, out _);
        }

        public static void DisableThermal(int clientID)
        {
            Game.WriteProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + (Offsets.PC_ArraySize_Offset * clientID) + Offsets.PC_InfraredVision, 0x0, 4, out _);
        }
    }
}
