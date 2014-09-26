// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.Ui.DefinitionFiles;

namespace Sitana.Framework.Ui.Views
{
    public class UiPage: UiContainer
    {
        public enum Status
        {
            Show,
            Visible,
            Hide,
            Done
        }

        public Status PageStatus { get; private set; }
        protected float Transition { get; private set; }

        private float _showSpeed = 1;
        private float _hideSpeed = 1;

        public UiPage()
        {
            PageStatus = Status.Show;
        }

        public static UiPage Load(DefinitionFile def)
        {
            return null;
        }

        internal void Hide()
        {
            switch(PageStatus)
            {
                case Status.Show:
                case Status.Visible:
                    PageStatus = Status.Hide;
                    break;
            }
        }

        protected override void Update(float time)
        {
            switch(PageStatus)
            {
                case Status.Show:
                    Transition += time * _showSpeed;
                    if ( Transition >= 1 )
                    {
                        Transition = 1;
                        PageStatus = Status.Visible;
                    }
                    break;

                case Status.Hide:
                    Transition -= time * _hideSpeed;
                    if (Transition <= 0)
                    {
                        Transition = 0;
                        PageStatus = Status.Done;
                    }
                    break;
            }

            if (PageStatus != Status.Done)
            {
                base.Update(time);
            }
        }
    }
}
