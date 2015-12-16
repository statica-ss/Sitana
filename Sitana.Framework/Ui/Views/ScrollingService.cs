using System;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Ui.Interfaces;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Ui.Views
{
    public class ScrollingService: IUpdatable
    {
        public enum ExceedRule
        {
            Allow,
            AllowWhenScroll,
            Forbid
        }

        float _scrollPositionX = 0;

        public float ScrollPositionX
        {
            get
            {
                return _scrollPositionX;
            }

            set
            {
                _scrollPositionX = value;
            }
        }
        public float ScrollPositionY = 0;

        public float ScrollSpeedX = 0;
        public float ScrollSpeedY = 0;

        public bool IsDraggingX = false;
        public bool IsDraggingY = false;

        Vector2 _lastScroll = Vector2.Zero;

        Rectangle _bounds = new Rectangle(0,0,1,1);
        ExceedRule _mode;

        public Length MaxExceed { get; private set; }

        public IScrolledElement ScrolledElement { get; private set;}

        public static Length MaxScrollExceed = new Length(10);

        public ScrollingService(IScrolledElement scrolled, ExceedRule mode, Length maxExceed)
        {
            ScrolledElement = scrolled;
            AppMain.Current.RegisterUpdatable(this);

            MaxExceed = maxExceed;
            _mode = mode;
        }

        public void Remove()
        {
            AppMain.Current.UnregisterUpdatable(this);
        }

        public void Process()
        {
            (this as IUpdatable).Update(0);
        }

        void IUpdatable.Update(float time)
        {
            float lastScrollX = ScrollPositionX;
            float lastScrollY = ScrollPositionY;

            var bounds = ScrolledElement.ScreenBounds;

            var maxScrollX = ScrolledElement.MaxScrollX;
            var maxScrollY = ScrolledElement.MaxScrollY;

            if (_bounds != bounds)
            {
                _bounds = bounds;
            }

            float desiredScrollX = Math.Max(0, Math.Min(maxScrollX - bounds.Width, ScrollPositionX));
            float desiredScrollY = Math.Max(0, Math.Min(maxScrollY - bounds.Height, ScrollPositionY));

            float maxExceedX = (float)MaxExceed.ComputeDouble();
            float maxExceedY = (float)MaxExceed.ComputeDouble();

            if (Math.Abs(desiredScrollX - ScrollPositionX) > maxExceedX)
            {
                ScrollSpeedX = 0;
            }

            if (Math.Abs(desiredScrollY - ScrollPositionY) > maxExceedY)
            {
                ScrollSpeedY = 0;
            }

            if(ScrollSpeedX != 0)
            {
                ScrollPositionX += ScrollSpeedX * time;
                ScrollSpeedX -= ScrollSpeedX * time * 10;

                if (Math.Abs(ScrollSpeedX) < 1)
                {
                    ScrollSpeedX = 0;
                }
            }

            if(ScrollSpeedY != 0)
            {
                ScrollPositionY += ScrollSpeedY * time;
                ScrollSpeedY -= ScrollSpeedY * time * 10;

                if (Math.Abs(ScrollSpeedY) < 1)
                {
                    ScrollSpeedY = 0;
                }
            }

            bool scroll = maxScrollX-bounds.Width > 0 || maxScrollY-bounds.Height > 0;

            if (_mode == ExceedRule.Allow || (_mode == ExceedRule.AllowWhenScroll && scroll))
            {
                if (!IsDraggingX)
                {
                    ScrollPositionX = ComputeScroll(time, ScrollPositionX, 0, maxScrollX, bounds.Width);
                }

                if (!IsDraggingY)
                {
                    ScrollPositionY = ComputeScroll(time, ScrollPositionY, 0, maxScrollY, bounds.Height);
                }

                ScrollPositionX = ComputeScroll(1, ScrollPositionX, -maxExceedX, maxScrollX + maxExceedX, bounds.Width);
                ScrollPositionY = ComputeScroll(1, ScrollPositionY, -maxExceedY, maxScrollY + maxExceedY, bounds.Height);
            }
            else
            {
                ScrollPositionX = ComputeScroll(1, ScrollPositionX, 0, maxScrollX, bounds.Width);
                ScrollPositionY = ComputeScroll(1, ScrollPositionY, 0, maxScrollY, bounds.Height);
            }

            if(lastScrollX != ScrollPositionX || lastScrollY != ScrollPositionY)
            {
				AppMain.Redraw( ScrolledElement as UiView );
            }
        }

        private float ComputeScroll(float time, float scrollPosition, float minScroll, float maxScroll, int size)
        {
            maxScroll = Math.Max(maxScroll - size, 0);
            float desiredScroll = Math.Max(minScroll, Math.Min(maxScroll, scrollPosition));

            if (desiredScroll != scrollPosition)
            {
                int sign = Math.Sign(desiredScroll - scrollPosition);

                for (int idx = 0; idx < 10; ++idx)
                {
                    scrollPosition = time * desiredScroll + (1 - time) * scrollPosition;
                }

                if (Math.Abs(desiredScroll - scrollPosition) < 1)
                {
                    scrollPosition = desiredScroll;
                }

                if (Math.Sign(desiredScroll - scrollPosition) != sign)
                {
                    scrollPosition = desiredScroll;
                }
            }

            return scrollPosition;
        }

    }
}

