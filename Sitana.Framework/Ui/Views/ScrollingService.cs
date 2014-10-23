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
            Forbid
        }

        public float ScrollPositionX = 0;
        public float ScrollPositionY = 0;

        public float ScrollSpeedX = 0;
        public float ScrollSpeedY = 0;

        Rectangle _bounds = new Rectangle(0,0,1,1);
        ExceedRule _mode;

        public IScrolledElement ScrolledElement { get; private set;}

        public ScrollingService(IScrolledElement scrolled, ExceedRule mode)
        {
            ScrolledElement = scrolled;
            AppMain.Current.RegisterUpdatable(this);

            _mode = mode;
        }

        public void Remove()
        {
            AppMain.Current.UnregisterUpdatable(this);
        }

        void IUpdatable.Update(float time)
        {
            var bounds = ScrolledElement.ScreenBounds;

            var maxScrollX = ScrolledElement.MaxScrollX;
            var maxScrollY = ScrolledElement.MaxScrollY;

            if (_bounds != bounds)
            {
                _bounds = bounds;
            }

            float desiredScrollX = Math.Max(0, Math.Min(maxScrollX - bounds.Width, ScrollPositionX));
            float desiredScrollY = Math.Max(0, Math.Min(maxScrollY - bounds.Height, ScrollPositionY));

            if (Math.Abs(desiredScrollX - ScrollPositionX) > bounds.Width / 5)
            {
                ScrollSpeedX = 0;
            }

            if (Math.Abs(desiredScrollY - ScrollPositionY) > bounds.Height / 5)
            {
                ScrollSpeedY = 0;
            }

            if (_mode == ExceedRule.Allow)
            {
                ScrollPositionX = ComputeScroll(time, ScrollPositionX, maxScrollX, bounds.Width);
                ScrollPositionY = ComputeScroll(time, ScrollPositionY, maxScrollY, bounds.Height);
            }
            else
            {
                ScrollPositionX = ComputeScroll(1, ScrollPositionX, maxScrollX, bounds.Width);
                ScrollPositionY = ComputeScroll(1, ScrollPositionY, maxScrollY, bounds.Height);
            }
        }

        private float ComputeScroll(float time, float scrollPosition, float maxScroll, int size)
        {
            float desiredScroll = Math.Max(0, Math.Min(maxScroll - size, scrollPosition));

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

