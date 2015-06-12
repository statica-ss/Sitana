using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views.Parameters;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Diagnostics;
using Sitana.Framework.Ui.Views.ButtonDrawables;
using Microsoft.Xna.Framework;
using Sitana.Framework.Cs;
using Sitana.Framework.Input.TouchPad;
using Sitana.Framework.Ui.Interfaces;

namespace Sitana.Framework.Ui.Views
{
    public class UiIndexSelector : UiButton
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiButton.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Context"] = parser.ParseString("Context");

            file["Spacing"] = parser.ParseLength("Spacing");

            file["ElementWidth"] = parser.ParseLength("ElementWidth");
            file["ElementHeight"] = parser.ParseLength("ElementHeight");
            file["Mode"] = parser.ParseEnum<Mode>("Mode");

            file["HorizontalContentAlignment"] = parser.ParseEnum<HorizontalContentAlignment>("HorizontalContentAlignment");
            file["VerticalContentAlignment"] = parser.ParseEnum<VerticalContentAlignment>("VerticalContentAlignment");
        }

        enum Mode
        {
            Horizontal,
            Vertical
        }

        private Length _spacing;
        private Length _elementWidth;
        private Length _elementHeight;
        private int _pushedIndex = -1;

        private bool _vertical = false;

        private HorizontalContentAlignment _contentHorizontalAlignment;
        private VerticalContentAlignment _contentVerticalAlignment;

        private string _context;
        IIndexedElement _element;

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            int spacing = _spacing.Compute(_vertical ? Bounds.Width : Bounds.Height);

            Rectangle rect = GetFirstRect();

            int size = _vertical ? rect.Height : rect.Width;
            int selected = _element.SelectedIndex;

            int count = _element.Count;

            var drawInfo = new DrawButtonInfo();

            drawInfo.Opacity = opacity;
            drawInfo.EllapsedTime = parameters.EllapsedTime;

            for (int idx = 0; idx < count; ++idx)
            {
                _element.GetText(_text, idx);

                drawInfo.ButtonState = ButtonState.None;
                if (idx == _pushedIndex)
                {
                    drawInfo.ButtonState |= ButtonState.Pushed;
                }

                if (idx == selected)
                {
                    drawInfo.ButtonState |= ButtonState.Checked;
                }

                drawInfo.Text = _text;
                drawInfo.Target = rect;

                for (int di = 0; di < _drawables.Count; ++di)
                {
                    _drawables[di].DontForceRedraw();
                    _drawables[di].Draw(parameters.DrawBatch, drawInfo);
                }

                if (_vertical)
                {
                    rect.Y += size + spacing;
                }
                else
                {
                    rect.X += size + spacing;
                }
            }
        }

        Rectangle GetFirstRect()
        {
            Rectangle rect = ScreenBounds;

            int sizeContext = _vertical ? Bounds.Width : Bounds.Height;

            int count = _element.Count;
            int spacing = _spacing.Compute(sizeContext);

            int width = _elementWidth.Compute(sizeContext);
            int height = _elementHeight.Compute(sizeContext);

            int posX = 0;
            int posY = 0;

            if (_vertical)
            {
                switch (_contentHorizontalAlignment)
                {
                    case HorizontalContentAlignment.Left:
                    case HorizontalContentAlignment.Auto:
                        posX = rect.Center.X;
                        break;

                    case HorizontalContentAlignment.Center:
                        posX = rect.Center.X - width / 2;
                        break;

                    case HorizontalContentAlignment.Right:
                        posX = rect.Right - width;
                        break;
                }

                switch (_contentVerticalAlignment)
                {
                    case VerticalContentAlignment.Top:
                    case VerticalContentAlignment.Auto:
                        posY = rect.Y;
                        break;

                    case VerticalContentAlignment.Center:
                        posY = rect.Center.Y - (height * count + spacing * (count - 1)) / 2;
                        break;

                    case VerticalContentAlignment.Bottom:
                        posY = rect.Bottom - (height * count + spacing * (count - 1));
                        break;
                }
            }
            else
            {
                switch (_contentHorizontalAlignment)
                {
                    case HorizontalContentAlignment.Left:
                    case HorizontalContentAlignment.Auto:
                        posX = rect.X;
                        break;
                    case HorizontalContentAlignment.Center:
                        posX = rect.Center.X - (width * count + spacing * (count - 1)) / 2;
                        break;
                    case HorizontalContentAlignment.Right:
                        posX = rect.Right - (width * count + spacing * (count - 1));
                        break;
                }

                switch (_contentVerticalAlignment)
                {
                    case VerticalContentAlignment.Top:
                    case VerticalContentAlignment.Auto:
                        posY = rect.Y;
                        break;

                    case VerticalContentAlignment.Center:
                        posY = rect.Center.Y - height / 2;
                        break;

                    case VerticalContentAlignment.Bottom:
                        posY = rect.Bottom - height;
                        break;
                }
            }

            rect = new Rectangle(posX, posY, width, height);

            return rect;
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiIndexSelector));

            var context = file["Context"];

            if((context is MethodName) || (context is FieldName))
            {
                var obj = DefinitionResolver.GetValueFromMethodOrField(Controller, Binding, context);

                if (obj is IIndexedElement)
                {
                    _element = obj as IIndexedElement;
                }
                else if(obj is string)
                {
                    _context = obj as string;
                }
            }
            else
            {
                _context = DefinitionResolver.GetString(Controller, Binding, context);
            }

            if (_element == null)
            {    
                _element = Controller.Find(_context) as IIndexedElement;
            }

            _spacing = DefinitionResolver.Get<Length>(Controller, Binding, file["Spacing"], Length.Zero);
            _vertical = DefinitionResolver.Get<Mode>(Controller, Binding, file["Mode"], Mode.Horizontal) == Mode.Vertical;

            _elementWidth = DefinitionResolver.Get<Length>(Controller, Binding, file["ElementWidth"], Length.Stretch);
            _elementHeight = DefinitionResolver.Get<Length>(Controller, Binding, file["ElementHeight"], Length.Stretch);

            _contentHorizontalAlignment = DefinitionResolver.Get<HorizontalContentAlignment>(Controller, Binding, file["HorizontalContentAlignment"], HorizontalContentAlignment.Center);
            _contentVerticalAlignment = DefinitionResolver.Get<VerticalContentAlignment>(Controller, Binding, file["VerticalContentAlignment"], VerticalContentAlignment.Center);

            return true;
        }

        protected override void Update(float time)
        {
            base.Update(time);

            if (_element == null)
            {
                _element = Controller.Find(_context) as IIndexedElement;
            }
        }

        protected override void OnGesture(Gesture gesture)
        {
            base.OnGesture(gesture);

            _pushedIndex = -1;

            if (IsPushed && _touchId == gesture.TouchId)
            {
                int spacing = _spacing.Compute(_vertical ? Bounds.Width : Bounds.Height);
                int count = _element.Count;

                Rectangle rect = GetFirstRect();

                int size = _vertical ? rect.Height : rect.Width;

                Point point = gesture.Origin.ToPoint();

                for (int idx = 0; idx < count; ++idx)
                {
                    if (rect.Contains(point))
                    {
                        _pushedIndex = idx;
                        _checkRect = rect;
                    }

                    if (_vertical)
                    {
                        rect.Y += size + spacing;
                    }
                    else
                    {
                        rect.X += size + spacing;
                    }
                }
            }
        }

        protected override void DoAction()
        {
            if (_pushedIndex >= 0)
            {
                _element.SelectedIndex = _pushedIndex;
            }
        }

    }
}
