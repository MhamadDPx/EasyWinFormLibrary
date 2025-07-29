
namespace EasyWinFormLibrary.CustomControls
{
    partial class AdvancedDashboardCard
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainPanel = new EasyWinFormLibrary.CustomControls.AdvancedPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblValue = new System.Windows.Forms.Label();
            this.picCardIcon = new System.Windows.Forms.PictureBox();
            this.MainPanelElipse = new EasyWinFormLibrary.CustomControls.AdvancedElipse();
            this.MainPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCardIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.BackColor = System.Drawing.Color.Transparent;
            this.MainPanel.BorderThickness = 0;
            this.MainPanel.Controls.Add(this.tableLayoutPanel1);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.MainPanel.GradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(126)))), ((int)(((byte)(125)))));
            this.MainPanel.GradientStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(190)))), ((int)(((byte)(188)))));
            this.MainPanel.HighlightColor = System.Drawing.Color.Empty;
            this.MainPanel.Location = new System.Drawing.Point(0, 0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(433, 193);
            this.MainPanel.TabIndex = 0;
            this.MainPanel.UseGradient = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 98F));
            this.tableLayoutPanel1.Controls.Add(this.lblTitle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblValue, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.picCardIcon, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(413, 173);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.SetColumnSpan(this.lblTitle, 2);
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Rabar_013", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(407, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Title";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblValue
            // 
            this.lblValue.BackColor = System.Drawing.Color.Transparent;
            this.lblValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblValue.Font = new System.Drawing.Font("Rabar_013", 16F, System.Drawing.FontStyle.Bold);
            this.lblValue.ForeColor = System.Drawing.Color.White;
            this.lblValue.Location = new System.Drawing.Point(3, 40);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(309, 133);
            this.lblValue.TabIndex = 2;
            this.lblValue.Text = "0";
            this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picCardIcon
            // 
            this.picCardIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picCardIcon.Location = new System.Drawing.Point(315, 40);
            this.picCardIcon.Margin = new System.Windows.Forms.Padding(0);
            this.picCardIcon.Name = "picCardIcon";
            this.picCardIcon.Size = new System.Drawing.Size(98, 133);
            this.picCardIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picCardIcon.TabIndex = 3;
            this.picCardIcon.TabStop = false;
            // 
            // MainPanelElipse
            // 
            this.MainPanelElipse.TargetControl = this;
            // 
            // AdvancedDashboardCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.MainPanel);
            this.Font = new System.Drawing.Font("Rabar_013", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AdvancedDashboardCard";
            this.Size = new System.Drawing.Size(433, 193);
            this.MainPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picCardIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AdvancedPanel MainPanel;
        private AdvancedElipse MainPanelElipse;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.PictureBox picCardIcon;
    }
}
