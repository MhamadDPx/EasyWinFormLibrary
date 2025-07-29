using EasyWinFormLibrary.WinAppNeeds;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    public partial class AdvancedAlertForm : Form
    {
        public Image MessageIcon
        {
            get { return pictureBox.Image; }
            set { pictureBox.Image = value; }
        }
        public string Message
        {
            get { return lblMessage.Text; }
            set { lblMessage.Text = value; }
        }
        public int CountDown = 3;

        public AdvancedAlertForm()
        {
            InitializeComponent();
            this.Left = (Screen.PrimaryScreen.Bounds.Width / 2) - 180;
            this.Top = Screen.PrimaryScreen.Bounds.Top + 10;
            this.TopMost = true;
        }

        private async void AdvancedAlertForm_Load(object sender, EventArgs e)
        {
            LblClose.Focus();

            // Use TaskDelayNamed with a unique name for each alert instance
            string uniqueTaskName = $"AlertClose_{this.GetHashCode()}_{DateTime.Now.Ticks}";

            await TaskDelayUtils.TaskDelayNamed(uniqueTaskName, () =>
            {
                if (!this.IsDisposed && !this.Disposing)
                {
                    this.Close();
                }
            }, CountDown * 1000);
        }

        private void LblClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
