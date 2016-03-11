using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Views.Parameters;
using Sitana.Framework.Input.TouchPad;
using Sitana.Framework.Ui.Interfaces;

namespace Sitana.Framework.Ui.Views
{
    public class UiScrollView: UiBorder, IScrolledElement
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

            var parser = new DefinitionParser(node);
            file["Mode"] = parser.ParseEnum<Scroller.Mode>("Mode");
			file["WheelScrollSpeed"] = parser.ParseDouble("WheelScrollSpeed");
            file["MaxScrollExceed"] = parser.ParseLength("MaxScrollExceed");
            file["ExceedRule"] = parser.ParseEnum<ScrollingService.ExceedRule>("ExceedRule");
        }

        Scroller _scroller = null;
        Point _updateScrollPosition = Point.Zero;
        ScrollingService _scrollingService;
        Point _maxScroll = Point.Zero;
        ScrollingService.ExceedRule _rule = ScrollingService.ExceedRule.Allow;
        Scroller.Mode _mode = Scroller.Mode.None;
		float _wheelSpeed = 0;
        Length _maxScrollExceed;

        protected override void OnAdded()
        {
            _scrollingService = new ScrollingService(this, _rule, _maxScrollExceed);
			_scroller = new Scroller(this, _mode, _scrollingService, _wheelSpeed);

            base.OnAdded();
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();

            _scrollingService.Remove();
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            var file = new DefinitionFileWithStyle(definition, typeof(UiScrollView));

            _mode = DefinitionResolver.Get<Scroller.Mode>(Controller, Binding, file["Mode"], Scroller.Mode.BothDrag);
            _rule = DefinitionResolver.Get<ScrollingService.ExceedRule>(Controller, Binding, file["ExceedRule"], ScrollingService.ExceedRule.Allow);
			_wheelSpeed = (float)DefinitionResolver.Get<double>(Controller, Binding, file["WheelScrollSpeed"], 0);
            _maxScrollExceed = DefinitionResolver.Get<Length>(Controller, Binding, file["MaxScrollExceed"], ScrollingService.MaxScrollExceed);

            return true;
        }

        protected override Rectangle CalculateChildBounds(UiView view)
        {
            Point size = view.ComputeSize(Bounds.Width, Bounds.Height);
            PositionParameters pos = view.PositionParameters;

            Rectangle childRect = new Rectangle(0, 0, size.X, size.Y);

            int posX = pos.X.Compute(Bounds.Width);
            int posY = pos.Y.Compute(Bounds.Height);

            switch (pos.HorizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    childRect.X = posX - size.X / 2;
                    break;

                case HorizontalAlignment.Left:
                    childRect.X = posX;
                    break;

                case HorizontalAlignment.Right:
                    childRect.X = posX - size.X;
                    break;

                case HorizontalAlignment.Stretch:
                    childRect.X = 0;
                    childRect.Width = Bounds.Width;
                    break;
            }

            switch (pos.VerticalAlignment)
            {
                case VerticalAlignment.Center:
                    childRect.Y = posY - size.Y / 2;
                    break;

                case VerticalAlignment.Top:
                    childRect.Y = posY;
                    break;

                case VerticalAlignment.Bottom:
                    childRect.Y = posY - size.Y;
                    break;

                case VerticalAlignment.Stretch:
                    childRect.Y = 0;
                    childRect.Height = Bounds.Height;
                    break;
            }

            Rectangle bounds = childRect;
            Point offset = _scroller != null ? ScrollPosition : Point.Zero;

            bounds.X -= offset.X;
            bounds.Y -= offset.Y;

            return bounds;
        }

        protected override void OnViewDisplayChanged(bool isDisplayed)
        {
            if (!isDisplayed)
            {
                _scroller.OnViewHidden();
            }
        }

        public override void RecalcLayout()
        {
            base.RecalcLayout();

            Point maxBounds = Point.Zero;

            Point offset = _scroller != null ? ScrollPosition : Point.Zero;

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx];
                maxBounds.X = Math.Max(child.Bounds.Right + offset.X, maxBounds.X);
                maxBounds.Y = Math.Max(child.Bounds.Bottom + offset.Y, maxBounds.Y);
            }

            if (_scroller != null)
            {
                if (_maxScroll != maxBounds)
                {
                    _maxScroll = maxBounds;
                }
                _updateScrollPosition = ScrollPosition;
            }
        }

        public void ScrollToChild(UiView view, bool vertical)
        {
            Rectangle myBounds = ScreenBounds;
            Rectangle viewBounds = view.ScreenBounds;

            if(vertical)
            {
                float scrollPosition = _scrollingService.ScrollPositionY;

                if (viewBounds.Bottom > myBounds.Bottom)
                {
                    scrollPosition += viewBounds.Bottom - myBounds.Bottom;
                }

                if (viewBounds.Top < myBounds.Top)
                {
                    scrollPosition -= myBounds.Top - viewBounds.Top;
                }

                _scrollingService.ScrollPositionY = scrollPosition;
            }
            else
            {
                float scrollPosition = _scrollingService.ScrollPositionX;

                if(viewBounds.Right > myBounds.Right)
                {
                    scrollPosition += viewBounds.Right - myBounds.Right;
                }

                if (viewBounds.Left < myBounds.Left)
                {
                    scrollPosition -= myBounds.Left - viewBounds.Left;
                }

                _scrollingService.ScrollPositionX = scrollPosition;
            }

        }

        protected override void Update(float time)
        {
            base.Update(time);

            Point scrollPosition = ScrollPosition;

            if (scrollPosition != _updateScrollPosition)
            {
                Point diff = new Point(_updateScrollPosition.X - scrollPosition.X, _updateScrollPosition.Y - scrollPosition.Y);
                _updateScrollPosition = scrollPosition;

                for (int idx = 0; idx < _children.Count; ++idx)
                {
                    var child = _children[idx];
                    child.Move(diff);
                }
            }
        }

        public Point ScrollPosition
        {
            get
            {
                return new Point((int)_scrollingService.ScrollPositionX, (int)_scrollingService.ScrollPositionY);
            }
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            DrawBackground(ref parameters);

            UiViewDrawParameters drawParams = parameters;
            drawParams.Opacity = opacity;

            if (_clipChildren)
            {
                parameters.DrawBatch.PushClip(ScreenBounds);
            }

            Rectangle bounds = new Rectangle(0, 0, Bounds.Width, Bounds.Height);

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx];

                if (bounds.Intersects(child.Bounds))
                {
                    child.ViewDraw(ref drawParams);
                }
            }

            if (_clipChildren)
            {
                parameters.DrawBatch.PopClip();
            }
        }

        protected override void OnGesture(Gesture gesture)
        {
            _scroller.OnGesture(gesture);
        }

        Rectangle IScrolledElement.ScreenBounds { get{ return ScreenBounds;} }

        int IScrolledElement.MaxScrollX { get{return _maxScroll.X;} }
        int IScrolledElement.MaxScrollY { get{return _maxScroll.Y;} }

        ScrollingService IScrolledElement.ScrollingService {get{ return _scrollingService;}}
    }
}
