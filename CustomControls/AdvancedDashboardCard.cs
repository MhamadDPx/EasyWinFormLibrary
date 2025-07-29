using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    public partial class AdvancedDashboardCard : UserControl
    {
        [Category("Advanced Appearance")]
        public RightToLeft CardRightToLeft
        {
            get { return this.RightToLeft; }
            set { RightToLeft = value; this.Invalidate(); }
        }

        [Category("Advanced Appearance")]
        public int CardRadius
        {
            get { return MainPanelElipse.ElipseRadius; }
            set { MainPanelElipse.ElipseRadius = value; this.Invalidate(); }
        }
        [Category("Advanced Appearance")]
        public Color CardFillColor1
        {
            get { return MainPanel.GradientStartColor; }
            set { MainPanel.GradientStartColor = value; this.Invalidate(); }
        }
        [Category("Advanced Appearance")]
        public Color CardFillColor2
        {
            get { return MainPanel.GradientEndColor; }
            set { MainPanel.GradientEndColor = value; this.Invalidate(); }
        }
        [Category("Advanced Appearance")]
        public Color CardTitleTextColor
        {
            get { return lblTitle.ForeColor; }
            set { lblTitle.ForeColor = value; this.Invalidate(); }
        }
        [Category("Advanced Appearance")]
        public Color CardTitleValueColor
        {
            get { return lblValue.ForeColor; }
            set { lblValue.ForeColor = value; this.Invalidate(); }
        }
        [Category("Advanced Appearance")]
        public bool CardShowIcon
        {
            get { return picCardIcon.Visible; }
            set
            {
                picCardIcon.Visible = value;
                if (value)
                    tableLayoutPanel1.ColumnStyles[1].Width = 100;
                else
                    tableLayoutPanel1.ColumnStyles[1].Width = 1;

                this.Invalidate();
            }
        }
        [Category("Advanced Appearance")]
        public string CardTitleText
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; this.Invalidate(); }
        }
        [Category("Advanced Appearance")]
        public string CardValue
        {
            get { return lblValue.Text; }
            set { lblValue.Text = value; this.Invalidate(); }
        }
        [Category("Advanced Appearance")]
        public Image CardIcon
        {
            get { return picCardIcon.Image; }
            set { picCardIcon.Image = value; this.Invalidate(); }
        }

        public AdvancedDashboardCard()
        {
            InitializeComponent();
        }
    }
}
