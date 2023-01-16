using System.Diagnostics;

namespace MixerMixer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Settings1.Default.Profiles ??= new[] { new Profile() { Name = "Default" } };

            /*
            var lst = Settings1.Default.Profiles.ToList();
            ProfileListBox.DataSource = lst;
            ProfileListBox.DisplayMember = nameof(Profile.Name);
            Process[] processlist = Process.GetProcesses();
            foreach (Process theprocess in processlist)
            {
                Debug.WriteLine("Process: {0} ID: {1}", theprocess.ProcessName, theprocess.Id);
                Debug.WriteLine(FileVersionInfo.GetVersionInfo(theprocess.MainModule.FileName).FileDescription);
            }*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetVolume.TestMute(null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetVolume.TestUnmute(null);
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            SetVolume.TestSetVolume(trackBar1.Value * 10.0f);
        }
    }
}