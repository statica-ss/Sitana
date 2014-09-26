using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Sitana.Framework.Input;

namespace Sitana.Framework.Gui.List
{
    public abstract class ListItem_ButtonBase : ListItemElement
    {
        protected Rectangle _buttonRect;
        protected Boolean IsPushed {get; private set;}

        private Action _action;

        private Boolean _returnTrueOnTap = false;

        protected void InitButton(Rectangle rect, Action action)
        {
            IsPushed = false;
            _buttonRect = rect;
            _action = action;
        }

        public override Boolean OnGesture(GestureType type, Vector2 position, params Object[] parameters)
        {
            switch (type)
            {
                case GestureType.Tap:
                    if (_returnTrueOnTap || HandleTouchUp(position))
                    {
                        _returnTrueOnTap = false;
                        return true;
                    }

                    break;

                case GestureType.None:

                    if ((GestureAdditionalType)parameters[0] == GestureAdditionalType.TouchDown)
                    {
                        _returnTrueOnTap = false;
                        if (HandleTouchDown(position))
                        {
                            return true;
                        }
                    }
                    else if ((GestureAdditionalType)parameters[0] == GestureAdditionalType.TouchUp)
                    {
                        if (HandleTouchUp(position))
                        {
                            _returnTrueOnTap = true;
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        private Boolean HandleTouchDown(Vector2 position)
        {
            Point pos = GraphicsHelper.PointFromVector2(position);

            IsPushed = false;

            if (_buttonRect.Contains(pos))
            {
                IsPushed = true;
            }

            InputHandler.Current.PointerInvalidated += PointerInvalidated;

            return IsPushed;
        }

        void PointerInvalidated(object sender, GestureEventArgs e)
        {
            InputHandler.Current.PointerInvalidated -= PointerInvalidated;
            IsPushed = false;
        }

        private Boolean HandleTouchUp(Vector2 position)
        {
            Point pos = GraphicsHelper.PointFromVector2(position);

            if (_buttonRect.Contains(pos) && IsPushed)
            {
                if (_action != null)
                {
                    _action.Invoke();
                }

                IsPushed = false;

                return true;
            }

            IsPushed = false;
            return false;
        }
    }
}
