using System.Diagnostics;

namespace MixerMixer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Settings1.Default.Profiles ??= new[] { new Profile() { Name = "Default" } };

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
            SetVolume.TestSetVolume("Mozilla Firefox", trackBar1.Value * 10.0f);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // update trackbar to current volume
            float? vol = SetVolume.TestGetVolume("Mozilla Firefox");
            if (vol is null)
                return;
            Debug.WriteLine("actual volume: {0}", vol);
            trackBar1.Value = (int) (vol/ 10);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach ((string name, string icon) in SetVolume.EnumerateApplications())
                Debug.WriteLine("app + {0}; {1}", name, icon);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (string name in SetVolume.EnumerateApplicationsPath())
                Debug.WriteLine("app + {0}", new object[]{name});
        }
    }
}