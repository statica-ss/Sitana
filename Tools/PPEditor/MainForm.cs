using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FarseerPhysics.Dynamics;
using Sitana.Framework.PP.Elements;

namespace Editor
{
    public partial class MainForm : Form
    {
        private EditControl _editControl;
        private MouseOperationType _lastType = MouseOperationType.None;
        private PpElement _lastSelection = null;

        Dictionary<MouseOperationType, ToolStripButton> _toolbarMap = new Dictionary<MouseOperationType, ToolStripButton>();

        public MainForm()
        {
            InitializeComponent();

            _editControl = new EditControl();

            _placeholder.Parent.Controls.Add(_editControl);

            _editControl.Bounds = _placeholder.Bounds;
            _editControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            _placeholder.Dispose();

            _toolbarMap.Add(MouseOperationType.Selection, _buttonOperationPan);
            _toolbarMap.Add(MouseOperationType.Polygon, _buttonOperationPolygon);
            _toolbarMap.Add(MouseOperationType.Rectangle, _buttonOperationRectangle);
            _toolbarMap.Add(MouseOperationType.Circle, _buttonOperationEllipse);

            Application.Idle += new EventHandler(Application_Idle);

            _propertiesToolbar.Visible = false;
            

            KeyPreview = true;
        }

        void UpdateProperties()
        {
            _propertyDensity.Text = String.Format("{0}", _lastSelection.Density);

            if (_lastSelection.Type.HasValue)
            {
                switch (_lastSelection.Type.Value)
                {
                    case BodyType.Static:
                        _propertyType.SelectedIndex = 0;
                        break;

                    case BodyType.Dynamic:
                        _propertyType.SelectedIndex = 1;
                        break;

                    case BodyType.Kinematic:
                        _propertyType.SelectedIndex = 2;
                        break;
                }
            }
            else
            {
                _propertyType.SelectedIndex = 3;
            }

            
        }

        void Application_Idle(object sender, EventArgs e)
        {
            bool snap = EditView.Instance.SnapToGrid;

            if (_snapToGrid.Checked != snap)
            {
                _snapToGrid.Checked = snap;
            }

            if (EditView.Instance.Selection != _lastSelection)
            {
                _lastSelection = EditView.Instance.Selection;

                if (_lastSelection == null)
                {
                    _propertiesToolbar.Visible = false;
                }
                else
                {
                    _propertiesToolbar.Visible = true;
                    UpdateProperties();
                }
            }

            if (EditView.Instance.Operation.Type != _lastType)
            {
                _lastType = EditView.Instance.Operation.Type;
                foreach (var btn in _toolbarMap)
                {
                    if (btn.Key != _lastType)
                    {
                        btn.Value.Checked = false;
                    }
                }

                ToolStripButton button;
                _toolbarMap.TryGetValue(_lastType, out button);

                if (button != null)
                {
                    button.Checked = true;
                }

                _editControl.Invalidate();
            }
        }

        private void _buttonOperationSelection_Click(object sender, EventArgs e)
        {
            EditView.Instance.Operation = new OperationSelection();
        }

        private void _buttonOperationAddPolygon_Click(object sender, EventArgs e)
        {
            EditView.Instance.Operation = new OperationPolygon();
        }

        private void _buttonOperationRectangle_Click(object sender, EventArgs e)
        {
            EditView.Instance.Operation = new OperationRectangle();
        }

        private void _buttonOperationEllipse_Click(object sender, EventArgs e)
        {
            EditView.Instance.Operation = new OperationCircle();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Document.Instance.New();
            EditView.Instance.Operation = new OperationSelection();
            EditView.Instance.Selection = null;

            _editControl.Invalidate();
        }

        private void _propertyColor_Click(object sender, EventArgs e)
        {
            if (_lastSelection == null) return;

            Microsoft.Xna.Framework.Color color = _lastSelection.Color;

            ColorDialog dlg = new ColorDialog();
            dlg.Color = System.Drawing.Color.FromArgb(color.R, color.G, color.B);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                System.Drawing.Color result = dlg.Color;
                _lastSelection.Color = new Microsoft.Xna.Framework.Color(result.R, result.G, result.B);

                OperationAdd.DefaultColor = _lastSelection.Color;

                _editControl.Invalidate();
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control && !e.Shift && !e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.S:
                        EditView.Instance.SnapToGrid = !EditView.Instance.SnapToGrid;
                        break;
                }
            }

            if (e.Control && !e.Shift && !e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.S:
                        Save();
                        break;

                    case Keys.O:
                        Open();
                        break;
                }
            }

            if (e.Control && e.Shift && !e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.S:
                        SaveAs();
                        break;
                }
            }

            if (EditView.Instance.Operation.KeyDown(e.KeyCode, e.Control, e.Shift))
            {
                _editControl.Invalidate();
            }
        }

        void Open()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "PP Map (*.pp)|*.pp";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Document.Instance.Open(dlg.FileName);
                _editControl.Invalidate();
            }
        }

        void Save()
        {
            if (Document.Instance.Filename == null)
            {
                SaveAs();
            }
            else
            {
                Document.Instance.Save(Document.Instance.Filename);
            }
        }

        void SaveAs()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "PP Map (*.pp)|*.pp";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Document.Instance.Save(dlg.FileName);
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Open();
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void _snapToGrid_Click(object sender, EventArgs e)
        {
            EditView.Instance.SnapToGrid = !EditView.Instance.SnapToGrid;
        }

        private void _propertyDensity_Leave(object sender, EventArgs e)
        {
            float result;
            if (!float.TryParse(_propertyDensity.Text, out result))
            {
                UpdateProperties();
            }
            else
            {
                _lastSelection.Density = result;
            }
        }

        private void _propertyType_DropDownClosed(object sender, EventArgs e)
        {
            if ( _lastSelection==null)return;

            switch (_propertyType.SelectedIndex)
            {
                case 0:
                    _lastSelection.Type = BodyType.Static;
                    break;

                case 1:
                    _lastSelection.Type = BodyType.Dynamic;
                    break;

                case 2:
                    _lastSelection.Type = BodyType.Kinematic;
                    break;

                case 3:
                    _lastSelection.Type = null;
                    break;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }
    }
}
