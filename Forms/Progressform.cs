#region

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Shell;
using Application = System.Windows.Application;

#endregion

namespace IV_Play
{
    /// <summary>
    /// Classic Progress form. NOT IN USE AS WERE USING PROGRESSWPF
    /// </summary>
    public partial class ProgressForm : Form
    {
        private const int CP_NOCLOSE_BUTTON = 0x200;
        private int i;

        public ProgressForm()
        {
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            _lblProgress.Invoke((MethodInvoker) delegate { _lblProgress.Text = "Building XML data from MAME"; });
            XmlParser.MakeDat();

            _lblProgress.Invoke((MethodInvoker) delegate { _lblProgress.Text = "Generating Game list"; });
            XmlParser.ReadDat();
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _lblTime.Text = string.Format("Elapsed time: {0} Seconds", ++i);
        }

        private void ProgressForm_Shown(object sender, EventArgs e)
        {
            TaskbarItemInfo taskbarItemInfo = new TaskbarItemInfo();
            taskbarItemInfo.Description = "test";
            Application.Current.MainWindow.TaskbarItemInfo = taskbarItemInfo;
            taskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            timer1.Start();
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer1.Stop();
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            backgroundWorker1.Dispose();
        }
    }
}
