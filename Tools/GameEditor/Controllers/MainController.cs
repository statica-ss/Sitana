using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Content;
using Sitana.Framework.Ui.Views;
using Sitana.Framework;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.Binding;

namespace GameEditor
{
    public class MainController: UiController
    {
        public static MainController Current { get; private set; }

        public SharedString FileName { get; private set; }

        public SharedValue<bool> ShowAllLayers
        { 
            get
            { 
                return EditorSettings.Instance.ShowAllLayersShared;
            }
        }

        public static void OnLoadContent(AppMain main)
        {
            FontManager.Instance.AddSpriteFont("Font", "Font", 8);
            FontManager.Instance.AddSpriteFont("Symbols", "Symbols", 8);
        }

        public MainController()
        {
            if (Current != null)
            {
                throw new Exception("There can be only one MainController!");
            }

            Current = this;
            FileName = Document.Instance.FileName;
        }

        public void OpenLink(UiButton sender)
        {
            SystemWrapper.OpenWebsite(sender.Text.StringValue);
        }
    }
}
