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
        public readonly SharedValue<bool> View150Shared = new SharedValue<bool>();

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public readonly SharedValue<bool> View125Shared = new SharedValue<bool>();

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

        public bool View125
        {
            get
            {
                return View125Shared.Value;
            }

            set
            {
                View125Shared.Value = value;
            }
        }

        public bool View150
        {
            get
            {
                return View150Shared.Value;
            }

            set
            {
                View150Shared.Value = value;
            }
        }

        protected override void Init()
        {
            if (!Loaded)
            {
                ShowAllLayers = true;
                View150 = false;
                View125 = false;
            }

            ShowAllLayersShared.ValueChanged += (v)=>
            {
                Serialize();
            };

            View150Shared.ValueChanged += (v) =>
            {
                if (v)
                {
                    View125Shared.Value = false;
                }

                MainController.MakeBigger(View125, View150);

                Serialize();
            };

            View125Shared.ValueChanged += (v) =>
            {
                if (v)
                {
                    View150Shared.Value = false;
                }

                MainController.MakeBigger(View125, View150);

                Serialize();
            };
        }
    }
}

