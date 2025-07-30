using EasyWinFormLibrary.WinAppNeeds;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// A custom user control that displays metadata such as entry and update information,
    /// localized according to the selected application language.
    /// </summary>
    public partial class AdvancedUserMetaLog : UserControl
    {
        #region Private Fields

        private string entryUserTitle;
        private string entryDateTitle;
        private string updateUserTitle;
        private string updateDateTitle;

        #endregion

        #region Public Properties

        /// <summary>
        /// Sets the display text for the user who created the entry.
        /// </summary>
        public string EntryUserText
        {
            set { lblEntryBy.Text = value; }
        }

        /// <summary>
        /// Sets the display date for when the entry was created.
        /// </summary>
        public string EntryDateText
        {
            set { lblEntryDate.Text = value; }
        }

        /// <summary>
        /// Sets the display text for the user who last updated the entry.
        /// </summary>
        public string UpdateUserText
        {
            set { lblUpdateBy.Text = value; }
        }

        /// <summary>
        /// Sets the display date for the last update.
        /// </summary>
        public string UpdateDateText
        {
            set { lblUpdateDate.Text = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedUserMetaLog"/> class,
        /// sets localized label titles and layout direction.
        /// </summary>
        public AdvancedUserMetaLog()
        {
            InitializeComponent();
            this.Size = new Size(650, 60);

            // Set localized titles based on current language
            switch (LanguageManager.SelectedLanguage)
            {
                case FormLanguage.English:
                    entryUserTitle = "Entry By :";
                    entryDateTitle = "On :";
                    updateUserTitle = "Updated By :";
                    updateDateTitle = "On :";
                    break;
                case FormLanguage.Arabic:
                    entryUserTitle = "المستورد :";
                    entryDateTitle = "في :";
                    updateUserTitle = "المحدث :";
                    updateDateTitle = "في :";
                    break;
                case FormLanguage.Kurdish:
                    entryUserTitle = "داخڵکار :";
                    entryDateTitle = "لە :";
                    updateUserTitle = "نوێکەرەوە :";
                    updateDateTitle = "لە :";
                    break;
            }

            // Set labels with titles
            lblEntryByTitle.Text = entryUserTitle;
            lblEntryDateTitle.Text = entryDateTitle;
            lblUpdateByTitle.Text = updateUserTitle;
            lblUpdateDateTitle.Text = updateDateTitle;

            // Adjust the width of the first column in the table layout
            using (Graphics graphics = Graphics.FromImage(new Bitmap(1, 1)))
            {
                float entryWidth = graphics.MeasureString(lblEntryByTitle.Text, lblEntryByTitle.Font).Width + 10;
                float updateWidth = graphics.MeasureString(lblUpdateByTitle.Text, lblUpdateByTitle.Font).Width + 10;
                tableLayoutPanel1.ColumnStyles[0].Width = Math.Max(entryWidth, updateWidth);
            }

            // Set layout direction based on selected language
            this.RightToLeft = tableLayoutPanel1.RightToLeft =
                LanguageManager.SelectedLanguage == FormLanguage.English ? RightToLeft.No : RightToLeft.Yes;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears all user and date labels.
        /// </summary>
        public void ClearText()
        {
            lblEntryBy.Text = lblEntryDate.Text = lblUpdateBy.Text = lblUpdateDate.Text = "-";
        }

        #endregion
    }
}
