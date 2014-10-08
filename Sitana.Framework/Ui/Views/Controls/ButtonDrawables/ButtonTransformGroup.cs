using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Graphics;
using Microsoft.Xna.Framework;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Diagnostics;

namespace Sitana.Framework.Ui.Views.ButtonDrawables
{
    public class TransformGroup : ButtonDrawable
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            ButtonDrawable.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Reverse"] = parser.ParseBoolean("Reverse");

            foreach (var cn in node.Nodes)
            {
                if (cn.Tag == "TransformGroup.TransitionEffect")
                {
                    if (cn.Nodes.Count != 1)
                    {
                        string error = node.NodeError("TransformGroup.TransitionEffect must have exactly 1 child.");
                        if (DefinitionParser.EnableCheckMode)
                        {
                            ConsoleEx.WriteLine(error);
                        }
                        else
                        {
                            throw new Exception(error);
                        }
                    }

                    file["TransitionEffect"] = DefinitionFile.LoadFile(cn.Nodes[0]);
                }
                else if (cn.Tag == "TransformGroup.Drawables")
                {
                    UiView.ParseDrawables(cn, file, typeof(ButtonDrawable));
                }
            }
        }

        TransitionEffect _transitionPushed;
        bool _reverse = false;

        List<ButtonDrawable> _drawables = new List<ButtonDrawable>();

        protected override void Init(Controllers.UiController controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(ButtonDrawable));

            _reverse = DefinitionResolver.Get<bool>(controller, binding, file["Reverse"], false);

            List<DefinitionFile> drawableFiles = file["Drawables"] as List<DefinitionFile>;

            if (drawableFiles != null)
            {
                foreach (var def in drawableFiles)
                {
                    ButtonDrawable drawable = def.CreateInstance(controller, binding) as ButtonDrawable;

                    if (drawable != null)
                    {
                        _drawables.Add(drawable);
                    }
                }
            }

            _transitionPushed = (file["TransitionEffect"] as DefinitionFile).CreateInstance(controller, binding) as TransitionEffect;
        }

        public override void Draw(AdvancedDrawBatch drawBatch, UiButton.DrawButtonInfo info)
        {
            Update(info.EllapsedTime, info.ButtonState);

            float opacity;
            Matrix transform;

            _transitionPushed.Get(_reverse ? 1 - PushedState : PushedState, info.Target, info.Target, out transform, out opacity);

            drawBatch.PushTransform(transform);
            info.Opacity *= opacity;

            for (int idx = 0; idx < _drawables.Count; ++idx)
            {
                _drawables[idx].Draw(drawBatch, info);
            }

            drawBatch.PopTransform();
        }
    }
}
