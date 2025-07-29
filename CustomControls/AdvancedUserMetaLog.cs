using EasyWinFormLibrary.WinAppNeeds;
using System.Drawing;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    public partial class AdvancedUserMetaLog : UserControl
    {
        private string entryUserTitle;
        private string entryDateTitle;

        private string updateUserTitle;
        private string updateDateTitle;

        public string EntryUserText
        {
            set { lblEntryBy.Text = value; }
        }
        public string EntryDateText
        {
            set { lblEntryDate.Text = value; }
        }

        public string UpdateUserText
        {
            set { lblUpdateBy.Text = value; }
        }
        public string UpdateDateText
        {
            set { lblUpdateDate.Text = value; }
        }

        public AdvancedUserMetaLog()
        {
            InitializeComponent();

            this.Size = new Size(650, 60);

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

            lblEntryByTitle.Text = entryUserTitle;
            lblEntryDateTitle.Text = entryDateTitle;

            lblUpdateByTitle.Text = updateUserTitle;
            lblUpdateDateTitle.Text = updateDateTitle;

            using (Graphics graphics = Graphics.FromImage(new Bitmap(1, 1)))
            {
                float EntryTitleWidth = graphics.MeasureString(lblEntryByTitle.Text, lblEntryByTitle.Font).Width + 10;
                float UpdateTitleWidth = graphics.MeasureString(lblUpdateByTitle.Text, lblUpdateByTitle.Font).Width + 10;

                tableLayoutPanel1.ColumnStyles[0].Width = EntryTitleWidth > UpdateTitleWidth ? EntryTitleWidth : UpdateTitleWidth;
            }

            this.RightToLeft = tableLayoutPanel1.RightToLeft = LanguageManager.SelectedLanguage == FormLanguage.English ? RightToLeft.No : RightToLeft.Yes;
        }

        public void ClearText()
        {
            lblEntryBy.Text = lblEntryDate.Text = lblUpdateBy.Text = lblUpdateDate.Text = "-";
        }

    }
}
