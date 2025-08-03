
namespace EasyWinFormLibrary.CustomControls
{
    partial class AdvancedAlertForm
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblMessage = new System.Windows.Forms.TextBox();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.LblClose = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LblClose)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Controls.Add(this.lblMessage, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LblClose, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(549, 93);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblMessage
            // 
            this.lblMessage.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.lblMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Font = new System.Drawing.Font("Rabar_013", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.Color.White;
            this.lblMessage.Location = new System.Drawing.Point(99, 10);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(3, 10, 10, 10);
            this.lblMessage.Multiline = true;
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.ReadOnly = true;
            this.lblMessage.Size = new System.Drawing.Size(400, 73);
            this.lblMessage.TabIndex = 5;
            this.lblMessage.TabStop = false;
            this.lblMessage.Text = "Successfully Inserted";
            // 
            // pictureBox
            // 
            this.pictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(10, 10);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(10);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(76, 73);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // LblClose
            // 
            this.LblClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LblClose.Image = global::EasyWinFormLibrary.Properties.Resources.multiply_32px;
            this.LblClose.Location = new System.Drawing.Point(514, 3);
            this.LblClose.Name = "LblClose";
            this.LblClose.Size = new System.Drawing.Size(32, 33);
            this.LblClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.LblClose.TabIndex = 0;
            this.LblClose.TabStop = false;
            this.LblClose.Click += new System.EventHandler(this.LblClose_Click);
            // 
            // AdvancedAlertForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.ClientSize = new System.Drawing.Size(549, 93);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Rabar_013", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AdvancedAlertForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.AdvancedAlertForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LblClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox LblClose;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.TextBox lblMessage;
    }
}