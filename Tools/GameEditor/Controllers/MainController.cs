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
using Sitana.Framework.Input;
using Microsoft.Xna.Framework.Input;

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

        public SharedValue<bool> View140
        {
            get
            {
                return EditorSettings.Instance.View140Shared;
            }
        }

        public SharedValue<bool> View120
        {
            get
            {
                return EditorSettings.Instance.View120Shared;
            }
        }

        public static void OnLoadContent(AppMain main)
        {
            using (Stream stream = ContentLoader.Current.Open("Templates/SampleTemplate.zip"))
            {
                CurrentTemplate.Instance.Load(stream);
            }

            Document.Current.New();

            UiUnit.FontScaling = UiUnit.ScalingMode.Floating;

            FontManager.Instance.AddSitanaFont("Font", "Fonts/Font", 16,20,24,28,32);
            ChangeViewScaling(EditorSettings.Instance.View120, EditorSettings.Instance.View140);
        }

        public static void ChangeViewScaling(bool view120, bool view140)
        {
            if (view140)
            {
                UiUnit.Unit = 1.4;
                UiUnit.FontUnit = 1.4 * 1.5;
            }
            else if (view120)
            {
                UiUnit.Unit = 1.2;
                UiUnit.FontUnit = 1.2 * 1.5;

            }
            else
            {
                UiUnit.Unit = 1;
                UiUnit.FontUnit = 1 * 1.5;
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
