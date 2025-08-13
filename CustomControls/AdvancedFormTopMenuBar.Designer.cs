namespace EasyWinFormLibrary.CustomControls
{
    partial class AdvancedFormTopMenuBar
    {

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TopMenuPanel = new EasyWinFormLibrary.CustomControls.AdvancedPanel();
            this.tblpForm = new System.Windows.Forms.TableLayoutPanel();
            this.lblFormTitle = new System.Windows.Forms.Label();
            this.btnClose = new EasyWinFormLibrary.CustomControls.AdvancedButton();
            this.btnMinimize = new EasyWinFormLibrary.CustomControls.AdvancedButton();
            this.TopMenuPanel.SuspendLayout();
            this.tblpForm.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopMenuPanel
            // 
            this.TopMenuPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(121)))), ((int)(((byte)(140)))));
            this.TopMenuPanel.BorderThickness = 0;
            this.TopMenuPanel.Controls.Add(this.tblpForm);
            this.TopMenuPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TopMenuPanel.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.TopMenuPanel.GradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(124)))), ((int)(((byte)(169)))));
            this.TopMenuPanel.GradientStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(124)))), ((int)(((byte)(169)))));
            this.TopMenuPanel.Location = new System.Drawing.Point(0, 0);
            this.TopMenuPanel.Name = "TopMenuPanel";
            this.TopMenuPanel.Size = new System.Drawing.Size(864, 47);
            this.TopMenuPanel.TabIndex = 0;
            // 
            // tblpForm
            // 
            this.tblpForm.BackColor = System.Drawing.Color.Transparent;
            this.tblpForm.ColumnCount = 3;
            this.tblpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tblpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tblpForm.Controls.Add(this.lblFormTitle, 0, 0);
            this.tblpForm.Controls.Add(this.btnClose, 2, 0);
            this.tblpForm.Controls.Add(this.btnMinimize, 1, 0);
            this.tblpForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblpForm.Location = new System.Drawing.Point(0, 0);
            this.tblpForm.Margin = new System.Windows.Forms.Padding(2);
            this.tblpForm.Name = "tblpForm";
            this.tblpForm.RowCount = 1;
            this.tblpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tblpForm.Size = new System.Drawing.Size(864, 47);
            this.tblpForm.TabIndex = 1;
            // 
            // lblFormTitle
            // 
            this.lblFormTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFormTitle.AutoSize = true;
            this.lblFormTitle.Font = new System.Drawing.Font("Rabar_013", 12F, System.Drawing.FontStyle.Bold);
            this.lblFormTitle.ForeColor = System.Drawing.Color.White;
            this.lblFormTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblFormTitle.Location = new System.Drawing.Point(2, 3);
            this.lblFormTitle.Margin = new System.Windows.Forms.Padding(2, 3, 2, 0);
            this.lblFormTitle.Name = "lblFormTitle";
            this.lblFormTitle.Size = new System.Drawing.Size(54, 44);
            this.lblFormTitle.TabIndex = 4;
            this.lblFormTitle.Text = "Title";
            this.lblFormTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(124)))), ((int)(((byte)(169)))));
            this.btnClose.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(121)))), ((int)(((byte)(140)))));
            this.btnClose.BorderColor = System.Drawing.Color.Gray;
            this.btnClose.BorderRadius = 0;
            this.btnClose.BorderSize = 0;
            this.btnClose.ButtonImage = global::EasyWinFormLibrary.Properties.Resources.multiply_32px;
            this.btnClose.ButtonImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.HoverBackColor = System.Drawing.Color.RosyBrown;
            this.btnClose.HoverBorderColor = System.Drawing.Color.RosyBrown;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnClose.ImageSize = new System.Drawing.Size(25, 25);
            this.btnClose.ImageTextSpacing = 5;
            this.btnClose.Location = new System.Drawing.Point(817, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.PressedBackColor = System.Drawing.Color.IndianRed;
            this.btnClose.Size = new System.Drawing.Size(44, 40);
            this.btnClose.TabIndex = 5;
            this.btnClose.TextColor = System.Drawing.Color.White;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnCloseForm_Click);
            // 
            // btnMinimize
            // 
            this.btnMinimize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(124)))), ((int)(((byte)(169)))));
            this.btnMinimize.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(121)))), ((int)(((byte)(140)))));
            this.btnMinimize.BorderColor = System.Drawing.Color.Gray;
            this.btnMinimize.BorderRadius = 0;
            this.btnMinimize.BorderSize = 0;
            this.btnMinimize.ButtonImage = global::EasyWinFormLibrary.Properties.Resources.minus_24px;
            this.btnMinimize.ButtonImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.ForeColor = System.Drawing.Color.White;
            this.btnMinimize.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(121)))), ((int)(((byte)(140)))));
            this.btnMinimize.HoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(121)))), ((int)(((byte)(140)))));
            this.btnMinimize.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnMinimize.ImageSize = new System.Drawing.Size(25, 25);
            this.btnMinimize.ImageTextSpacing = 5;
            this.btnMinimize.Location = new System.Drawing.Point(767, 3);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.PressedBackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnMinimize.Size = new System.Drawing.Size(44, 40);
            this.btnMinimize.TabIndex = 5;
            this.btnMinimize.TextColor = System.Drawing.Color.White;
            this.btnMinimize.UseVisualStyleBackColor = false;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // AdvancedFormTopMenuBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(121)))), ((int)(((byte)(140)))));
            this.Controls.Add(this.TopMenuPanel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Rabar_013", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AdvancedFormTopMenuBar";
            this.Size = new System.Drawing.Size(864, 47);
            this.TopMenuPanel.ResumeLayout(false);
            this.tblpForm.ResumeLayout(false);
            this.tblpForm.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private AdvancedPanel TopMenuPanel;
        private System.Windows.Forms.TableLayoutPanel tblpForm;
        private System.Windows.Forms.Label lblFormTitle;
        private AdvancedButton btnClose;
        private AdvancedButton btnMinimize;
    }
}
