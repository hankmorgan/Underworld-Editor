namespace UnderworldEditor
{
    partial class FrmSelect
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnLoadUW1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSS1Nova = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.BtnLoad = new System.Windows.Forms.Button();
            this.btnLoadUW2 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.btnLoadUW1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnLoadUW2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 156F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1200, 704);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnLoadUW1
            // 
            this.btnLoadUW1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLoadUW1.Location = new System.Drawing.Point(4, 4);
            this.btnLoadUW1.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoadUW1.Name = "btnLoadUW1";
            this.btnLoadUW1.Size = new System.Drawing.Size(1192, 150);
            this.btnLoadUW1.TabIndex = 0;
            this.btnLoadUW1.Text = "Load UW1";
            this.btnLoadUW1.UseVisualStyleBackColor = true;
            this.btnLoadUW1.Click += new System.EventHandler(this.btnUW1_CLICK);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSS1Nova);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.BtnLoad);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 552);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1192, 148);
            this.panel1.TabIndex = 1;
            // 
            // btnSS1Nova
            // 
            this.btnSS1Nova.Location = new System.Drawing.Point(548, 0);
            this.btnSS1Nova.Margin = new System.Windows.Forms.Padding(6);
            this.btnSS1Nova.Name = "btnSS1Nova";
            this.btnSS1Nova.Size = new System.Drawing.Size(280, 142);
            this.btnSS1Nova.TabIndex = 2;
            this.btnSS1Nova.Text = "Load Extraction Tools for SS1 and TNova";
            this.btnSS1Nova.UseVisualStyleBackColor = true;
            this.btnSS1Nova.Visible = false;
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Left;
            this.button2.Location = new System.Drawing.Point(0, 0);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(202, 148);
            this.button2.TabIndex = 1;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // BtnLoad
            // 
            this.BtnLoad.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtnLoad.Location = new System.Drawing.Point(924, 0);
            this.BtnLoad.Margin = new System.Windows.Forms.Padding(4);
            this.BtnLoad.Name = "BtnLoad";
            this.BtnLoad.Size = new System.Drawing.Size(268, 148);
            this.BtnLoad.TabIndex = 0;
            this.BtnLoad.Text = "Load UW1 or UW2 Tools";
            this.BtnLoad.UseVisualStyleBackColor = true;
            this.BtnLoad.Visible = false;
            this.BtnLoad.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnLoadUW2
            // 
            this.btnLoadUW2.Location = new System.Drawing.Point(3, 161);
            this.btnLoadUW2.Name = "btnLoadUW2";
            this.btnLoadUW2.Size = new System.Drawing.Size(1194, 153);
            this.btnLoadUW2.TabIndex = 2;
            this.btnLoadUW2.Text = "Load UW2";
            this.btnLoadUW2.UseVisualStyleBackColor = true;
            this.btnLoadUW2.Click += new System.EventHandler(this.btnLoadUW2_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Game Executable|*.exe";
            // 
            // FrmSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 704);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSelect";
            this.Text = "Game Select";
            this.Load += new System.EventHandler(this.FrmSelect_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnLoadUW1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button BtnLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnSS1Nova;
        private System.Windows.Forms.Button btnLoadUW2;
    }
}