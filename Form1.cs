namespace MixerMixer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Settings1.Default.Profiles ??= new[] { new Profile() { Name = "Default" } };

            var lst = Settings1.Default.Profiles.ToList();
            ProfileListBox.DataSource = lst;
            ProfileListBox.DisplayMember = nameof(Profile.Name);

        }
    }
}