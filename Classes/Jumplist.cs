#region

using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Shell;
using IV_Play.Properties;
using IV_Play.Data;
using IV_Play.Data.Models;

#endregion

namespace IV_Play
{

    /// <summary>
    /// Class to load our jumplist items
    /// What were doing here is a BIG workaround to how jumplists work.
    /// </summary>
    internal class JumpListClass
    {
        private JumpList jumpList;

        /// <summary>
        /// Adds a JumpTask and if needed creates the JumpList
        /// </summary>
        /// <param name="game"></param>
        public void AddTask(Game game)
        {
            try
            {
                Game g = game.Copy();
                g.Name = g.Name.Replace("fav_", "");
                // Configure a new JumpTask.
                JumpTask jumpTask1 = CreateJumpTask(g);

                // Get the JumpList from the application and update it.                                 
                if (jumpList == null)
                    LoadJumpList();


                if (!jumpList.JumpItems.Exists(j => ((JumpTask) j).Title == g.Description))
                {
                    jumpList.JumpItems.Insert(0, jumpTask1);
                    SettingsManager.AddGameToJumpList(g.Name);
                }


                jumpList.Apply();
            }
            catch (Exception)
            {
                //No jump list, we're on XP/Vista
            }
        }

        /// <summary>
        /// Creates a JumpTask
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private JumpTask CreateJumpTask(Game game)
        {
            JumpTask jumpTask1 = new JumpTask();
            jumpTask1.ApplicationPath = Settings.Default.MAME_EXE;
            jumpTask1.WorkingDirectory = Path.GetDirectoryName(Settings.Default.MAME_EXE);
            jumpTask1.Arguments = game.Name;

            string iconPath = game.IsParent
                                  ? Settings.Default.icons_directory + game.Name + ".ico"
                                  : Settings.Default.icons_directory + game.ParentSet + ".ico";
            if (!File.Exists(iconPath))
                jumpTask1.IconResourcePath = Application.ExecutablePath;
            else
                jumpTask1.IconResourcePath = iconPath;
            jumpTask1.Title = game.Description;
            jumpTask1.Description = game.Year + " " + game.Manufacturer;
            jumpTask1.CustomCategory = "Recently Played Games";


            return jumpTask1;
        }


        /// <summary>
        /// Creates the jumplist
        /// </summary>
        private void LoadJumpList()
        {
            // Get the JumpList from the application and update it.                        
            jumpList = new JumpList();
            jumpList.ShowRecentCategory = false;

            JumpList.SetJumpList(System.Windows.Application.Current, jumpList);

            using (var dbm = new DatabaseManager())
            {
                foreach (string s in Settings.Default.jumplist.Split(','))
                {
                    Machine machine = dbm.GetMachineByName(s);
                    if (machine != null)
                    {
                        jumpList.JumpItems.Add(CreateJumpTask(new Game(machine)));
                    }
                }
            }
               
            jumpList.Apply();
        }
    }
}