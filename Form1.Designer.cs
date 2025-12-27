namespace Ireland
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel2 = new System.Windows.Forms.Panel();
            this.Rotate_btn = new System.Windows.Forms.Button();
            this.btn_zoom_in = new System.Windows.Forms.Button();
            this.txt_count = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_Next = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.btn_ZoomOut = new System.Windows.Forms.Button();
            this.lbl_img = new System.Windows.Forms.Label();
            this.btn_Previous = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.chk_Negative = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.UserCount = new System.Windows.Forms.Label();
            this.txt_ins = new System.Windows.Forms.TextBox();
            this.txt_del = new System.Windows.Forms.TextBox();
            this.btn_del = new System.Windows.Forms.Button();
            this.btn_Insert = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.DarkCyan;
            this.panel2.Controls.Add(this.Rotate_btn);
            this.panel2.Controls.Add(this.btn_zoom_in);
            this.panel2.Controls.Add(this.txt_count);
            this.panel2.Controls.Add(this.comboBox1);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.btn_Next);
            this.panel2.Controls.Add(this.trackBar1);
            this.panel2.Controls.Add(this.btn_ZoomOut);
            this.panel2.Controls.Add(this.lbl_img);
            this.panel2.Controls.Add(this.btn_Previous);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(12703, 54);
            this.panel2.TabIndex = 1;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // Rotate_btn
            // 
            this.Rotate_btn.Location = new System.Drawing.Point(1300, 11);
            this.Rotate_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Rotate_btn.Name = "Rotate_btn";
            this.Rotate_btn.Size = new System.Drawing.Size(83, 32);
            this.Rotate_btn.TabIndex = 57;
            this.Rotate_btn.Text = "Rotate";
            this.Rotate_btn.UseVisualStyleBackColor = true;
            this.Rotate_btn.Click += new System.EventHandler(this.Rotate_btn_Click);
            // 
            // btn_zoom_in
            // 
            this.btn_zoom_in.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btn_zoom_in.Location = new System.Drawing.Point(1124, 12);
            this.btn_zoom_in.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_zoom_in.Name = "btn_zoom_in";
            this.btn_zoom_in.Size = new System.Drawing.Size(83, 32);
            this.btn_zoom_in.TabIndex = 14;
            this.btn_zoom_in.Text = "Zoom In";
            this.btn_zoom_in.UseVisualStyleBackColor = false;
            this.btn_zoom_in.Click += new System.EventHandler(this.btn_zoom_in_Click);
            // 
            // txt_count
            // 
            this.txt_count.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.txt_count.Location = new System.Drawing.Point(817, 12);
            this.txt_count.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txt_count.Multiline = true;
            this.txt_count.Name = "txt_count";
            this.txt_count.ReadOnly = true;
            this.txt_count.Size = new System.Drawing.Size(63, 27);
            this.txt_count.TabIndex = 13;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(887, 14);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(231, 24);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.button1.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.button1.Location = new System.Drawing.Point(1388, 10);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 36);
            this.button1.TabIndex = 12;
            this.button1.Text = "Short Cut";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.Shortcut_Click);
            // 
            // btn_Next
            // 
            this.btn_Next.Location = new System.Drawing.Point(1212, 12);
            this.btn_Next.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.Size = new System.Drawing.Size(83, 32);
            this.btn_Next.TabIndex = 2;
            this.btn_Next.Text = "Next";
            this.btn_Next.UseVisualStyleBackColor = true;
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(269, 12);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(313, 56);
            this.trackBar1.TabIndex = 2;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // btn_ZoomOut
            // 
            this.btn_ZoomOut.Location = new System.Drawing.Point(709, 12);
            this.btn_ZoomOut.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_ZoomOut.Name = "btn_ZoomOut";
            this.btn_ZoomOut.Size = new System.Drawing.Size(103, 32);
            this.btn_ZoomOut.TabIndex = 1;
            this.btn_ZoomOut.Text = " Zoom out";
            this.btn_ZoomOut.UseVisualStyleBackColor = true;
            this.btn_ZoomOut.Click += new System.EventHandler(this.btn_ZoomOut_Click);
            // 
            // lbl_img
            // 
            this.lbl_img.AutoSize = true;
            this.lbl_img.Location = new System.Drawing.Point(120, 25);
            this.lbl_img.Name = "lbl_img";
            this.lbl_img.Size = new System.Drawing.Size(44, 16);
            this.lbl_img.TabIndex = 1;
            this.lbl_img.Text = "label2";
            // 
            // btn_Previous
            // 
            this.btn_Previous.Location = new System.Drawing.Point(621, 12);
            this.btn_Previous.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Previous.Name = "btn_Previous";
            this.btn_Previous.Size = new System.Drawing.Size(83, 32);
            this.btn_Previous.TabIndex = 0;
            this.btn_Previous.Text = "Previous";
            this.btn_Previous.UseVisualStyleBackColor = true;
            this.btn_Previous.Click += new System.EventHandler(this.btn_Previous_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Image Number :";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(5, 52);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1481, 537);
            this.panel1.TabIndex = 2;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(3, 2);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(983, 425);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.AutoScroll = true;
            this.panel3.Location = new System.Drawing.Point(0, 677);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1483, 226);
            this.panel3.TabIndex = 1;
            this.panel3.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Panel_Scroll);
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.AutoScroll = true;
            this.panel5.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel5.Location = new System.Drawing.Point(5, 640);
            this.panel5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1483, 32);
            this.panel5.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panel4.Controls.Add(this.chk_Negative);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.UserCount);
            this.panel4.Controls.Add(this.txt_ins);
            this.panel4.Controls.Add(this.txt_del);
            this.panel4.Controls.Add(this.btn_del);
            this.panel4.Controls.Add(this.btn_Insert);
            this.panel4.Location = new System.Drawing.Point(5, 591);
            this.panel4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1483, 42);
            this.panel4.TabIndex = 0;
            // 
            // chk_Negative
            // 
            this.chk_Negative.AutoSize = true;
            this.chk_Negative.Font = new System.Drawing.Font("Arial Unicode MS", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chk_Negative.Location = new System.Drawing.Point(464, 10);
            this.chk_Negative.Margin = new System.Windows.Forms.Padding(4);
            this.chk_Negative.Name = "chk_Negative";
            this.chk_Negative.Size = new System.Drawing.Size(143, 23);
            this.chk_Negative.TabIndex = 18;
            this.chk_Negative.Text = "Nagative Image";
            this.chk_Negative.UseVisualStyleBackColor = true;
            this.chk_Negative.CheckedChanged += new System.EventHandler(this.chk_Negative_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(123, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 20);
            this.label3.TabIndex = 17;
            // 
            // UserCount
            // 
            this.UserCount.AutoSize = true;
            this.UserCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserCount.Location = new System.Drawing.Point(13, 14);
            this.UserCount.Name = "UserCount";
            this.UserCount.Size = new System.Drawing.Size(104, 18);
            this.UserCount.TabIndex = 16;
            this.UserCount.Text = "User Count :";
            // 
            // txt_ins
            // 
            this.txt_ins.Location = new System.Drawing.Point(768, 2);
            this.txt_ins.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txt_ins.Multiline = true;
            this.txt_ins.Name = "txt_ins";
            this.txt_ins.Size = new System.Drawing.Size(39, 29);
            this.txt_ins.TabIndex = 4;
            // 
            // txt_del
            // 
            this.txt_del.Location = new System.Drawing.Point(1003, 2);
            this.txt_del.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txt_del.Multiline = true;
            this.txt_del.Name = "txt_del";
            this.txt_del.Size = new System.Drawing.Size(39, 29);
            this.txt_del.TabIndex = 3;
            // 
            // btn_del
            // 
            this.btn_del.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btn_del.Location = new System.Drawing.Point(876, 1);
            this.btn_del.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_del.Name = "btn_del";
            this.btn_del.Size = new System.Drawing.Size(112, 34);
            this.btn_del.TabIndex = 1;
            this.btn_del.Text = "Delete";
            this.btn_del.UseVisualStyleBackColor = false;
            this.btn_del.Click += new System.EventHandler(this.btn_del_Click);
            // 
            // btn_Insert
            // 
            this.btn_Insert.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btn_Insert.Location = new System.Drawing.Point(636, 2);
            this.btn_Insert.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Insert.Name = "btn_Insert";
            this.btn_Insert.Size = new System.Drawing.Size(112, 34);
            this.btn_Insert.TabIndex = 0;
            this.btn_Insert.Text = "Insert Row";
            this.btn_Insert.UseVisualStyleBackColor = false;
            this.btn_Insert.Click += new System.EventHandler(this.btn_Insert_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1487, 928);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "ID_BMD";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_ZoomOut;
        private System.Windows.Forms.Button btn_Previous;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label lbl_img;
        private System.Windows.Forms.Button btn_Next;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox txt_del;
        private System.Windows.Forms.Button btn_del;
        private System.Windows.Forms.Button btn_Insert;
        private System.Windows.Forms.TextBox txt_ins;
        private System.Windows.Forms.TextBox txt_count;
        private System.Windows.Forms.Button btn_zoom_in;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label UserCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Rotate_btn;
        private System.Windows.Forms.CheckBox chk_Negative;
    }
}

