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

        public MainForm()
        {
            InitializeComponent();

            InstalledFontCollection fonts = new InstalledFontCollection();

            foreach (FontFamily font in fonts.Families)
                fontName.Items.Add(font.Name);

            fontName.Text = "Segoe UI";
            AdditionalCharacters.Text = "";
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
            dialog.Color = borderColor.BackColor;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                borderColor.BackColor = dialog.Color;
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
            Bitmap bitmap;
            var font = Generate(_previewCharacters, out bitmap);

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

        private SitanaFont Generate(List<char> characters, out Bitmap bitmap)
        {
            SitanaFont sitanaFont;

            FontStyle style = (FontStyle)Enum.Parse(typeof(FontStyle), fontStyle.Text);
            int size = int.Parse(fontSize.Text);

            using (Font font = new Font(new FontFamily(fontName.Text), size, style))
            {
                using (Brush brush = new SolidBrush(FillColor.BackColor))
                {
                    Pen pen = null;

                    if ( BorderOpacity.Value > 0 && BorderSize.Value > 0)
                    {
                        float border = (float)BorderSize.Value;
                        int alpha = (int)((double)BorderOpacity.Value * 255.0);

                        pen = new Pen(Color.FromArgb(alpha, borderColor.BackColor), border);

                        if ( BorderRound.Checked )
                        {
                            pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                        }
                        else
                        {
                            pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Miter;
                        }
                    }

                    new SitanaFontGenerator(font, pen, brush).Generate(characters, 256, out sitanaFont, out bitmap);

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
                SitanaFont font = Generate(list, out bitmap);

                string directory = Path.GetDirectoryName(fileSelector.FileName);
                string infoFile = Path.GetFileNameWithoutExtension(fileSelector.FileName) + ".sft";

                infoFile = Path.Combine(directory, infoFile);

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
