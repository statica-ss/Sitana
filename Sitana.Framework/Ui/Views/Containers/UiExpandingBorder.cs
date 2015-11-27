using Microsoft.Xna.Framework;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.Views
{
    public class UiExpandingBorder: UiBorder
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiBorder.Parse(node, file);

            var parser = new DefinitionParser(node);
            
            file["ExpandedWidth"] = parser.ParseLength("ExpandedWidth");
            file["ExpandedHeight"] = parser.ParseLength("ExpandedHeight");
            file["ExpandTime"] = parser.ParseInt("ExpandTime");
            file["Expanded"] = parser.ParseBoolean("Expanded");

            file["CollapseFinished"] = parser.ParseDelegate("CollapseFinished");
            file["ExpandFinished"] = parser.ParseDelegate("ExpandFinished");
        }

        SharedValue<bool> _expanded;
        Length _expandWidth;
        Length _expandHeight;

        double _expandSpeed;
        double _expandedValue;


        public override bool ForceUpdate
        {
            get
            {
                return _expandedValue != 0 && _expandedValue != 1;
            }
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if(!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiExpandingBorder));

            _expanded = DefinitionResolver.GetShared<bool>(Controller, Binding, file["Expanded"], true);

            _expandWidth = DefinitionResolver.Get<Length>(Controller, Binding, file["ExpandedWidth"], PositionParameters.Width);
            _expandHeight = DefinitionResolver.Get<Length>(Controller, Binding, file["ExpandedHeight"], PositionParameters.Height);

            _expandSpeed = DefinitionResolver.Get<int>(Controller, Binding, file["ExpandTime"], 0);

            _expandedValue = _expanded.Value ? 1 : 0;

            RegisterDelegate("CollapseFinished", file["CollapseFinished"]);
            RegisterDelegate("ExpandFinished", file["ExpandFinished"]);

            if(_expandSpeed>0)
            {
                _expandSpeed = 1000 / _expandSpeed;
            }
            else
            {
                _expandSpeed = 10000;
            }

            return true;
        }

        protected override void Update(float time)
        {
            base.Update(time);

            double desiredValue = _expanded.Value ? 1 : 0;
            bool update = false;

            if(_expandedValue<desiredValue)
            {
                _expandedValue += time * _expandSpeed;
                _expandedValue = Math.Min(1, _expandedValue);

                if(_expandedValue == 1)
                {
                    CallDelegate("ExpandFinished");
                }

				_sizeCanChange = SizeChangeDimension.Both;
                update = true;
            }
            else if(_expandedValue > desiredValue)
            {
                _expandedValue -= time * _expandSpeed;
                _expandedValue = Math.Max(0, _expandedValue);
                update = true;
				_sizeCanChange = SizeChangeDimension.Both; 

                if (_expandedValue == 0)
                {
                    CallDelegate("CollapseFinished");
                }
            }

            if(update)
            {
                Parent.RecalcLayout();
            }
        }

        protected override void Draw(ref Parameters.UiViewDrawParameters parameters)
        {
            if (!_clipChildren && _expandedValue < 1)
            {
                parameters.DrawBatch.PushClip(ScreenBounds);
            }

            base.Draw(ref parameters);

            if (!_clipChildren && _expandedValue <1)
            {
                parameters.DrawBatch.PopClip();
            }
        }

        public Point ComputeExpandedSize(int width, int height)
        {
            var size = new Point(_expandWidth.Compute(width - PositionParameters.Margin.Width), _expandHeight.Compute(height - PositionParameters.Margin.Height));

            if (size.X == 0 && PositionParameters.HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                size.X = width;
            }

            if(_expandWidth.IsAuto)
            {
                size.X = 0;
                for (int idx = 0; idx < _children.Count; ++idx)
                {
                    var child = _children[idx];

                    if (child.DisplayVisibility > 0)
                    {
                        size.X = Math.Max(child.Bounds.Right + child.Margin.Right, size.X);
                    }
                }
            }

            if (size.Y == 0 && PositionParameters.VerticalAlignment == VerticalAlignment.Stretch)
            {
                size.Y = height;
            }

            if (_expandHeight.IsAuto)
            {
                size.Y = 0;
                for(int idx = 0; idx < _children.Count; ++idx)
                {
                    var child = _children[idx];

                    if (child.DisplayVisibility > 0)
                    {
                        size.Y = Math.Max(child.Bounds.Bottom + child.Margin.Bottom, size.Y);
                    }
                }
            }

            return size;
        }

        public override Point ComputeSize(int width, int height)
        {
            Point size = base.ComputeSize(width, height);
            Point exSize = ComputeExpandedSize(width, height);

            return new Point((int)(size.X * (1 - _expandedValue) + exSize.X * _expandedValue), (int)(size.Y * (1 - _expandedValue) + exSize.Y * _expandedValue));
        }
    }
}
