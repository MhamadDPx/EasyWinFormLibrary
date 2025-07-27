using EasyWinFormLibrary.WinAppNeeds;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    public partial class FormTopMenuBar : UserControl
    {
        #region Private Fields
        private string _englishTitle = string.Empty;
        private string _kurdishTitle = string.Empty;
        private string _arabicTitle = string.Empty;
        private Color _backgroundColor = Color.FromArgb(66, 124, 169);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether to show the minimize button
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Shows or hides the minimize button")]
        [DefaultValue(true)]
        public bool ShowMinimizeButton
        {
            get { return btnMinimize.Visible; }
            set { btnMinimize.Visible = value; this.Invalidate(); }
        }

        /// <summary>
        /// Select Top Menu Bar Height
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Top Menu Bar Height")]
        [DefaultValue(true)]
        public int TopMenuBarHeight
        {
            get { return Height; }
            set
            {
                this.Height = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the form title (English)
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The title of the form (English en-001)")]
        public string FormTitlEnglish
        {
            get { return _englishTitle; }
            set
            {
                _englishTitle = value;
                if (LanguageManager.SelectedLanguage == FormLanguage.English)
                {
                    lblFormTitle.Text = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the form title (Kurdish)
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The title of the form (Kurdish en-US)")]
        public string FormTitlKurdish
        {
            get { return _kurdishTitle; }
            set
            {
                _kurdishTitle = value;
                if (LanguageManager.SelectedLanguage == FormLanguage.Kurdish)
                {
                    lblFormTitle.Text = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the form title (Arabic)
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The title of the form (English en-GB)")]
        public string FormTitlArabic
        {
            get { return _arabicTitle; }
            set
            {
                _arabicTitle = value;
                if (LanguageManager.SelectedLanguage == FormLanguage.Arabic)
                {
                    lblFormTitle.Text = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets Top Menu Bar background color
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Top Menu Bar background color")]
        [DefaultValue(true)]
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                TopMenuPanel.GradientEndColor = TopMenuPanel.GradientStartColor = btnClose.BackgroundColor = btnMinimize.BackgroundColor = value;
                this.Invalidate();
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the AdvancedForm class
        /// </summary>
        public FormTopMenuBar()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles minimize button click
        /// </summary>
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.FindForm().WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// Handles close button click
        /// </summary>
        private void btnCloseForm_Click(object sender, EventArgs e)
        {
            this.FindForm().DialogResult = DialogResult.Cancel;
            this.FindForm().Close();
        }
        #endregion
    }
}
