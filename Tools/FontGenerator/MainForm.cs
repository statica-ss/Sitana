using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing.Text;

using SitanaFont = Sitana.Framework.Graphics.Font;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;

namespace FontGenerator
{
    public partial class MainForm : Form
    {
        List<char> _previewCharacters = new List<char>() {'A','b','c','@','!','0','1','2' };

        private bool _skipGeneration = true;

        public MainForm()
        {
            InitializeComponent();

            InstalledFontCollection fonts = new InstalledFontCollection();

            foreach (FontFamily font in fonts.Families)
                FontFace.Items.Add(font.Name);

            Settings.Instance.Init();

            FontFace.Text = Settings.Instance.Face;
            FontSize.Text = Settings.Instance.Size.ToString();
            FontStyle.SelectedIndex = Settings.Instance.Style;

            BorderColor.BackColor = Color.FromArgb(Settings.Instance.BorderColor);
            FillColor.BackColor = Color.FromArgb(Settings.Instance.FillColor);

            BorderSize.Value = Settings.Instance.BorderSize;
            BorderOpacity.Value = (Decimal)((double)Settings.Instance.BorderOpacity / 100.0);

            MinChar.Text = Settings.Instance.MinChar;
            MaxChar.Text = Settings.Instance.MaxChar;

            AdditionalCharacters.Text = Settings.Instance.AdditionalCharacters;

            BorderRound.Checked = Settings.Instance.RoundBorder;

            _skipGeneration = false;
            GeneratePreview();
            
        }

        private void textColor_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();

            dialog.FullOpen = true;
            dialog.Color = FillColor.BackColor;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FillColor.BackColor = dialog.Color;
                GeneratePreview();
            }
        }

        private void borderColor_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();

            dialog.FullOpen = true;
            dialog.Color = BorderColor.BackColor;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                BorderColor.BackColor = dialog.Color;
                GeneratePreview();
            }
        }

        private void minChar_Validating(object sender, CancelEventArgs e)
        {
            Int32 value = 0;

            if (!ParseInt(MaxChar.Text, out value))
            {
                e.Cancel = true;
                return;
            }

            GeneratePreview();
        }

        private void maxChar_Validating(object sender, CancelEventArgs e)
        {
            Int32 value = 0;

            if (!ParseInt(MaxChar.Text, out value))
            {
                e.Cancel = true;
                return;
            }

            GeneratePreview();
        }

        static private bool ParseInt(String text, out Int32 value)
        {
            NumberStyles style;

            if (text.StartsWith("0x"))
            {
                style = NumberStyles.HexNumber;
                text = text.Substring(2);
            }
            else
            {
                style = NumberStyles.Integer;
            }

            return int.TryParse(text, style, null, out value);
        }

        private void ReadProperties()
        {
            int value = 0;
            ParseInt(MaxChar.Text, out value);
        }

        void GeneratePreview()
        {
            if (_skipGeneration)
            {
                return;
            }

            Bitmap bitmap;
            var font = Generate(_previewCharacters, preview.Width - 10, out bitmap);

            if (preview.Image != null)
            {
                preview.Image.Dispose();
            }

            preview.Image = bitmap;
        }

        private void fontSize_TextUpdate(object sender, EventArgs e)
        {
            GeneratePreview();
        }

        private void borderOpacity_ValueChanged(object sender, EventArgs e)
        {
            GeneratePreview();
        }

        private void blurSize_ValueChanged(object sender, EventArgs e)
        {
            GeneratePreview();
        }

        private void fontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            GeneratePreview();
        }

        private void fontStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            GeneratePreview();
        }

        private void fontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            GeneratePreview();
        }

        private void BorderSize_ValueChanged(object sender, EventArgs e)
        {
            GeneratePreview();
        }

        private void BorderRound_CheckedChanged(object sender, EventArgs e)
        {
            GeneratePreview();
        }

        private void SaveSettings()
        {
            Settings.Instance.Face = FontFace.Text;
            Settings.Instance.Size = int.Parse(FontSize.Text);
            Settings.Instance.Style = FontStyle.SelectedIndex;

            Settings.Instance.BorderColor = BorderColor.BackColor.ToArgb();
            Settings.Instance.FillColor = FillColor.BackColor.ToArgb();

            Settings.Instance.BorderSize = (int)BorderSize.Value;
            Settings.Instance.BorderOpacity = (int)(BorderOpacity.Value * 100);


            Settings.Instance.MinChar = MinChar.Text;
            Settings.Instance.MaxChar = MaxChar.Text;

            Settings.Instance.AdditionalCharacters = AdditionalCharacters.Text;

            Settings.Instance.RoundBorder = BorderRound.Checked;

            Settings.Instance.Serialize();
        }

        private SitanaFont Generate(List<char> characters, int width, out Bitmap bitmap)
        {
            SaveSettings();

            SitanaFont sitanaFont;

            FontStyle style = (FontStyle)Enum.Parse(typeof(FontStyle), FontStyle.Text);
            int size = int.Parse(FontSize.Text);
            
            using (Font font = new Font(new FontFamily(FontFace.Text), size, style, GraphicsUnit.Point))
            {
                using (Brush brush = new SolidBrush(FillColor.BackColor))
                {
                    Pen pen = null;

                    if ( BorderOpacity.Value > 0 && BorderSize.Value > 0)
                    {
                        float border = (float)BorderSize.Value * (float)size / 100f;
                        int alpha = (int)((double)BorderOpacity.Value * 255.0);

                        pen = new Pen(Color.FromArgb(alpha, BorderColor.BackColor), border);

                        if ( BorderRound.Checked )
                        {
                            pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                        }
                        else
                        {
                            pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Miter;
                        }
                    }

                    new SitanaFontGenerator(font, pen, brush).Generate(characters, width, out sitanaFont, out bitmap);

                    return sitanaFont;
                }
            }
        }

        private void GenerateBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileSelector = new SaveFileDialog();

            fileSelector.Title = "Export Font";
            fileSelector.DefaultExt = "png";
            fileSelector.Filter = "Image files (*.png)|*.png|All files (*.*)|*.*";

            if (fileSelector.ShowDialog() == DialogResult.OK)
            {
                List<char> list = new List<char>();

                int minChar;
                int maxChar;

                ParseInt(MinChar.Text, out minChar);
                ParseInt(MaxChar.Text, out maxChar);

                for (int idx = minChar; idx <= maxChar; ++idx)
                {
                    list.Add((char)idx);
                }

                foreach (var ch in AdditionalCharacters.Text)
                {
                    if (!list.Contains(ch))
                    {
                        list.Add(ch);
                    }
                }

                Bitmap bitmap;
                SitanaFont font = Generate(list, 2048, out bitmap);

                string directory = Path.GetDirectoryName(fileSelector.FileName);
                string fileNoExt = Path.GetFileNameWithoutExtension(fileSelector.FileName);
                string infoFile = fileNoExt + ".sft";

                infoFile = Path.Combine(directory, infoFile);

                font.FontSheetPath = String.Empty;// fileNoExt;

                using (Stream stream = new FileStream(infoFile, FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        font.Save(writer);
                    }
                }

                bitmap.Save(fileSelector.FileName, ImageFormat.Png);
            }
        }

        
    }
}
