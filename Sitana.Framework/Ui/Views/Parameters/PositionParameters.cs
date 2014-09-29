// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework;
using Sitana.Framework.Essentials.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;

namespace Sitana.Framework.Ui.Views
{
    public class PositionParameters: IDefinitionClass
    {
        public Margin Margin = Margin.Zero;
        public Align  Align = Align.StretchHorz | Align.StretchVert;

        public Length Width;
        public Length Height;

        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["Width"] = parser.ParseLength("Width");
            file["Height"] = parser.ParseLength("Height");
            file["Margin"] = parser.ParseMargin("Margin");
        }

        void IDefinitionClass.Init(UiController controller, object binding, DefinitionFile file)
        {
            Init(controller, binding, file);
        }

        protected virtual void Init(UiController controller, object binding, DefinitionFile file)
        {
            Width = DefinitionResolver.Get<Length>(controller, binding, file["Width"]);
            Height = DefinitionResolver.Get<Length>(controller, binding, file["Height"]);
            Margin = DefinitionResolver.Get<Margin>(controller, binding, file["Margin"]);
        }

        public Rectangle ComputePosition(Rectangle target)
        {
            Rectangle bounds = target;

            int width = Width.Compute(target.Width);
            int height = Height.Compute(target.Height);

            switch(Align & Sitana.Framework.Align.Horz)
            {
                case Sitana.Framework.Align.Left:
                    bounds.X = target.Left;
                    bounds.Width = width;
                    break;

                case Sitana.Framework.Align.Right:
                    bounds.X = target.Right - width;
                    bounds.Width = width;
                    break;

                case Sitana.Framework.Align.Center:
                    bounds.X = target.Center.X - width / 2;
                    bounds.Width = width;
                    break;
            }

            switch (Align & Sitana.Framework.Align.Vert)
            {
                case Sitana.Framework.Align.Top:
                    bounds.Y = target.Top;
                    bounds.Height = height;
                    break;

                case Sitana.Framework.Align.Bottom:
                    bounds.Y = target.Bottom - height;
                    bounds.Height = height;
                    break;

                case Sitana.Framework.Align.Middle:
                    bounds.Y = target.Center.Y - height / 2;
                    bounds.Height = height;
                    break;
            }

            return Margin.ComputeRect(bounds);
        }
    }
}
