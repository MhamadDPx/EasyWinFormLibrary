using EasyWinFormLibrary.WinAppNeeds;
using System.Drawing;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    public partial class AdvancedMessageBoxForm : Form
    {
        private bool _showYesOrNo = false;
        public bool ShowYesOrNo
        {
            get { return _showYesOrNo; }
            set
            {
                _showYesOrNo = value;
                ApplyButtonChanges();
            }
        }
        public AdvancedMessageBoxForm()
        {
            InitializeComponent();
            lblCaption.BackColor = LibrarySettings.CustomMessageBoxTitleColor;
        }
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
        public string Caption
        {
            get { return lblCaption.Text; }
            set { lblCaption.Text = value; }
        }
        private void ApplyButtonChanges()
        {
            this.RightToLeft = LanguageManager.SelectedLanguage == FormLanguage.English ? RightToLeft.No : RightToLeft.Yes;
            this.RightToLeftLayout = LanguageManager.SelectedLanguage != FormLanguage.English;

            switch (LanguageManager.SelectedLanguage)
            {
                case FormLanguage.English:
                    btnOK.Text = _showYesOrNo ? "Yes" : "OK";
                    btnNo.Text = "No";
                    break;
                case FormLanguage.Kurdish:
                    btnOK.Text = _showYesOrNo ? "بەڵێ" : "باشه";
                    btnNo.Text = "نەخێر";
                    break;
                case FormLanguage.Arabic:
                    btnOK.Text = _showYesOrNo ? "نعم" : "موافق";
                    btnNo.Text = "لا";
                    break;
            }
            if (!_showYesOrNo)
            {
                btnNo.Visible = false;
                tableLayoutPanel1.ColumnStyles[2].Width = 0;
            }
        }
    }
}
