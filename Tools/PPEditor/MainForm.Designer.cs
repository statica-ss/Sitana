namespace Editor
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
            this._toolbar = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._buttonOperationPan = new System.Windows.Forms.ToolStripButton();
            this._buttonOperationRectangle = new System.Windows.Forms.ToolStripButton();
            this._buttonOperationEllipse = new System.Windows.Forms.ToolStripButton();
            this._buttonOperationPolygon = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this._snapToGrid = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this._placeholder = new System.Windows.Forms.Panel();
            this._propertiesToolbar = new System.Windows.Forms.ToolStrip();
            this._propertyColor = new System.Windows.Forms.ToolStripButton();
            this._propertyTexture = new System.Windows.Forms.ToolStripDropDownButton();
            this._propertySetTexture = new System.Windows.Forms.ToolStripMenuItem();
            this._propertyTextureMapping = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this._propertyDensity = new System.Windows.Forms.ToolStripTextBox();
            this._propertyType = new System.Windows.Forms.ToolStripComboBox();
            this._toolbar.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this._propertiesToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // _toolbar
            // 
            this._toolbar.Dock = System.Windows.Forms.DockStyle.None;
            this._toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSplitButton1,
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton2,
            this.toolStripSeparator1,
            this._buttonOperationPan,
            this._buttonOperationRectangle,
            this._buttonOperationEllipse,
            this._buttonOperationPolygon,
            this.toolStripSeparator3,
            this._snapToGrid});
            this._toolbar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this._toolbar.Location = new System.Drawing.Point(3, 23);
            this._toolbar.Name = "_toolbar";
            this._toolbar.Size = new System.Drawing.Size(434, 23);
            this._toolbar.TabIndex = 0;
            this._toolbar.Text = "toolStrip1";
            this._toolbar.TextDirection = System.Windows.Forms.ToolStripTextDirection.Vertical90;
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem1,
            this.saveToolStripMenuItem1,
            this.saveAsToolStripMenuItem1});
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(38, 19);
            this.toolStripButton1.Text = "&File";
            this.toolStripButton1.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
            this.openToolStripMenuItem1.Text = "Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem1_Click);
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
            this.saveToolStripMenuItem1.Text = "Save";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // saveAsToolStripMenuItem1
            // 
            this.saveAsToolStripMenuItem1.Name = "saveAsToolStripMenuItem1";
            this.saveAsToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
            this.saveAsToolStripMenuItem1.Text = "Save As...";
            this.saveAsToolStripMenuItem1.Click += new System.EventHandler(this.saveAsToolStripMenuItem1_Click);
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripMenuItem1,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripMenuItem2,
            this.deleteToolStripMenuItem});
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(40, 19);
            this.toolStripSplitButton1.Text = "&Edit";
            this.toolStripSplitButton1.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.toolStripSplitButton1.ToolTipText = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(104, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(104, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(45, 19);
            this.toolStripDropDownButton1.Text = "&View";
            this.toolStripDropDownButton1.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(45, 19);
            this.toolStripDropDownButton2.Text = "&Help";
            this.toolStripDropDownButton2.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            this.toolStripSeparator1.TextDirection = System.Windows.Forms.ToolStripTextDirection.Vertical90;
            // 
            // _buttonOperationPan
            // 
            this._buttonOperationPan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._buttonOperationPan.Image = ((System.Drawing.Image)(resources.GetObject("_buttonOperationPan.Image")));
            this._buttonOperationPan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonOperationPan.Name = "_buttonOperationPan";
            this._buttonOperationPan.Size = new System.Drawing.Size(23, 20);
            this._buttonOperationPan.Text = "&Selection Tool";
            this._buttonOperationPan.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this._buttonOperationPan.ToolTipText = "Selection Tool";
            this._buttonOperationPan.Click += new System.EventHandler(this._buttonOperationSelection_Click);
            // 
            // _buttonOperationRectangle
            // 
            this._buttonOperationRectangle.Image = ((System.Drawing.Image)(resources.GetObject("_buttonOperationRectangle.Image")));
            this._buttonOperationRectangle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonOperationRectangle.Name = "_buttonOperationRectangle";
            this._buttonOperationRectangle.Size = new System.Drawing.Size(79, 20);
            this._buttonOperationRectangle.Text = "&Rectangle";
            this._buttonOperationRectangle.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this._buttonOperationRectangle.Click += new System.EventHandler(this._buttonOperationRectangle_Click);
            // 
            // _buttonOperationEllipse
            // 
            this._buttonOperationEllipse.Image = ((System.Drawing.Image)(resources.GetObject("_buttonOperationEllipse.Image")));
            this._buttonOperationEllipse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonOperationEllipse.Name = "_buttonOperationEllipse";
            this._buttonOperationEllipse.Size = new System.Drawing.Size(57, 20);
            this._buttonOperationEllipse.Text = "&Circle";
            this._buttonOperationEllipse.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this._buttonOperationEllipse.ToolTipText = "Circle";
            this._buttonOperationEllipse.Click += new System.EventHandler(this._buttonOperationEllipse_Click);
            // 
            // _buttonOperationPolygon
            // 
            this._buttonOperationPolygon.Image = ((System.Drawing.Image)(resources.GetObject("_buttonOperationPolygon.Image")));
            this._buttonOperationPolygon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._buttonOperationPolygon.Name = "_buttonOperationPolygon";
            this._buttonOperationPolygon.Size = new System.Drawing.Size(71, 20);
            this._buttonOperationPolygon.Text = "&Polygon";
            this._buttonOperationPolygon.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this._buttonOperationPolygon.Click += new System.EventHandler(this._buttonOperationAddPolygon_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 23);
            this.toolStripSeparator3.TextDirection = System.Windows.Forms.ToolStripTextDirection.Vertical90;
            // 
            // _snapToGrid
            // 
            this._snapToGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._snapToGrid.Image = ((System.Drawing.Image)(resources.GetObject("_snapToGrid.Image")));
            this._snapToGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._snapToGrid.Name = "_snapToGrid";
            this._snapToGrid.Size = new System.Drawing.Size(23, 20);
            this._snapToGrid.Text = "Snap to grid";
            this._snapToGrid.Click += new System.EventHandler(this._snapToGrid_Click);
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this._placeholder);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(957, 572);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(957, 618);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this._toolbar);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this._propertiesToolbar);
            // 
            // _placeholder
            // 
            this._placeholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this._placeholder.Location = new System.Drawing.Point(0, 0);
            this._placeholder.Name = "_placeholder";
            this._placeholder.Size = new System.Drawing.Size(957, 572);
            this._placeholder.TabIndex = 0;
            // 
            // _propertiesToolbar
            // 
            this._propertiesToolbar.Dock = System.Windows.Forms.DockStyle.None;
            this._propertiesToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._propertyColor,
            this._propertyTexture,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this._propertyDensity,
            this._propertyType});
            this._propertiesToolbar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this._propertiesToolbar.Location = new System.Drawing.Point(3, 0);
            this._propertiesToolbar.Name = "_propertiesToolbar";
            this._propertiesToolbar.Size = new System.Drawing.Size(219, 23);
            this._propertiesToolbar.TabIndex = 1;
            // 
            // _propertyColor
            // 
            this._propertyColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._propertyColor.Image = ((System.Drawing.Image)(resources.GetObject("_propertyColor.Image")));
            this._propertyColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._propertyColor.Name = "_propertyColor";
            this._propertyColor.Size = new System.Drawing.Size(23, 20);
            this._propertyColor.Text = "Color";
            this._propertyColor.Click += new System.EventHandler(this._propertyColor_Click);
            // 
            // _propertyTexture
            // 
            this._propertyTexture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._propertyTexture.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._propertySetTexture,
            this._propertyTextureMapping});
            this._propertyTexture.Image = ((System.Drawing.Image)(resources.GetObject("_propertyTexture.Image")));
            this._propertyTexture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._propertyTexture.Name = "_propertyTexture";
            this._propertyTexture.Size = new System.Drawing.Size(29, 20);
            this._propertyTexture.Text = "Texture";
            // 
            // _propertySetTexture
            // 
            this._propertySetTexture.Name = "_propertySetTexture";
            this._propertySetTexture.Size = new System.Drawing.Size(164, 22);
            this._propertySetTexture.Text = "Set texture";
            // 
            // _propertyTextureMapping
            // 
            this._propertyTextureMapping.Name = "_propertyTextureMapping";
            this._propertyTextureMapping.Size = new System.Drawing.Size(164, 22);
            this._propertyTextureMapping.Text = "Texture mapping";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Margin = new System.Windows.Forms.Padding(0, 4, 0, 2);
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(49, 15);
            this.toolStripLabel1.Text = "Density:";
            // 
            // _propertyDensity
            // 
            this._propertyDensity.BackColor = System.Drawing.Color.Black;
            this._propertyDensity.Font = new System.Drawing.Font("Segoe UI", 7F);
            this._propertyDensity.ForeColor = System.Drawing.Color.White;
            this._propertyDensity.Margin = new System.Windows.Forms.Padding(1, 2, 1, 0);
            this._propertyDensity.Name = "_propertyDensity";
            this._propertyDensity.Size = new System.Drawing.Size(32, 20);
            this._propertyDensity.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this._propertyDensity.Leave += new System.EventHandler(this._propertyDensity_Leave);
            // 
            // _propertyType
            // 
            this._propertyType.BackColor = System.Drawing.Color.Black;
            this._propertyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._propertyType.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this._propertyType.ForeColor = System.Drawing.Color.White;
            this._propertyType.Items.AddRange(new object[] {
            "Static",
            "Dynamic",
            "Kinetic",
            "None"});
            this._propertyType.Margin = new System.Windows.Forms.Padding(1, 2, 1, 0);
            this._propertyType.Name = "_propertyType";
            this._propertyType.Size = new System.Drawing.Size(75, 20);
            this._propertyType.DropDownClosed += new System.EventHandler(this._propertyType_DropDownClosed);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(957, 618);
            this.Controls.Add(this.toolStripContainer1);
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Ebatianos Platform Editor";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this._toolbar.ResumeLayout(false);
            this._toolbar.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this._propertiesToolbar.ResumeLayout(false);
            this._propertiesToolbar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip _toolbar;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton _buttonOperationPolygon;
        private System.Windows.Forms.ToolStripButton _buttonOperationRectangle;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton _buttonOperationPan;
        private System.Windows.Forms.ToolStripButton _buttonOperationEllipse;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.Panel _placeholder;
        private System.Windows.Forms.ToolStrip _propertiesToolbar;
        private System.Windows.Forms.ToolStripButton _propertyColor;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox _propertyType;
        private System.Windows.Forms.ToolStripTextBox _propertyDensity;
        private System.Windows.Forms.ToolStripDropDownButton _propertyTexture;
        private System.Windows.Forms.ToolStripMenuItem _propertySetTexture;
        private System.Windows.Forms.ToolStripMenuItem _propertyTextureMapping;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton _snapToGrid;
    }
}

