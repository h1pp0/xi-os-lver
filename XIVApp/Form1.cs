using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Xml.Serialization;
using XIVApp.Properties;

namespace XIVApp
{
    // We are using FFACETools
    using FFACETools;

    public partial class Form1 : Form
    {
        // Character Session/Instance Handlers
        private Process[] pol;                                  // Get the POL Processes
        private List<FFACE> Sessions = new List<FFACE>();       // Set FFACE to a sessions list
        private FFACE Session;                                  // Set FFACE current session
        private FFACE PowerSession;                             // Set FFACE Power Leveller

        // Process found or not
        private bool XIFound = false;
        

        // Initialize
        public Form1()
        {
            pol = Process.GetProcessesByName("pol");

            //make sure pol is running
            if (1 > pol.Length)
            {
                //let the user know what went wrong
                MessageBox.Show(Resources.Form1_Form1_FFXI_not_found);
                System.Environment.Exit(0);     //close the form
                return;
            }

            // Add processes to array and create fface object foreach
            foreach (Process POLProcess in pol)
            {
                Sessions.Add(new FFACE(POLProcess.Id));
                XIFound = true;
            }

            if (XIFound)
            {
                InitializeComponent();
            }
        }

        // Loader
        private void Form1_Load(object sender, EventArgs e)
        {
            //my_static.Debugger.Show();

            // Start out Debugging
            StartDebug();

            // Set Fields
            SetFields();

            // Get list of spells and abilities

            // Set Character Menu
            CharacterList.SelectedIndex = 0;
            PowerLevelCharacters.SelectedIndex = 0;

            // Set Skill Menu (Default cure 3)
            ManualSkillList.SelectedIndex = 1;
            ManualSkillList2.SelectedIndex = 2;
            SkillType.SelectedIndex = 0;
            WhenToDoSkillOption.SelectedIndex = 0;

            // Visisble due to MS Shitty designs
            WayPointsToolStrip.Visible = true;
            MobListToolStrip.Visible = true;
            WSToolStrip.Visible = true;

            // Default Path
            DebugBox.AppendText("Default Save: " + Application.StartupPath + "\\" + Session.Player.Name + "\\" + Environment.NewLine);

            //FormAdditionalSettings = new FormAdditionalSettings(Session, DebugBox);

        }

        // Set GUI Fields
        private void SetFields()
        {
            // Character List
            foreach (FFACE Character in Sessions)
            {
                CharacterList.Items.Add(Character.Player.Name);
                PowerLevelCharacters.Items.Add(Character.Player.Name);
            }
        }

        // Sets Character Data
        private void SetCharacterInfo()
        {
            // Character
            lbl_CharacterName.Text = Session.Player.Name;
            lbl_Class.Text = Session.Player.MainJobLevel + " " + Session.Player.MainJob + " / " + Session.Player.SubJobLevel + " " + Session.Player.SubJob;
            lbl_Nation.Text = Session.Player.Nation.ToString();
            // FIXME: Uncomment
            //lbl_HomePoint.Text = FFACE.ParseResources.GetAreaName(Enum.Parse(typeof(FFACETools.Zone), Session.Player.HomePoint_ID.ToString()));

            // Target
            lbl_TargetName.Text = Session.Target.Name;
            lbl_TargetHP.Text = Session.Target.HPPCurrent.ToString();
            lbl_TargetType.Text = Session.Target.Type.ToString();
            lbl_TargetStatus.Text = Session.Target.Status.ToString();
        }

        // Shows Debug
        private void StartDebug()
        {
            // Display Information
            DebugBox.AppendText("Processes: " + pol.Count() + ", Sessions: " + Sessions.Count() + Environment.NewLine);

            // Display all characters
            int i = 0;
            foreach (FFACE Character in Sessions)
            {
                DebugBox.AppendText(i +". Name: " + Character.Player.Name + " (" + Character.Player.ID + ")" + Environment.NewLine);
                i++;
            }
            
            // Seperator
            DebugBox.AppendText("----------------------------------------------------------------" + Environment.NewLine);
        }

        // Method to test the current character
        private void btn_TestEcho_Click(object sender, EventArgs e)
        {
            Session.Windower.SendString("/echo Hello " + Session.Player.Name);
        }

        // Method to change the selected character index.
        private void CharacterList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset Chatlog
            ChatLog.Clear();

            // Set the index of the selected character
            int CharacterIndex = CharacterList.SelectedIndex;
            Session = Sessions.ElementAt(CharacterIndex);
            
            // Display Info in debug
            DebugBox.AppendText("Active Character: " + Session.Player.Name + Environment.NewLine);

            // Change Character Info
            SetCharacterInfo();

            // Check Folders
            if (!Directory.Exists(Session.Player.Name))
            {
                // Display Info in debug
                DebugBox.AppendText("Folder for: " + Session.Player.Name + " has been created." + Environment.NewLine);

                Directory.CreateDirectory(Session.Player.Name);
            }
        }

        // Set the Power Leveller
        private void PowerLevelCharacters_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Set the index of the selected character
            int CharacterIndex = PowerLevelCharacters.SelectedIndex;
            PowerSession = Sessions.ElementAt(CharacterIndex);

            // Display Info in debug
            DebugBox.AppendText("Power Leveller: " + PowerSession.Player.Name + Environment.NewLine);
        }

        // Parse Chatlog
        private void Chatlogtimer_Tick(object sender, EventArgs e)
        {
            // Get our new color coded line
            FFACE.ChatTools.ChatLine NewLine = new FFACE.ChatTools.ChatLine();
            NewLine = Session.Chat.GetNextLine(LineSettings.CleanAll);
            string NewLine_Text = "";

            // Check to make sure we have text
            if (NewLine != null)
            {
                NewLine_Text = NewLine.Text;
                // Change the color of the line to match the color in FFXI
                ChatLog.SelectionColor = NewLine.Color;

                // Output our line to the textbox
                ChatLog.AppendText("[" + NewLine.NowDate + "] " + NewLine_Text + Environment.NewLine);

                // Scroll our chatlog window to the last line
                ChatLog.ScrollToCaret();
            }

            //--------------------------------------------------------------------------------
            // PL Auto na spells
            if (AutoNASpells.Checked)
            {
                // Check to ensure line not null
                if (!string.IsNullOrEmpty(NewLine_Text))
                {
                    // Check for paralyzed
                    if (NewLine_Text.Contains(PowerSession.Target.Name) && NewLine_Text.Contains("paralyzed"))
                    {
                        // Fix
                        string Command = "/ma \"Paralyna\" " + PowerSession.Target.Name + "";
                        PowerSession.Windower.SendString(Command);
                    }

                    // Check for blind
                    if (NewLine_Text.Contains(PowerSession.Target.Name) && NewLine_Text.Contains("blind"))
                    {
                        // Fix
                        string Command = "/ma \"Blindna\" " + PowerSession.Target.Name + "";
                        PowerSession.Windower.SendString(Command);
                    }

                    // Check for poisoned
                    if (NewLine_Text.Contains(PowerSession.Target.Name) && NewLine_Text.Contains("poisoned"))
                    {
                        // Fix
                        string Command = "/ma \"Poisona\" " + PowerSession.Target.Name + "";
                        PowerSession.Windower.SendString(Command);
                    }

                    // Check for sleep
                    if (NewLine_Text.Contains(PowerSession.Target.Name) && NewLine_Text.Contains("sleep"))
                    {
                        // Fix
                        string Command = "/ma \"Cure\" " + PowerSession.Target.Name + "";
                        PowerSession.Windower.SendString(Command);
                    }
                }
            }
        }

        // Buff Information
        private const int buff_Protect = (1800*2);
        private bool buff_Protect_on = false;
        private int buff_Protect_timer = 0;

        private const int buff_Shell = (1800*2);
        private bool buff_Shell_on = false;
        private int buff_Shell_timer = 0;

        private const int casting_duration = (5*2);
        private int casting_timer = 0;
        private bool casting = false;

        private bool Raising_Target = false;

        private bool FollowingCharacter = false;
        private bool LostCharacter = false;
        
        // Updates character timer every second
        private void CharacterTimer_Tick(object sender, EventArgs e)
        {
            // If currently following a character
            if (FollowingCharacter)
            {
                // Check to see if no target
                if (String.IsNullOrEmpty(PowerSession.Target.Name))
                {
                    // Stop Running
                    PowerSession.Windower.SendKeyPress(KeyCode.NP_Number7);
                    LostCharacter = true;
                }

                // If character lost, try and follow again
                if (LostCharacter)
                {
                    // Target
                    string Command = "/target " + PLFollowingChar.Text;
                    PowerSession.Windower.SendString(Command);

                    if (String.IsNullOrEmpty(PowerSession.Target.Name))
                    {
                        // No longer a lost character
                        LostCharacter = false;

                        // Lockon
                        Command = "/lockon <t>";
                        PowerSession.Windower.SendString(Command);
                        // Follow
                        Command = "/follow";
                        PowerSession.Windower.SendString(Command);
                    }
                }

            }


            // Set Character Info
            SetCharacterInfo();

            // Update PL Information
            lbl_plHP.Text = PowerSession.Player.HPCurrent + " (" + PowerSession.Player.HPPCurrent + ")";
            lbl_plMP.Text = PowerSession.Player.MPCurrent + " (" + PowerSession.Player.MPPCurrent + ")";

            // throw a try here as we trying to convert and might fuck up.
            try
            {
                // If automatic heal for character one is checked.
                if (PLAutoHeal.Checked)
                {
                    int percent = 75;
                    int percent2 = 50;
                    if (!String.IsNullOrEmpty(PowerSession.Target.Name))
                    {
                       percent = Convert.ToInt16(AutoHealPercent.Text);
                       percent2 = Convert.ToInt16(AutoHealPercent2.Text);
                    }

                    if (PowerSession.Target.HPPCurrent < percent2 && PowerSession.Target.Name == PLMemberA.Text && PowerSession.Target.Status != Status.Dead2)
                    {
                        string Command = "/ma \"" + ManualSkillList.SelectedItem + "\" " + PLMemberA.Text + "";
                        PowerSession.Windower.SendString(Command);
                        Command = "/ma \"" + ManualSkillList2.SelectedItem + "\" " + PLMemberA.Text + "";
                        PowerSession.Windower.SendString(Command);
                    }
                    else if (PowerSession.Target.HPPCurrent < percent && PowerSession.Target.Name == PLMemberA.Text && PowerSession.Target.Status != Status.Dead2)
                    {
                        string Command = "/ma \"" + ManualSkillList.SelectedItem + "\" " + PLMemberA.Text + "";
                        PowerSession.Windower.SendString(Command);
                        Command = "/ma \"" + ManualSkillList2.SelectedItem + "\" " + PLMemberA.Text + "";
                        PowerSession.Windower.SendString(Command);
                    }
                }
            }
            catch (Exception exc)
            {

            }

            // If automatic buffs is checked
            if (AutoBuffSpells.Checked)
            {
                // Check if protect is on, if not cast it
                if (!buff_Protect_on && !casting)
                {
                    casting = true;
                    string Command = "/ma \"Protect II\" " + PowerSession.Target.Name + "";
                    PowerSession.Windower.SendString(Command);
                    buff_Protect_on = true;
                }

                // Check if protect is on, if not cast it
                if (!buff_Shell_on && !casting)
                {
                    casting = true;
                    string Command = "/ma \"Shell II\" " + PowerSession.Target.Name + "";
                    PowerSession.Windower.SendString(Command);
                    buff_Shell_on = true;
                }

                // If the protect timer is above the protect duration, protect is off, reset timer.
                if (buff_Protect_timer > buff_Protect)
                {
                    buff_Protect_on = false;
                    buff_Protect_timer = 0;
                }

                // If the shell timer is above the shell duration, protect is off, reset timer.
                if (buff_Shell_timer > buff_Shell)
                {
                    buff_Shell_on = false;
                    buff_Shell_timer = 0;
                }

                // Increment Timers
                if (casting)
                {
                    // If casting timer is above duration, we no longer casting buffs.
                    if (casting_timer > casting_duration)
                    {
                        casting = false;
                        casting_timer = 0;
                    }
                    else
                    {
                        // Else we are so increment casting timer
                        casting_timer++;
                    }
                }

                buff_Protect_timer++;
                buff_Shell_timer++;
            }

            // If self heal checked, heal self at X amount of hp
            if (SelfHeal.Checked)
            {
                // Set at 80%
                if (PowerSession.Player.HPPCurrent < 80)
                {
                    string Command = "/ma \"" + ManualSkillList.SelectedItem + "\" <me>";
                    PowerSession.Windower.SendString(Command);
                    Command = "/ma \"" + ManualSkillList2.SelectedItem + "\" <me>";
                    PowerSession.Windower.SendString(Command);
                }
            }

            // If target is dead
            if (PowerSession.Target.Status == Status.Dead2 && PowerSession.Target.Name == PLMemberA.Text && !Raising_Target)
            {
                LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Raising: " + Session.Target.Name);
                string Command = "/ma \"Raise\" <t>";
                PowerSession.Windower.SendString(Command);
                Raising_Target = true;
            }

            if (Raising_Target && PowerSession.Target.Status == Status.Standing)
            {
                LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Raised: " + Session.Target.Name);
                Raising_Target = false;
                DeathTimer.Enabled = true;
            }
                

        }

        //-----------------------------------------------------------------------------------------------
        #region CORE LEVELLING BOT
        // Levelling 
        private int seconds = 0;
        private int Tab_Count = 0;
        private bool Levelling_Running = false;
        private bool Running_To_Mob = false;
        private bool At_Mob = false;
        private bool Fighting = false;
        private bool End_Of_Fight = false;
        private bool End_Of_Fight_Wait = true;
        private double range = 10.0;
        private string Mob_Currently_Fighting = null;
        private bool Record_Label_Status = false;
        private int Camera_Strength = 4;
        private int Start_Delay = 5;
        private int Start_Delay_timer = 0;
        private int Current_EXP_Value = 0;
        private int Current_EXP_This_Level_Value = 0;
        private int Gained_EXP_Value = 0;
        private int Gained_Levels_Value = 0;
        private int Current_Level = 0;
        private int Killed_Mob_Amount = 0;
        private int TP_To_Ws = 100;
        private int TP_To_Ws_Mob_HPP = 0;
        public static int Rest_HPP = 80;
        public static int RestUntil_HPP = 90;
        public static int Rest_MP = 0;
        public static int RestUntil_MP = 0;
        private bool is_Mob_Valid;
        private int last_dist = 0;
        private int last_dist_duration = 0;
        private bool Move_direction = true;
        private double distance;
        private bool Aggro = false;
        private bool Resting = false;

        // Waypoints
        private List<float> Way_Points_X = new List<float>();
        private List<float> Way_Points_Z = new List<float>(); 

        // Lists
        private List<String> Valid_Mobs = new List<String>();

        // Skills
        private List<String> Skill_List_start = new List<String>();
        private List<String> Skill_List_end = new List<String>();
        private List<String> Skill_List_fighting = new List<String>();

        // timer
        private void LevellingTimer_Tick(object sender, EventArgs e)
        {
            #region Aggro Checks
            //if (Session.Target.Type == NPCType.Mob)
            //{
            //    Aggro = true;
            //}

            if (!String.IsNullOrEmpty(Session.Target.Name) && Session.Target.Type == NPCType.Mob && Session.Target.Status == Status.Fighting && !Session.NPC.IsClaimed(Session.Target.ID))
            {
                LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Aggro/Link" + Environment.NewLine);
                Aggro = true;
            }

            if (Aggro && !Fighting)
            {
                is_Mob_Valid = true;
                Fighting = true;
                Thread.Sleep(50);
                Mob_Currently_Fighting = Session.Target.Name;
                Thread.Sleep(50);
                Session.Windower.SendString("/attack <t>");
                Thread.Sleep(50);
                Session.Windower.SendString("/lockon <t>");
                Thread.Sleep(50);
                Session.Windower.SendString("/follow <t>");
            }
            #endregion

            #region Are we alive?
            if (Session.Player.Status == Status.Dead2)
            {
                LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "We died!!!" + Environment.NewLine);
                LevellingTimer.Enabled = false;
            }
            #endregion

            #region Some nice stuff for the Stats
            seconds++;
            TimeSpan ds = TimeSpan.FromSeconds(seconds);
            string DurationSpan = string.Format("{0:D2}d {1:D2}h {2:D2}m {3:D2}s", ds.Days, ds.Hours, ds.Minutes, ds.Seconds);
            DurationValue.Text = DurationSpan;

            // Stats
            double Average_EXP_Gain = Math.Round((double)Gained_EXP_Value / Killed_Mob_Amount);
            AverageValue.Text = Average_EXP_Gain.ToString();
            int EXP_Required = Session.Player.EXPForLevel - Session.Player.EXPIntoLevel;
            MobsTillLevelValue.Text = Math.Round((double)EXP_Required / Average_EXP_Gain).ToString();

            // Values
            EXPGainedValue.Text = string.Format("{0:###,###,###}", Gained_EXP_Value);
            StartDelayShow.Text = Start_Delay_timer + " / " + Start_Delay;
            LevelsGainedValue.Text = Gained_Levels_Value.ToString();
            MobsKilledAmount.Text = Resources.Form1_LevellingTimer_Tick_Killed__ + Killed_Mob_Amount;
            #endregion

            #region Check for new EXP for stats
            // Check for EXP
            if (Session.Player.EXPIntoLevel > Current_EXP_Value)
            {
                int EXPGained = Session.Player.EXPIntoLevel - Current_EXP_Value;
                Gained_EXP_Value += EXPGained;
                Current_EXP_Value = Session.Player.EXPIntoLevel;
            }
            #endregion

            #region Check if we have levelled or not
            // If levelled
            if (Session.Player.MainJobLevel > Current_Level)
            {
                // Get EXP from before to what it was up till the current
                int last_exp_leftover = Current_EXP_This_Level_Value - Current_EXP_Value;
                int EXPGained = last_exp_leftover + Session.Player.EXPIntoLevel;
                Gained_EXP_Value += EXPGained;
                Current_EXP_Value = Session.Player.EXPIntoLevel;
                Current_EXP_This_Level_Value = Session.Player.EXPForLevel;
                Gained_Levels_Value++;
                Current_Level = Session.Player.MainJobLevel;
            }
            #endregion

            #region Look for mobs if: NOT fighting, NOT running to mob, NOT end of fight, Currently Fighting is EMPTY, NOT skipping
            // Tab if not fighting or running
            if (!Fighting && !Running_To_Mob && !End_Of_Fight && String.IsNullOrEmpty(Mob_Currently_Fighting) && !Skipping)
            {
                // Ensure Third person
                if (Session.Player.ViewMode != ViewMode.ThirdPerson)
                {
                    Session.Windower.SendKeyPress(KeyCode.NP_Number5);
                }

                if (Tab_Count == 3) { Session.Windower.SendKeyPress(KeyCode.F1Key); }
                if (Tab_Count == 6) { Session.Windower.SendKeyPress(KeyCode.F1Key); }
                if (Tab_Count == 9) { Session.Windower.SendKeyPress(KeyCode.F1Key); }

                // If Tabbing has hit 5, begin turning camera
                int Camera_Strength_current = 0;
                if (Tab_Count > 3 && !Skipping && !is_Mob_Valid)
                {
                    while (Camera_Strength_current < Camera_Strength)
                    {
                        Session.Windower.SendKeyPress(KeyCode.LeftArrow);
                        Camera_Strength_current++;
                    }
                }

                // If Tab count above 10, begin following waypoints
                if (Tab_Count > 8 && Way_Points_X.Count > 0 && WPTesting.Enabled == false)
                {
                    WPTesting.Enabled = true;
                }

                if (!Valid_Mobs.Contains(Session.Target.Name))
                {
                    Session.Windower.SendKeyPress(KeyCode.TabKey);
                }
                //Session.Windower.SendString("/targetnpc");
                Thread.Sleep(200);
                Tab_Count++;
            }
            #endregion

            // Show Distance from mob
            DistanceNumber.Text = Session.Navigator.DistanceTo(Session.Target.PosX, Session.Target.PosZ).ToString();

            #region Scanning for mobs (the above one is for tabbing this deals with target view and decisions
            /* Check if mob is in our valid list, if it is and 
             * the character is not running, go to mob      */
            if (Scanning_for_mobs)
            {
                // No Longer End of Fight
                End_Of_Fight = false;

                is_Mob_Valid = Valid_Mobs.Contains(Session.Target.Name);
                Thread.Sleep(150);
                if (!String.IsNullOrEmpty(Session.Target.Name) && Session.Target.Name != Session.Player.Name && !Fighting && !Session.NPC.IsClaimed(Session.Target.ID) && Session.Target.Type == NPCType.Mob)
                {
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Target: " + Session.Target.Name + " (" + is_Mob_Valid + ")" + Environment.NewLine);
                }

                if (is_Mob_Valid && !Fighting && !Running_To_Mob && !End_Of_Fight && Session.Target.Status != Status.Fighting && Session.Target.SubID == 0 && Session.Target.Type == NPCType.Mob)
                {
                    // Valid Mob found!
                    Scanning_for_mobs = false;
                    WPTesting.Enabled = false;
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Valid Mob Found! (" + Session.Target.Name + ")" + Environment.NewLine);
                    Tab_Count = 0;

                    // Lockon
                    Session.Windower.SendString("/lockon <t>");
                    Thread.Sleep(100);
                    Session.Windower.SendString("/follow <t>");
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "/lockon <t> + /follow <t>" + Environment.NewLine);

                    // Run to mob
                    //Session.Navigator.GotoTargetXZ();
                    //Session.Navigator.Reset();

                    // Set Variable
                    Running_To_Mob = true;
                }

            }
            #endregion

            #region Attempt to run to the mob if NOT at the mob, NOT fighting NOT end of fight
            /* Check if the character is running and to a mob */
            if (Running_To_Mob && !At_Mob && !Fighting && !End_Of_Fight)
            {
                if (String.IsNullOrEmpty(Session.Target.Name) || Session.Target.Type != NPCType.Mob)
                {
                    Running_To_Mob = false;
                    Scanning_for_mobs = true;
                    At_Mob = false;
                    Session.Windower.SendKeyPress(KeyCode.NP_Number7);
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Seem to have lost sight of mob... Start over." + Environment.NewLine);
                }
				
				//if conditions above are met, we may want to not do this, not sure yet.
                distance = Session.Navigator.DistanceTo(Session.Target.PosX, Session.Target.PosZ);
                ShowDistance.Text = distance.ToString();

                int check_distance = Convert.ToInt16(distance);
                distanceinfo.Text = check_distance + " <> " + last_dist + " (" + last_dist_duration  + ")";
                if (last_dist == check_distance)
                {
                    if (last_dist_duration > 4)
                    {
                        LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Seem to be stuck? ... ");
                        Session.Windower.SendKeyPress(KeyCode.NP_Number7);
                        // Stuck, try moving
                        if (Move_direction)
                        {
                            // left
                            LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Moving LEFT" + Environment.NewLine);
                            Move_direction = false;
                            Session.Windower.SendKey(KeyCode.NP_Number2, true);
                            Thread.Sleep(3000);
                            // We no longer want to lock UI so we will create an int and just loop DoEvents() till tiemr expires
                            //System.Timers.Timer randomsleeptimer = new System.Timers.Timer(3000);
                            //randomsleeptimer.Elapsed += new System.Timers.ElapsedEventHandler(timerended);
                            //randomsleeptimer.AutoReset = false;
                            //randomsleeptimer.Enabled = true;
                            //do
                            //{
                            //    Application.DoEvents();
                            //} while (timervalue == 0);
                            //timervalue = 0;
                            Session.Windower.SendKey(KeyCode.NP_Number2, false);
                            Session.Windower.SendKey(KeyCode.NP_Number4, true);
                            Thread.Sleep(4000);
                            // We no longer want to lock UI so we will create an int and just loop DoEvents() till tiemr expires
                            //System.Timers.Timer randomsleeptimer2 = new System.Timers.Timer(4000);
                            //randomsleeptimer2.Elapsed += new System.Timers.ElapsedEventHandler(timerended);
                            //randomsleeptimer2.AutoReset = false;
                            //randomsleeptimer2.Enabled = true;
                            //do
                            //{
                            //    Application.DoEvents();
                            //} while (timervalue == 0);
                            //timervalue = 0;
                            Session.Windower.SendKey(KeyCode.NP_Number4, false);
                            Thread.Sleep(100);
                            Session.Windower.SendString("/follow <t>");
                            last_dist_duration = 0;
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            // right
                            LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Moving RIGHT" + Environment.NewLine);
                            Move_direction = true;
                            Session.Windower.SendKey(KeyCode.NP_Number2, true);
                            Thread.Sleep(3000);
                            // We no longer want to lock UI so we will create an int and just loop DoEvents() till tiemr expires
                            //System.Timers.Timer randomsleeptimer = new System.Timers.Timer(3000);
                            //randomsleeptimer.Elapsed += new System.Timers.ElapsedEventHandler(timerended);
                            //randomsleeptimer.AutoReset = false;
                            //randomsleeptimer.Enabled = true;
                            //do
                            //{
                            //    Application.DoEvents();
                            //} while (timervalue == 0);
                            //timervalue = 0;
                            Session.Windower.SendKey(KeyCode.NP_Number2, false);
                            Session.Windower.SendKey(KeyCode.NP_Number6, true);
                            Thread.Sleep(4000);
                            // We no longer want to lock UI so we will create an int and just loop DoEvents() till tiemr expires
                            //System.Timers.Timer randomsleeptimer2 = new System.Timers.Timer(4000);
                            //randomsleeptimer2.Elapsed += new System.Timers.ElapsedEventHandler(timerended);
                            //randomsleeptimer2.AutoReset = false;
                            //randomsleeptimer2.Enabled = true;
                            //do
                            //{
                            //    Application.DoEvents();
                            //} while (timervalue == 0);
                            //timervalue = 0;
                            Session.Windower.SendKey(KeyCode.NP_Number6, false);
                            Session.Windower.SendString("/follow <t>");
                            last_dist_duration = 0;
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        last_dist_duration++;
                    }
                }

                if (distance < range)
                {
                    At_Mob = true;
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "At the mob, begin Fighting. " + Environment.NewLine);
                }
                last_dist = Convert.ToInt16(distance);
            }
            #endregion

            #region Are we at the mob? Attack it then!!!
            /* If at the mob, attack it */

            if (At_Mob && !Fighting && !End_Of_Fight && PreFightActions.Checked && !Session.NPC.IsClaimed(Session.Target.ID))
            {
                foreach (var skillstouse in Skill_List_start)
                {
                    Session.Windower.SendString(skillstouse);
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "[Skill-Start] " + skillstouse + Environment.NewLine);
                    Thread.Sleep(Start_Delay);
                }
            }

            if (At_Mob && !Fighting && !End_Of_Fight && !Session.NPC.IsClaimed(Session.Target.ID))
            {
                // Send command twice incase
                String Command = "/attack <t>";
                Session.Windower.SendString(Command);

                // Now fighting
                Fighting = true;
                LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Beginning to Attack: " + Session.Target.Name + Environment.NewLine);
                Mob_Currently_Fighting = Session.Target.Name;
                LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Fighting + Following target" + Environment.NewLine);
            }
            #endregion

            #region Deals with fighting
            /* If fighting */
            if (Fighting && !End_Of_Fight)
            {
                // No longer aggro
                Aggro = false;

                #region Ensure we are fighting through a few conditional checks
                // Ensure Fighting
                if (Session.Player.Status != Status.Fighting)
                {
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Not Fighting? Attack..." + Environment.NewLine);
                    Session.Windower.SendString("/attack <t>");
                }

                // Follow mob incase it moves
                if (!Session.Target.IsLocked)
                {
                    Session.Windower.SendString("/lockon <t>");
                }
                if (distance > 3 && !(String.IsNullOrEmpty(Session.Target.Name)))
                {
                    Session.Windower.SendString("/follow <t>");
                }
                #endregion

                Running_To_Mob = false;
                Scanning_for_mobs = false;

                // Check for loss of target
                bool Mob_No_Longer_Targetted = String.IsNullOrEmpty(Session.Target.Name);
                bool Mob_Targetted_is_Current = Session.Target.Name.Equals(Mob_Currently_Fighting);

                #region Start of fight skills actions
                // Check HP, if 100% its most likely start of fight and trying to get to

                if (Session.Target.HPPCurrent > 95 && Start_Delay == Start_Delay_timer)
                {
                    Start_Delay_timer = 0;

                    // Start fight skills
                    if (Skill_List_start.Count > 0)
                    {
                        // Pick random skill from start list
                        // TODO: Change this so it can be used for trial NMs
                        int random_skill = new Random().Next(0, (Skill_List_start.Count - 1));
                        string Skill = Skill_List_start.ElementAt(random_skill);
                        
                        if (IsSpell(Skill.Split(' ')))
                        {

                            short currentspellcastingpercent;
                            // Stop Follow
                            Session.Windower.SendKeyPress(KeyCode.DownArrow);
                            Thread.Sleep(250);
                            Session.Windower.SendString(Skill);
                            LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "[Magic-Skill-Start] " + Skill + Environment.NewLine);
                            Thread.Sleep(150);
                            while (Session.Player.CastPercentEx < 100)
                            {
                                currentspellcastingpercent = Session.Player.CastPercentEx;
                                Thread.Sleep(250);
                                if (currentspellcastingpercent > Session.Player.CastPercentEx || Session.Player.CastPercentEx == 0)
                                {
                                    // Something happened and casting stopped
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Session.Windower.SendString(Skill);
                            LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + ("[Skill-Start] " + Skill + Environment.NewLine));
                            
                        }
                    }
                }
                else
                {
                    Start_Delay_timer++;
                }
                #endregion

                // Get our line text
                FFACE.ChatTools.ChatLine NewLine = Session.Chat.GetNextLine(LineSettings.CleanAll);
                string NewLine_Text = "";

                #region Checks chatlog for things like cant see mob (it moved?) and cant attack (it claimed?)
                // Check to make sure we have text
                if (NewLine != null)
                {
                    NewLine_Text = NewLine.Text;
                    // check if we can attack
                    if (NewLine_Text.Contains("Cannot attack target"))
                    {
                        //.. do something about it!
                        LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Cannot attack, cancelling..." + Environment.NewLine);
                        Session.Windower.SendKeyPress(KeyCode.EscapeKey);
                        Session.Windower.SendKeyPress(KeyCode.EscapeKey);
                    }
                    else if (NewLine_Text.Contains("Unable to see the " + Session.Target.Name))
                        {
                            //.. do something about it!
                            LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Cannot see target" + Environment.NewLine);
                            Session.Windower.SendString("/follow");
                            Thread.Sleep(500);
                            Session.Windower.SendKeyPress(KeyCode.NP_Number7);
                        }
                }
                #endregion

                #region Do fighting skills
                // Do Skills, only if target's HP is above 10%, and below 98%
                if (Skill_List_fighting.Count > 0 && Session.Target.HPPCurrent > 0 && Session.Target.HPPCurrent < 100)
                {
                    if (Session.Player.TPCurrent >= TP_To_Ws && TP_To_Ws_Mob_HPP == 0)
                    {
                        int random_skill = new Random().Next(0, (Skill_List_fighting.Count - 1));
                        string Skill = Skill_List_fighting.ElementAt(random_skill);
                        Session.Windower.SendString(Skill);
                        LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "[Skill-Fighting] " + Skill + Environment.NewLine);
                    }
                    // WS when mob hits a certain HPP only
                    if (Session.Player.TPCurrent >= 100 && TP_To_Ws_Mob_HPP > 0)
                    {
                        if (Session.Target.HPPCurrent <= TP_To_Ws_Mob_HPP)
                        {
                            int random_skill = new Random().Next(0, (Skill_List_fighting.Count - 1));
                            string Skill = Skill_List_fighting.ElementAt(random_skill);
                            Session.Windower.SendString(Skill);
                            LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "[Skill-Fighting-HPP] - " + Session.Target.HPPCurrent + "% - " + Skill + Environment.NewLine);
                        }
                    }
                }
                #endregion

                #region If we was fighting but mob isnt targetted then its dead
                // if no mob targetted
                if (Mob_No_Longer_Targetted)
                {
                    // Mob dead
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Defeated target, fight over." + Environment.NewLine);
                    // Stop Follow
                    ListOfDefeatedMobs.Items.Add(Mob_Currently_Fighting);
                    Killed_Mob_Amount++;

                    Mob_Currently_Fighting = null;

                    // Currently end of fight
                    End_Of_Fight = true;
                    End_Of_Fight_Wait = true;

                    // Reset Some stuff
                    Fighting = false;
                    At_Mob = false;
                    Running_To_Mob = false;
                }
                #endregion
            }
            #endregion

            #region if its the end of the fight
            /* If end of fight */
            if (End_Of_Fight && Session.Target.Type != NPCType.Mob)
            {
                if (RestAtEnd.Checked && Session.Player.Status != Status.Healing && RestBelowHPCheckbox.Checked && Session.Player.HPPCurrent <= Rest_MP)
                {
                    Resting = false;
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "(HP) Resting... " + Session.Player.HPPCurrent + " (below " + Rest_HPP + " to rest)" + Environment.NewLine);
                }
                if (RestAtEnd.Checked && Session.Player.Status != Status.Healing && RestBelowMPCheckbox.Checked && Session.Player.MPCurrent <= Rest_HPP)
                {
                    Resting = false;
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "(MP) Resting... " + Session.Player.MPCurrent + " (below " + Rest_MP + " to rest)" + Environment.NewLine);
                }
                // Do something about healings
                if (!Resting && RestAtEnd.Checked && RestBelowHPCheckbox.Checked && Session.Player.HPPCurrent <= Rest_HPP)
                {
                    Session.Windower.SendString("/heal on");
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "/heal command sent! (HP)" + Environment.NewLine);
                    Resting = true;
                    Thread.Sleep(3000);
                }
                if (!Resting && RestAtEnd.Checked && RestBelowMPCheckbox.Checked && Session.Player.MPCurrent <= Rest_MP)
                {
                    Session.Windower.SendString("/heal on");
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "/heal command sent! (MP)" + Environment.NewLine);
                    Resting = true;
                    Thread.Sleep(3000);
                }

                if (Resting && ( RestBelowHPCheckbox.Checked && RestBelowMPCheckbox.Checked ) && ( Session.Player.HPPCurrent >= RestUntil_HPP && Session.Player.MPCurrent >= RestUntil_MP ))
                {
                    Session.Windower.SendString("/heal off");
                    Resting = false;
                    Thread.Sleep(3000);
                }
                else
                {
                    if (Resting && RestBelowHPCheckbox.Checked && !RestBelowMPCheckbox.Checked && Session.Player.HPPCurrent >= RestUntil_HPP)
                    {
                        Session.Windower.SendString("/heal off");
                        Resting = false;
                        Thread.Sleep(3000);
                    }
                    if (Resting && RestBelowMPCheckbox.Checked && !RestBelowHPCheckbox.Checked && Session.Player.MPCurrent >= RestUntil_MP)
                    {
                        Session.Windower.SendString("/heal off");
                        Resting = false;
                        Thread.Sleep(3000);
                    }
                }

                if (!Resting)
                {
                    Skipping = false;
                    is_Mob_Valid = false;

                    // End of fight actions
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Stop Autorun, Running end of fight actions" + Environment.NewLine);

                    // Stop auto run
                    Session.Windower.SendKeyPress(KeyCode.NP_Number7);
                    if (Session.Target.Type != NPCType.Mob)
                    {
                        Session.Windower.SendKeyPress(KeyCode.EscapeKey);

                        // Anythign else here
                        // Ensure Third person
                        if (Session.Player.ViewMode != ViewMode.ThirdPerson)
                        {
                            Session.Windower.SendKeyPress(KeyCode.NP_Number5);
                        }

                        // this is a little Wait to ensure above got processed.
                        if (!End_Of_Fight_Wait)
                        {
                            // End of ...End of fight :D
                            End_Of_Fight = false;
                            Scanning_for_mobs = true;
                            LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + ( "---[Start New]---" + Environment.NewLine ));
                            Start_Delay_timer = 0;
                        }
                        else
                        {
                            End_Of_Fight_Wait = false;
                        }
                    }
                    else
                    {
                        End_Of_Fight = false;
                        Scanning_for_mobs = true;
                        Mob_Currently_Fighting = Session.Target.Name;
                    }
                }
            }
            #endregion

            #region Else could be aggro detection
            else if (End_Of_Fight && Session.Target.Type == NPCType.Mob)
            {
                LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Link/Aggro-" + Environment.NewLine);
                Fighting = true;
                End_Of_Fight = false;
                Thread.Sleep(100);
                Mob_Currently_Fighting = Session.Target.Name;
                Thread.Sleep(100);
                Session.Windower.SendString("/attack <t>");
                Thread.Sleep(100);
                Session.Windower.SendString("/lockon <t>");
                Thread.Sleep(100);
                Session.Windower.SendString("/follow <t>");
            }
            #endregion
           
            // Scroll to bottom of log
            LevellingLog.ScrollToCaret();

        }

        // Start Botton
        private void StartBotting_Click(object sender, EventArgs e)
        {

            // Check if running or not
            if (Levelling_Running)
            {
                // Reset all
                Running_To_Waypoint = false;
                Arrived_At_Position = false;
                Scanning_for_mobs = true;
                Skipping = false;
                seconds = 0;
                Tab_Count = 0;
                Levelling_Running = false;
                Running_To_Mob = false;
                At_Mob = false;
                Fighting = false;
                End_Of_Fight = false;
                End_Of_Fight_Wait = true;
                Mob_Currently_Fighting = null;
                Record_Label_Status = false;

                //-------------------------------------------------------------------
                // Disable timers
                LevellingTimer.Enabled = false;
                WPTesting.Enabled = false;

                // Change text on button
                StartBotting.Text = Resources.Form1_StartBotting_Click_START;

                if (Session.Player.ViewMode != ViewMode.ThirdPerson)
                {
                    Session.Windower.SendKeyPress(KeyCode.NP_Number5);
                }

                Tab_Count = 0;
            }
            else
            {
                Current_EXP_This_Level_Value = Session.Player.EXPForLevel;
                Current_EXP_Value = Session.Player.EXPIntoLevel;
                Current_Level = Session.Player.MainJobLevel;
                seconds = 0;

                // If go to checked enable, change tabe
                if (GoToLogOnStart.Checked)
                {
                    LevellingTabs.SelectedIndex = 0;
                }

                // Inform Log
                LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + ("Starting..." + Environment.NewLine));

                if (Way_Points_X.Count == 0)
                {
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Fighting on the spot" + Environment.NewLine);
                }

                // Start
                LevellingTimer.Enabled = true;

                // Scroll our chatlog window to the last line
                ChatLog.ScrollToCaret();

                // Set Running Status
                Levelling_Running = true;

                // Change text on button
                StartBotting.Text = Resources.Form1_StartBotting_Click_STOP;

                if (Session.Player.ViewMode != ViewMode.FirstPerson)
                {
                    Session.Windower.SendKeyPress(KeyCode.NP_Number5);
                }

                Tab_Count = 0;
            }
        }

        // Start recording waypoints
        private void WPRecord_Click(object sender, EventArgs e)
        {
            if (WaypointRecrd.Enabled == true)
            {
                WaypointRecrd.Enabled = false;
                RecodingLabel.Text = "---------";
                WPRecord.Text = Resources.Form1_WPRecord_Click_Record;
            }
            else
            {
                WaypointRecrd.Enabled = true;
                RecodingLabel.Text = Resources.Form1_WPRecord_Click_Recording;
                WPRecord.Text = Resources.Form1_StartBotting_Click_STOP;
            }
        }

        // Clear Way points
        private void WPDelete_Click(object sender, EventArgs e)
        {
            // Clear Lists
            WayPointList.Items.Clear();
            Way_Points_X.Clear();
            Way_Points_Z.Clear();

            // Confirm
            InformationLabel.Text = Resources.Form1_WPDelete_Click_Way_Points_Cleared;
        }

        // Record Way Points
        private void WaypointRecrd_Tick(object sender, EventArgs e)
        {
            // If recording or not
            if (Record_Label_Status)
            {
                RecodingLabel.Text = "---------";
                Record_Label_Status = false;
            }
            else
            {
                RecodingLabel.Text = Resources.Form1_WPRecord_Click_Recording;
                Record_Label_Status = true;
            }

            float X = Session.Player.PosX;
            float Z = Session.Player.PosZ;

            // Presentable List
            WayPointList.Items.Add(X + ", " + Z);
            Way_Points_X.Add(X);
            Way_Points_Z.Add(Z);
        }

        // Add a way point manually
        private void ManualWP_Click(object sender, EventArgs e)
        {
            // Add WP
            float X = Session.Player.PosX;
            float Z = Session.Player.PosZ;

            // Presentable List
            WayPointList.Items.Add(X + ", " + Z);
            Way_Points_X.Add(X);
            Way_Points_Z.Add(Z);
        }

        // Test Way Points
        private void button3_Click(object sender, EventArgs e)
        {
            if (WPTesting.Enabled == true)
            {
                WPTesting.Enabled = false;
                TestWPBtn.Text = Resources.Form1_button3_Click_Test_WPs;
                Session.Navigator.Reset();
            }
            else
            {
                TestWPBtn.Text = Resources.Form1_button3_Click_Running;
                WPTesting.Enabled = true;
            }
        }

        // Clear all skills
        private void RemoveSkillBtn_Click(object sender, EventArgs e)
        {
            Skill_List_start.Clear(); Skill_List_end.Clear(); Skill_List_fighting.Clear();
            SkillBoxSTART.Items.Clear(); SkillBoxEND.Items.Clear(); SkillBoxFIGHTING.Items.Clear();
        }    

        private float Going_To_X, Going_To_Z;
        private int List_Position = 0;
        private bool Running_To_Waypoint = false;
        private bool Arrived_At_Position = false;
        private int WP_Wait = 12;
        private int WP_Wait_Timer = 0;
        private int Scan_At_Every_Waypoints = 3;
        private int WayPoint_Scan_increment = 1;
        private bool Scanning_for_mobs = true;
        private bool Skipping = false;

        private void WPTesting_Tick(object sender, EventArgs e)
        {
            WayPointWaitShow.Text = WayPoint_Scan_increment.ToString() + " < " + Scan_At_Every_Waypoints.ToString();
            WayPointNum.Text = List_Position.ToString();
            WPTimer.Text = WP_Wait_Timer.ToString();
            WPStatus.Text = Arrived_At_Position.ToString();

            // If not running
            if (!Session.Navigator.IsRunning() && !Running_To_Waypoint)
            {
                if ((Session.Player.PosX != Going_To_X || Session.Player.PosZ != Going_To_Z) && !Arrived_At_Position)
                {
                    // Ensure first person
                    if (Session.Player.ViewMode != ViewMode.FirstPerson)
                    {
                        Session.Windower.SendKeyPress(KeyCode.NP_Number5);
                    }

                    // Set going to floats
                    if (Way_Points_X.Capacity > 0) Going_To_X = Way_Points_X.ElementAt(List_Position);
                    else return;
                    if (Way_Points_Z.Capacity > 0) Going_To_Z = Way_Points_Z.ElementAt(List_Position);
                    else return;

                    // Face Position
                    double fX = Convert.ToDouble(Going_To_X);
                    double fZ = Convert.ToDouble(Going_To_Z);

                    Session.Navigator.FaceHeading(fX, fZ);

                    // Set dPoints 
                    FFACE.NavigatorTools.dPoint dx = new FFACE.NavigatorTools.dPoint(() => Going_To_X);
                    FFACE.NavigatorTools.dPoint dz = new FFACE.NavigatorTools.dPoint(() => Going_To_Z);

                    // Run to points
                    Running_To_Waypoint = true;
                    LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "POS(" + Going_To_X + ", " + Going_To_Z + ")" + Environment.NewLine);
                    Session.Navigator.Goto(dx, dz, false);
                    Session.Navigator.Reset();

                    // Arrived
                    Running_To_Waypoint = false;
                    Arrived_At_Position = true;
                }

                // If we've arrived, do next one
                if (Arrived_At_Position)
                {
                    if (WayPoint_Scan_increment <= Scan_At_Every_Waypoints && !Scanning_for_mobs)
                    {
                        List_Position++;
                        Arrived_At_Position = false;
                        Running_To_Waypoint = false;
                        WP_Wait_Timer = 0;
                        WayPoint_Scan_increment++;
                        LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Skipping Scan at (" + List_Position + ")" + Environment.NewLine);
                        Skipping = true;
                    }
                    else
                    {
                        // Wait x amount of seconds, tabbing to find mobs
                        if (WP_Wait_Timer == WP_Wait)
                        {
                            List_Position++;
                            Arrived_At_Position = false;
                            Running_To_Waypoint = false;
                            Scanning_for_mobs = false;
                            WP_Wait_Timer = 0;
                            WayPoint_Scan_increment++;
                        }
                        else
                        {
                            Scanning_for_mobs = true;

                            // Check skip
                            if (WayPoint_Scan_increment == 0)
                            {
                                Skipping = false;
                            }

                            // Ensure third person
                            if (Session.Player.ViewMode != ViewMode.ThirdPerson && WayPoint_Scan_increment == 0 && !Skipping)
                            {
                                Session.Windower.SendKeyPress(KeyCode.NP_Number5);
                                LevellingLog.AppendText("[" + DateTime.Now.ToLongTimeString() + "] " + "Scanning for Valid Mobs..." + Environment.NewLine);
                            }

                            // Increase wait timer
                            WP_Wait_Timer++;
                            WayPoint_Scan_increment = 0;
                        }
                    }
                }
            }
        }

        // Backup timer 2 seconds
        private void Backup_Tick(object sender, EventArgs e)
        {
            if (WPTesting.Enabled == false && Scanning_for_mobs && Tab_Count > 10 && Way_Points_X.Count != 0)
            {
                WPTesting.Enabled = true;
            }

            my_static.Debugger.DebugLines.Clear();
            my_static.Debugger.DebugLines.Add("Levelling_Running : " + Levelling_Running);
            my_static.Debugger.DebugLines.Add("Running_To_Mob : " + Running_To_Mob);
            my_static.Debugger.DebugLines.Add("At_Mob : " + At_Mob);
            my_static.Debugger.DebugLines.Add("Fighting : " + Fighting);
            my_static.Debugger.DebugLines.Add("End_Of_Fight : " + End_Of_Fight);
            my_static.Debugger.DebugLines.Add("End_Of_Fight_Wait : " + End_Of_Fight_Wait);
            my_static.Debugger.DebugLines.Add("Mob_Currently_Fighting : " + Mob_Currently_Fighting);
            my_static.Debugger.DebugLines.Add("Running_To_Waypoint : " + Running_To_Waypoint);
            my_static.Debugger.DebugLines.Add("Arrived_At_Position : " + Arrived_At_Position);
            my_static.Debugger.DebugLines.Add("Scanning_for_mobs : " + Scanning_for_mobs);
            my_static.Debugger.DebugLines.Add("Skipping : " + Skipping);
            my_static.Debugger.DebugLines.Add("WPTesting_Timer : " + WPTesting.Enabled);
            my_static.Debugger.DebugLines.Add("------------------------------------------------");
            my_static.Debugger.DebugLines.Add("is_Mob_Valid && !Session.Navigator.IsRunning() && !Running_To_Mob && !Fighting && !End_Of_Fight");
            my_static.Debugger.DebugLines.Add(is_Mob_Valid.ToString() + Session.Navigator.IsRunning().ToString() + Running_To_Mob + Fighting + End_Of_Fight);
            
        }  

        // Reset Navigation
        private void button3_Click_1(object sender, EventArgs e)
        {
            Session.Navigator.Reset();
            Session.Windower.SendKey(KeyCode.NP_Number6, false);
            Session.Windower.SendKey(KeyCode.NP_Number4, false);
            Session.Windower.SendKey(KeyCode.NP_Number2, false);
        }

        // Add mob to valid list
        private void AddMobToValid_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Session.Target.Name) && !Valid_Mobs.Contains(Session.Target.Name))
            {
                Valid_Mobs.Add(Session.Target.Name);
                ValidMobList.Items.Add(Session.Target.Name);
            }

            if (ValidMobList.Items.Count > 0)
            {
                StartBotting.Enabled = true;
            }
        }
        
        // Removes a target from valid mob list
        private void RemoveTargetFromList_Click(object sender, EventArgs e)
        {
            int RemovedSoFar = 0;
            foreach(int indexChecked in ValidMobList.CheckedIndices) 
            {
                ValidMobList.Items.RemoveAt(indexChecked - RemovedSoFar);
                Valid_Mobs.RemoveAt(indexChecked - RemovedSoFar);
                RemovedSoFar++;
            }

            if (ValidMobList.Items.Count == 0)
            {
                StartBotting.Enabled = false;
            }
        }

        // Add a skill
        private void AddtypedSkill_Click(object sender, EventArgs e)
        {
            string whentodoskill = WhenToDoSkillOption.SelectedItem.ToString();
            if (whentodoskill == "start")
            {
                Skill_List_start.Add(SkillType.SelectedItem + " \"" + SkillInput.Text + "\" " + targetSelector.Text);
                SkillBoxSTART.Items.Add(SkillType.SelectedItem + " \"" + SkillInput.Text + "\" " + targetSelector.Text);
            }
            else if (whentodoskill == "end")
            {
                Skill_List_end.Add(SkillType.SelectedItem + " \"" + SkillInput.Text + "\" " + targetSelector.Text);
                SkillBoxEND.Items.Add(SkillType.SelectedItem + " \"" + SkillInput.Text + "\" " + targetSelector.Text);
            }
            else if (whentodoskill == "fighting")
            {
                Skill_List_fighting.Add(SkillType.SelectedItem + " \"" + SkillInput.Text + "\" " + targetSelector.Text);
                SkillBoxFIGHTING.Items.Add(SkillType.SelectedItem + " \"" + SkillInput.Text + "\" " + targetSelector.Text);
            }
        }

        // Way Poitn Options
        private void WayPointWaitInput_TextChanged(object sender, EventArgs e)
        {
            try
            {
                WP_Wait = Convert.ToInt16(WayPointWaitInput.Text);
            }
            catch (Exception exc)
            {
                WayPointWaitInput.Text = "12";
                DebugBox.AppendText("Error: " + exc.Message + Environment.NewLine);
            }
        }
        private void CameraStrengthInput_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Camera_Strength = Convert.ToInt16(CameraStrengthInput.Text);
            }
            catch (Exception exc)
            {
                CameraStrengthInput.Text = "4";
                DebugBox.AppendText("Error: " + exc.Message + Environment.NewLine);
            }
        }
        private void DistanceTillAttackInput_TextChanged(object sender, EventArgs e)
        {
            try
            {
                range = Convert.ToDouble(DistanceTillAttackInput.Text);
            }
            catch (Exception exc)
            {
                DistanceTillAttackInput.Text = "5.5";
                DebugBox.AppendText("Error: " + exc.Message + Environment.NewLine);
            }
        }
        private void WayPointScanningInput_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Scan_At_Every_Waypoints = Convert.ToInt16(WayPointScanningInput.Text);
            }
            catch (Exception exc)
            {
                WayPointScanningInput.Text = "3";
                DebugBox.AppendText("Error: " + exc.Message + Environment.NewLine);
            }
        }
        private void StartDelayInput_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Start_Delay = Convert.ToInt16(StartDelayInput.Text);
            }
            catch (Exception exc)
            {
                StartDelayInput.Text = "5";
                DebugBox.AppendText("Error: " + exc.Message + Environment.NewLine);
            }
        }
        private void TPPercentToWS_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TP_To_Ws = Convert.ToInt16(TPPercentToWS.Text);
            }
            catch (Exception exc)
            {
                TPPercentToWS.Text = "100";
                DebugBox.AppendText("Error: " + exc.Message + Environment.NewLine);
            }
        }

        private void TPMobHPP_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TP_To_Ws_Mob_HPP = Convert.ToInt16(TPMobHPP.Text);
            }
            catch (Exception exc)
            {
                TPMobHPP.Text = "0";
                DebugBox.AppendText("Error: " + exc.Message + Environment.NewLine);
            }
        }

        private void DistanceBeforeAction_TextChanged (object sender, EventArgs e)
        {

        }

        // Stuff for if we die
        private int DeathWait = 300;
        private int DeathWait_current = 0;
        private void DeathTimer_Tick(object sender, EventArgs e)
        {
            DeathWait_current++;
            if (DeathWait >= DeathWait_current)
            {
                DeathWait_current = 0;
                DeathTimer.Enabled = false;
                LevellingTimer.Enabled = true;
            }
        }
        #endregion
        //-----------------------------------------------------------------------------------------------
        #region Way Point Handling
        // Load/Save Lists
        // Load Way Points
        int Way_Points_Loaded;
        private void LoadWayPointsBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenDialog = new OpenFileDialog();
            OpenDialog.InitialDirectory = Application.StartupPath + "\\" + Session.Player.Name + "\\";
            OpenDialog.Filter = Resources.Form1_LoadWayPointsBtn_Click_XI_Way_Points___xiwps;
            OpenDialog.FilterIndex = 1;

            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                Way_Points_X.Clear();
                Way_Points_Z.Clear();
                WayPointList.Items.Clear();

                string Waypoint_Filename = OpenDialog.FileName;
                DebugBox.AppendText("Opening File: " + Waypoint_Filename + Environment.NewLine);

                string[] break_filename = OpenDialog.FileName.Split('\\');
                int name_pos = (break_filename.Count() - 1);
                WPLoadedList.Text = break_filename.ElementAt(name_pos);

                FileStream fs = new FileStream(Waypoint_Filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);

                String Line;
                while ((Line = sr.ReadLine()) != null)
                {
                    string[] Positions = Line.Split('|');

                    // Add WP
                    float X = float.Parse(Positions.ElementAt(0));
                    float Z = float.Parse(Positions.ElementAt(1));

                    // Presentable List
                    WayPointList.Items.Add(X + ", " + Z);
                    Way_Points_X.Add(X);
                    Way_Points_Z.Add(Z);

                    // Increment
                    Way_Points_Loaded++;

                }

                sr.Dispose();
                sr.Close();

                DebugBox.AppendText(Way_Points_Loaded + " waypoints added." + Environment.NewLine);

                // Saved
                InformationLabel.Text = Resources.Form1_LoadWayPointsBtn_Click_Loaded_Waypoints;

            }
        }
        // Save Way Points
        private void SaveWayPointsBtn_Click(object sender, EventArgs e)
        {
            if (Way_Points_X.Count > 0)
            {
                SaveFileDialog SaveDialog = new SaveFileDialog();
                SaveDialog.InitialDirectory = Application.StartupPath + "\\" + Session.Player.Name + "\\";
                SaveDialog.Filter = Resources.Form1_LoadWayPointsBtn_Click_XI_Way_Points___xiwps;
                SaveDialog.FilterIndex = 1;

                string Waypoint_Filename;
                if (SaveDialog.ShowDialog() == DialogResult.OK)
                {
                    if (SaveDialog.FileName.Contains(".xiwps"))
                        Waypoint_Filename = SaveDialog.FileName;
                    else
                        Waypoint_Filename = SaveDialog.FileName + ".xiwps";

                    string[] break_filename = SaveDialog.FileName.Split('\\');
                    int name_pos = (break_filename.Count() - 1);
                    WPLoadedList.Text = break_filename.ElementAt(name_pos);

                    // Set File Info
                    FileInfo fi = new FileInfo(Waypoint_Filename);
                    FileStream fs = fi.Create();
                    StreamWriter sw = new StreamWriter(fs);

                    // Write out each text line
                    int MaxPos = (Way_Points_X.Count);
                    int i = 0;

                    while (i < MaxPos)
                    {
                        string Line = Way_Points_X.ElementAt(i) + "|" + Way_Points_Z.ElementAt(i);
                        sw.WriteLine(Line.Trim());
                        i++;
                    }

                    // Close
                    sw.Dispose();
                    sw.Close();
                    fs.Dispose();
                    fs.Close();

                    // Saved
                    InformationLabel.Text = Resources.Form1_SaveWayPointsBtn_Click_Saved_Waypoints;
                    DebugBox.AppendText("Waypoints Saved: " + Waypoint_Filename + Environment.NewLine);
                }
            }
        }
        // Load Mob Lists
        int Mobs_Loaded;
        private void LoadMobListBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenDialog = new OpenFileDialog();
            OpenDialog.InitialDirectory = Application.StartupPath + "\\" + Session.Player.Name + "\\";
            OpenDialog.Filter = Resources.Form1_LoadMobListBtn_Click_XI_Mob_List___ximbl;
            OpenDialog.FilterIndex = 1;

            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                Valid_Mobs.Clear();
                ValidMobList.Items.Clear();

                string Moblist_Filename = OpenDialog.FileName;
                DebugBox.AppendText("Opening File: " + Moblist_Filename + Environment.NewLine);

                string[] break_filename = OpenDialog.FileName.Split('\\');
                int name_pos = (break_filename.Count() - 1);
                MobLoadedList.Text = break_filename.ElementAt(name_pos);

                FileStream fs = new FileStream(Moblist_Filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);

                String Line;
                while ((Line = sr.ReadLine()) != null)
                {
                    Valid_Mobs.Add(Line);
                    ValidMobList.Items.Add(Line);

                    // Increment
                    Mobs_Loaded++;
                }

                sr.Dispose();
                sr.Close();

                DebugBox.AppendText(Mobs_Loaded + " Mobs added." + Environment.NewLine);

                // Saved
                InformationLabel.Text = Resources.Form1_LoadWayPointsBtn_Click_Loaded_Waypoints;

            }

            if (ValidMobList.Items.Count > 0)
            {
                StartBotting.Enabled = true;
            }
        }
        // Save Mob Lists
        private void SaveMobListBtn_Click(object sender, EventArgs e)
        {
            if (Valid_Mobs.Count > 0)
            {
                SaveFileDialog SaveDialog = new SaveFileDialog();
                SaveDialog.InitialDirectory = Application.StartupPath + "\\" + Session.Player.Name + "\\";
                SaveDialog.Filter = Resources.Form1_LoadMobListBtn_Click_XI_Mob_List___ximbl;
                SaveDialog.FilterIndex = 1;

                string Moblist_Filename;
                if (SaveDialog.ShowDialog() == DialogResult.OK)
                {
                    if (SaveDialog.FileName.Contains(".ximbl"))
                        Moblist_Filename = SaveDialog.FileName;
                    else
                        Moblist_Filename = SaveDialog.FileName + ".ximbl";

                    string[] break_filename = SaveDialog.FileName.Split('\\');
                    int name_pos = (break_filename.Count() - 1);
                    MobLoadedList.Text = break_filename.ElementAt(name_pos);

                    // Set File Info
                    FileInfo fi = new FileInfo(Moblist_Filename);
                    FileStream fs = fi.Create();
                    StreamWriter sw = new StreamWriter(fs);

                    // Write out each text line
                    int MaxMobs = (Valid_Mobs.Count);
                    int i = 0;

                    while (i < MaxMobs)
                    {
                        string Line = Valid_Mobs.ElementAt(i);
                        sw.WriteLine(Line.Trim());
                        i++;
                    }

                    // Close
                    sw.Dispose();
                    sw.Close();
                    fs.Dispose();
                    fs.Close();

                    // Saved
                    InformationLabel.Text = Resources.Form1_SaveMobListBtn_Click_Saved_Mob_List;
                    DebugBox.AppendText("Mob List Saved: " + Moblist_Filename + Environment.NewLine);
                }
            }
        }
        // Load weapon skills
        int Skills_Loaded;
        private void LoadWSListBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenDialog = new OpenFileDialog();
            OpenDialog.InitialDirectory = Application.StartupPath + "\\" + Session.Player.Name + "\\";
            OpenDialog.Filter = Resources.Form1_SaveWSListBtn_Click_XI_Skill_List___xiwsl;
            OpenDialog.FilterIndex = 1;

            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                Skill_List_start.Clear(); Skill_List_end.Clear(); Skill_List_fighting.Clear();
                SkillBoxSTART.Items.Clear(); SkillBoxEND.Items.Clear(); SkillBoxFIGHTING.Items.Clear();

                string WSlist_Filename = OpenDialog.FileName;
                DebugBox.AppendText("Opening File: " + WSlist_Filename + Environment.NewLine);

                string[] break_filename = OpenDialog.FileName.Split('\\');
                int name_pos = (break_filename.Count() - 1);
                WSLoadedList.Text = break_filename.ElementAt(name_pos);

                FileStream fs = new FileStream(WSlist_Filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);

                String Line;
                while ((Line = sr.ReadLine()) != null)
                {
                    if (Line.Contains("start"))
                    {
                        string skillz = Line.Replace("start|", "");
                        Skill_List_start.Add(skillz);
                        SkillBoxSTART.Items.Add(skillz);
                    }
                    else if (Line.Contains("end"))
                    {
                        string skillz = Line.Replace("end|", "");
                        Skill_List_end.Add(skillz);
                        SkillBoxEND.Items.Add(skillz);
                    }
                    else if (Line.Contains("fighting"))
                    {
                        string skillz = Line.Replace("fighting|", "");
                        Skill_List_fighting.Add(skillz);
                        SkillBoxFIGHTING.Items.Add(skillz);
                    }

                    // Increment
                    Skills_Loaded++;
                }

                sr.Dispose();
                sr.Close();

                DebugBox.AppendText(Skills_Loaded + " Skills added." + Environment.NewLine);

                // Saved
                InformationLabel.Text = Resources.Form1_LoadWSListBtn_Click_Loaded_Skill_List;

            }
        }
        // Save Weapon Skills
        private void SaveWSListBtn_Click(object sender, EventArgs e)
        {
            if (Skill_List_start.Count > 0 || Skill_List_end.Count > 0 || Skill_List_fighting.Count > 0)
            {
                SaveFileDialog SaveDialog = new SaveFileDialog();
                SaveDialog.InitialDirectory = Application.StartupPath + "\\" + Session.Player.Name + "\\";
                SaveDialog.Filter = Resources.Form1_SaveWSListBtn_Click_XI_Skill_List___xiwsl;
                SaveDialog.FilterIndex = 1;

                string SkillList_Filename;
                if (SaveDialog.ShowDialog() == DialogResult.OK)
                {
                    if (SaveDialog.FileName.Contains(".xiwsl"))
                        SkillList_Filename = SaveDialog.FileName;
                    else
                        SkillList_Filename = SaveDialog.FileName + ".xiwsl";

                    string[] break_filename = SaveDialog.FileName.Split('\\');
                    int name_pos = (break_filename.Count() - 1);
                    WSLoadedList.Text = break_filename.ElementAt(name_pos);

                    // Set File Info
                    FileInfo fi = new FileInfo(SkillList_Filename);
                    FileStream fs = fi.Create();
                    StreamWriter sw = new StreamWriter(fs);

                    // Start Skills
                    if (Skill_List_start.Count > 0)
                    {
                        foreach (String skillz in Skill_List_start)
                        {
                            sw.WriteLine("start|" + skillz.Trim());
                        }
                    }
                    // End Skills
                    if (Skill_List_end.Count > 0)
                    {
                        foreach (String skillz in Skill_List_end)
                        {
                            sw.WriteLine("end|" + skillz.Trim());
                        }
                    }
                    // Fighting Skills
                    if (Skill_List_fighting.Count > 0)
                    {
                        foreach (String skillz in Skill_List_fighting)
                        {
                            sw.WriteLine("fighting|" + skillz.Trim());
                        }
                    }


                    // Close
                    sw.Dispose();
                    sw.Close();
                    fs.Dispose();
                    fs.Close();

                    // Saved
                    InformationLabel.Text = Resources.Form1_SaveWSListBtn_Click_Saved_Skill_List;
                    DebugBox.AppendText("Skill List Saved: " + SkillList_Filename + Environment.NewLine);
                }
            }
        }
        // Change waypoint Speed
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                WaypointRecrd.Interval = Convert.ToInt16(WayPointRecordSpeedInput.Text);
            }
            catch (Exception exc)
            {
                WayPointRecordSpeedInput.Text = "5000";
                DebugBox.AppendText(exc.Message + Environment.NewLine);
            }
        }
        #endregion
        //-----------------------------------------------------------------------------------------------
        // Test Keys
        private void TestTab_Click(object sender, EventArgs e)
        {
            Session.Windower.SendKeyPress(KeyCode.TabKey);
        }
        //-----------------------------------------------------------------------------------------------
        // Powerlevelling
        private void ManualSkillList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        //-----------------------------------------------------------------------------------------------
        #region Enable Cure Buttons
        private void PLMemberA_TextChanged(object sender, EventArgs e)
        {
            if (PLMemberA.Text.Length > 3)
                CureAButton.Enabled = true;
            else
                CureAButton.Enabled = false;
        }
        private void PLMemberB_TextChanged(object sender, EventArgs e)
        {
            if (PLMemberB.Text.Length > 3)
                CureBButton.Enabled = true;
            else
                CureBButton.Enabled = false;
        }
        private void PLMemberC_TextChanged(object sender, EventArgs e)
        {
            if (PLMemberC.Text.Length > 3)
                CureCButton.Enabled = true;
            else
                CureCButton.Enabled = false;
        }
        private void PLMemberD_TextChanged(object sender, EventArgs e)
        {
            if (PLMemberD.Text.Length > 3)
                CureDButton.Enabled = true;
            else
                CureDButton.Enabled = false;
        }
        private void PLMemberE_TextChanged(object sender, EventArgs e)
        {
            if (PLMemberE.Text.Length > 3)
                CureEButton.Enabled = true;
            else
                CureEButton.Enabled = false;
        }
        private void PLMemberF_TextChanged(object sender, EventArgs e)
        {
            if (PLMemberF.Text.Length > 3)
                CureFButton.Enabled = true;
            else
                CureFButton.Enabled = false;
        }
        #endregion

        #region Cure Buttons
        // Cure Buttons
        private bool switchheal = true;
        private void CureAButton_Click(object sender, EventArgs e)
        {
            if (switchheal)
            {
                string Command = "/ma \"" + ManualSkillList.SelectedItem + "\" " + PLMemberA.Text + "";
                PowerSession.Windower.SendString(Command);
                switchheal = false;
            }
            else
            {
                string Command = "/ma \"" + ManualSkillList2.SelectedItem + "\" " + PLMemberA.Text + "";
                PowerSession.Windower.SendString(Command);
                switchheal = true;
            }
        }
        private void CureBButton_Click(object sender, EventArgs e)
        {
            if (switchheal)
            {
                string Command = "/ma \"" + ManualSkillList.SelectedItem + "\" " + PLMemberB.Text + "";
                PowerSession.Windower.SendString(Command);
                switchheal = false;
            }
            else
            {
                string Command = "/ma \"" + ManualSkillList2.SelectedItem + "\" " + PLMemberB.Text + "";
                PowerSession.Windower.SendString(Command);
                switchheal = true;
            }
        }
        private void CureCButton_Click(object sender, EventArgs e)
        {
            if (switchheal)
            {
                string Command = "/ma \"" + ManualSkillList.SelectedItem + "\" " + PLMemberC.Text + "";
                PowerSession.Windower.SendString(Command);
                switchheal = false;
            }
            else
            {
                string Command = "/ma \"" + ManualSkillList2.SelectedItem + "\" " + PLMemberC.Text + "";
                PowerSession.Windower.SendString(Command);
                switchheal = true;
            }
        }
        private void CureDButton_Click(object sender, EventArgs e)
        {
            if (switchheal)
            {
                string Command = "/ma \"" + ManualSkillList.SelectedItem + "\" " + PLMemberD.Text + "";
                PowerSession.Windower.SendString(Command);
                switchheal = false;
            }
            else
            {
                string Command = "/ma \"" + ManualSkillList2.SelectedItem + "\" " + PLMemberD.Text + "";
                PowerSession.Windower.SendString(Command);
                switchheal = true;
            }
        }
        private void CureEButton_Click(object sender, EventArgs e)
        {
            if (switchheal)
            {
                string Command = "/ma \"" + ManualSkillList.SelectedItem + "\" " + PLMemberE.Text + "";
                PowerSession.Windower.SendString(Command);
                switchheal = false;
            }
            else
            {
                string Command = "/ma \"" + ManualSkillList2.SelectedItem + "\" " + PLMemberE.Text + "";
                PowerSession.Windower.SendString(Command);
                switchheal = true;
            }
        }
        private void CureFButton_Click(object sender, EventArgs e)
        {
            if (switchheal)
            {
                string Command = "/ma \"" + ManualSkillList.SelectedItem + "\" " + PLMemberF.Text + "";
                PowerSession.Windower.SendString(Command);
                switchheal = false;
            }
            else
            {
                string Command = "/ma \"" + ManualSkillList2.SelectedItem + "\" " + PLMemberF.Text + "";
                PowerSession.Windower.SendString(Command);
                switchheal = true;
            }
        }
        private void PLHealButton_Click(object sender, EventArgs e)
        {
            string Command = "/heal";
            PowerSession.Windower.SendString(Command);
        }
        private void PLInviteButton_Click(object sender, EventArgs e)
        {
            string Command = "/pcmd add <t>";
            PowerSession.Windower.SendString(Command);
        }
        private void PLSneakButton_Click(object sender, EventArgs e)
        {
            string Command = "/ma \"Sneak\" <t>";
            PowerSession.Windower.SendString(Command);
        }
        private void PLInvisButton_Click(object sender, EventArgs e)
        {
            string Command = "/ma \"Invisible\" <t>";
            PowerSession.Windower.SendString(Command);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string Command = "/pcmd leave";
            PowerSession.Windower.SendString(Command);
        }
        #endregion

        #region More Buttons
        // More Buttonzz
        private void PLFollowButton_Click(object sender, EventArgs e)
        {
            // Target
            string Command = "/target " + PLFollowingChar.Text;
            PowerSession.Windower.SendString(Command);
            // Lockon
            Command = "/lockon <t>";
            PowerSession.Windower.SendString(Command);
            // Follow
            Command = "/follow";
            PowerSession.Windower.SendString(Command);
            
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            InformationLabel.Text = Resources.Form1_checkBox1_CheckedChanged_Auto_heal_for__ + PLMemberA.Text + Resources.Form1_checkBox1_CheckedChanged__activated__Heal_ + AutoHealPercent.Text + "%";
            string Command = "/target " + PLMemberA.Text;
            PowerSession.Windower.SendString(Command);
        }
        private void PLHealButton_Click_1(object sender, EventArgs e)
        {
            string Command = "/heal";
            PowerSession.Windower.SendString(Command);
        }
        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (FollowingCharacter)
                FollowingCharacter = false;
            else
                FollowingCharacter = true;

            InformationLabel.Text = PowerSession.Player.Name + Resources.Form1_checkBox1_CheckedChanged_1__will_auto_follow__ + PLFollowingChar.Text;
        }
        private void ClearLogButton_Click(object sender, EventArgs e)
        {
            LevellingLog.Clear();
        }
        #endregion
        //-----------------------------------------------------------------------------------------------
        // Auto set PL Fields
        private void AutoSetPLFields_Click(object sender, EventArgs e)
        {
            PLFollowingChar.Text = PowerSession.Target.Name;
            PLMemberA.Text = PowerSession.Target.Name;
            PLAutoHeal.Checked = true;
            AutoLockOnCheck.Checked = true;
            AutoNASpells.Checked = true;
            SelfHeal.Checked = true;
        }

        private void TestTab_Click_1(object sender, EventArgs e)
        {
            Session.Windower.SendKeyPress(KeyCode.TabKey);
        }

        private void AlwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
            if (AlwaysOnTop.Checked)
                this.TopMost = true;
            else
                this.TopMost = false;
        }

        #region Delete actions for skill menus
        private void removeSkillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = SkillBoxFIGHTING.SelectedIndex;
            SkillBoxFIGHTING.Items.RemoveAt(index);
            Skill_List_fighting.RemoveAt(index);
        }
        private void RemoveSTART_Opening(object sender, CancelEventArgs e)
        {
            int index = SkillBoxSTART.SelectedIndex;
            SkillBoxSTART.Items.RemoveAt(index);
            Skill_List_start.RemoveAt(index);
        }
        private void RemoveEND_Opening(object sender, CancelEventArgs e)
        {
            int index = SkillBoxEND.SelectedIndex;
            SkillBoxEND.Items.RemoveAt(index);
            Skill_List_end.RemoveAt(index);
        }
        #endregion

        #region Mouse down events for skill menus
        void SkillBoxFIGHTING_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Skill_List_fighting.Count > 0)
            {
                int index = SkillBoxFIGHTING.IndexFromPoint(e.X, e.Y);
                if (index >= 0 && index < SkillBoxFIGHTING.Items.Count)
                {
                    SkillBoxFIGHTING.SelectedIndex = index;
                }
            }
        }
        void SkillBoxSTART_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Skill_List_start.Count > 0)
            {
                int index = SkillBoxSTART.IndexFromPoint(e.X, e.Y);
                if (index >= 0 && index < SkillBoxSTART.Items.Count)
                {
                    SkillBoxSTART.SelectedIndex = index;
                }
            }
        }
        void SkillBoxEND_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Skill_List_end.Count > 0)
            {
                int index = SkillBoxEND.IndexFromPoint(e.X, e.Y);
                if (index >= 0 && index < SkillBoxEND.Items.Count)
                {
                    SkillBoxEND.SelectedIndex = index;
                }
            }
        }
        #endregion

        private void ChatlogBox_KeyPress (object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                Session.Windower.SendString(ChatlogBox.Text);
                ChatlogBox.Clear();
                // Remove annoying Beep
                e.Handled = true;
            }
        }

        private void button1_Click (object sender, EventArgs e)
        {
            //FormAdditionalSettings.TopMost = true;
            //FormAdditionalSettings.Show();
            SerializeDataToXML();
        }
        //-----------------------------------------------------------------------------------------------
        
#region Serialization of data
        // Reference: http://www.jonasjohn.de/snippets/csharp/xmlserializer-example.htm
        private void SerializeDataToXML()
        {
            DataSerialization datatoxml = new DataSerialization();

            // Not Needed
            //datatoxml.Settings.Add("BBB");
            //datatoxml.Settings.Add("CCC");

            XmlSerializer SerializerXML = new XmlSerializer(typeof (DataSerialization));
            TextWriter XMLOutputFile = new StreamWriter(@"settings.xml");
            SerializerXML.Serialize(XMLOutputFile,datatoxml);
            XMLOutputFile.Close();
            DeserializeXMLToData();

        }
        private bool DeserializeXMLToData()
        {
            if (!File.Exists(@"settings.xml"))
            {
                return false;
            }
            XmlSerializer SerializerXML = new XmlSerializer(typeof (DataSerialization));
            FileStream XMLDataToRead = new FileStream(@"settings.xml",FileMode.Open, FileAccess.Read,FileShare.Read);
            DataSerialization XMLData = (DataSerialization)SerializerXML.Deserialize(XMLDataToRead);
            //MessageBox.Show(XMLData.Settings.ElementAt(0));
            return true;
        }
#endregion

        private bool IsAbilityActive(StatusEffect ability)
        {
            StatusEffect[] statuseffects = Session.Player.StatusEffects;
            foreach (var statusEffect in statuseffects)
            {
                if (statusEffect == ability)
                    return true;
            }
            return false;
        }

        private bool IsSpell(string[] Skill)
        {
            if (Skill[0] == "/ma")
                return true;
            return false;

            //throw new NotImplementedException();
        }
        private void RestUntilMPTextBox_TextChanged (object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt16(RestUntilMPTextBox.Text) > 0)
                {
                    RestUntil_MP = Convert.ToInt16(RestUntilMPTextBox.Text);
                    RestBelowMPCheckbox.Checked = true;
                }
            }
            catch (Exception exc)
            {
                RestUntilMPTextBox.Text = "95";
                RestBelowMPCheckbox.Checked = false;
                DebugBox.AppendText("Error: " + exc.Message + Environment.NewLine);
            }
        }

        private void RestBelowMPTextbox_TextChanged (object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt16(RestBelowMPTextbox.Text) > 0)
                {
                    Rest_MP = Convert.ToInt16(RestBelowMPTextbox.Text);
                    RestBelowMPCheckbox.Checked = true;
                }
            }
            catch (Exception exc)
            {
                RestBelowMPTextbox.Text = "95";
                RestBelowMPCheckbox.Checked = false;
                DebugBox.AppendText("Error: " + exc.Message + Environment.NewLine);
            }
        }

        private void RestUntilHPPTextbox_TextChanged (object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt16(RestUntilHPPTextbox.Text) <= 100 && Convert.ToInt16(RestUntilHPPTextbox.Text) > 0)
                {
                    RestUntil_HPP = Convert.ToInt16(RestUntilHPPTextbox.Text);
                    RestBelowHPCheckbox.Checked = true;
                }
            }
            catch (Exception exc)
            {
                RestUntilHPPTextbox.Text = "95";
                RestBelowHPCheckbox.Checked = false;
                DebugBox.AppendText("Error: " + exc.Message + Environment.NewLine);
            }
        }

        private void RestBelowTextbox_TextChanged (object sender, EventArgs e)
        {
            try
            {
                Rest_HPP = Convert.ToInt16(RestBelowTextbox.Text);
                RestBelowHPCheckbox.Checked = true;
            }
            catch (Exception exc)
            {
                RestBelowTextbox.Text = "80";
                RestBelowHPCheckbox.Checked = false;
                DebugBox.AppendText("Error: " + exc.Message + Environment.NewLine);
            }
        }

        private void RestAtEnd_CheckedChanged (object sender, EventArgs e)
        {
            // Set focus to tab 6 (Adv Settings)
            if (RestAtEnd.Checked)
            {
                RestUntilMPTextBox.Text = Session.Player.MPMax.ToString();
                LevellingTabs.SelectedIndex = 5;
            }
        }

        private void MoveItemsToSatchel()
        {
            
        }
    }
}


/* TODO: add temp nav path so you can return to original path detour when you do not have mobs to kill 
 * then continue the route. If it's fight on the spot, return to the last mob killed location
 * TODO: Add option to have bot perform action if timeout is reached
 * TODO: Add SetTarget
 * TODO: Ability buff self with spells/abilities
 * TODO: Limit range of detection
 * TODO: Loop Waypoints
 * TODO: Add PL Server ID so it mob is aggro to them it should attack it
 * TODO: Add Haste/refresh/SCH abilities
 * TODO: Fix GUI not responding when stuck "Seem to be stuck? ... Moving LEFT"
 * TODO: Create timer for status effect checks ever 1000ms
 * TODO:
 * TODO: 
 * TODO: 
 * TODO: 
 * TODO: 
 * TODO: 
 * TODO: 
 * TODO: 
 * TODO: 
 * TODO: 
 * TODO: 
 * TODO: 
 */