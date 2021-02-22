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

        //Players Names
        public bool playersNameChecked = false;

        //Player Cords
        public Vector3 currentPlayerPos = Vector3.Zero;

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

                    //We need to update the playerPOS constantly as we move and we can then format the string and provide the POS x,y,z
                    currentPlayerPos = IAPI.GetPlayerPos();
                    label3.Text = string.Format("X: {0}", currentPlayerPos.X.ToString());
                    label4.Text = string.Format("Y: {0}", currentPlayerPos.Y.ToString());
                    label5.Text = string.Format("Z: {0}", currentPlayerPos.Z.ToString());



                    #region Update Player Names

                    /* 
                        - We shouldn't be starting the program until we are ingame so we can assume this boolean will be good enough to obtain and update the players names 1 time.
                        - This prevents the gui from flashing out of control.
                    */
                    if (!playersNameChecked)
                    {
                        tabPage1.Text = IAPI.GetName(IAPI.playerOne);
                        tabPage2.Text = IAPI.GetName(IAPI.playerTwo);
                        tabPage3.Text = IAPI.GetName(IAPI.playerThree);
                        tabPage4.Text = IAPI.GetName(IAPI.playerFour);
                        playersNameChecked = true;
                    }
                    #endregion


                    #region Godmode

                    if (godModePlayerOne)
                    { 
                        IAPI.GodModeOn(IAPI.playerOne);

                    }
                    else
                    {
                        IAPI.GodModeOff(IAPI.playerOne);

                    }

                    if (godModePlayerTwo)
                    {
                        IAPI.GodModeOn(IAPI.playerTwo);
                        button7.ForeColor = Color.Green;
                        button7.Text = "God Mode - On";
                    }
                    else
                    {
                        IAPI.GodModeOff(IAPI.playerTwo);
                        button7.ForeColor = Color.Red;
                        button7.Text = "God mode - Off";
                    }


                    if (godModePlayerThree)
                    {
                        IAPI.GodModeOn(IAPI.playerThree);
                        button12.ForeColor = Color.Green;
                        button12.Text = "God Mode - On";
                    }
                    else
                    {
                        IAPI.GodModeOff(IAPI.playerThree);
                        button12.ForeColor = Color.Red;
                        button12.Text = "God Mode - Off";
                    }


                    if (godModePlayerFour)
                    {
                        IAPI.GodModeOn(IAPI.playerFour);
                        button15.ForeColor = Color.Green;
                        button15.Text = "God Mode - On";
                    }
                    else
                    {
                        IAPI.GodModeOff(IAPI.playerFour);
                        button15.ForeColor = Color.Red;
                        button15.Text = "God Mode - Off";
                    }
                    #endregion

                    #region UnlimitedAmmo

                    if (uaPlayerOne)
                    {
                        IAPI.FreezeAmmo(IAPI.playerOne, 50);

                    }

                    if (uaPlayerTwo)
                    {
                        IAPI.FreezeAmmo(IAPI.playerTwo, 50);
                    }


                    if (uaPlayerThree)
                    {
                        IAPI.FreezeAmmo(IAPI.playerThree, 50);
                    }


                    if (uaPlayerFour)
                    {
                        IAPI.FreezeAmmo(IAPI.playerFour, 50);
                    }


                    #endregion

                    #region UnlimitedMoeny

                    if (umPlayerOne)
                    {
                        IAPI.SetMoney(IAPI.playerOne, 100000);

                    }

                    if (umPlayerTwo)
                    {
                        IAPI.SetMoney(IAPI.playerTwo, 100000);
                    }


                    if (umPlayerThree)
                    {
                        IAPI.SetMoney(IAPI.playerThree, 100000);
                    }


                    if (umPlayerFour)
                    {
                        IAPI.SetMoney(IAPI.playerFour, 100000);
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

        private void Main_Load(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy) backgroundWorker1.RunWorkerAsync();

        }

        public Main()
        {
            InitializeComponent();
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
            IAPI.SetSpeed(IAPI.playerOne, Convert.ToInt32(textBox4.Text));
        }

        private void button17_Click(object sender, EventArgs e)
        {
            IAPI.SetSpeed(IAPI.playerTwo, Convert.ToInt32(textBox5.Text));
        }

        private void button18_Click(object sender, EventArgs e)
        {
            IAPI.SetSpeed(IAPI.playerThree, Convert.ToInt32(textBox6.Text));
        }

        private void button19_Click(object sender, EventArgs e)
        {
            IAPI.SetSpeed(IAPI.playerFour, Convert.ToInt32(textBox7.Text));
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

       

        private void button21_Click(object sender, EventArgs e)
        {
            Log("Test Message", Color.Aqua);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            Vector3 test = IAPI.GetPlayerPos();
            MessageBox.Show(string.Format("X: {0}, Y: {1}, Z: {2}", test.X.ToString(), test.Y.ToString(), test.Z.ToString())); ;

            int test2 = IAPI.GetHP(0);
            MessageBox.Show(string.Format("HP: {0}", test2.ToString()));
        }


        //better ways to do this but it works.
        public void Log(string strLogMessage, Color TextColor = default(Color))
        {
            string dtnow = "<" + DateTime.Now.ToString("HH:mm:ss") + "> ";
            strLogMessage = dtnow + strLogMessage + Environment.NewLine;
            richTextBox1.SelectionColor = TextColor;
            richTextBox1.AppendText(strLogMessage);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //toggle true/false of God mode for PlayerTwo
            godModePlayerFour = !godModePlayerFour;
            if (godModePlayerFour)
            {
                button4.ForeColor = Color.Green;
                button4.Text = "God Mode - On";
            }
            else
            {
                button4.ForeColor = Color.Red;
                button4.Text = "God mode - Off";
            }

        }
    }
}
