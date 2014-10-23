using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Controllers;
using Microsoft.Xna.Framework;


namespace Sitana.Framework.Ui.Views.ButtonDrawables
{
    public abstract class ButtonDrawable : StateDrawable<DrawButtonInfo>, IDefinitionClass
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["ChangeTime"] = parser.ParseFloat("ChangeTime");
            file["ColorPushed"] = parser.ParseColor("ColorPushed");
            file["ColorReleased"] = parser.ParseColor("ColorReleased");
            file["ColorDisabled"] = parser.ParseColor("ColorDisabled");
            file["Margin"] = parser.ParseMargin("Margin");

            file["Checked"]= parser.ParseBoolean("Checked");
        }

        protected float CheckedState = float.NaN;
        protected float DisabledState = float.NaN;
        protected float PushedState = float.NaN;

        protected ColorWrapper _colorPushed;
        protected ColorWrapper _colorReleased;
        protected ColorWrapper _colorDisabled;

        protected bool? _checked;

        protected Margin _margin;

        float _changeSpeed = 1;

        protected float Opacity
        {
            get
            {
                if (_checked.HasValue)
                {
                    return _checked.Value ? CheckedState : 1 - CheckedState;
                }
                return 1;
            }
        }

        void IDefinitionClass.Init(UiController controller, object binding, DefinitionFile file)
        {
            Init(controller, binding, file);
        }

        protected void Update(float time, ButtonState state)
        {
            float check = ((state & ButtonState.Checked) == ButtonState.Checked) ? 1 : 0;
            float disable = ((state & ButtonState.Disabled) == ButtonState.Disabled) ? 1 : 0;
            float push = ((state & ButtonState.Pushed) == ButtonState.Pushed) ? 1 : 0;

            ComputeState(ref CheckedState, check, time);
            ComputeState(ref DisabledState, disable, time);
            ComputeState(ref PushedState, push, time);
        }

        private void ComputeState(ref float current, float desired, float time)
        {
            if (float.IsNaN(current))
            {
                current = desired;
            }
            else if ( current != desired)
            {
                float sign = Math.Sign(desired - current);
                current += time * sign * _changeSpeed;

                current = Math.Max(0, Math.Min(1, current));
            }
        }

        protected virtual void Init(UiController controller, object binding, DefinitionFile definition)
        {
            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(ButtonDrawable));

            float changeTime = (float)DefinitionResolver.Get<double>(controller, binding, file["ChangeTime"], 0) / 1000.0f;

            _changeSpeed = changeTime > 0 ? 1 / changeTime : 10000;

            _colorDisabled = DefinitionResolver.GetColorWrapper(controller, binding, file["ColorDisabled"]);
            _colorReleased = DefinitionResolver.GetColorWrapper(controller, binding, file["ColorReleased"]);
            _colorPushed = DefinitionResolver.GetColorWrapper(controller, binding, file["ColorPushed"]);

            _margin = DefinitionResolver.Get<Margin>(controller, binding, file["Margin"], Margin.None);

            if ( file["Checked"] != null )
            {
                _checked = DefinitionResolver.Get<bool>(controller, binding, file["Checked"], false);
            }
            else
            {
                _checked = null;
            }
        }

        protected Color ColorFromState
        {
            get
            {
                Color color = _colorReleased.Value;

                if (DisabledState > 0)
                {
                    color = GraphicsHelper.MixColors(color, _colorDisabled.Value, DisabledState);
                }

                if (PushedState > 0)
                {
                    color = GraphicsHelper.MixColors(color, _colorPushed.Value, PushedState);
                }

                return color;
            }
        }
    }
}
