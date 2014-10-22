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

        protected override void Init()
        {
            if (!Loaded)
            {
                ShowAllLayers = true;
            }

            ShowAllLayersShared.ValueChanged += (v)=>
            {
                Serialize();
            };
        }
    }
}

