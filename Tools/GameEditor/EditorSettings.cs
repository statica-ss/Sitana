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
        public readonly SharedValue<bool> View140Shared = new SharedValue<bool>();

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public readonly SharedValue<bool> View120Shared = new SharedValue<bool>();

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

        public bool View120
        {
            get
            {
                return View120Shared.Value;
            }

            set
            {
                View120Shared.Value = value;
            }
        }

        public bool View140
        {
            get
            {
                return View140Shared.Value;
            }

            set
            {
                View140Shared.Value = value;
            }
        }

        protected override void Init()
        {
            if (!Loaded)
            {
                ShowAllLayers = true;
                View140 = false;
                View120 = false;
            }

            ShowAllLayersShared.ValueChanged += (v)=>
            {
                Serialize();
            };

            View140Shared.ValueChanged += (v) =>
            {
                if (v)
                {
                    View120Shared.Value = false;
                }

                MainController.ChangeViewScaling(View120, View140);

                Serialize();
            };

            View120Shared.ValueChanged += (v) =>
            {
                if (v)
                {
                    View140Shared.Value = false;
                }

                MainController.ChangeViewScaling(View120, View140);

                Serialize();
            };
        }
    }
}

