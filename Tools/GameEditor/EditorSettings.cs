using System;
using Sitana.Framework.Settings;
using Sitana.Framework.Cs;
using Microsoft.Xna.Framework;

namespace GameEditor
{
    public class EditorSettings: SingletonSettings<EditorSettings>
    {
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public readonly SharedValue<bool> ShowAllLayersShared = new SharedValue<bool>();

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public readonly SharedValue<bool> ViewBigShared = new SharedValue<bool>();

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public readonly SharedValue<bool> ViewSmallShared = new SharedValue<bool>();

        public int WindowWidth = 0;
        public int WindowHeight = 0;

        public bool Fullscreen = false;

        public bool ShowAllLayers
        {
            get
            {
                return ShowAllLayersShared.Value;
            }

            set
            {
                ShowAllLayersShared.Value = value;
            }
        }

        public bool SmallView
        {
            get
            {
                return ViewSmallShared.Value;
            }

            set
            {
                ViewSmallShared.Value = value;
            }
        }

        public bool BigView
        {
            get
            {
                return ViewBigShared.Value;
            }

            set
            {
                ViewBigShared.Value = value;
            }
        }

        protected override void Init()
        {
            if (!Loaded)
            {
                ShowAllLayers = true;
                BigView = false;
                SmallView = false;
            }

            ShowAllLayersShared.ValueChanged += (v)=>
            {
                Serialize();
            };

            ViewBigShared.ValueChanged += (v) =>
            {
                if (v)
                {
                    ViewSmallShared.Value = false;
                }

                MainController.ChangeViewScaling(SmallView, BigView);

                Serialize();
            };

            ViewSmallShared.ValueChanged += (v) =>
            {
                if (v)
                {
                    ViewBigShared.Value = false;
                }

                MainController.ChangeViewScaling(SmallView, BigView);

                Serialize();
            };
        }
    }
}

