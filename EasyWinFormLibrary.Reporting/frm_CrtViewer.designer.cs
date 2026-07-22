
namespace EasyWinFormLibrary.CustomControls
{
    partial class frm_CrtViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_CrtViewer));
            this.panel1 = new System.Windows.Forms.Panel();
            this.crt_Viewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.formTopMenuBar1 = new EasyWinFormLibrary.CustomControls.AdvancedFormTopMenuBar();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.crt_Viewer);
            this.panel1.Controls.Add(this.formTopMenuBar1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // crt_Viewer
            // 
            this.crt_Viewer.ActiveViewIndex = -1;
            this.crt_Viewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crt_Viewer.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.crt_Viewer, "crt_Viewer");
            this.crt_Viewer.Name = "crt_Viewer";
            // 
            // formTopMenuBar1
            // 
            this.formTopMenuBar1.BackColor = System.Drawing.Color.White;
            this.formTopMenuBar1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(26)))), ((int)(((byte)(52)))));
            resources.ApplyResources(this.formTopMenuBar1, "formTopMenuBar1");
            this.formTopMenuBar1.FormTitlKurdish = "بینینی ڕاپۆرت";
            this.formTopMenuBar1.Name = "formTopMenuBar1";
            this.formTopMenuBar1.ShowMinimizeButton = false;
            this.formTopMenuBar1.TopMenuBarHeight = 40;
            // 
            // frm_CrtViewer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frm_CrtViewer";
            this.ShowIcon = false;
            this.Tag = "ڕاپۆرت";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        public CrystalDecisions.Windows.Forms.CrystalReportViewer crt_Viewer;
        private AdvancedFormTopMenuBar formTopMenuBar1;
    }
}