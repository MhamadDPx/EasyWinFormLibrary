namespace EasyWinFormLibrary.CustomControls
{
    partial class AdvancedAddDialogForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedAddDialogForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgvBackPanel = new EasyWinFormLibrary.CustomControls.AdvancedPanel();
            this.DGV = new EasyWinFormLibrary.CustomControls.AdvancedDataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbl = new System.Windows.Forms.Label();
            this.txt = new EasyWinFormLibrary.CustomControls.AdvancedTextBox();
            this.btnSave = new EasyWinFormLibrary.CustomControls.AdvancedActionButton();
            this.formTopMenuBar1 = new EasyWinFormLibrary.CustomControls.AdvancedFormTopMenuBar();
            this.dgvElipse = new EasyWinFormLibrary.CustomControls.AdvancedElipse();
            this.panel1.SuspendLayout();
            this.dgvBackPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.dgvBackPanel);
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(508, 615);
            this.panel1.TabIndex = 1;
            // 
            // dgvBackPanel
            // 
            this.dgvBackPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBackPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(121)))), ((int)(((byte)(140)))));
            this.dgvBackPanel.BorderRadius = 10;
            this.dgvBackPanel.BorderThickness = 0;
            this.dgvBackPanel.Controls.Add(this.DGV);
            this.dgvBackPanel.Location = new System.Drawing.Point(11, 142);
            this.dgvBackPanel.Name = "dgvBackPanel";
            this.dgvBackPanel.Size = new System.Drawing.Size(484, 460);
            this.dgvBackPanel.TabIndex = 1;
            // 
            // DGV
            // 
            this.DGV.AllowHideColumns = false;
            this.DGV.AllowUserToAddRows = false;
            this.DGV.AllowUserToDeleteRows = false;
            this.DGV.AllowUserToOrderColumns = true;
            this.DGV.AllowUserToResizeRows = false;
            this.DGV.AlternatingRowColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.DGV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DGV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DGV.BackgroundColor = System.Drawing.Color.White;
            this.DGV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DGV.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(121)))), ((int)(((byte)(140)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Rabar_013", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(121)))), ((int)(((byte)(140)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DGV.ColumnHeadersHeight = 40;
            this.DGV.ColumnsHeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(121)))), ((int)(((byte)(140)))));
            this.DGV.ColumnsMenuStrip = null;
            this.DGV.ColumnsToSkip = new string[0];
            this.DGV.ColumnsType = ((System.Collections.Generic.Dictionary<string, string>)(resources.GetObject("DGV.ColumnsType")));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Rabar_013", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(121)))), ((int)(((byte)(140)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV.DefaultCellStyle = dataGridViewCellStyle2;
            this.DGV.DefaultSelectedColumns = new string[0];
            this.DGV.EnableHeadersVisualStyles = false;
            this.DGV.ExportPath = "C:\\Program Files\\Microsoft Visual Studio\\2022\\Enterprise\\Common7\\IDE";
            this.DGV.FlowSelectedColumn = null;
            this.DGV.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.DGV.Location = new System.Drawing.Point(3, 3);
            this.DGV.MultiSelect = false;
            this.DGV.Name = "DGV";
            this.DGV.ReadOnly = true;
            this.DGV.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.DGV.RowHeadersVisible = false;
            this.DGV.RowHeadersWidth = 51;
            this.DGV.RowTemplate.Height = 40;
            this.DGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DGV.Size = new System.Drawing.Size(478, 454);
            this.DGV.TabIndex = 0;
            this.DGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCellClick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.24793F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.75207F));
            this.tableLayoutPanel1.Controls.Add(this.lbl, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txt, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnSave, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(11, 52);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(484, 84);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lbl
            // 
            this.lbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl.AutoSize = true;
            this.lbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lbl.Location = new System.Drawing.Point(3, 13);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(31, 29);
            this.lbl.TabIndex = 0;
            this.lbl.Text = "lbl";
            // 
            // txt
            // 
            this.txt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt.BackColor = System.Drawing.Color.White;
            this.txt.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.txt.BorderFocusColor = System.Drawing.Color.ForestGreen;
            this.txt.ForeColor = System.Drawing.Color.Black;
            this.txt.Location = new System.Drawing.Point(3, 45);
            this.txt.Name = "txt";
            this.txt.SelectedText = "";
            this.txt.SelectionLength = 0;
            this.txt.SelectionStart = 0;
            this.txt.Size = new System.Drawing.Size(334, 36);
            this.txt.TabIndex = 1;
            this.txt.TextPadding = new System.Windows.Forms.Padding(4);
            this.txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txt_KeyUp);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.White;
            this.btnSave.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.btnSave.BorderRadius = 5;
            this.btnSave.BorderSize = 2;
            this.btnSave.ButtonImageSize = 20;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(80)))), ((int)(((byte)(93)))));
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.Location = new System.Drawing.Point(343, 45);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.btnSave.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnSave.Size = new System.Drawing.Size(138, 36);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "زیادکردن";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnClick);
            // 
            // formTopMenuBar1
            // 
            this.formTopMenuBar1.BackColor = System.Drawing.Color.White;
            this.formTopMenuBar1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(121)))), ((int)(((byte)(140)))));
            this.formTopMenuBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.formTopMenuBar1.Font = new System.Drawing.Font("Rabar_013", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formTopMenuBar1.FormTitlArabic = "ملاحضة";
            this.formTopMenuBar1.FormTitlEnglish = "Title";
            this.formTopMenuBar1.FormTitlKurdish = "تایتڵ";
            this.formTopMenuBar1.Location = new System.Drawing.Point(0, 0);
            this.formTopMenuBar1.Name = "formTopMenuBar1";
            this.formTopMenuBar1.ShowMinimizeButton = false;
            this.formTopMenuBar1.Size = new System.Drawing.Size(508, 40);
            this.formTopMenuBar1.TabIndex = 0;
            this.formTopMenuBar1.TopMenuBarHeight = 40;
            // 
            // dgvElipse
            // 
            this.dgvElipse.TargetControl = this.DGV;
            // 
            // AdvancedAddDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(508, 615);
            this.Controls.Add(this.formTopMenuBar1);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Rabar_013", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AdvancedAddDialogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.AdvancedAddDialogForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AdvancedAddDialogForm_KeyDown);
            this.panel1.ResumeLayout(false);
            this.dgvBackPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private CustomControls.AdvancedFormTopMenuBar formTopMenuBar1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lbl;
        private AdvancedTextBox txt;
        private AdvancedActionButton btnSave;
        private AdvancedPanel dgvBackPanel;
        private AdvancedDataGridView DGV;
        private AdvancedElipse dgvElipse;
    }
}