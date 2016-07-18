using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework;
using Sitana.Framework.Graphics;
using System;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace Sitana.Framework.Ui.Views.HtmlRendererImpl
{
    public class HtmlViewGraphics : RGraphics
    {
        AdvancedDrawBatch _drawBatch;

        Point _topLeft;
        Rectangle _clickedArea = Rectangle.Empty;
        Color _clickedColor = Color.White;

        public HtmlViewGraphics(AdvancedDrawBatch drawBatch): base(HtmlViewAdapter.Instance, new RRect(0,0,int.MaxValue, int.MaxValue))
        {
            _drawBatch = drawBatch;
            _topLeft = Point.Zero;
        }

        public void Prepare(Point topLeft)
        {
            _topLeft = topLeft;
        }

        public void AddActiveArea(Rectangle area, Color color)
        {
            _clickedArea = area;
            _clickedColor = color;
        }

        public override void Dispose()
        {
            _drawBatch.Flush();
        }

        public override void DrawImage(RImage image, RRect destRect)
        {
            Rectangle destRectXna = destRect.ToXnaRectangle();

            destRectXna.Offset(_topLeft);
            _drawBatch.DrawImage((image as HtmlViewImage).Texture, destRectXna, new Rectangle(0, 0, (int)image.Width, (int)image.Height), Color.White);
        }

        public override void DrawImage(RImage image, RRect destRect, RRect srcRect)
        {
            Rectangle destRectXna = destRect.ToXnaRectangle();

            destRectXna.Offset(_topLeft);
            _drawBatch.DrawImage((image as HtmlViewImage).Texture, destRectXna, srcRect.ToXnaRectangle(), Color.White);
        }

        public override void DrawLine(RPen pen, double x1, double y1, double x2, double y2)
        {
            _drawBatch.DrawTextureLine(new Point((int)x1, (int)y1) + _topLeft, new Point((int)x2, (int)y2) + _topLeft, (pen as HtmlViewPen).Color, (float)pen.Width);
        }

        public override void DrawPath(RBrush brush, RGraphicsPath path)
        {
            //throw new NotImplementedException();
        }

        public override void DrawPath(RPen pen, RGraphicsPath path)
        {
            //throw new NotImplementedException();
        }

        public override void DrawPolygon(RBrush brush, RPoint[] points)
        {
            //throw new NotImplementedException();
        }

        public override void DrawRectangle(RBrush brush, double x, double y, double width, double height)
        {
            _drawBatch.DrawRectangle(new Rectangle((int)x + _topLeft.X, (int)y + _topLeft.Y, (int)width, (int)height), (brush as HtmlViewBrush).Color);
        }

        public override void DrawRectangle(RPen pen, double x, double y, double width, double height)
        {
            _drawBatch.DrawRectangleBorder(new Rectangle((int)x + _topLeft.X, (int)y + _topLeft.Y, (int)width, (int)height), (pen as HtmlViewPen).Color);
        }

        public override void DrawString(string str, RFont font, RColor color, RPoint point, RSize size, bool rtl)
        {
            Point xnaPoint = new Point((int)point.X, (int)point.Y);
            Color xnaColor = color.ToXnaColor();

            if (!_clickedArea.IsEmpty)
            {
                Point middle = new Point(xnaPoint.X + (int)size.Width / 2, xnaPoint.Y + (int)size.Height / 2);
                
                if(_clickedArea.Contains(middle))
                {
                    xnaColor = _clickedColor;
                }
            }

            UiFont uiFont = (font as HtmlViewFont).UiFont;
            _drawBatch.DrawText(uiFont.Font, str, xnaPoint + _topLeft, TextAlign.Left, xnaColor, uiFont.Spacing, uiFont.LineHeight, uiFont.Scale);
        }

        public override RGraphicsPath GetGraphicsPath()
        {
            return null;
            //throw new NotImplementedException();
        }

        public override RBrush GetTextureBrush(RImage image, RRect dstRect, RPoint translateTransformLocation)
        {
            throw new NotImplementedException();
        }

        public override RSize MeasureString(string str, RFont font)
        {
            var theFont = font as HtmlViewFont;
            var size = theFont.UiFont.MeasureString(str);

            return new RSize(size.X, size.Y);
        }

        public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth)
        {
            throw new NotImplementedException();
        }

        public override void PopClip()
        {
         //   _drawBatch.PopClip();
        }

        public override void PushClip(RRect rect)
        {
            //Rectangle xnaRect = rect.ToXnaRectangle();
            //xnaRect.Offset(_topLeft);
            //_drawBatch.PushClip(xnaRect);
        }

        public override void PushClipExclude(RRect rect)
        {
            throw new NotImplementedException();
        }

        public override void ReturnPreviousSmoothingMode(object prevMode)
        {
            if (prevMode != null)
            {
                _drawBatch.SamplerState = (SamplerState)prevMode;
            }
        }

        public override object SetAntiAliasSmoothingMode()
        {
            var samplerState = _drawBatch.SamplerState;
            _drawBatch.SamplerState = SamplerState.LinearClamp;
            return samplerState;
        }
    }
}
