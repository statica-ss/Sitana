using System;
using Sitana.Framework.Graphics;
using Sitana.Framework.Cs;
using Sitana.Framework.Content;
using Microsoft.Xna.Framework;
using System.Text;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;

namespace Sitana.Framework.Ui.Views.ButtonDrawables
{
    public class EditText: Text
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            Text.Parse(node, file);
        }

        private FontFace _fontFace = null;

        StringBuilder _string = new StringBuilder();

        double _flash = 0;

        public override void Draw(AdvancedDrawBatch drawBatch, DrawButtonInfo info)
        {
			int carretPosition = info.Additional != null ? (int)info.Additional : -1;

            Update(info.EllapsedTime, info.ButtonState);

            if( info.ButtonState.HasFlag(ButtonState.Checked) && carretPosition>=0)
            {
                _flash += info.EllapsedTime * 2;
                _flash %= 2;
            }
            else
            {
                _flash = 0;
            }

            SharedString str = info.Text;

            if (_fontFace == null)
            {
                _fontFace = FontManager.Instance.FindFont(_font);
            }

            float scale;
            UniversalFont font = _fontFace.Find(_fontSize, out scale);

            Color color = ColorFromState * info.Opacity * Opacity;

            Rectangle target = _margin.ComputeRect(info.Target);
            float spacing = (float)_fontSpacing / 1000.0f;

            lock(str)
            {
                StringBuilder builder = str.StringBuilder;

                if (_flash > 1)
                {
                    _string.Clear();

                    for (int idx = 0; idx < carretPosition; ++idx)
                    {
                        _string.Append(builder[idx]);
                    }

                    carretPosition = (int)(font.MeasureString(_string, spacing, 0) * scale).X;
                }

                _string.Clear();
                _string.Append(builder);
            }

            Vector2 size = font.MeasureString(_string, spacing, 0) * scale;

            int positionX = target.X;

            switch (_textAlign & TextAlign.Horz)
            {
            case TextAlign.Right:
                positionX = target.Right - (int)size.X;
                break;

            case TextAlign.Center:
                positionX = target.Center.X - (int)size.X / 2;
                break;
            }

            target.X = positionX;

            drawBatch.DrawText(font, _string, target, _textAlign & TextAlign.Vert, color, spacing, 0, scale);

            if(_flash>1)
            {
                target.X += carretPosition;

                drawBatch.DrawText(font, "|", target, _textAlign & TextAlign.Vert, color, spacing, 0, scale);
            }
        }

//        public override object OnAction(DrawButtonInfo info, params object[] parameters)
//        {
//            SharedString str = info.Text;
//
//            int clickPosition = (int)parameters[0];
//
//            if (_fontFace == null)
//            {
//                _fontFace = FontManager.Instance.FindFont(_font);
//            }
//
//            float scale;
//            UniversalFont font = _fontFace.Find(_fontSize, out scale);
//
//            Rectangle target = _margin.ComputeRect(info.Target);
//            float spacing = (float)_fontSpacing / 1000.0f;
//
//            int position = 0;
//
//            Vector2 size = Vector2.Zero;
//
//            lock(str)
//            {
//                size = font.MeasureString(str.StringBuilder, spacing, 0) * scale;
//            }
//
//            int positionX = target.X;
//
//            switch (_textAlign & TextAlign.Horz)
//            {
//            case TextAlign.Right:
//                positionX = target.Right - (int)size.X;
//                break;
//
//            case TextAlign.Center:
//                positionX = target.Center.X - (int)size.X / 2;
//                break;
//            }
//
//            lock(str)
//            {
//                StringBuilder builder = str.StringBuilder;
//                _string.Clear();
//
//                int oldPosition = positionX;
//
//                for(int idx = 0; idx < builder.Length; ++idx)
//                {
//                    _string.Append(builder[idx]);
//                    int pos = (int)(font.MeasureString(_string, spacing, 0) * scale).X + positionX;
//
//                    if(clickPosition>=oldPosition && clickPosition<pos)
//                    {
//                        position = idx;
//                        break;
//                    }
//
//                    oldPosition = pos;
//                }
//
//                position = builder.Length;
//            }
//
//            return position;
//        }
    }
}

