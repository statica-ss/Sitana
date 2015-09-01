// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;

namespace Sitana.Framework.Ui.Views
{
    public class PositionParameters
    {
        public Margin Margin = Margin.None;
        public HorizontalAlignment  HorizontalAlignment = HorizontalAlignment.Stretch;
        public VerticalAlignment  VerticalAlignment = VerticalAlignment.Stretch;

        public Length Width;
        public Length Height;

        public Length X;
        public Length Y;

        public string BindWidthId;
        public string BindHeightId;

        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["Width"] = parser.ParseLength("Width");
            file["Height"] = parser.ParseLength("Height");
            file["Margin"] = parser.ParseMargin("Margin");

            file["HorizontalAlignment"] = parser.ParseEnum<HorizontalAlignment>("HorizontalAlignment");
            file["VerticalAlignment"] = parser.ParseEnum<VerticalAlignment>("VerticalAlignment");

            file["X"] = parser.ParseLength("X");
            file["Y"] = parser.ParseLength("Y");

            file["BindHeight"] = parser.ParseString("BindHeight");
            file["BindWidth"] = parser.ParseString("BindWidth");
        }

        internal void Init(UiController controller, object binding, DefinitionFile definition)
        {
            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiView));

            Width = DefinitionResolver.Get<Length>(controller, binding, file["Width"], Length.Default);
            Height = DefinitionResolver.Get<Length>(controller, binding, file["Height"], Length.Default);
            Margin = DefinitionResolver.Get<Margin>(controller, binding, file["Margin"], Margin.None);

            HorizontalAlignment = DefinitionResolver.Get<HorizontalAlignment>(controller, binding, file["HorizontalAlignment"], HorizontalAlignment.Stretch);
            VerticalAlignment = DefinitionResolver.Get<VerticalAlignment>(controller, binding, file["VerticalAlignment"], VerticalAlignment.Stretch);

            BindWidthId = DefinitionResolver.GetString(controller, binding, file["BindWidth"]);
            BindHeightId = DefinitionResolver.GetString(controller, binding, file["BindHeight"]);

            X = DefinitionResolver.Get<Length>(controller, binding, file["X"], Length.Default);
            Y = DefinitionResolver.Get<Length>(controller, binding, file["Y"], Length.Default);

            if (X.IsAuto)
            {
                switch (HorizontalAlignment)
                {
                    case Ui.HorizontalAlignment.Center:
                        X = new Length(0, 0.5);
                        break;

                    case Ui.HorizontalAlignment.Right:
                        X = new Length(0, 1);
                        break;
                }
            }

            if (Y.IsAuto)
            {
                switch (VerticalAlignment)
                {
                    case Ui.VerticalAlignment.Center:
                        Y = new Length(0, 0.5);
                        break;

                    case Ui.VerticalAlignment.Bottom:
                        Y = new Length(0, 1);
                        break;
                }
            }
        }
    }
}
