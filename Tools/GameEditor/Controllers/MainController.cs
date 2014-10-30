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
using System.IO;

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

        public SharedValue<bool> View150
        {
            get
            {
                return EditorSettings.Instance.View150Shared;
            }
        }

        public SharedValue<bool> View125
        {
            get
            {
                return EditorSettings.Instance.View125Shared;
            }
        }

        public static void OnLoadContent(AppMain main)
        {
            using (Stream stream = ContentLoader.Current.Open("SampleTemplate.zip"))
            {
                CurrentTemplate.Instance.Load(stream);
            }

            Document.Current.New();

            FontManager.Instance.AddSpriteFont("Font", "Font8", 8);
            FontManager.Instance.AddSpriteFont("Font", "Font10", 10);
            FontManager.Instance.AddSpriteFont("Font", "Font12", 12);

            MakeBigger(EditorSettings.Instance.View125, EditorSettings.Instance.View150);
        }

        public static void MakeBigger(bool make125, bool make150)
        {
            if (make150)
            {
                UiUnit.Unit = 1.5f;
                UiUnit.FontUnit = 1.5f;
            }
            else if (make125)
            {
                UiUnit.Unit = 1.25f;
                UiUnit.FontUnit = 1.25f;
            }
            else
            {
                UiUnit.Unit = 1;
                UiUnit.FontUnit = 1;
            }

            if (AppMain.Current.MainView != null)
            {
                AppMain.Current.MainView.RecalculateAll();
            }
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
            Platform.OpenWebsite(sender.Text.StringValue);
        }
    }
}
