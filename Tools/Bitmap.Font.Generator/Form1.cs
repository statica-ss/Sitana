using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Diagnostics.CodeAnalysis;
using System.IO;


namespace XnaFontGenerator
{
    public partial class XnaFontGenerator : Form
    {
        class CharacterInfo
        {
            public Char Character;
            public Int32 StartX;
            public Int32 StartY;
            public Int32 Width;
            public Int32 Height;
            public Int32 Spacing;
        }

        class Kerning
        {
            public Dictionary<Char, Single> SpacingBetweenChars = new Dictionary<Char, Single>();
        }

        private Dictionary<Char, Kerning> kerning = new Dictionary<Char, Kerning>();

        private Parameters parameters = new Parameters();

        private Bitmap globalBitmap;
        private Graphics globalGraphics;
        private Font font;
        private String fontError;
        private String lastStrokeModeBorder;

        public XnaFontGenerator()
        {
            InitializeComponent();

            InstalledFontCollection fonts = new InstalledFontCollection();

            globalBitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            globalGraphics = Graphics.FromImage(globalBitmap);

            foreach (FontFamily font in fonts.Families)
                fontName.Items.Add(font.Name);

            fontName.Text = "Segoe UI";
            lastStrokeModeBorder = "0";

            borderSize.Text = lastStrokeModeBorder;
            additionalCharacters.Text = "ĄĆĘŁŃÓŚŹŻąćęłńóśźż©•";
        }

        private void textColor_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();

            dialog.FullOpen = true;
            dialog.Color = textColor.BackColor;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textColor.BackColor = dialog.Color;
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

            if (!ParseInt(maxChar.Text, out value))
            {
                e.Cancel = true;
                return;
            }

            GeneratePreview();
        }

        private void maxChar_Validating(object sender, CancelEventArgs e)
        {
            Int32 value = 0;

            if (!ParseInt(maxChar.Text, out value))
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

            return Int32.TryParse(text, style, null, out value);
        }

        private bool ParseBorder(String text, ref Rectangle rect)
        {
            String[] texts = text.Replace(" ", "").Split(";".ToCharArray());

            Int32 left = 0;
            Int32 top = 0;
            Int32 right = 0;
            Int32 bottom = 0;

            if (!ParseInt(texts[0], out left))
                return false;

            rect = new Rectangle(-left, -top, left + right + 1, top + bottom + 1);
            return true;
        }

        private Int32 CalculateBaseLine(Bitmap bitmap)
        {
            Int32 baseLine = 0;

            for (Int32 x = 0; x < bitmap.Width; ++x)
            {
                for (Int32 y = bitmap.Height - 1; y >= 0; --y)
                {
                    Color color = bitmap.GetPixel(x, y);

                    if ( color.A > 0)
                    {
                        baseLine = Math.Max(baseLine, y);
                    }
                }
            }

            return baseLine+1;
        }

        private Int32 CalculateCapLine(Bitmap bitmap)
        {
            Int32 capLine = bitmap.Height;

            for (Int32 x = 0; x < bitmap.Width; ++x)
            {
                for (Int32 y = 0; y < bitmap.Height; ++y)
                {
                    Color color = bitmap.GetPixel(x, y);

                    if ( color.A > 0)
                    {
                        capLine = Math.Min(capLine, y);
                    }
                }
            }

            return capLine;
        }

        private Int32 FindEnd(Bitmap bitmap)
        {
            Int32 cropRight = bitmap.Width-1;

            // Remove unused space from the right.
            while ((cropRight > 0) && (BitmapIsEmpty(bitmap, cropRight)))
                cropRight--;

            return cropRight;
        }

        private Int32 FindStart(Bitmap bitmap)
        {
            Int32 cropLeft = 0;

            // Remove unused space from the right.
            while ((cropLeft < bitmap.Width-1) && (BitmapIsEmpty(bitmap, cropLeft)))
                cropLeft++;

            return cropLeft;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void export_Click(object sender, EventArgs e)
        {
            ReadProperties();

            try
            {
                // If the current font is invalid, report that to the user.
                if (fontError != null)
                    throw new ArgumentException(fontError);

                List<CharacterInfo> characters = new List<CharacterInfo>();

                // Convert the character range from string to integer,
                // and validate it.
                Int32 minChar = parameters.FirstCharacter;
                Int32 maxChar = parameters.LastCharacter;

                if ((minChar >= maxChar) ||
                    (minChar < 0) || (minChar > 0xFFFF) ||
                    (maxChar < 0) || (maxChar > 0xFFFF))
                {
                    throw new ArgumentException("Invalid character range " +
                                                minChar.ToString() + " - " + maxChar.ToString());
                }

                // Choose the output file.
                SaveFileDialog fileSelector = new SaveFileDialog();

                fileSelector.Title = "Export Font";
                fileSelector.DefaultExt = "png";
                fileSelector.Filter = "Image files (*.png)|*.png|All files (*.*)|*.*";

                if (fileSelector.ShowDialog() == DialogResult.OK)
                {
                    // Build up a list of all the glyphs to be output.
                    List<Bitmap> bitmaps = new List<Bitmap>();
                    List<Int32> xPositions = new List<Int32>();
                    List<Int32> yPositions = new List<Int32>();

                    List<Char> charactersToExport = new List<Char>();

                    for (char ch = (char)minChar; ch < maxChar; ++ch)
                    {
                        charactersToExport.Add(ch);
                    }

                    foreach (Char ch in parameters.AdditionalCharacters.ToCharArray())
                    {
                        if (!charactersToExport.Contains(ch))
                        {
                            charactersToExport.Add(ch);
                        }
                    }

                    try
                    {
                        const Int32 padding = 4;

                        Int32 width = padding;
                        Int32 height = padding;
                        Int32 lineWidth = padding;
                        Int32 lineHeight = padding;
                        Int32 count = 0;

                        Bitmap tempBitmap = GenerateStrokeCharacter('X');

                        Int32 baseLine = CalculateBaseLine(tempBitmap);
                        Int32 capLine = CalculateCapLine(tempBitmap);

                        kerning.Clear();

                        Kerning zeroKerning = new Kerning();
                        kerning.Add('\0', zeroKerning);

                        Single startAll = -10000;
                        Single endAll = -10000;

                        // Rasterize each character in turn,
                        // and add it to the output list.
                        foreach (char ch in charactersToExport)
                        {
                            kerning.Add(ch, CalculateKerning(ch, charactersToExport));
                            Bitmap bitmap = GenerateStrokeCharacter(ch);

                            Int32 end = FindEnd(bitmap) + 1;
                            Int32 start = FindStart(bitmap) - 1;

                            
                            Int32 diff = bitmap.Width-end;

                            startAll = Math.Max(-(Single)start / (Single)bitmap.Height, startAll);
                            endAll = Math.Max(-(Single)diff / (Single)bitmap.Height, endAll);

                            bitmaps.Add(bitmap);

                            xPositions.Add(lineWidth);
                            yPositions.Add(height);

                            Int32 smallerWidth = (parameters.BorderSize.Left * 2 + (Int32)(Math.Ceiling(parameters.BlurSize*2)));

                            characters.Add(
                               new CharacterInfo() { Character = ch, StartX = lineWidth, StartY = height, Width = bitmap.Width, Height = bitmap.Height, Spacing = (bitmap.Width - smallerWidth) }
                               );

                            lineWidth += bitmap.Width + padding;
                            lineHeight = Math.Max(lineHeight, bitmap.Height + padding);

                            // Output 16 glyphs per line, then wrap to the next line.
                            if (++count == 16)
                            {
                                width = Math.Max(width, lineWidth);
                                height += lineHeight;
                                lineWidth = padding;
                                lineHeight = padding;
                                count = 0;
                            }
                        }

                        foreach (char ch in charactersToExport)
                        {
                            zeroKerning.SpacingBetweenChars.Add(ch, startAll);
                            kerning[ch].SpacingBetweenChars.Add('\0', endAll);
                        }

                        if (count != 0)
                        {
                            width = Math.Max(width, lineWidth);
                            height += lineHeight;
                            lineWidth = padding;
                            lineHeight = padding;
                        }

                        using (Bitmap bitmap = new Bitmap(width, height,
                                                          PixelFormat.Format32bppArgb))
                        {
                            // Arrage all the glyphs onto a single larger bitmap.
                            using (Graphics graphics = Graphics.FromImage(bitmap))
                            {
                                graphics.Clear(Color.Transparent);
                                graphics.CompositingMode = CompositingMode.SourceCopy;

                                for (Int32 i = 0; i < bitmaps.Count; i++)
                                {
                                    graphics.DrawImage(bitmaps[i], xPositions[i],
                                                                   yPositions[i]);
                                }

                                graphics.Flush();
                            }

                            // Save out the combined bitmap.
                            bitmap.Save(fileSelector.FileName, ImageFormat.Png);

                            String filePath = Path.GetDirectoryName(fileSelector.FileName);
                            String fileName = Path.GetFileNameWithoutExtension(fileSelector.FileName);
                            String infoFile = Path.Combine(filePath, fileName + ".ff0");

                            using (Stream stream = new FileStream(infoFile, FileMode.Create))
                            {
                                BinaryWriter writer = new BinaryWriter(stream);

                                writer.Write(characters.Count);

                                foreach (var character in characters)
                                {
                                    writer.Write(character.Character);
                                    writer.Write(character.StartX);
                                    writer.Write(character.StartY);
                                    writer.Write(character.Width);
                                    writer.Write(character.Height);
                                    writer.Write(character.Spacing);
                                }

                                writer.Write(capLine);
                                writer.Write(baseLine);

                                writer.Write(kerning.Count);

                                foreach (var kern in kerning)
                                {
                                    writer.Write(kern.Key);
                                    writer.Write(kern.Value.SpacingBetweenChars.Count);

                                    foreach (var info in kern.Value.SpacingBetweenChars)
                                    {
                                        writer.Write(info.Key);
                                        writer.Write(info.Value);
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        // Clean up temporary objects.
                        foreach (Bitmap bitmap in bitmaps)
                            bitmap.Dispose();
                    }
                }
            }
            catch (Exception exception)
            {
                // Report any errors to the user.
                MessageBox.Show(exception.Message, Text + " Error");
            }
        }

        private Kerning CalculateKerning(Char ch, List<Char> charactersToExport)
        {
            Kerning kerning = new Kerning();

            globalGraphics.PageUnit = GraphicsUnit.Point;
            
            SizeF size = globalGraphics.MeasureString(ch.ToString(), font);

            foreach (var next in charactersToExport)
            {
                SizeF aloneSize = globalGraphics.MeasureString(next.ToString(), font);
                SizeF bothSize = globalGraphics.MeasureString(ch.ToString() + next.ToString(), font);

                Single diff = bothSize.Width - (aloneSize.Width + size.Width);

                Single kern = diff / size.Height;

                kerning.SpacingBetweenChars.Add(next, kern);
            }


            return kerning;
        }

        private Bitmap GenerateStrokeCharacter(Char ch)
        {
            Color stroke = parameters.BorderColor;

            Double alpha = (parameters.BorderOpacity) * 255.0;

            stroke = Color.FromArgb((Byte)Math.Min(255, alpha), stroke);

            Color fill = parameters.TextColor;

            String text = ch.ToString();

            globalGraphics.PageUnit = GraphicsUnit.Point;
            SizeF size = globalGraphics.MeasureString(text, font);

            Int32 strokeWidth = -parameters.BorderSize.X;
            Int32 additional = (Int32)(strokeWidth);

            Int32 width = (Int32)Math.Ceiling(size.Width) + additional * 2;
            Int32 height = (Int32)Math.Ceiling(size.Height) + strokeWidth;

            Single offset = additional;

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.Clear(Color.Transparent);

                using (Pen pen = new Pen(stroke, (Single)strokeWidth))
                using (Brush brush = new SolidBrush(fill))
                using (StringFormat format = new StringFormat())
                using (GraphicsPath path = new GraphicsPath(FillMode.Winding))
                {
                    pen.LineJoin = LineJoin.Round;
                    pen.EndCap = LineCap.Round;
                    
                    path.AddString(text, font.FontFamily, (Int32)font.Style, font.Size, new PointF(offset, additional), StringFormat.GenericDefault);

                    graphics.DrawPath(pen, path);
                }
            }

            if (parameters.BlurSize > 0)
            {
                bitmap = Blur(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), parameters.BlurSize);
            }

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (Pen pen = new Pen(stroke, (Single)strokeWidth))
                using (Brush brush = new SolidBrush(fill))
                using (StringFormat format = new StringFormat())
                using (GraphicsPath path = new GraphicsPath(FillMode.Winding))
                {
                    pen.LineJoin = LineJoin.Round;
                    pen.EndCap = LineCap.Round;

                    path.AddString(text, font.FontFamily, (Int32)font.Style, font.Size, new PointF(offset, additional), StringFormat.GenericDefault);
                    // Draw the text by filling the path
                    graphics.FillPath(brush, path);
                }

                graphics.Flush();
            }

            return bitmap;
        }

        private static Bitmap Blur(Bitmap image, Rectangle rectangle, double blurSizeFloat)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
            {
                graphics.Clear(Color.Transparent);
            }

            Int32 blurSize = (Int32)Math.Ceiling(blurSizeFloat);

            // look at every pixel in the blur rectangle
            for (Int32 xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                Int32 startX = Math.Max(0, xx - blurSize);


                for (Int32 yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    Int32 startY = Math.Max(0, yy - blurSize);

                    Int32 avgR = 0, avgG = 0, avgB = 0, avgA = 0;
                    double blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (Int32 x = startX; (x <= xx + blurSize && x < image.Width); x++)
                    {
                        for (Int32 y = startY; (y <= yy + blurSize && y < image.Height); y++)
                        {
                            Color pixel = image.GetPixel(x, y);

                            double a = pixel.A / 255.0;
                            double r = pixel.R / 255.0 * a;
                            double g = pixel.G / 255.0 * a;
                            double b = pixel.B / 255.0 * a;

                            double dist = (x - xx) * (x - xx) + (y - yy) * (y - yy);
                            double maxDist = 2 * blurSizeFloat * blurSizeFloat;
                            dist = Math.Min(maxDist, dist);

                            dist /= maxDist;

                            double mul = 1 - dist;
                            mul *= mul * mul;

                            a *= mul;
                            r *= mul;
                            g *= mul;
                            b *= mul;

                            avgR += (int)(r * 255.0);
                            avgG += (int)(g * 255.0);
                            avgB += (int)(b * 255.0);
                            avgA += (int)(a * 255.0);

                            blurPixelCount += mul;
                        }
                    }

                    avgA = Math.Min(255, (Int32)(avgA / blurPixelCount));

                    double alpha = avgA / 255.0;

                    if (alpha > 0)
                    {
                        avgR = Math.Min(255, (Int32)(avgR / alpha / blurPixelCount));
                        avgG = Math.Min(255, (Int32)(avgG / alpha / blurPixelCount));
                        avgB = Math.Min(255, (Int32)(avgB / alpha / blurPixelCount));
                    }
                    else
                    {
                        avgB = avgG = avgR = 0;
                    }



                    blurred.SetPixel(xx, yy, Color.FromArgb(avgA, avgR, avgG, avgB));
                }
            }

            return blurred;
        }

        /// <summary>
        /// Helper for rendering out a single font character
        /// into a System.Drawing bitmap.
        /// </summary>
        private Bitmap RasterizeCharacter(Char ch, Color color)
        {
            String text = ch.ToString();

            SizeF size = globalGraphics.MeasureString(text, font);

            Int32 width = (Int32)Math.Ceiling(size.Width);
            Int32 height = (Int32)Math.Ceiling(size.Height);

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.TextRenderingHint =
                   TextRenderingHint.AntiAliasGridFit;

                graphics.Clear(Color.Transparent);

                using (Brush brush = new SolidBrush(color))
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Near;

                    graphics.DrawString(text, font, brush, 0, 0, format);
                }

                graphics.Flush();
            }

            return bitmap;//CropCharacter(bitmap);
        }

        /// <summary>
        /// Helper for cropping ununsed space from the sides of a bitmap.
        /// </summary>
        private static Bitmap CropCharacter(Bitmap bitmap)
        {
            Int32 cropLeft = 0;
            Int32 cropRight = bitmap.Width - 1;

            // Remove unused space from the left.
            while ((cropLeft < cropRight) && (BitmapIsEmpty(bitmap, cropLeft)))
                cropLeft++;

            // Remove unused space from the right.
            while ((cropRight > cropLeft) && (BitmapIsEmpty(bitmap, cropRight)))
                cropRight--;

            // Don't crop if that would reduce the glyph down to nothing at all!
            if (cropLeft == cropRight)
                return bitmap;

            // Add some padding back in.
            cropLeft = Math.Max(cropLeft, 0);
            cropRight = Math.Min(cropRight, bitmap.Width - 1);

            Int32 width = cropRight - cropLeft + 2;

            // Crop the glyph.
            Bitmap croppedBitmap = new Bitmap(width, bitmap.Height, bitmap.PixelFormat);

            using (Graphics graphics = Graphics.FromImage(croppedBitmap))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;

                graphics.DrawImage(bitmap, 0, 0,
                                   new Rectangle(cropLeft, 0, width, bitmap.Height),
                                   GraphicsUnit.Pixel);

                graphics.Flush();
            }

            bitmap.Dispose();

            return croppedBitmap;
        }


        /// <summary>
        /// Helper for testing whether a column of a bitmap is entirely empty.
        /// </summary>
        private static bool BitmapIsEmpty(Bitmap bitmap, Int32 x)
        {
            for (Int32 y = 0; y < bitmap.Height; y++)
            {
                if (bitmap.GetPixel(x, y).A != 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// When the font selection changes, create a new Font
        /// instance and update the preview text label.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void ReadProperties()
      {
         try
            {
                // Parse the size selection.
                float size;

                if (!float.TryParse(fontSize.Text, out size) || (size <= 0))
                {
                    fontError = "Invalid font size '" + fontSize.Text + "'";
                    return;
                }

                // Parse the font style selection.
                FontStyle style;

                try
                {
                    style = (FontStyle)Enum.Parse(typeof(FontStyle), fontStyle.Text);
                }
                catch
                {
                    fontError = "Invalid font style '" + fontStyle.Text + "'";
                    return;
                }


                using (Graphics graphics = Graphics.FromHwnd(this.Handle))
                {
                    if (sizeInPixels.Checked)
                    {
                        Single dpi = Math.Max(graphics.DpiX, graphics.DpiY);
                        size = 72 / dpi * size;
                    }
                }

                // Create the new font.
                Font newFont = new Font(fontName.Text, size, style);

                if (font != null)
                    font.Dispose();

                font = newFont;

                fontError = null;
            }
            catch (Exception exception)
            {
                fontError = exception.Message;
            }

         parameters.BorderColor = borderColor.BackColor;
         parameters.TextColor = textColor.BackColor;
         parameters.BorderOpacity = Decimal.ToDouble(borderOpacity.Value);
         parameters.BlurSize = Decimal.ToDouble(blurSize.Value);
         ParseInt(minChar.Text, out parameters.FirstCharacter);
         ParseInt(maxChar.Text, out parameters.LastCharacter);
         
         parameters.AdditionalCharacters = additionalCharacters.Text;

         ParseBorder(borderSize.Text, ref parameters.BorderSize);
      }

        private void GeneratePreview()
        {
            ReadProperties();

            String previewText = "AWp%^0i12@ś!źŹ";
            Image[] images = new Image[previewText.Length];

            Int32 width = 0;
            Int32 height = 0;

            for (Int32 index = 0; index < previewText.Length; ++index)
            {
                Char character = previewText.ElementAt(index);

                images[index] = GenerateStrokeCharacter(character);

                width += images[index].Width + 1;
                height = Math.Max(height, images[index].Height);
            }

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            Graphics graphics = Graphics.FromImage(bitmap);

            Int32 position = 0;

            for (Int32 index = 0; index < images.Length; ++index)
            {
                graphics.DrawImageUnscaled(images[index], position, 0);

                position += images[index].Width + 1;
            }

            preview.Image = bitmap;
        }

        private void borderSize_Validating(object sender, CancelEventArgs e)
        {
            if (!ParseBorder(borderSize.Text, ref parameters.BorderSize))
            {
                e.Cancel = true;
                return;
            }

            GeneratePreview();
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
    }
}
