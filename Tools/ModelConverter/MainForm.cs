using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WinFormsGraphicsDevice;
using Microsoft.Xna.Framework;
using System.Globalization;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Ebatianos.Graphics.Model.Exporters;
using Ebatianos.Graphics.Model.Importers;
using Ebatianos.Graphics.Model;
using Ebatianos.Content;

namespace ModelConverter
{
    public partial class MainForm : Form
    {
        List<IImporter> _importers = new List<IImporter>();
        List<IExporter> _exporters = new List<IExporter>();

        ModelX _model = null;
        PreviewControl _preview;

        Vector3 _exportSize;

        Boolean _insideTexChange = false;

        ModelX _previewModel;

        Int32 _currentSet = 0;
        Int32 _currentMaterial = 0;

        Boolean _inUpdate = false;

        String _importFile;

        List<String> _avaliableTextures = new List<String>();

        int[] _customColors;

        public MainForm()
        {
            InitializeComponent();

            _preview = new PreviewControl();
            _preview.Parent = this;
            _preview.Visible = true;
            _preview.SetBounds(pictureBox.Bounds.X+1, pictureBox.Bounds.Y+1, pictureBox.Bounds.Width-2, pictureBox.Bounds.Height-2);

            _preview.BringToFront();

            UpdateEdits();

            _importers.Add(new ObjImporter());
            _importers.Add(new PlyImporter());
            _importers.Add(new EmxImporter());

            _exporters.Add(new EmxExporter());
        }

        private void UpdateEdits()
        {
            Boolean enabled = _model != null;

            exportBtn.Enabled = enabled;
            sizeXedit.Enabled = enabled;
            sizeYedit.Enabled = enabled;
            sizeZedit.Enabled = enabled;
            sizeLabel.Enabled = enabled;

            centerX.Enabled = enabled;
            centerY.Enabled = enabled;
            centerZ.Enabled = enabled;

            layXy.Enabled = enabled;
            layXz.Enabled = enabled;

            swapZyCheckBox.Enabled = enabled && !swapYzCheckBox.Checked;
            swapYzCheckBox.Enabled = enabled && !swapZyCheckBox.Checked;

            if (enabled)
            {
                _insideTexChange = true;

                sizeXedit.Text = String.Format(CultureInfo.InvariantCulture, "{0}", _exportSize.X);
                sizeYedit.Text = String.Format(CultureInfo.InvariantCulture, "{0}", _exportSize.Y);
                sizeZedit.Text = String.Format(CultureInfo.InvariantCulture, "{0}", _exportSize.Z);

                _insideTexChange = false;
            }
        }

        private void importBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = "*.ply";
            dlg.CheckFileExists = true;
            dlg.Filter = "Supported model files (*.obj, *.ply, *.emx)|*.obj;*.ply;*.emx|Polygon File Format(*.ply)|*.ply|Ebatiano's ModelX(*.emx)|*.emx";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String ext = Path.GetExtension(dlg.FileName);
                foreach (var importer in _importers)
                {
                    if (ext.ToLowerInvariant() == importer.SupportedFileExt)
                    {
                        using (Stream stream = dlg.OpenFile())
                        {
                            try
                            {
                                _model = importer.Import(stream, (fname) =>
                                    {
                                        return new FileStream(Path.Combine(Path.GetDirectoryName(dlg.FileName), fname), FileMode.Open);
                                    });

                                checkZisUp.Checked = _model.Up.Z > 0;

                                _exportSize = OriginalSize;
                                _importFile = dlg.FileName;

                                _currentSet = 0;
                                _currentMaterial = 0;

                                FillAvaliableTextures(Path.GetDirectoryName(dlg.FileName));
                                UpdatePreview();
                                UpdateEdits();
                                UpdateCombos();

                                

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(String.Format("Error while importing model:\n{0}", ex.ToString()));
                            }
                        }
                        return;
                    }
                }

                MessageBox.Show("No importer avaliable to open this file.");
            }
        }

        private String GetFileNameWithoutExtension(String name)
        {
            if (name == null)
            {
                return null;
            }

            switch (Path.GetExtension(name).ToLowerInvariant())
            {
                case ".dds":
                case ".png":
                case ".tiff":
                case ".bmp":
                case ".jpg":
                case ".jpeg":
                case ".tga":
                    return Path.GetFileNameWithoutExtension(name);
            }

            return name;
        }

        private void LoadTextures(ModelX model, String directory)
        {
            for (Int32 set = 0; set < model.MaterialsSetsCount; ++set)
            {
                foreach (var material in model.Materials(set))
                {
                    String texName = material.Texture;

                    if (material.Textures != null)
                    {
                        material.Textures.Dispose();
                    }

                    if (String.IsNullOrWhiteSpace(texName))
                    {
                        material.Textures = null;
                        continue;
                    }

                    String fileName = Path.Combine(directory, texName);

                    try
                    {
                        using (Stream stream = new FileStream(fileName, FileMode.Open))
                        {
                            Texture2D texture = LoadTexture2DFromPng.FromStream(_preview.GraphicsDevice, stream);
                            material.Textures = new MaterialTextures(texture);
                        }
                    }
                    catch
                    {
                        fileName = GetFileNameWithoutExtension(texName) + ".png";
                        fileName = Path.Combine(directory, fileName);

                        try
                        {
                            using (Stream stream = new FileStream(fileName, FileMode.Open))
                            {
                                Texture2D texture = LoadTexture2DFromPng.FromStream(_preview.GraphicsDevice, stream);
                                material.Textures = new MaterialTextures(texture);
                            }
                        }
                        catch
                        {
                            material.Textures = null;
                        }
                    }
                }
            }
        }

        private void exportBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.DefaultExt = "*.emx";
            dlg.Filter = "Ebatiano's ModelX(*.emx)|*.emx";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String ext = Path.GetExtension(dlg.FileName);
                foreach (var exporter in _exporters)
                {
                    if (ext.ToLowerInvariant() == exporter.SupportedFileExt)
                    {
                        using (Stream stream = dlg.OpenFile())
                        {
                            ModelX model = ApplyChanges(_model);
                            exporter.Export(stream, model);
                        }
                        return;
                    }
                }

                MessageBox.Show("No exporter avaliable to save file using specyfied format.");
            }
        }

        private Vector3 OriginalSize
        {
            get
            {
                return _model.BoundB - _model.BoundA;
            }
        }

        private void sizeXedit_TextChanged(object sender, EventArgs e)
        {
            if ( _insideTexChange )
            {
                return;
            }

            Vector3 size = OriginalSize;

            try
            {
                Single x = Single.Parse(sizeXedit.Text, CultureInfo.InvariantCulture);

                Single scale = x / size.X;
                _exportSize = size * scale;

                _insideTexChange = true;

                sizeYedit.Text = String.Format(CultureInfo.InvariantCulture, "{0}", _exportSize.Y);
                sizeZedit.Text = String.Format(CultureInfo.InvariantCulture, "{0}", _exportSize.Z);

                _insideTexChange = false;
            }
            catch
            {
            }
        }

        private void sizeYedit_TextChanged(object sender, EventArgs e)
        {
            if (_insideTexChange)
            {
                return;
            }

            Vector3 size = OriginalSize;

            try
            {
                Single y = Single.Parse(sizeYedit.Text, CultureInfo.InvariantCulture);

                Single scale = y / size.Y;
                _exportSize = size * scale;

                _insideTexChange = true;

                sizeXedit.Text = String.Format(CultureInfo.InvariantCulture, "{0}", _exportSize.X);
                sizeZedit.Text = String.Format(CultureInfo.InvariantCulture, "{0}", _exportSize.Z);

                _insideTexChange = false;
            }
            catch
            {
            }
        }

        private void sizeZedit_TextChanged(object sender, EventArgs e)
        {
            if (_insideTexChange)
            {
                return;
            }

            Vector3 size = OriginalSize;

            try
            {
                Single z = Single.Parse(sizeZedit.Text, CultureInfo.InvariantCulture);

                Single scale = z / size.Z;
                _exportSize = size * scale;

                _insideTexChange = true;

                sizeXedit.Text = String.Format(CultureInfo.InvariantCulture, "{0}", _exportSize.X);
                sizeYedit.Text = String.Format(CultureInfo.InvariantCulture, "{0}", _exportSize.Y);

                _insideTexChange = false;
            }
            catch
            {
            }
        }

        private void OnCheckBoxChecked(object sender, EventArgs e)
        {
            Boolean enabled = _model != null;

            layXz.Enabled = !centerY.Checked && enabled;
            layXy.Enabled = !centerZ.Checked && enabled;
            centerY.Enabled = !layXz.Checked && enabled;
            centerZ.Enabled = !layXy.Checked && enabled;

            swapZyCheckBox.Enabled = !swapYzCheckBox.Checked && enabled;
            swapYzCheckBox.Enabled = !swapZyCheckBox.Checked && enabled;
            
            UpdatePreview();
        }

        private ModelX ApplyChanges(ModelX model)
        {
            Boolean swapZy = swapZyCheckBox.Checked;
            Boolean swapYz = swapYzCheckBox.Checked;

            Vector3 scale = _exportSize / OriginalSize;

            Vector3 move = Vector3.Zero;

            if (centerX.Checked)
            {
                move.X = -(_model.BoundA.X + _model.BoundB.X) / 2;
            }

            if (centerY.Checked)
            {
                move.Y = -(_model.BoundA.Y + _model.BoundB.Y) / 2;
            }

            if (centerZ.Checked)
            {
                move.Z = -(_model.BoundA.Z + _model.BoundB.Z) / 2;
            }

            if (layXz.Checked)
            {
                move.Y = -_model.BoundA.Y;
            }

            if (layXy.Checked)
            {
                move.Z = -_model.BoundA.Z;
            }

            if (Math.Abs(scale.X - scale.Y) > 0.0000001 || Math.Abs(scale.X - scale.Z) > 0.0000001)
            {
                throw new Exception("Niepoprawna wielkość modelu.");
            }

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[_model.Vertices.Length];

            for (Int32 idx = 0; idx < vertices.Length; ++idx)
            {
                vertices[idx] = _model.Vertices[idx];
                vertices[idx].Position = (vertices[idx].Position + move) * scale.X;

                if (swapZy || swapYz)
                {
                    Single mul = swapYz ? -1 : 1;

                    vertices[idx].Position = new Vector3(-vertices[idx].Position.X, vertices[idx].Position.Z, vertices[idx].Position.Y);
                    vertices[idx].Normal = new Vector3(-vertices[idx].Normal.X, vertices[idx].Normal.Z, vertices[idx].Normal.Y);
                }
            }

            String newName;

            List<Material[]> materials = new List<Material[]>();

            for (Int32 set = 0; set < _model.MaterialsSetsCount; ++set)
            {
                materials.Add(new Material[_model.Materials(set).Length]);

                Material[] newSet = materials.Last();

                for (Int32 idx = 0; idx < newSet.Length; ++idx)
                {
                    Material mat = _model.Materials(set)[idx];
                    newName = mat.Texture;

                    newName = GetFileNameWithoutExtension(newName);

                    if (newName != mat.Texture)
                    {
                        mat = new Material(newName, mat.Diffuse, mat.Ambient, mat.Specular, mat.SpecularExponent, mat.Emissive, mat.Opacity);
                    }

                    newSet[idx] = mat;
                }
            }

            ModelX modelX = new ModelX(materials, _model.Subsets, vertices);

            modelX.Up = checkZisUp.Checked ? new Vector3(0, 0, 1) : new Vector3(0, 1, 0);
            return modelX;
        }

        private void UpdateCombos()
        {
            Int32 sets = _model.MaterialsSetsCount;

            comboMaterialsSets.Items.Clear();

            for (Int32 idx = 0; idx < sets; ++idx)
            {
                comboMaterialsSets.Items.Add(String.Format("Set {0}", idx + 1));
            }

            comboMaterialsSets.SelectedIndex = _currentSet;

            comboMaterial.Items.Clear();

            for (Int32 idx = 0; idx < _model.Materials(0).Length; ++idx)
            {
                comboMaterial.Items.Add(String.Format("Material {0}", idx + 1));
            }

            comboMaterial.SelectedIndex = _currentMaterial;

            UpdateMaterialControls();
        }

        private void UpdateMaterialControls()
        {
            Material material = CurrentMaterial;

            buttonAmbient.BackColor = ColorFromVector3(material.Ambient);
            buttonDiffuse.BackColor = ColorFromVector3(material.Diffuse);
            buttonSpecular.BackColor = ColorFromVector3(material.Specular);
            buttonEmmisive.BackColor = ColorFromVector3(material.Emissive);

            buttonAmbient.ForeColor = TextColorFromVector3(material.Ambient);
            buttonDiffuse.ForeColor = TextColorFromVector3(material.Diffuse);
            buttonSpecular.ForeColor = TextColorFromVector3(material.Specular);
            buttonEmmisive.ForeColor = TextColorFromVector3(material.Emissive);

            _inUpdate = true;

            opacityTrack.Value = (Int32)Math.Min(100, material.Opacity * 100);
            specularExp.Text = String.Format("{0:0}", (Int32)material.SpecularExponent);

            String texture = material.Texture == null ? null : GetFileNameWithoutExtension(material.Texture.ToLowerInvariant());

            for (Int32 idx = 0; idx < _avaliableTextures.Count; ++idx)
            {
                String avaliableTexture = _avaliableTextures[idx];//

                if (avaliableTexture != null)
                {
                    avaliableTexture = avaliableTexture.ToLowerInvariant();
                }
                
                if (texture == avaliableTexture)
                {
                    comboTexture.SelectedIndex = idx;
                    break;
                }
            }

            _inUpdate = false;
        }

        private System.Drawing.Color ColorFromVector3(Vector3 color)
        {
            Int32 r = (Int32)Math.Min(255, color.X * 255);
            Int32 g = (Int32)Math.Min(255, color.Y * 255);
            Int32 b = (Int32)Math.Min(255, color.Z * 255);

            return System.Drawing.Color.FromArgb(r, g, b);
        }

        private System.Drawing.Color TextColorFromVector3(Vector3 color)
        {
            Single lightness = color.X * 0.299f + color.Y * 0.587f + color.Z * 0.114f;

            if (lightness > 0.5)
            {
                return System.Drawing.Color.Black;
            }

            return System.Drawing.Color.White;
        }

        private void comboMaterialsSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentSet = comboMaterialsSets.SelectedIndex;
            UpdateMaterialControls();
            UpdatePreview();
        }

        private void comboMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentMaterial = comboMaterial.SelectedIndex;
            UpdateMaterialControls();
        }

        private void opacityTrack_Scroll(object sender, EventArgs e)
        {
            if (!_inUpdate)
            {
                CurrentMaterial.Opacity = (Single)opacityTrack.Value / 100.0f;
                _preview.Invalidate();
            }
        }

        private Material CurrentMaterial
        {
            get
            {
                return _model.Materials(_currentSet)[_currentMaterial];
            }
        }

        private void specularExp_TextChanged(object sender, EventArgs e)
        {
            if (!_inUpdate)
            {
                try
                {
                    Int32 exp = Int32.Parse(specularExp.Text);
                    CurrentMaterial.SpecularExponent = exp;
                    _preview.Invalidate();
                }
                catch
                {
                }
            }
        }

        private void buttonAmbient_Click(object sender, EventArgs e)
        {
            CurrentMaterial.Ambient = GetColor(CurrentMaterial.Ambient);
            UpdateMaterialControls();
            _preview.Invalidate();
        }

        private void buttonDiffuse_Click(object sender, EventArgs e)
        {
            CurrentMaterial.Diffuse = GetColor(CurrentMaterial.Diffuse);
            UpdateMaterialControls();
            _preview.Invalidate();
        }

        private void buttonSpecular_Click(object sender, EventArgs e)
        {
            CurrentMaterial.Specular = GetColor(CurrentMaterial.Specular);
            UpdateMaterialControls();
            _preview.Invalidate();
        }

        private void buttonEmmisive_Click(object sender, EventArgs e)
        {
            CurrentMaterial.Emissive = GetColor(CurrentMaterial.Emissive);
            UpdateMaterialControls();
            _preview.Invalidate();
        }

        private Vector3 GetColor(Vector3 current)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = ColorFromVector3(current);

            if (_customColors != null)
            {
                dlg.CustomColors = _customColors;
            }

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _customColors = dlg.CustomColors;

                var color = dlg.Color;
                current = new Vector3((Single)color.R / 255.0f, (Single)color.G / 255.0f, (Single)color.B / 255.0f);
            }

            return current;
        }

        private void buttonAddSet_Click(object sender, EventArgs e)
        {
            Material[] materials = new Material[_model.Materials(_currentSet).Length];

            for (Int32 idx = 0; idx < materials.Length; ++idx)
            {
                var mat = _model.Materials(_currentSet)[idx];

                materials[idx] = new Material(mat.Texture, mat.Diffuse, mat.Ambient, mat.Specular, mat.SpecularExponent, mat.Emissive, mat.Opacity);
            }

            _currentSet = _model.MaterialsSetsCount;
            _model.MaterialsSets.Add(materials);

            UpdatePreview();

            UpdateCombos();
        }

        private void buttonDeleteSet_Click(object sender, EventArgs e)
        {
            if (_model.MaterialsSetsCount > 1)
            {
                if (MessageBox.Show("Delete material set?", "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    _model.MaterialsSets.RemoveAt(_currentSet);

                    while (_currentSet >= _model.MaterialsSetsCount)
                    {
                        _currentSet--;
                    }
                }

                UpdatePreview();
                UpdateCombos();
            }
            else
            {
                MessageBox.Show("Cannot delete last set?", "Delete");
            }
        }

        private void UpdatePreview()
        {
            ModelX newModel = ApplyChanges(_model);

            if (_previewModel != null)
            {
                _previewModel.DisposeBuffers();

                for (Int32 set = 0; set < _previewModel.MaterialsSetsCount; ++set)
                {
                    foreach (var material in _previewModel.Materials(set))
                    {
                        String texName = material.Texture;

                        if (material.Textures != null)
                        {
                            material.Textures.Dispose();
                        }
                    }
                }
            }

            _previewModel = newModel;
            LoadTextures(_previewModel, Path.GetDirectoryName(_importFile));

            _preview.Model = _previewModel;
            _preview.MaterialSet = _currentSet;
        }

        private void FillAvaliableTextures(String directory)
        {
            String[] files = Directory.GetFiles(directory, "*.png", SearchOption.TopDirectoryOnly);

            _avaliableTextures.Clear();
            comboTexture.Items.Clear();

            foreach(var file in files )
            {
                String fname = GetFileNameWithoutExtension(file);

                _avaliableTextures.Add(fname);
                comboTexture.Items.Add(_avaliableTextures.Last());
            }

            _avaliableTextures.Add(null);
            comboTexture.Items.Add("{null}");
        }

        private void comboTexture_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_inUpdate)
            {
                _model.Materials(_currentSet)[_currentMaterial].Texture = _avaliableTextures[comboTexture.SelectedIndex];
                UpdatePreview();
            }
        }

        private void checkZisUp_CheckedChanged(object sender, EventArgs e)
        {
            _preview.ZisUp = checkZisUp.Checked;
        }

    }
}
