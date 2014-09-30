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
        public Margin Margin = Margin.None;
        public Align  Align = Align.StretchHorz | Align.StretchVert;

        public Length Width;
        public Length Height;

        public Length X;
        public Length Y;

        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["Width"] = parser.ParseLength("Width");
            file["Height"] = parser.ParseLength("Height");
            file["Margin"] = parser.ParseMargin("Margin");

            file["Align"] = parser.ParseEnum<Align>("Align");

            file["X"] = parser.ParseLength("X");
            file["Y"] = parser.ParseLength("Y");
        }

        void IDefinitionClass.Init(UiController controller, object binding, DefinitionFile file)
        {
            Init(controller, binding, file);
        }

        protected virtual void Init(UiController controller, object binding, DefinitionFile file)
        {
            Width = DefinitionResolver.Get<Length>(controller, binding, file["Width"], Length.Default);
            Height = DefinitionResolver.Get<Length>(controller, binding, file["Height"], Length.Default);
            Margin = DefinitionResolver.Get<Margin>(controller, binding, file["Margin"], Margin.None);

            Align = DefinitionResolver.Get<Align>(controller, binding, file["Align"], Align.StretchHorz | Align.StretchVert);

            X = DefinitionResolver.Get<Length>(controller, binding, file["X"], Length.Default);
            Y = DefinitionResolver.Get<Length>(controller, binding, file["Y"], Length.Default);
        }
    }
}
