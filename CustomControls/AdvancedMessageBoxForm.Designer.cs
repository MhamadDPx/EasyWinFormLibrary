namespace EasyWinFormLibrary.CustomControls
{
    partial class AdvancedMessageBoxForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedMessageBoxForm));
            this.lblCaption = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.btnOK = new EasyWinFormLibrary.CustomControls.AdvancedButton();
            this.lblMessage = new System.Windows.Forms.TextBox();
            this.btnNo = new EasyWinFormLibrary.CustomControls.AdvancedButton();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCaption
            // 
            this.lblCaption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(118)))), ((int)(((byte)(182)))));
            resources.ApplyResources(this.lblCaption, "lblCaption");
            this.lblCaption.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblCaption.ForeColor = System.Drawing.Color.White;
            this.lblCaption.Name = "lblCaption";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.pictureBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnOK, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblMessage, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnNo, 2, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // pictureBox
            // 
            resources.ApplyResources(this.pictureBox, "pictureBox");
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.TabStop = false;
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(136)))), ((int)(((byte)(77)))));
            this.btnOK.BorderColor = System.Drawing.Color.Gray;
            this.btnOK.BorderRadius = 5;
            this.btnOK.BorderSize = 0;
            this.btnOK.ButtonImage = null;
            this.btnOK.ButtonImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(136)))), ((int)(((byte)(77)))));
            this.btnOK.HoverBorderColor = System.Drawing.Color.MediumSlateBlue;
            this.btnOK.ImageSize = new System.Drawing.Size(16, 16);
            this.btnOK.ImageTextSpacing = 5;
            this.btnOK.Name = "btnOK";
            this.btnOK.PressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(136)))), ((int)(((byte)(77)))));
            this.btnOK.TextColor = System.Drawing.Color.White;
            this.btnOK.UseVisualStyleBackColor = false;
            // 
            // lblMessage
            // 
            resources.ApplyResources(this.lblMessage, "lblMessage");
            this.lblMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel1.SetColumnSpan(this.lblMessage, 2);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.TabStop = false;
            // 
            // btnNo
            // 
            resources.ApplyResources(this.btnNo, "btnNo");
            this.btnNo.BackColor = System.Drawing.Color.Transparent;
            this.btnNo.BackgroundColor = System.Drawing.Color.IndianRed;
            this.btnNo.BorderColor = System.Drawing.Color.Gray;
            this.btnNo.BorderRadius = 5;
            this.btnNo.BorderSize = 0;
            this.btnNo.ButtonImage = null;
            this.btnNo.ButtonImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnNo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnNo.ForeColor = System.Drawing.Color.White;
            this.btnNo.HoverBackColor = System.Drawing.Color.IndianRed;
            this.btnNo.HoverBorderColor = System.Drawing.Color.MediumSlateBlue;
            this.btnNo.ImageSize = new System.Drawing.Size(16, 16);
            this.btnNo.ImageTextSpacing = 5;
            this.btnNo.Name = "btnNo";
            this.btnNo.PressedBackColor = System.Drawing.Color.IndianRed;
            this.btnNo.TextColor = System.Drawing.Color.White;
            this.btnNo.UseVisualStyleBackColor = false;
            // 
            // AdvancedMessageBoxForm
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnNo;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblCaption);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AdvancedMessageBoxForm";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBox;
        private AdvancedButton btnOK;
        private System.Windows.Forms.TextBox lblMessage;
        private AdvancedButton btnNo;
    }
}