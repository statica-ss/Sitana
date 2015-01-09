namespace FontGenerator
{
    partial class MainForm
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose( bool disposing )
      {
         if ( disposing && ( components != null ) )
         {
            components.Dispose();
         }
         base.Dispose( disposing );
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.FontFace = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.FontStyle = new System.Windows.Forms.ComboBox();
            this.FontSize = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.MinChar = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.MaxChar = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.BorderColor = new System.Windows.Forms.Button();
            this.FillColor = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.preview = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.GenerateBtn = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.BorderOpacity = new System.Windows.Forms.NumericUpDown();
            this.AdditionalCharacters = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.BorderSize = new System.Windows.Forms.NumericUpDown();
            this.BorderRound = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.SizeSerie = new System.Windows.Forms.TextBox();
            this.GenerateSerieBtn = new System.Windows.Forms.Button();
            this.Kerning = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.preview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BorderOpacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BorderSize)).BeginInit();
            this.SuspendLayout();
            // 
            // FontFace
            // 
            this.FontFace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.FontFace.FormattingEnabled = true;
            this.FontFace.Location = new System.Drawing.Point(15, 30);
            this.FontFace.Name = "FontFace";
            this.FontFace.Size = new System.Drawing.Size(179, 215);
            this.FontFace.TabIndex = 1;
            this.FontFace.SelectedIndexChanged += new System.EventHandler(this.fontName_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Font face";
            // 
            // FontStyle
            // 
            this.FontStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.FontStyle.FormattingEnabled = true;
            this.FontStyle.Items.AddRange(new object[] {
            "Regular",
            "Italic",
            "Bold",
            "Bold, Italic"});
            this.FontStyle.Location = new System.Drawing.Point(200, 30);
            this.FontStyle.Name = "FontStyle";
            this.FontStyle.Size = new System.Drawing.Size(63, 111);
            this.FontStyle.TabIndex = 3;
            this.FontStyle.Text = "Regular";
            this.FontStyle.SelectedIndexChanged += new System.EventHandler(this.fontStyle_SelectedIndexChanged);
            // 
            // FontSize
            // 
            this.FontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.FontSize.FormattingEnabled = true;
            this.FontSize.Items.AddRange(new object[] {
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "20",
            "22",
            "23",
            "24",
            "26",
            "28",
            "36",
            "48",
            "72"});
            this.FontSize.Location = new System.Drawing.Point(269, 30);
            this.FontSize.Name = "FontSize";
            this.FontSize.Size = new System.Drawing.Size(59, 215);
            this.FontSize.TabIndex = 5;
            this.FontSize.Text = "23";
            this.FontSize.SelectedIndexChanged += new System.EventHandler(this.fontSize_SelectedIndexChanged);
            this.FontSize.TextUpdate += new System.EventHandler(this.fontSize_TextUpdate);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(269, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(197, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Font style";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 317);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Min char";
            // 
            // MinChar
            // 
            this.MinChar.Location = new System.Drawing.Point(15, 333);
            this.MinChar.Name = "MinChar";
            this.MinChar.Size = new System.Drawing.Size(65, 22);
            this.MinChar.TabIndex = 17;
            this.MinChar.Text = "32";
            this.MinChar.Validating += new System.ComponentModel.CancelEventHandler(this.minChar_Validating);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(87, 317);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Max char";
            // 
            // MaxChar
            // 
            this.MaxChar.Location = new System.Drawing.Point(90, 333);
            this.MaxChar.Name = "MaxChar";
            this.MaxChar.Size = new System.Drawing.Size(65, 22);
            this.MaxChar.TabIndex = 19;
            this.MaxChar.Text = "127";
            this.MaxChar.Validating += new System.ComponentModel.CancelEventHandler(this.maxChar_Validating);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 256);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Fill color";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(87, 258);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Border clr.";
            // 
            // BorderColor
            // 
            this.BorderColor.BackColor = System.Drawing.Color.White;
            this.BorderColor.Location = new System.Drawing.Point(90, 274);
            this.BorderColor.Name = "BorderColor";
            this.BorderColor.Size = new System.Drawing.Size(64, 30);
            this.BorderColor.TabIndex = 9;
            this.BorderColor.UseVisualStyleBackColor = false;
            this.BorderColor.Click += new System.EventHandler(this.borderColor_Click);
            // 
            // FillColor
            // 
            this.FillColor.BackColor = System.Drawing.Color.White;
            this.FillColor.Location = new System.Drawing.Point(15, 274);
            this.FillColor.Name = "FillColor";
            this.FillColor.Size = new System.Drawing.Size(65, 30);
            this.FillColor.TabIndex = 7;
            this.FillColor.UseVisualStyleBackColor = false;
            this.FillColor.Click += new System.EventHandler(this.textColor_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(158, 258);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(39, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "B. size";
            // 
            // preview
            // 
            this.preview.BackColor = System.Drawing.SystemColors.HotTrack;
            this.preview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.preview.Location = new System.Drawing.Point(15, 381);
            this.preview.Name = "preview";
            this.preview.Size = new System.Drawing.Size(313, 164);
            this.preview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.preview.TabIndex = 17;
            this.preview.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 365);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Preview:";
            // 
            // GenerateBtn
            // 
            this.GenerateBtn.Location = new System.Drawing.Point(264, 333);
            this.GenerateBtn.Name = "GenerateBtn";
            this.GenerateBtn.Size = new System.Drawing.Size(64, 23);
            this.GenerateBtn.TabIndex = 20;
            this.GenerateBtn.Text = "Generate";
            this.GenerateBtn.UseVisualStyleBackColor = true;
            this.GenerateBtn.Click += new System.EventHandler(this.GenerateBtn_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(216, 258);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "B. opacity";
            // 
            // BorderOpacity
            // 
            this.BorderOpacity.DecimalPlaces = 2;
            this.BorderOpacity.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.BorderOpacity.Location = new System.Drawing.Point(219, 276);
            this.BorderOpacity.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.BorderOpacity.Name = "BorderOpacity";
            this.BorderOpacity.Size = new System.Drawing.Size(51, 22);
            this.BorderOpacity.TabIndex = 13;
            this.BorderOpacity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.BorderOpacity.ValueChanged += new System.EventHandler(this.borderOpacity_ValueChanged);
            // 
            // AdditionalCharacters
            // 
            this.AdditionalCharacters.Location = new System.Drawing.Point(161, 333);
            this.AdditionalCharacters.Name = "AdditionalCharacters";
            this.AdditionalCharacters.Size = new System.Drawing.Size(97, 22);
            this.AdditionalCharacters.TabIndex = 22;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(158, 317);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "Additional";
            // 
            // BorderSize
            // 
            this.BorderSize.Location = new System.Drawing.Point(161, 276);
            this.BorderSize.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.BorderSize.Name = "BorderSize";
            this.BorderSize.Size = new System.Drawing.Size(51, 22);
            this.BorderSize.TabIndex = 24;
            this.BorderSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.BorderSize.ValueChanged += new System.EventHandler(this.BorderSize_ValueChanged);
            // 
            // BorderRound
            // 
            this.BorderRound.AutoSize = true;
            this.BorderRound.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.BorderRound.Location = new System.Drawing.Point(279, 278);
            this.BorderRound.Name = "BorderRound";
            this.BorderRound.Size = new System.Drawing.Size(49, 16);
            this.BorderRound.TabIndex = 25;
            this.BorderRound.Text = "Round";
            this.BorderRound.UseVisualStyleBackColor = true;
            this.BorderRound.CheckedChanged += new System.EventHandler(this.BorderRound_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(276, 258);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(40, 13);
            this.label12.TabIndex = 26;
            this.label12.Text = "B. join";
            // 
            // SizeSerie
            // 
            this.SizeSerie.Location = new System.Drawing.Point(16, 551);
            this.SizeSerie.Name = "SizeSerie";
            this.SizeSerie.Size = new System.Drawing.Size(196, 22);
            this.SizeSerie.TabIndex = 27;
            // 
            // GenerateSerieBtn
            // 
            this.GenerateSerieBtn.Location = new System.Drawing.Point(219, 551);
            this.GenerateSerieBtn.Name = "GenerateSerieBtn";
            this.GenerateSerieBtn.Size = new System.Drawing.Size(109, 23);
            this.GenerateSerieBtn.TabIndex = 28;
            this.GenerateSerieBtn.Text = "Generate Serie";
            this.GenerateSerieBtn.UseVisualStyleBackColor = true;
            this.GenerateSerieBtn.Click += new System.EventHandler(this.GenerateSerieBtn_Click);
            // 
            // Kerning
            // 
            this.Kerning.AutoSize = true;
            this.Kerning.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Kerning.Location = new System.Drawing.Point(200, 147);
            this.Kerning.Name = "Kerning";
            this.Kerning.Size = new System.Drawing.Size(54, 16);
            this.Kerning.TabIndex = 29;
            this.Kerning.Text = "Kerning";
            this.Kerning.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(343, 585);
            this.Controls.Add(this.Kerning);
            this.Controls.Add(this.GenerateSerieBtn);
            this.Controls.Add(this.SizeSerie);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.BorderRound);
            this.Controls.Add(this.BorderSize);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.AdditionalCharacters);
            this.Controls.Add(this.FontFace);
            this.Controls.Add(this.BorderOpacity);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.GenerateBtn);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.preview);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.FillColor);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.BorderColor);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.MaxChar);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.MinChar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.FontSize);
            this.Controls.Add(this.FontStyle);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sitana Font Generator";
            ((System.ComponentModel.ISupportInitialize)(this.preview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BorderOpacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BorderSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.ComboBox FontFace;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.ComboBox FontStyle;
      private System.Windows.Forms.ComboBox FontSize;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.TextBox MinChar;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.TextBox MaxChar;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Button BorderColor;
      private System.Windows.Forms.Button FillColor;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.PictureBox preview;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Button GenerateBtn;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.NumericUpDown BorderOpacity;
      private System.Windows.Forms.TextBox AdditionalCharacters;
      private System.Windows.Forms.Label label11;
      private System.Windows.Forms.NumericUpDown BorderSize;
      private System.Windows.Forms.CheckBox BorderRound;
      private System.Windows.Forms.Label label12;
      private System.Windows.Forms.TextBox SizeSerie;
      private System.Windows.Forms.Button GenerateSerieBtn;
      private System.Windows.Forms.CheckBox Kerning;
   }
}

