using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using ___.util;
using System.IO;

namespace ___
{
    public partial class Main : Form
    {
        public bool trainerEnabled = false;
        public Single playerSpeed = -1f;
        public int zmTeleportDistance = 150;
        public bool unlimitedAmmo = false;
        public int[] ammoVals = new int[6];
        public int[] maxAmmoVals = new int[6];
        public bool freezePlayer;
        public Vector3 frozenPlayerPos = Vector3.Zero;
        public Vector3 lastKnownPlayerPos = Vector3.Zero;
        public Vector3 updatedPlayerPos = Vector3.Zero;
        public Single xpModifier = 175.0f;
        public Single gunXpModifier = 175.0f;

        //God mode Booleans
        public bool godModePlayerOne = false;
        public bool godModePlayerTwo = false;
        public bool godModePlayerThree = false;
        public bool godModePlayerFour = false;

        //Unlimited Ammo Booleans
        public bool uaPlayerOne = false;
        public bool uaPlayerTwo = false;
        public bool uaPlayerThree = false;
        public bool uaPlayerFour = false;

        //Unlimited Money Booleans
        public bool umPlayerOne = false;
        public bool umPlayerTwo = false;
        public bool umPlayerThree = false;
        public bool umPlayerFour = false;

        public bool tpZombiesToCursor = false;
        public bool zombiesInstaKill = false;
        public bool globalXpBoost = false;


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {

                    // if trainer isnt enabled, do nothing yet
                    if (!trainerEnabled) continue;

                    // get all processes called "BlackOpsColdWar"
                    var gameProcs = Process.GetProcessesByName("BlackOpsColdWar");

                    // if there aren't any processes, update the game message label and do nothing
                    if (gameProcs.Length < 1)
                    {
                        //UpdateLabel(lblGameRunning, "Game is not running", "Red");
                        continue;
                    }

                    // get first process from the gameProcs array
                    util.Offsets.gameProc = gameProcs[0];

                    // set gamePID to the Id of the gameProc
                    util.Offsets.gamePID = util.Offsets.gameProc.Id;

                    // update the label as needed, if for whatever reason the gamePID doesnt exist, update the label and do nothing
                    if (util.Offsets.gamePID > 0)
                    {
                        //MessageBox.Show("Game is Running!");
                    }
                    else
                    {
                        //UpdateLabel(lblGameRunning, "Game is not running", "Red");
                        continue;
                    }

                    // opens the process or something, not 100% still learning all this terminology
                    util.Offsets.hProc = util.Game.OpenProcess(util.Game.ProcessAccessFlags.All, false, util.Offsets.gameProc.Id);

                    // if the base address isn't uptodate, update it
                    if (util.Offsets.baseAddress != util.Game.GetModuleBaseAddress(util.Offsets.gameProc, "BlackOpsColdWar.exe")) util.Offsets.baseAddress = util.Game.GetModuleBaseAddress(util.Offsets.gameProc, "BlackOpsColdWar.exe");

                    // cache the base addresses for these various pointers
                    if (util.Offsets.PlayerCompPtr != util.Game.FindDMAAddy(util.Offsets.hProc, (IntPtr)(util.Offsets.baseAddress.ToInt64() + util.Offsets.PlayerBase.ToInt64()), new int[] { 0 }))
                        util.Offsets.PlayerCompPtr = util.Game.FindDMAAddy(util.Offsets.hProc, (IntPtr)(util.Offsets.baseAddress.ToInt64() + util.Offsets.PlayerBase.ToInt64()), new int[] { 0 });

                    if (util.Offsets.PlayerPedPtr != util.Game.FindDMAAddy(util.Offsets.hProc, (IntPtr)(util.Offsets.baseAddress.ToInt64() + util.Offsets.PlayerBase.ToInt64() + 0x8), new int[] { 0 }))
                        util.Offsets.PlayerPedPtr = util.Game.FindDMAAddy(util.Offsets.hProc, (IntPtr)(util.Offsets.baseAddress.ToInt64() + util.Offsets.PlayerBase.ToInt64() + 0x8), new int[] { 0 });

                    if (util.Offsets.ZMGlobalBase != util.Game.FindDMAAddy(util.Offsets.hProc, (IntPtr)(util.Offsets.baseAddress.ToInt64() + util.Offsets.PlayerBase.ToInt64() + 0x60), new int[] { 0 }))
                        util.Offsets.ZMGlobalBase = util.Game.FindDMAAddy(util.Offsets.hProc, (IntPtr)(util.Offsets.baseAddress.ToInt64() + util.Offsets.PlayerBase.ToInt64() + 0x60), new int[] { 0 });

                    if (util.Offsets.ZMBotBase != util.Game.FindDMAAddy(util.Offsets.hProc, (IntPtr)(util.Offsets.baseAddress.ToInt64() + util.Offsets.PlayerBase.ToInt64() + 0x68), new int[] { 0 }))
                        util.Offsets.ZMBotBase = util.Game.FindDMAAddy(util.Offsets.hProc, (IntPtr)(util.Offsets.baseAddress.ToInt64() + util.Offsets.PlayerBase.ToInt64()) + 0x68, new int[] { 0 });

                    if (util.Offsets.ZMBotBase != (IntPtr)0x0 && util.Offsets.ZMBotBase != (IntPtr)0x68 && util.Offsets.ZMBotListBase != util.Game.FindDMAAddy(util.Offsets.hProc, util.Offsets.ZMBotBase + util.Offsets.ZM_Bot_List_Offset, new int[] { 0 }))
                        util.Offsets.ZMBotListBase = util.Game.FindDMAAddy(util.Offsets.hProc, util.Offsets.ZMBotBase + util.Offsets.ZM_Bot_List_Offset, new int[] { 0 });

                    // create new byte array for player coordinates, reads them, and then sets the XYZ coordinates accordingly
                    byte[] playerCoords = new byte[12];
                    util.Game.ReadProcessMemory(util.Offsets.hProc, util.Offsets.PlayerPedPtr + util.Offsets.PP_Coords, playerCoords, 12, out _);
                    var origx = BitConverter.ToSingle(playerCoords, 0);
                    var origy = BitConverter.ToSingle(playerCoords, 4);
                    var origz = BitConverter.ToSingle(playerCoords, 8);
                    // updates the current playerposition with a Vector3 created from the xyz coordinates
                    updatedPlayerPos = new Vector3((float)Math.Round(origx, 4), (float)Math.Round(origy, 4), (float)Math.Round(origz, 4));



                    //if ingame ( we need to figure out how to determine if we're ingame or not.
                    label1.Text = "Zombies Left: " + util.IAPI.GetZombieCount();


                    #region Update Player Names

                    tabPage1.Text = IAPI.GetName(0);
                    tabPage2.Text = IAPI.GetName(1);
                    tabPage3.Text = IAPI.GetName(2);
                    tabPage4.Text = IAPI.GetName(3);

                    #endregion


                    #region Godmode

                    if (godModePlayerOne)
                    {
                        IAPI.GodModeOn(IAPI.playerOne);
                        //IAPI.GodModeOn(0);
                    }
                    else
                    {
                        IAPI.GodModeOff(0);
                    }

                    if (godModePlayerTwo)
                    {
                        IAPI.GodModeOn(1);
                        button7.ForeColor = Color.Green;
                        button7.Text = "God Mode - On";
                    }
                    else
                    {
                        IAPI.GodModeOff(1);
                        button7.ForeColor = Color.Red;
                        button7.Text = "God mode - Off";
                    }


                    if (godModePlayerThree)
                    {
                        IAPI.GodModeOn(2);
                        button12.ForeColor = Color.Green;
                        button12.Text = "God Mode - On";
                    }
                    else
                    {
                        IAPI.GodModeOff(2);
                        button12.ForeColor = Color.Red;
                        button12.Text = "God Mode - Off";
                    }


                    if (godModePlayerFour)
                    {
                        IAPI.GodModeOn(3);
                        button15.ForeColor = Color.Green;
                        button15.Text = "God Mode - On";
                    }
                    else
                    {
                        IAPI.GodModeOff(3);
                        button15.ForeColor = Color.Red;
                        button15.Text = "God Mode - Off";
                    }
                    #endregion

                    #region UnlimitedAmmo

                    if (uaPlayerOne)
                    {
                        IAPI.FreezeAmmo(0);
                    }

                    if (uaPlayerTwo)
                    {
                        IAPI.FreezeAmmo(1);
                    }


                    if (uaPlayerThree)
                    {
                        IAPI.FreezeAmmo(2);
                    }


                    if (uaPlayerFour)
                    {
                        IAPI.FreezeAmmo(3);
                    }


                    #endregion

                    #region UnlimitedMoeny

                    if (umPlayerOne)
                    {
                        IAPI.SetMoney(0, 100000);

                    }

                    if (umPlayerTwo)
                    {
                        IAPI.SetMoney(1, 100000);
                    }


                    if (umPlayerThree)
                    {
                        IAPI.SetMoney(2, 100000);
                    }


                    if (umPlayerFour)
                    {
                        IAPI.SetMoney(3, 100000);
                    }


                    #endregion

                    #region InstaKill/TP

                    // post 1.1.2 - combined tp zombies to cursor and 1HP zombies into a single loop, no point in looping twice for the same thing

                    byte[] enemyPosBuffer = new byte[12];

                    if (tpZombiesToCursor)
                    {
                        // gets current player position
                        byte[] playerHeadingXY = new byte[4];
                        byte[] playerHeadingZ = new byte[4];
                        util.Game.ReadProcessMemory(Offsets.hProc, Offsets.PlayerPedPtr + Offsets.PP_Heading_XY, playerHeadingXY, 4, out _);
                        util.Game.ReadProcessMemory(Offsets.hProc, Offsets.PlayerPedPtr + Offsets.PP_Heading_Z, playerHeadingZ, 4, out _);

                        // some stack overflow magic to get the direction the player is facing and getting a position in front of the player
                        var pitch = -ConvertToRadians(BitConverter.ToSingle(playerHeadingZ, 0));
                        var yaw = ConvertToRadians(BitConverter.ToSingle(playerHeadingXY, 0));
                        var x = Convert.ToSingle(Math.Cos(yaw) * Math.Cos(pitch));
                        var y = Convert.ToSingle(Math.Sin(yaw) * Math.Cos(pitch));
                        var z = Convert.ToSingle(Math.Sin(pitch));

                        // im guessing just a straight up BitConverter.GetBytes could have worked for writing vector3s to memory instead of this kinda messy solution
                        var newEnemyPos = updatedPlayerPos + (new Vector3(x, y, z) * zmTeleportDistance);

                        Buffer.BlockCopy(BitConverter.GetBytes(newEnemyPos.X), 0, enemyPosBuffer, 0, 4);
                        Buffer.BlockCopy(BitConverter.GetBytes(newEnemyPos.Y), 0, enemyPosBuffer, 4, 4);
                        Buffer.BlockCopy(BitConverter.GetBytes(newEnemyPos.Z), 0, enemyPosBuffer, 8, 4);
                    }

                    for (int i = 0; i < 90; i++)
                    {
                        // if 1hp zombies is checked, set all zombie hp and max hp to 1
                        if (zombiesInstaKill)
                        {
                            util.Game.WriteProcessMemory(Offsets.hProc, Offsets.ZMBotListBase + (Offsets.ZM_Bot_ArraySize_Offset * i) + Offsets.ZM_Bot_Health, 1, 4, out _);
                            util.Game.WriteProcessMemory(Offsets.hProc, Offsets.ZMBotListBase + (Offsets.ZM_Bot_ArraySize_Offset * i) + Offsets.ZM_Bot_MaxHealth, 1, 4, out _);
                        }

                        // if tp zombies is checked, set their position to the position we got earlier
                        if (tpZombiesToCursor)
                        {
                            util.Game.WriteProcessMemory(Offsets.hProc, Offsets.ZMBotListBase + (Offsets.ZM_Bot_ArraySize_Offset * i) + Offsets.ZM_Bot_Coords, enemyPosBuffer, 12, out _);
                        }
                    }

                    #endregion

                    // infrared vision toggle
                    //  if (checkBox6.Checked)
                    //  {
                    //      util.Game.WriteProcessMemory(hProc, PlayerCompPtr + PC_InfraredVision, new byte[] { 0x10 }, 1, out _);
                    //  }
                    //   else
                    //    {
                    //         util.Game.WriteProcessMemory(hProc, PlayerCompPtr + PC_InfraredVision, new byte[] { 0x0 }, 1, out _);
                    //    }


                    #region XPboost

                    // xp modifiers
                    if (globalXpBoost)
                    {
                        // writes the xp modifier values to memory, i guess just a straight up BitConverter.GetBytes could've worked without the creation of the byte buffers
                        byte[] tempBuffer1 = new byte[4];
                        Buffer.BlockCopy(BitConverter.GetBytes(gunXpModifier), 0, tempBuffer1, 0, 4);
                        byte[] tempBuffer2 = new byte[4];
                        Buffer.BlockCopy(BitConverter.GetBytes(xpModifier), 0, tempBuffer2, 0, 4);

                        util.Game.WriteProcessMemory(Offsets.hProc, (IntPtr)(Offsets.baseAddress.ToInt64() + Offsets.ZMXPScaleBase.ToInt64()) + Offsets.XPGun_Offset, tempBuffer1, 4, out _);
                        util.Game.WriteProcessMemory(Offsets.hProc, (IntPtr)(Offsets.baseAddress.ToInt64() + Offsets.ZMXPScaleBase.ToInt64()) + Offsets.XPUNK02_Offset, tempBuffer2, 4, out _);
                    }
                    #endregion



                    // updates the lastknownplayerpos variable to the current players position
                    lastKnownPlayerPos = updatedPlayerPos;
                }
                // if an error happened during the loop, output that to the gui console
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }
        }
        private void ComboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // string key = ((KeyValuePair<string, string>)comboBox1.SelectedItem).Key;
            // string value = ((KeyValuePair<string, string>)comboBox1.SelectedItem).Value;
            //  var test = Int64.Parse(value);

            // util.Game.WriteProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + ( Offsets.PC_ArraySize_Offset * 1 ) + Offsets.PC_SetWeaponID, test, 1, out _);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy) backgroundWorker1.RunWorkerAsync();

        }

        public Main()
        {

            InitializeComponent();
            //this.Text = OPSEC.RandomString(15);
            // this.Update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            trainerEnabled = !trainerEnabled;

            if (trainerEnabled)
            {
                button1.Text = "Running";
                button1.ForeColor = Color.Green;
            }
            else
            {
                button1.Text = "Stopped";
                button1.ForeColor = Color.Red;
            }
        }
        public double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            //toggle true/false of God mode for PlayerOne
            godModePlayerOne = !godModePlayerOne;
            if (godModePlayerOne)
            {
                button4.ForeColor = Color.Green;
                button4.Text = "God Mode - On";
            }
            else
            {
                button4.ForeColor = Color.Red;
                button4.Text = "God Mode - Off";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //toggle true/false of God mode for PlayerTwo
            godModePlayerTwo = !godModePlayerTwo;
            if (godModePlayerTwo)
            {
                button7.ForeColor = Color.Green;
                button7.Text = "God Mode - On";
            }
            else
            {
                button7.ForeColor = Color.Red;
                button7.Text = "God mode - Off";
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            godModePlayerThree = !godModePlayerThree;


            if (godModePlayerThree)
            {
                button12.ForeColor = Color.Green;
                button12.Text = "God Mode - On";
            }
            else
            {
                button12.ForeColor = Color.Red;
                button12.Text = "God Mode - Off";
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            godModePlayerFour = !godModePlayerFour;
            if (godModePlayerFour)
            {
                button15.ForeColor = Color.Green;
                button15.Text = "God Mode - On";
            }
            else
            {
                button15.ForeColor = Color.Red;
                button15.Text = "God Mode - Off";
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            uaPlayerFour = !uaPlayerFour;

            if (uaPlayerFour)
            {
                button14.ForeColor = Color.Green;
                button14.Text = "Unlimited Ammo - On";
            }
            else
            {
                button14.ForeColor = Color.Red;
                button14.Text = "Unlimited Ammo - Off";
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            uaPlayerThree = !uaPlayerThree;

            if (uaPlayerThree)
            {
                button11.ForeColor = Color.Green;
                button11.Text = "Unlimited Ammo - On";
            }
            else
            {
                button11.ForeColor = Color.Red;
                button11.Text = "Unlimited Ammo - Off";
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            uaPlayerTwo = !uaPlayerTwo;

            if (uaPlayerTwo)
            {
                button8.ForeColor = Color.Green;
                button8.Text = "Unlimited Ammo - On";
            }
            else
            {
                button8.ForeColor = Color.Red;
                button8.Text = "Unlimited Ammo - Off";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            uaPlayerOne = !uaPlayerOne;
            if (uaPlayerOne)
            {
                button5.ForeColor = Color.Green;
                button5.Text = "Unlimited Ammo - On";
            }
            else
            {
                button5.ForeColor = Color.Red;
                button5.Text = "Unlimited Ammo - Off";
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            IAPI.SetSpeed(0, Convert.ToInt32(textBox4.Text));
        }

        private void button17_Click(object sender, EventArgs e)
        {
            IAPI.SetSpeed(1, Convert.ToInt32(textBox5.Text));
        }

        private void button18_Click(object sender, EventArgs e)
        {
            IAPI.SetSpeed(2, Convert.ToInt32(textBox6.Text));
        }

        private void button19_Click(object sender, EventArgs e)
        {
            IAPI.SetSpeed(3, Convert.ToInt32(textBox7.Text));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tpZombiesToCursor = !tpZombiesToCursor;
            if (tpZombiesToCursor)
            {
                button3.ForeColor = Color.Green;
                button3.Text = "TP2C - On";
            }
            else
            {
                button3.ForeColor = Color.Red;
                button3.Text = "TP2C - Off";
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            globalXpBoost = !globalXpBoost;
            if (globalXpBoost)
            {
                button20.ForeColor = Color.Green;
                button20.Text = "XX Boost - On";
            }
            else
            {
                button20.ForeColor = Color.Red;
                button20.Text = "XP Boost - Off";
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            zombiesInstaKill = !zombiesInstaKill;
            if (zombiesInstaKill)
            {
                button2.ForeColor = Color.Green;
                button2.Text = "Insta Kill - On";
            }
            else
            {
                button2.ForeColor = Color.Red;
                button2.Text = "Insta Kill - Off";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            umPlayerOne = !umPlayerOne;
            if (umPlayerOne)
            {
                button6.ForeColor = Color.Green;
                button6.Text = "Unlimited Money - On";
            }
            else
            {
                button6.ForeColor = Color.Red;
                button6.Text = "Unlimited Money - Off";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            umPlayerTwo = !umPlayerTwo;
            if (umPlayerTwo)
            {
                button9.ForeColor = Color.Green;
                button9.Text = "Unlimited Money - On";
            }
            else
            {
                button9.ForeColor = Color.Red;
                button9.Text = "Unlimited Money - Off";
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            umPlayerThree = !umPlayerThree;
            if (umPlayerThree)
            {
                button10.ForeColor = Color.Green;
                button10.Text = "Unlimited Money - On";
            }
            else
            {
                button10.ForeColor = Color.Red;
                button10.Text = "Unlimited Money - Off";
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            umPlayerFour = !umPlayerFour;
            if (umPlayerFour)
            {
                button13.ForeColor = Color.Green;
                button13.Text = "Unlimited Money - On";
            }
            else
            {
                button13.ForeColor = Color.Red;
                button13.Text = "Unlimited Money - Off";
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            util.Game.WriteProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + (Offsets.PC_ArraySize_Offset * 0) + Offsets.PC_SetWeaponID, textBox1.Text, 1, out _);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            util.Game.WriteProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + (Offsets.PC_ArraySize_Offset * 3) + Offsets.PC_SetWeaponID, textBox8.Text, 1, out _);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            util.Game.WriteProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + (Offsets.PC_ArraySize_Offset * 2) + Offsets.PC_SetWeaponID, textBox3.Text, 1, out _);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            util.Game.WriteProcessMemory(Offsets.hProc, Offsets.PlayerCompPtr + (Offsets.PC_ArraySize_Offset * 1) + Offsets.PC_SetWeaponID, textBox2.Text, 1, out _);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }
        private void TrackBar1_ValueChanged(object sender, System.EventArgs e)
        {
            if (trackBar1.Value == 1)
            {
                xpModifier = 10.0f;
                gunXpModifier = 10.0f;
                label3.Text = "10x";
            }
            else if (trackBar1.Value == 2)
            {
                xpModifier = 20.0f;
                gunXpModifier = 20.0f;
                label3.Text = "20x";
            }
            else if (trackBar1.Value == 3)
            {
                xpModifier = 30.0f;
                gunXpModifier = 30.0f;
                label3.Text = "30x";
            }
            else if (trackBar1.Value == 4)
            {
                xpModifier = 40.0f;
                gunXpModifier = 40.0f;
                label3.Text = "40x";
            }
            else if (trackBar1.Value == 5)
            {
                xpModifier = 50.0f;
                gunXpModifier = 50.0f;
                label3.Text = "50x";
            }
            else if (trackBar1.Value == 6)
            {
                xpModifier = 60.0f;
                gunXpModifier = 60.0f;
                label3.Text = "60x";
            }
            else if (trackBar1.Value == 7)
            {
                xpModifier = 70.0f;
                gunXpModifier = 70.0f;
                label3.Text = "70x";
            }
            else if (trackBar1.Value == 8)
            {
                xpModifier = 80.0f;
                gunXpModifier = 80.0f;
                label3.Text = "80x";
            }
            else if (trackBar1.Value == 9)
            {
                xpModifier = 90.0f;
                gunXpModifier = 90.0f;
                label3.Text = "90x";
            }
            else if (trackBar1.Value == 10)
            {
                xpModifier = 100.0f;
                gunXpModifier = 100.0f;
                label3.Text = "100x";
            }
            else if (trackBar1.Value == 11)
            {
                xpModifier = 110.0f;
                gunXpModifier = 110.0f;
                label3.Text = "110x";
            }
            else if (trackBar1.Value == 12)
            {
                xpModifier = 120.0f;
                gunXpModifier = 120.0f;
                label3.Text = "120x";
            }
            else if (trackBar1.Value == 13)
            {
                xpModifier = 130.0f;
                gunXpModifier = 130.0f;
                label3.Text = "130x";
            }
            else if (trackBar1.Value == 14)
            {
                xpModifier = 140.0f;
                gunXpModifier = 140.0f;
                label3.Text = "140x";
            }
            else if (trackBar1.Value == 15)
            {
                xpModifier = 150.0f;
                gunXpModifier = 150.0f;
                label3.Text = "150x";
            }
            else if (trackBar1.Value == 16)
            {
                xpModifier = 160.0f;
                gunXpModifier = 160.0f;
                label3.Text = "160x";
            }
            else if (trackBar1.Value == 17)
            {
                xpModifier = 170.0f;
                gunXpModifier = 170.0f;
                label3.Text = "170x";
            }
            else if (trackBar1.Value == 18)
            {
                xpModifier = 180.0f;
                gunXpModifier = 180.0f;
                label3.Text = "180x";
            }
            else if (trackBar1.Value == 19)
            {
                xpModifier = 190.0f;
                gunXpModifier = 190.0f;
                label3.Text = "190x";
            }
            else if (trackBar1.Value == 20)
            {
                xpModifier = 200.0f;
                gunXpModifier = 200.0f;
                label3.Text = "200x";
            }
            else
            {
                xpModifier = 0.0f;
                gunXpModifier = 0.0f;
                label3.Text = "0x";
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
           Vector3 MyPos = IAPI.GetPlayerPos();
            MessageBox.Show(MyPos.ToString());
        }

        private void button26_Click(object sender, EventArgs e)
        {
            Vector3 test = IAPI.GetPlayerPos();
            MessageBox.Show(string.Format("X: {0}, Y: {1}, Z: {2}", test.X.ToString(), test.Y.ToString(), test.Z.ToString())); ;

            int test2 = IAPI.GetHP(0);
            MessageBox.Show(string.Format("HP: {0}", test2.ToString()));
        }
    }
}
