#region

using IV_Play.DataAccess;
using IV_Play.Properties;
using IV_Play.ViewModel;
using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Shell;

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
        public void AddTask(MachineViewModel machine)
        {
            try
            {                
                // Configure a new JumpTask.
                JumpTask jumpTask1 = CreateJumpTask(machine);

                // Get the JumpList from the application and update it.                                 
                if (jumpList == null)
                    LoadJumpList();


                if (!jumpList.JumpItems.Exists(j => ((JumpTask)j).Title == machine.Description))
                {
                    jumpList.JumpItems.Insert(0, jumpTask1);
                    SettingsManager.AddGameToJumpList(machine.Name);
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
        private JumpTask CreateJumpTask(MachineViewModel machine)
        {
            JumpTask jumpTask1 = new JumpTask();
            jumpTask1.ApplicationPath = Settings.Default.MAME_EXE;
            jumpTask1.WorkingDirectory = Path.GetDirectoryName(Settings.Default.MAME_EXE);
            jumpTask1.Arguments = machine.Name;

            string iconPath = machine.CloneOf == null
                                  ? Settings.Default.icons_directory + machine.Name + ".ico"
                                  : Settings.Default.icons_directory + machine.CloneOf + ".ico";
            if (!File.Exists(iconPath))
                jumpTask1.IconResourcePath = Application.ExecutablePath;
            else
                jumpTask1.IconResourcePath = iconPath;
            jumpTask1.Title = machine.Description;
            jumpTask1.Description = machine.Year + " " + machine.Manufacturer;
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
            
            foreach (string s in Settings.Default.jumplist.Split(','))
            {
                MachineViewModel machineViewModel = new MachineViewModel(DatabaseManager.GetMachineByName(s));
                if (machineViewModel != null)
                {
                    jumpList.JumpItems.Add(CreateJumpTask(machineViewModel));
                }
            }

            jumpList.Apply();
        }
    }
}