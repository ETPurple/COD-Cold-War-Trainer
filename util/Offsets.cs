using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ___.util
{
    class Offsets
    {
        // Big thanks to JayKoZa2015 on UnKnoWnCheaTs for the following addresses and offsets.
        // Source will have these blanked, be sure to change them to their latest values!
        public static int gamePID = 0;
        public static IntPtr hProc;
        public static IntPtr baseAddress = IntPtr.Zero;
        public static Process gameProc;

        public static IntPtr PlayerCompPtr, PlayerPedPtr, ZMGlobalBase, ZMBotBase, ZMBotListBase, ZMXPScalePtr;

        //public const int PC_ArraySize_Offset = 0xB830;
        public static IntPtr PlayerBase = (IntPtr)0x10888578;
        public static IntPtr ZMXPScaleBase = (IntPtr)0x108B0570; //ban will happen
        public static IntPtr TimeScaleBase = (IntPtr)0xF94C794 + 0x7C; //ban will happen
        public static IntPtr CMDBufferBase = (IntPtr)0x120ECA40 + 0x50418;
        public static IntPtr XPScaleZM = (IntPtr)0x0; //ban will happen
        public static IntPtr GunXPScaleZM = (IntPtr)0x0; //ban will happen

        public static int ZMPlayerArrayOffset = 0xB6E0;

        public static int PC_ArraySize_Offset = 0xB940; // Size of Array between Players Data [IDK if it got changed with 1.7.1 if it got changed, please post the new Offset, i will update them here. dont have the time to check it self]
        public static int PC_CurrentUsedWeaponID = 0x28; // Shows Current Used WeaponID (this are only Readable IDs, so change is not working with them on me)
        public static int PC_SetWeaponID = 0xB0; // +(1-5 * 0x40 for WP2 to WP6) Can be used to change a WeaponID correctly from ID 1-300 (! info !, some IDs can result in GameCrashes!).
        public static int PC_InfraredVision = 0xE66; // (byte) On=0x10|Off=0x0
        public static int PC_GodMode = 0xE67; // (byte) On=0xA0|Off=0x20
        public static int PC_RapidFire1 = 0xE6C; // Freeze to 0 how long you press Left Mouse-Key or Reloading and other stuff is not working.
        public static int PC_RapidFire2 = 0xE80; // Freeze to 0 how long you press Left Mouse-Key or Reloading and other stuff is not working.
        public static int PC_MaxAmmo = 0x1360; // +(1-5 * 0x8 for WP1 to WP6) (WP0 Mostly used in MP, ZM first WP is WP1 | WP3-6 Mostly used for Granades and Special) The Game assign the next Free WP Slot so WP1 is MainWeapon, you get a granade, then WP2 is the Granade, you buy a Weapon from wall then this is WP3 and so on..
        public static int PC_Ammo = 0x13D4; // +(1-5 * 0x4 for WP1 to WP6) (WP0 Mostly used in MP, ZM first WP is WP1 | WP3-6 Mostly used for Granades and Special)
        public static int PC_Points = 0x5D14; // ZM Points / Money
        public static int PC_Name = 0x5C0A; // Playername
        public static int PC_RunSpeed = 0x5D14; // (float)
        public static int PC_ClanTags = 0x605C; // Player Clan/Crew-Tag
        public static int PC_Crit = 0x10CC; //crit

        // PlayerPedPtr - Offsets
        public static int PP_ArraySize_Offset = 0x5F8; // ArraySize to next Player.
        public static int PP_Health = 0x398;
        public static int PP_MaxHealth = 0x39C; // Max Health dont increase by using Perk Juggernog
        public static int PP_Coords = 0x2D4; // Vector3
        public static int PP_Heading_Z = 0x34; // float
        public static int PP_Heading_XY = 0x38; // float | can be used to TP Zombies in front of you by your Heading Position and Forward Distance.

        // ZMGlobalBase - Offsets
        // The Move Offset got removed with Patch 1.6.0 so Move Offset is no longer needed! Use 0x0 if you have add this Offset to your code... OLD: public int ZM_Global_MovedOffset = 0x2F20; // Since 1.5.0 The data got moved by this Offset so ZM_Global_MovedOffset + ZM_Global_ZombiesIgnoreAll is the corretly Offset to ZombiesIgnoreAll
        public static int ZM_Global_ZombiesIgnoreAll = 0x14; // Zombies Ignore any Player in the Lobby.
        public static int ZM_Global_ZMLeftCount = 0x3C; // Zombies Left

        // ZMBotBase - Offsets
        public static int ZM_Bot_List_Offset = 0x8; // Offset to Pointer at ZMBotBase + 0x8 -> ZMBotListBase

        // ZMBotListBase - Offsets
        public static int ZM_Bot_ArraySize_Offset = 0x5F8; // ArraySize to next Zombie.
        public static int ZM_Bot_Health = 0x398;
        public static int ZM_Bot_MaxHealth = 0x39C;
        public static int ZM_Bot_Coords = 0x2D4; // Cam be used to Teleport all Zombies in front of any Player with a Heading Variable from the Players.


        // ZMXPScaleBase - Offsets (All Offsets add to Base Address == ZMXPScaleBase + XPEP_Offset as example)
        public static int XPEP_InGame_Offset = 0x20; // Use 0x28 too for Real Added XP not only Visible InGame, 0x20 only shows InGame. 0x28 Add it really! So Combine 0x20 and 0x28 with same Values.
        public static int XPUNK01_Offset = 0x24; // K/A Modifier
        public static int XPUNK02_Offset = 0x28; // 0x28 Add it really! Use 0x20 for the InGame Visibility. So Combine 0x20 and 0x28 with same Values.
        public static int XPUNK03_Offset = 0x2c; // K/A Modifier
        public static int XPGun_Offset = 0x30; // works like it is, 1.00f == Normal, 2.00f == x2 etc...
        public static int XPUNK04_Offset = 0x34; // K/A Modifier
        public static int XPUNK05_Offset = 0x38; // K/A Modifier
        public static int XPUNK06_Offset = 0x3c; // K/A Modifier
        public static int XPUNK07_Offset = 0x40; // currently it is 0.00f idk what id do.
        public static int XPUNK08_Offset = 0x44; // K/A Modifier
        public static int XPUNK09_Offset = 0x48; // K/A Modifier
        public static int XPUNK10_Offset = 0x4C; // K/A Modifier




        // CMDBufferBase - Offsets
        public static int CMDBB_Exec = -0x1B;
    }
}
