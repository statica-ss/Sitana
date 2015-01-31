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

        public SharedValue<bool> BigView
        {
            get
            {
                return EditorSettings.Instance.ViewBigShared;
            }
        }

        public SharedValue<bool> SmallView
        {
            get
            {
                return EditorSettings.Instance.ViewSmallShared;
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
            ChangeViewScaling(EditorSettings.Instance.SmallView, EditorSettings.Instance.BigView);
        }

        public static void ChangeViewScaling(bool smallView, bool bigView)
        {
            if (bigView)
            {
                UiUnit.Unit = 1.4f;
                UiUnit.FontUnit = 1.4f * 1.5f;
            }
            else if (smallView)
            {
                UiUnit.Unit = 1;
                UiUnit.FontUnit = 1 * 1.5f;

            }
            else
            {
                UiUnit.Unit = 1.15f;
                UiUnit.FontUnit = 1.15f * 1.5f;
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
