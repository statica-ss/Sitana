using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views.ButtonDrawables;
using Sitana.Framework.Cs;

namespace Sitana.Framework.Ui.Views
{
    public class UiCheckBox: UiButton
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiButton.Parse(node, file);

            var parser = new DefinitionParser(node);
            file["Checked"] = parser.ParseBoolean("Checked");
        }

        public SharedValue<bool> Checked { get; private set; }

        public override ButtonState ButtonState
        {
            get
            {
                return base.ButtonState | (Checked.Value ? ButtonState.Checked : ButtonState.None);
            }
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiCheckBox));

            Checked = DefinitionResolver.GetShared<bool>(Controller, Binding, file["Checked"], false);
        }

        protected override void Draw(ref Parameters.UiViewDrawParameters parameters)
        {
            base.Draw(ref parameters);
        }

        protected override void DoAction()
        {
            Checked.Value = !Checked.Value;

            CallDelegate("Click", 
                new InvokeParam("sender", this),
                new InvokeParam("checked", Checked.Value));

            if (_actionSound != null)
            {
                _actionSound.Play();
            }
        }
    }
}
