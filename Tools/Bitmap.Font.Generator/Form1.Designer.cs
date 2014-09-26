namespace XnaFontGenerator
{
   partial class XnaFontGenerator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XnaFontGenerator));
            this.fontName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fontStyle = new System.Windows.Forms.ComboBox();
            this.fontSize = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.minChar = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.maxChar = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.borderColor = new System.Windows.Forms.Button();
            this.textColor = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.preview = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.export = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.borderOpacity = new System.Windows.Forms.NumericUpDown();
            this.borderSize = new System.Windows.Forms.TextBox();
            this.additionalCharacters = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.blurSize = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.sizeInPixels = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.preview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.borderOpacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blurSize)).BeginInit();
            this.SuspendLayout();
            // 
            // fontName
            // 
            this.fontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.fontName.FormattingEnabled = true;
            this.fontName.Location = new System.Drawing.Point(15, 30);
            this.fontName.Name = "fontName";
            this.fontName.Size = new System.Drawing.Size(188, 215);
            this.fontName.TabIndex = 1;
            this.fontName.SelectedIndexChanged += new System.EventHandler(this.fontName_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Font";
            // 
            // fontStyle
            // 
            this.fontStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.fontStyle.FormattingEnabled = true;
            this.fontStyle.Items.AddRange(new object[] {
            "Regular",
            "Italic",
            "Bold",
            "Bold, Italic"});
            this.fontStyle.Location = new System.Drawing.Point(209, 30);
            this.fontStyle.Name = "fontStyle";
            this.fontStyle.Size = new System.Drawing.Size(77, 215);
            this.fontStyle.TabIndex = 3;
            this.fontStyle.Text = "Regular";
            this.fontStyle.SelectedIndexChanged += new System.EventHandler(this.fontStyle_SelectedIndexChanged);
            // 
            // fontSize
            // 
            this.fontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.fontSize.FormattingEnabled = true;
            this.fontSize.Items.AddRange(new object[] {
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
            this.fontSize.Location = new System.Drawing.Point(292, 30);
            this.fontSize.Name = "fontSize";
            this.fontSize.Size = new System.Drawing.Size(77, 215);
            this.fontSize.TabIndex = 5;
            this.fontSize.Text = "23";
            this.fontSize.SelectedIndexChanged += new System.EventHandler(this.fontSize_SelectedIndexChanged);
            this.fontSize.TextUpdate += new System.EventHandler(this.fontSize_TextUpdate);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(289, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(206, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Font style";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 329);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Min char";
            // 
            // minChar
            // 
            this.minChar.Location = new System.Drawing.Point(15, 345);
            this.minChar.Name = "minChar";
            this.minChar.Size = new System.Drawing.Size(65, 22);
            this.minChar.TabIndex = 17;
            this.minChar.Text = "32";
            this.minChar.Validating += new System.ComponentModel.CancelEventHandler(this.minChar_Validating);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(87, 329);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Max char";
            // 
            // maxChar
            // 
            this.maxChar.Location = new System.Drawing.Point(90, 345);
            this.maxChar.Name = "maxChar";
            this.maxChar.Size = new System.Drawing.Size(65, 22);
            this.maxChar.TabIndex = 19;
            this.maxChar.Text = "127";
            this.maxChar.Validating += new System.ComponentModel.CancelEventHandler(this.maxChar_Validating);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 258);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Text color";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(84, 258);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Border clr.";
            // 
            // borderColor
            // 
            this.borderColor.BackColor = System.Drawing.Color.White;
            this.borderColor.Location = new System.Drawing.Point(87, 274);
            this.borderColor.Name = "borderColor";
            this.borderColor.Size = new System.Drawing.Size(65, 30);
            this.borderColor.TabIndex = 9;
            this.borderColor.UseVisualStyleBackColor = false;
            this.borderColor.Click += new System.EventHandler(this.borderColor_Click);
            // 
            // textColor
            // 
            this.textColor.BackColor = System.Drawing.Color.White;
            this.textColor.Location = new System.Drawing.Point(15, 274);
            this.textColor.Name = "textColor";
            this.textColor.Size = new System.Drawing.Size(65, 30);
            this.textColor.TabIndex = 7;
            this.textColor.UseVisualStyleBackColor = false;
            this.textColor.Click += new System.EventHandler(this.textColor_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(155, 258);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(64, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Border size";
            // 
            // preview
            // 
            this.preview.BackColor = System.Drawing.SystemColors.HotTrack;
            this.preview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.preview.Location = new System.Drawing.Point(15, 393);
            this.preview.Name = "preview";
            this.preview.Size = new System.Drawing.Size(354, 164);
            this.preview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.preview.TabIndex = 17;
            this.preview.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 377);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Preview:";
            // 
            // export
            // 
            this.export.Location = new System.Drawing.Point(264, 345);
            this.export.Name = "export";
            this.export.Size = new System.Drawing.Size(105, 23);
            this.export.TabIndex = 20;
            this.export.Text = "Export to texture";
            this.export.UseVisualStyleBackColor = true;
            this.export.Click += new System.EventHandler(this.export_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(261, 258);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "opacity";
            // 
            // borderOpacity
            // 
            this.borderOpacity.DecimalPlaces = 2;
            this.borderOpacity.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.borderOpacity.Location = new System.Drawing.Point(264, 275);
            this.borderOpacity.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.borderOpacity.Name = "borderOpacity";
            this.borderOpacity.Size = new System.Drawing.Size(52, 22);
            this.borderOpacity.TabIndex = 13;
            this.borderOpacity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.borderOpacity.ValueChanged += new System.EventHandler(this.borderOpacity_ValueChanged);
            // 
            // borderSize
            // 
            this.borderSize.Location = new System.Drawing.Point(158, 275);
            this.borderSize.Name = "borderSize";
            this.borderSize.Size = new System.Drawing.Size(100, 22);
            this.borderSize.TabIndex = 11;
            this.borderSize.Text = "0";
            this.borderSize.Validating += new System.ComponentModel.CancelEventHandler(this.borderSize_Validating);
            // 
            // additionalCharacters
            // 
            this.additionalCharacters.Location = new System.Drawing.Point(161, 345);
            this.additionalCharacters.Name = "additionalCharacters";
            this.additionalCharacters.Size = new System.Drawing.Size(97, 22);
            this.additionalCharacters.TabIndex = 22;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(158, 329);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "Additional";
            // 
            // blurSize
            // 
            this.blurSize.DecimalPlaces = 1;
            this.blurSize.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.blurSize.Location = new System.Drawing.Point(317, 275);
            this.blurSize.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.blurSize.Name = "blurSize";
            this.blurSize.Size = new System.Drawing.Size(52, 22);
            this.blurSize.TabIndex = 25;
            this.blurSize.ValueChanged += new System.EventHandler(this.blurSize_ValueChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(314, 258);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(28, 13);
            this.label12.TabIndex = 26;
            this.label12.Text = "blur";
            // 
            // sizeInPixels
            // 
            this.sizeInPixels.AutoSize = true;
            this.sizeInPixels.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.sizeInPixels.Location = new System.Drawing.Point(331, 13);
            this.sizeInPixels.Name = "sizeInPixels";
            this.sizeInPixels.Size = new System.Drawing.Size(38, 17);
            this.sizeInPixels.TabIndex = 27;
            this.sizeInPixels.Text = "px";
            this.sizeInPixels.UseVisualStyleBackColor = true;
            // 
            // XnaFontGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(385, 569);
            this.Controls.Add(this.sizeInPixels);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.blurSize);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.additionalCharacters);
            this.Controls.Add(this.fontName);
            this.Controls.Add(this.borderSize);
            this.Controls.Add(this.borderOpacity);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.export);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.preview);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textColor);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.borderColor);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.maxChar);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.minChar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.fontSize);
            this.Controls.Add(this.fontStyle);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "XnaFontGenerator";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bitmap Font Generator - Nagaka";
            ((System.ComponentModel.ISupportInitialize)(this.preview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.borderOpacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blurSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.ComboBox fontName;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.ComboBox fontStyle;
      private System.Windows.Forms.ComboBox fontSize;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.TextBox minChar;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.TextBox maxChar;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Button borderColor;
      private System.Windows.Forms.Button textColor;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.PictureBox preview;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Button export;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.NumericUpDown borderOpacity;
      private System.Windows.Forms.TextBox borderSize;
      private System.Windows.Forms.TextBox additionalCharacters;
      private System.Windows.Forms.Label label11;
      private System.Windows.Forms.NumericUpDown blurSize;
      private System.Windows.Forms.Label label12;
      private System.Windows.Forms.CheckBox sizeInPixels;
   }
}

