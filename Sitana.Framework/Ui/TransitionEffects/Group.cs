using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Xml;
using Sitana.Framework.Diagnostics;

namespace Sitana.Framework.Ui.Views.TransitionEffects
{
    public class Group : TransitionEffect
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            List<DefinitionFile> elements = new List<DefinitionFile>();

            foreach (var cn in node.Nodes)
            {
                DefinitionFile def = DefinitionFile.LoadFile(cn);

                if (!def.Class.IsSubclassOf(typeof(TransitionEffect)))
                {
                    string error = node.NodeError("Group of transitions can contain only other transition effects.");
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

        List<TransitionEffect> _effects = new List<TransitionEffect>();

        public override void Get(double transition, Rectangle containerRect, Rectangle elementRect, out Matrix transform, out float opacity)
        {
            opacity = 1;
            transform = Matrix.Identity;

            for (int idx = 0; idx < _effects.Count; ++idx)
            {
                Matrix tr;
                float op;
                _effects[idx].Get(transition, containerRect, elementRect, out tr, out op);

                opacity *= op;
                transform *= tr;
            }
        }

        protected override void Init(UiController controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            List<DefinitionFile> elements = definition["Elements"] as List<DefinitionFile>;

            if (elements != null)
            {
                foreach (var el in elements)
                {
                    var effect = el.CreateInstance(controller, binding) as TransitionEffect;

                    if (effect != null)
                    {
                        _effects.Add(effect);
                    }
                }
            }
        }

        public override TransitionEffect Reverse()
        {
            Group group = new Group()
            {
                _effects = new List<TransitionEffect>()
            };

            for (int idx = 0; idx < _effects.Count; ++idx)
            {
                group._effects.Add(_effects[idx].Reverse());
            }

            return group;
        }
    }
}
