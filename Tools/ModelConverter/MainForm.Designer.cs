namespace ModelConverter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.importBtn = new System.Windows.Forms.Button();
            this.exportBtn = new System.Windows.Forms.Button();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.sizeXedit = new System.Windows.Forms.TextBox();
            this.sizeYedit = new System.Windows.Forms.TextBox();
            this.sizeZedit = new System.Windows.Forms.TextBox();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.centerX = new System.Windows.Forms.CheckBox();
            this.centerY = new System.Windows.Forms.CheckBox();
            this.centerZ = new System.Windows.Forms.CheckBox();
            this.layXz = new System.Windows.Forms.CheckBox();
            this.layXy = new System.Windows.Forms.CheckBox();
            this.comboMaterialsSets = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboMaterial = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonAmbient = new System.Windows.Forms.Button();
            this.buttonDiffuse = new System.Windows.Forms.Button();
            this.buttonSpecular = new System.Windows.Forms.Button();
            this.buttonEmmisive = new System.Windows.Forms.Button();
            this.specularExp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.opacityTrack = new System.Windows.Forms.TrackBar();
            this.buttonAddSet = new System.Windows.Forms.Button();
            this.buttonDeleteSet = new System.Windows.Forms.Button();
            this.comboTexture = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.swapYzCheckBox = new System.Windows.Forms.CheckBox();
            this.swapZyCheckBox = new System.Windows.Forms.CheckBox();
            this.checkZisUp = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.opacityTrack)).BeginInit();
            this.SuspendLayout();
            // 
            // importBtn
            // 
            this.importBtn.Location = new System.Drawing.Point(12, 12);
            this.importBtn.Name = "importBtn";
            this.importBtn.Size = new System.Drawing.Size(75, 23);
            this.importBtn.TabIndex = 0;
            this.importBtn.Text = "import";
            this.importBtn.UseVisualStyleBackColor = true;
            this.importBtn.Click += new System.EventHandler(this.importBtn_Click);
            // 
            // exportBtn
            // 
            this.exportBtn.Location = new System.Drawing.Point(356, 12);
            this.exportBtn.Name = "exportBtn";
            this.exportBtn.Size = new System.Drawing.Size(75, 23);
            this.exportBtn.TabIndex = 1;
            this.exportBtn.Text = "export";
            this.exportBtn.UseVisualStyleBackColor = true;
            this.exportBtn.Click += new System.EventHandler(this.exportBtn_Click);
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.Location = new System.Drawing.Point(102, 17);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(62, 13);
            this.sizeLabel.TabIndex = 2;
            this.sizeLabel.Text = "Object size:";
            // 
            // sizeXedit
            // 
            this.sizeXedit.Location = new System.Drawing.Point(170, 14);
            this.sizeXedit.Name = "sizeXedit";
            this.sizeXedit.Size = new System.Drawing.Size(53, 20);
            this.sizeXedit.TabIndex = 3;
            this.sizeXedit.TextChanged += new System.EventHandler(this.sizeXedit_TextChanged);
            // 
            // sizeYedit
            // 
            this.sizeYedit.Location = new System.Drawing.Point(229, 14);
            this.sizeYedit.Name = "sizeYedit";
            this.sizeYedit.Size = new System.Drawing.Size(53, 20);
            this.sizeYedit.TabIndex = 4;
            this.sizeYedit.TextChanged += new System.EventHandler(this.sizeYedit_TextChanged);
            // 
            // sizeZedit
            // 
            this.sizeZedit.Location = new System.Drawing.Point(288, 14);
            this.sizeZedit.Name = "sizeZedit";
            this.sizeZedit.Size = new System.Drawing.Size(53, 20);
            this.sizeZedit.TabIndex = 5;
            this.sizeZedit.TextChanged += new System.EventHandler(this.sizeZedit_TextChanged);
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point(10, 113);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(419, 396);
            this.pictureBox.TabIndex = 7;
            this.pictureBox.TabStop = false;
            // 
            // centerX
            // 
            this.centerX.AutoSize = true;
            this.centerX.Location = new System.Drawing.Point(12, 41);
            this.centerX.Name = "centerX";
            this.centerX.Size = new System.Drawing.Size(67, 17);
            this.centerX.TabIndex = 9;
            this.centerX.Text = "Center X";
            this.centerX.UseVisualStyleBackColor = true;
            this.centerX.CheckedChanged += new System.EventHandler(this.OnCheckBoxChecked);
            // 
            // centerY
            // 
            this.centerY.AutoSize = true;
            this.centerY.Location = new System.Drawing.Point(12, 63);
            this.centerY.Name = "centerY";
            this.centerY.Size = new System.Drawing.Size(67, 17);
            this.centerY.TabIndex = 10;
            this.centerY.Text = "Center Y";
            this.centerY.UseVisualStyleBackColor = true;
            this.centerY.CheckedChanged += new System.EventHandler(this.OnCheckBoxChecked);
            // 
            // centerZ
            // 
            this.centerZ.AutoSize = true;
            this.centerZ.Location = new System.Drawing.Point(12, 86);
            this.centerZ.Name = "centerZ";
            this.centerZ.Size = new System.Drawing.Size(67, 17);
            this.centerZ.TabIndex = 11;
            this.centerZ.Text = "Center Z";
            this.centerZ.UseVisualStyleBackColor = true;
            this.centerZ.CheckedChanged += new System.EventHandler(this.OnCheckBoxChecked);
            // 
            // layXz
            // 
            this.layXz.AutoSize = true;
            this.layXz.Location = new System.Drawing.Point(237, 63);
            this.layXz.Name = "layXz";
            this.layXz.Size = new System.Drawing.Size(104, 17);
            this.layXz.TabIndex = 12;
            this.layXz.Text = "Lay on XZ plane";
            this.layXz.UseVisualStyleBackColor = true;
            this.layXz.CheckedChanged += new System.EventHandler(this.OnCheckBoxChecked);
            // 
            // layXy
            // 
            this.layXy.AutoSize = true;
            this.layXy.Location = new System.Drawing.Point(237, 41);
            this.layXy.Name = "layXy";
            this.layXy.Size = new System.Drawing.Size(104, 17);
            this.layXy.TabIndex = 13;
            this.layXy.Text = "Lay on XY plane";
            this.layXy.UseVisualStyleBackColor = true;
            this.layXy.CheckedChanged += new System.EventHandler(this.OnCheckBoxChecked);
            // 
            // comboMaterialsSets
            // 
            this.comboMaterialsSets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMaterialsSets.FormattingEnabled = true;
            this.comboMaterialsSets.Location = new System.Drawing.Point(82, 524);
            this.comboMaterialsSets.Name = "comboMaterialsSets";
            this.comboMaterialsSets.Size = new System.Drawing.Size(108, 21);
            this.comboMaterialsSets.TabIndex = 15;
            this.comboMaterialsSets.SelectedIndexChanged += new System.EventHandler(this.comboMaterialsSets_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 527);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Materials set:";
            // 
            // comboMaterial
            // 
            this.comboMaterial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMaterial.FormattingEnabled = true;
            this.comboMaterial.Location = new System.Drawing.Point(82, 551);
            this.comboMaterial.Name = "comboMaterial";
            this.comboMaterial.Size = new System.Drawing.Size(108, 21);
            this.comboMaterial.TabIndex = 17;
            this.comboMaterial.SelectedIndexChanged += new System.EventHandler(this.comboMaterial_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 554);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Material:";
            // 
            // buttonAmbient
            // 
            this.buttonAmbient.Location = new System.Drawing.Point(273, 524);
            this.buttonAmbient.Name = "buttonAmbient";
            this.buttonAmbient.Size = new System.Drawing.Size(75, 23);
            this.buttonAmbient.TabIndex = 19;
            this.buttonAmbient.Text = "ambient";
            this.buttonAmbient.UseVisualStyleBackColor = true;
            this.buttonAmbient.Click += new System.EventHandler(this.buttonAmbient_Click);
            // 
            // buttonDiffuse
            // 
            this.buttonDiffuse.Location = new System.Drawing.Point(354, 524);
            this.buttonDiffuse.Name = "buttonDiffuse";
            this.buttonDiffuse.Size = new System.Drawing.Size(75, 23);
            this.buttonDiffuse.TabIndex = 20;
            this.buttonDiffuse.Text = "diffuse";
            this.buttonDiffuse.UseVisualStyleBackColor = true;
            this.buttonDiffuse.Click += new System.EventHandler(this.buttonDiffuse_Click);
            // 
            // buttonSpecular
            // 
            this.buttonSpecular.Location = new System.Drawing.Point(273, 553);
            this.buttonSpecular.Name = "buttonSpecular";
            this.buttonSpecular.Size = new System.Drawing.Size(75, 23);
            this.buttonSpecular.TabIndex = 21;
            this.buttonSpecular.Text = "specular";
            this.buttonSpecular.UseVisualStyleBackColor = true;
            this.buttonSpecular.Click += new System.EventHandler(this.buttonSpecular_Click);
            // 
            // buttonEmmisive
            // 
            this.buttonEmmisive.Location = new System.Drawing.Point(354, 553);
            this.buttonEmmisive.Name = "buttonEmmisive";
            this.buttonEmmisive.Size = new System.Drawing.Size(75, 23);
            this.buttonEmmisive.TabIndex = 22;
            this.buttonEmmisive.Text = "emissive";
            this.buttonEmmisive.UseVisualStyleBackColor = true;
            this.buttonEmmisive.Click += new System.EventHandler(this.buttonEmmisive_Click);
            // 
            // specularExp
            // 
            this.specularExp.Location = new System.Drawing.Point(376, 582);
            this.specularExp.Name = "specularExp";
            this.specularExp.Size = new System.Drawing.Size(53, 20);
            this.specularExp.TabIndex = 23;
            this.specularExp.TextChanged += new System.EventHandler(this.specularExp_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(295, 585);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Specular exp.:";
            // 
            // opacityTrack
            // 
            this.opacityTrack.LargeChange = 10;
            this.opacityTrack.Location = new System.Drawing.Point(273, 608);
            this.opacityTrack.Maximum = 100;
            this.opacityTrack.Name = "opacityTrack";
            this.opacityTrack.Size = new System.Drawing.Size(156, 45);
            this.opacityTrack.TabIndex = 25;
            this.opacityTrack.Scroll += new System.EventHandler(this.opacityTrack_Scroll);
            // 
            // buttonAddSet
            // 
            this.buttonAddSet.Location = new System.Drawing.Point(196, 522);
            this.buttonAddSet.Name = "buttonAddSet";
            this.buttonAddSet.Size = new System.Drawing.Size(19, 23);
            this.buttonAddSet.TabIndex = 27;
            this.buttonAddSet.Text = "+";
            this.buttonAddSet.UseVisualStyleBackColor = true;
            this.buttonAddSet.Click += new System.EventHandler(this.buttonAddSet_Click);
            // 
            // buttonDeleteSet
            // 
            this.buttonDeleteSet.Location = new System.Drawing.Point(221, 522);
            this.buttonDeleteSet.Name = "buttonDeleteSet";
            this.buttonDeleteSet.Size = new System.Drawing.Size(19, 23);
            this.buttonDeleteSet.TabIndex = 28;
            this.buttonDeleteSet.Text = "D";
            this.buttonDeleteSet.UseVisualStyleBackColor = true;
            this.buttonDeleteSet.Click += new System.EventHandler(this.buttonDeleteSet_Click);
            // 
            // comboTexture
            // 
            this.comboTexture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTexture.FormattingEnabled = true;
            this.comboTexture.Location = new System.Drawing.Point(82, 608);
            this.comboTexture.Name = "comboTexture";
            this.comboTexture.Size = new System.Drawing.Size(108, 21);
            this.comboTexture.TabIndex = 29;
            this.comboTexture.SelectedIndexChanged += new System.EventHandler(this.comboTexture_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 611);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "Texture:";
            // 
            // swapYzCheckBox
            // 
            this.swapYzCheckBox.AutoSize = true;
            this.swapYzCheckBox.Location = new System.Drawing.Point(120, 40);
            this.swapYzCheckBox.Name = "swapYzCheckBox";
            this.swapYzCheckBox.Size = new System.Drawing.Size(89, 17);
            this.swapYzCheckBox.TabIndex = 14;
            this.swapYzCheckBox.Text = "Y-Up to Z-Up";
            this.swapYzCheckBox.UseVisualStyleBackColor = true;
            this.swapYzCheckBox.CheckedChanged += new System.EventHandler(this.OnCheckBoxChecked);
            // 
            // swapZyCheckBox
            // 
            this.swapZyCheckBox.AutoSize = true;
            this.swapZyCheckBox.Location = new System.Drawing.Point(120, 63);
            this.swapZyCheckBox.Name = "swapZyCheckBox";
            this.swapZyCheckBox.Size = new System.Drawing.Size(89, 17);
            this.swapZyCheckBox.TabIndex = 6;
            this.swapZyCheckBox.Text = "Z-Up to Y-Up";
            this.swapZyCheckBox.UseVisualStyleBackColor = true;
            this.swapZyCheckBox.CheckedChanged += new System.EventHandler(this.OnCheckBoxChecked);
            // 
            // checkZisUp
            // 
            this.checkZisUp.AutoSize = true;
            this.checkZisUp.Location = new System.Drawing.Point(376, 90);
            this.checkZisUp.Name = "checkZisUp";
            this.checkZisUp.Size = new System.Drawing.Size(50, 17);
            this.checkZisUp.TabIndex = 31;
            this.checkZisUp.Text = "Z-Up";
            this.checkZisUp.UseVisualStyleBackColor = true;
            this.checkZisUp.CheckedChanged += new System.EventHandler(this.checkZisUp_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(438, 645);
            this.Controls.Add(this.checkZisUp);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboTexture);
            this.Controls.Add(this.buttonDeleteSet);
            this.Controls.Add(this.buttonAddSet);
            this.Controls.Add(this.opacityTrack);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.specularExp);
            this.Controls.Add(this.buttonEmmisive);
            this.Controls.Add(this.buttonSpecular);
            this.Controls.Add(this.buttonDiffuse);
            this.Controls.Add(this.buttonAmbient);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboMaterial);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboMaterialsSets);
            this.Controls.Add(this.swapYzCheckBox);
            this.Controls.Add(this.layXy);
            this.Controls.Add(this.layXz);
            this.Controls.Add(this.centerZ);
            this.Controls.Add(this.centerY);
            this.Controls.Add(this.centerX);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.swapZyCheckBox);
            this.Controls.Add(this.sizeZedit);
            this.Controls.Add(this.sizeYedit);
            this.Controls.Add(this.sizeXedit);
            this.Controls.Add(this.sizeLabel);
            this.Controls.Add(this.exportBtn);
            this.Controls.Add(this.importBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Model Converter";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.opacityTrack)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button importBtn;
        private System.Windows.Forms.Button exportBtn;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.TextBox sizeXedit;
        private System.Windows.Forms.TextBox sizeYedit;
        private System.Windows.Forms.TextBox sizeZedit;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.CheckBox centerX;
        private System.Windows.Forms.CheckBox centerY;
        private System.Windows.Forms.CheckBox centerZ;
        private System.Windows.Forms.CheckBox layXz;
        private System.Windows.Forms.CheckBox layXy;
        private System.Windows.Forms.ComboBox comboMaterialsSets;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboMaterial;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonAmbient;
        private System.Windows.Forms.Button buttonDiffuse;
        private System.Windows.Forms.Button buttonSpecular;
        private System.Windows.Forms.Button buttonEmmisive;
        private System.Windows.Forms.TextBox specularExp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar opacityTrack;
        private System.Windows.Forms.Button buttonAddSet;
        private System.Windows.Forms.Button buttonDeleteSet;
        private System.Windows.Forms.ComboBox comboTexture;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox swapYzCheckBox;
        private System.Windows.Forms.CheckBox swapZyCheckBox;
        private System.Windows.Forms.CheckBox checkZisUp;
    }
}

