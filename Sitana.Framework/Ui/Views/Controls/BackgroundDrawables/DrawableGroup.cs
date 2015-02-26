using Microsoft.Xna.Framework;
using Sitana.Framework.Diagnostics;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using System;
using System.Collections.Generic;


namespace Sitana.Framework.Ui.Views.BackgroundDrawables
{
    public class DrawableGroup : IBackgroundDrawable, IDefinitionClass
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            List<DefinitionFile> elements = new List<DefinitionFile>();

            foreach (var cn in node.Nodes)
            {
                DefinitionFile def = DefinitionFile.LoadFile(cn);

                if (!typeof(IBackgroundDrawable).IsAssignableFrom(def.Class))
                {
                    string error = node.NodeError("Group of background drawables can contain only other background drawables.");
                    if (DefinitionParser.EnableCheckMode)
                    {
                        ConsoleEx.WriteLine(error);
                    }
                    else
                    {
                        throw new Exception(error);
                    }
                }

                elements.Add(def);
            }

            file["Elements"] = elements;
        }

        List<IBackgroundDrawable> _drawables = new List<IBackgroundDrawable>();

        void IBackgroundDrawable.Draw(AdvancedDrawBatch drawBatch, Rectangle target, Color color)
        {
            for(int idx = 0; idx < _drawables.Count; ++idx)
            {
                _drawables[idx].Draw(drawBatch, target, color);
            }
        }

        bool IDefinitionClass.Init(UiController controller, object binding, DefinitionFile definition)
        {
            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(NinePatchBackground));

            List<DefinitionFile> elements = definition["Elements"] as List<DefinitionFile>;

            if (elements != null)
            {
                foreach (var el in elements)
                {
                    var drawable = el.CreateInstance(controller, binding) as IBackgroundDrawable;

                    if (drawable != null)
                    {
                        _drawables.Add(drawable);
                    }
                }
            }

            return true;
        }
    }
}
