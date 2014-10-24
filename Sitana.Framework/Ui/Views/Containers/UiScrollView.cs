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
            file["ExceedRule"] = parser.ParseEnum<ScrollingService.ExceedRule>("ExceedRule");

            foreach (var cn in node.Nodes)
            {
                switch (cn.Tag)
                {
                    case "UiScrollView.Children":
                        ParseChildren(cn, file);
                        break;
                }
            }
        }

        Scroller _scroller = null;
        Point _updateScrollPosition = Point.Zero;
        ScrollingService _scrollingService;
        Point _maxScroll = Point.Zero;
        ScrollingService.ExceedRule _rule = ScrollingService.ExceedRule.Allow;
        Scroller.Mode _mode = Scroller.Mode.None;

        protected override void OnAdded()
        {
            _scrollingService = new ScrollingService(this, _rule);
            _scroller = new Scroller(this, _mode, _scrollingService);

            base.OnAdded();
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();

            _scroller.Remove();
            _scrollingService.Remove();
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            var file = new DefinitionFileWithStyle(definition, typeof(UiContainer));

            _mode = DefinitionResolver.Get<Scroller.Mode>(Controller, Binding, file["Mode"], Scroller.Mode.BothDrag);
            _rule = DefinitionResolver.Get<ScrollingService.ExceedRule>(Controller, Binding, file["ExceedRule"], ScrollingService.ExceedRule.Allow);
        }

        protected override Rectangle CalculateChildBounds(UiView view)
        {
            Rectangle bounds = base.CalculateChildBounds(view);
            Point offset = _scroller != null ? ScrollPosition : Point.Zero;

            bounds.X -= offset.X;
            bounds.Y -= offset.Y;

            return bounds;
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
                _maxScroll = maxBounds;
                _updateScrollPosition = ScrollPosition;
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

        Point ScrollPosition
        {
            get
            {
                return new Point((int)_scrollingService.ScrollPositionX, (int)_scrollingService.ScrollPositionY);
            }
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = DisplayOpacity * parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            base.Draw(ref parameters);

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
