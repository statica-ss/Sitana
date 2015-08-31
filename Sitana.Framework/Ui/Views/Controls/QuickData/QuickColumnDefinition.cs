using System;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui;
using Sitana.Framework.Graphics;

namespace Sitana.Framework.Ui.Views.QuickData
{
	public class QuickColumnDefinition: IDefinitionClass
	{
		public Length Width { get; private set; }
		public UiFont Font { get; private set; }
		public TextAlign TextAlign { get; private set; }
		public int LineHeight {get; private set;}
		public Margin TextMargin{ get; private set; }
	
		public static void Parse(XNode node, DefinitionFile file)
		{
			var parser = new DefinitionParser(node);

			file["Width"] = parser.ParseLength("Width");

			file["Font"] = parser.ValueOrNull("Font");
			file["FontSize"] = parser.ParseInt("FontSize");
			file["FontSpacing"] = parser.ParseInt("FontSpacing");

			file["TextAlign"] = parser.ParseEnum<TextAlign>("TextAlign");

			file["LineHeight"] = parser.ParseInt("LineHeight");

			file["TextMargin"] = parser.ParseMargin("TextMargin");
		}

		bool IDefinitionClass.Init(UiController controller, object binding, DefinitionFile definition)
		{
			DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(QuickColumnDefinition));

			Width = DefinitionResolver.Get<Length>(controller, binding, file["Width"], Length.Stretch);

			string font = DefinitionResolver.GetString(controller, binding, file["Font"]);
			int fontSize = DefinitionResolver.Get<int>(controller, binding, file["FontSize"], 0);
			int fontSpacing = DefinitionResolver.Get<int>(controller, binding, file["FontSpacing"], 0);

			Font = new UiFont(font, fontSize, fontSpacing);

			LineHeight = DefinitionResolver.Get<int>(controller, binding, file["LineHeight"], 100);
			TextAlign = DefinitionResolver.Get<TextAlign>(controller, binding, file["TextAlign"], TextAlign.Middle | TextAlign.Left);

			TextMargin = DefinitionResolver.Get<Margin>(controller, binding, file["TextMargin"], Margin.None);

			return true;
		}
	}
}

