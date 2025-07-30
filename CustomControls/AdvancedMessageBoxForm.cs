using EasyWinFormLibrary.WinAppNeeds;
using System.Drawing;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// A customizable message box form that supports multiple languages (English, Kurdish, Arabic)
    /// and provides options for both OK/Cancel and Yes/No button configurations.
    /// Features right-to-left layout support for Arabic and Kurdish languages.
    /// </summary>
    public partial class AdvancedMessageBoxForm : Form
    {
        #region Constants
        /// <summary>
        /// Default background color for the message box title.
        /// </summary>
        private static readonly Color DEFAULT_TITLE_COLOR = Color.FromArgb(122, 121, 140); // Default blue color

        #endregion

        #region Private Fields
        private bool _showYesOrNo = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets whether to display Yes/No buttons instead of OK/Cancel buttons.
        /// When set to true, shows Yes/No buttons. When false, shows only OK button.
        /// </summary>
        public bool ShowYesOrNo
        {
            get => _showYesOrNo;
            set
            {
                if (_showYesOrNo != value)
                {
                    _showYesOrNo = value;
                    ApplyButtonChanges();
                }
            }
        }

        /// <summary>
        /// Gets or sets the icon image displayed in the message box.
        /// </summary>
        public Image MessageIcon
        {
            get => pictureBox.Image;
            set => pictureBox.Image = value;
        }

        /// <summary>
        /// Gets or sets the main message text displayed in the message box.
        /// </summary>
        public string Message
        {
            get => lblMessage.Text;
            set => lblMessage.Text = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the caption/title text displayed in the message box header.
        /// </summary>
        public string Caption
        {
            get => lblCaption.Text;
            set => lblCaption.Text = value ?? string.Empty;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the AdvancedMessageBoxForm class.
        /// Sets up the form components and applies the custom title color from library settings.
        /// </summary>
        public AdvancedMessageBoxForm()
        {
            InitializeComponent();
            InitializeForm();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes form-specific settings and applies default styling.
        /// </summary>
        private void InitializeForm()
        {
            lblCaption.BackColor = DEFAULT_TITLE_COLOR;
            ApplyButtonChanges();
        }

        /// <summary>
        /// Applies language-specific button text and layout changes based on the current language setting.
        /// Handles right-to-left layout for Arabic and Kurdish languages.
        /// Configures button visibility and text based on ShowYesOrNo property.
        /// </summary>
        private void ApplyButtonChanges()
        {
            ConfigureLayoutDirection();
            SetButtonTexts();
            ConfigureButtonVisibility();
        }

        /// <summary>
        /// Configures the form's layout direction based on the selected language.
        /// </summary>
        private void ConfigureLayoutDirection()
        {
            bool isEnglish = LanguageManager.SelectedLanguage == FormLanguage.English;
            this.RightToLeft = isEnglish ? RightToLeft.No : RightToLeft.Yes;
            this.RightToLeftLayout = !isEnglish;
        }

        /// <summary>
        /// Sets the appropriate button texts based on the current language and button mode.
        /// </summary>
        private void SetButtonTexts()
        {
            var buttonTexts = GetButtonTexts();
            btnOK.Text = _showYesOrNo ? buttonTexts.Yes : buttonTexts.OK;
            btnNo.Text = buttonTexts.No;
        }

        /// <summary>
        /// Gets the localized button texts for the current language.
        /// </summary>
        /// <returns>A tuple containing the localized button texts.</returns>
        private (string OK, string Yes, string No) GetButtonTexts()
        {
            if (LanguageManager.SelectedLanguage == FormLanguage.English)
            {
                return ("OK", "Yes", "No");
            }
            else if (LanguageManager.SelectedLanguage == FormLanguage.Kurdish)
            {
                return ("باشه", "بەڵێ", "نەخێر");
            }
            else if (LanguageManager.SelectedLanguage == FormLanguage.Arabic)
            {
                return ("موافق", "نعم", "لا");
            }
            else
            {
                return ("OK", "Yes", "No"); // Default fallback
            }
        }

        /// <summary>
        /// Configures the visibility and layout of buttons based on the ShowYesOrNo property.
        /// </summary>
        private void ConfigureButtonVisibility()
        {
            if (!_showYesOrNo)
            {
                btnNo.Visible = false;
                // Hide the column containing the No button
                if (tableLayoutPanel1.ColumnStyles.Count > 2)
                {
                    tableLayoutPanel1.ColumnStyles[2].Width = 0;
                }
            }
            else
            {
                btnNo.Visible = true;
                // Restore the column width if previously hidden
                if (tableLayoutPanel1.ColumnStyles.Count > 2)
                {
                    tableLayoutPanel1.ColumnStyles[2].Width = 100; // Adjust as needed
                }
            }
        }
        #endregion
    }
}