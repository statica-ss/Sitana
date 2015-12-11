using System;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views.Parameters;
using System.Collections.Generic;
using Sitana.Framework.Graphics;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Views.QuickData;
using Sitana.Framework.Diagnostics;
using System.Collections;
using Sitana.Framework.Ui.Binding;
using Sitana.Framework.Ui.Interfaces;
using Sitana.Framework.Input.TouchPad;

namespace Sitana.Framework.Ui.Views
{
    public class UiQuickList : UiView, IScrolledElement, IItemsConsumer
	{
		public new static void Parse(XNode node, DefinitionFile file)
		{
			UiView.Parse(node, file);
			var parser = new DefinitionParser(node);

			file["Items"] = parser.ParseDelegate("Items");
			file["Reversed"] = parser.ParseBoolean("Reversed");
			file["RowHeight"] = parser.ParseLength("RowHeight");

            file["ExceedRule"] = parser.ParseEnum<ScrollingService.ExceedRule>("ExceedRule");
            file["WheelScrollSpeed"] = parser.ParseDouble("WheelScrollSpeed");

            file["SeparatorHeight"] = parser.ParseLength("SeparatorHeight");

            file["MaxScrollExceed"] = parser.ParseLength("MaxScrollExceed");

			foreach (var cn in node.Nodes)
			{
				switch (cn.Tag)
				{
				case "UiQuickList.Columns":
					ParseColumns(cn, file);
					break;

                case "UiQuickList.Separator":
                    {
                        if (cn.Nodes.Count != 1)
                        {
                            string error = node.NodeError("UiQuickList.Separator must have exactly 1 child.");

                            if (DefinitionParser.EnableCheckMode)
                            {
                                ConsoleEx.WriteLine(error);
                            }
                            else
                            {
                                throw new Exception(error);
                            }
                        }


                        DefinitionFile separatorDef = DefinitionFile.LoadFile(cn.Nodes[0]);
                        file["Separator"] = separatorDef;
                    }
                    break;
				}
			}
        }

        static void ParseColumns(XNode node, DefinitionFile file)
        {
            string targetId = "Columns";

            List<DefinitionFile> list = new List<DefinitionFile>();

            for (int idx = 0; idx < node.Nodes.Count; ++idx)
            {
                XNode childNode = node.Nodes[idx];
                DefinitionFile newFile = DefinitionFile.LoadFile(childNode);

                if (newFile.Class != typeof(QuickColumnDefinition))
                {
                    string error = node.NodeError("Column must be QuickColumnDefinition.");
                    if (DefinitionParser.EnableCheckMode)
                    {
                        ConsoleEx.WriteLine(error);
                    }
                    else
                    {
                        throw new Exception(error);
                    }
                }

                list.Add(newFile);
            }

            if (file[targetId] != null)
            {
                string error = node.NodeError("Columns already defined");
                if (DefinitionParser.EnableCheckMode)
                {
                    ConsoleEx.WriteLine(error);
                }
                else
                {
                    throw new Exception(error);
                }
            }
            else
            {
                file[targetId] = list;
            }
        }

		IItemsProvider _items;
		List<QuickColumnDefinition> _columns;

		bool _reversed;

		Length _rowHeight;
        Length _separatorHeight;

        ScrollingService _scrollingService;
        ScrollingService.ExceedRule _rule = ScrollingService.ExceedRule.Allow;

        Scroller _scroller = null;
        Point _maxScroll = Point.Zero;

        float _wheelSpeed = 0;

        bool _recalculateScroll = true;

        QuickSeparator _separator = null;
        Length _maxScrollExceed;

        public override Rectangle Bounds
        {
            get
            {
                return base.Bounds;
            }
            set
            {
                base.Bounds = value;
                _recalculateScroll = true;
            }
        }

        protected override void OnAdded()
        {
            Scroller.Mode mode = Scroller.Mode.VerticalDrag | Scroller.Mode.VerticalWheel;

            _scrollingService = new ScrollingService(this, _rule, _maxScrollExceed);
            _scroller = new Scroller(this, mode, _scrollingService, _wheelSpeed);

            base.OnAdded();
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();

            _items.Unsubscribe(this);
            _scrollingService.Remove();
        }

        protected override void OnGesture(Gesture gesture)
        {
            _scroller.OnGesture(gesture);
        }

		protected override bool Init(object controller, object binding, DefinitionFile definition)
		{
			if (!base.Init(controller, binding, definition))
			{
				return false;
			}

			DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiQuickList));

			_items = DefinitionResolver.GetValueFromMethodOrField(Controller, Binding, file["Items"]) as IItemsProvider;

			if (_items == null)
			{
				return false;
			}

            _items.Subscribe(this);

			_rowHeight = DefinitionResolver.Get<Length>(Controller, Binding, file["RowHeight"], Length.Default);
            _separatorHeight = DefinitionResolver.Get<Length>(Controller, Binding, file["SeparatorHeight"], Length.Zero);

            _maxScrollExceed = DefinitionResolver.Get<Length>(Controller, Binding, file["MaxScrollExceed"], ScrollingService.MaxScrollExceed);
			_reversed = DefinitionResolver.Get<bool>(Controller, Binding, file["Reversed"], false);

            _wheelSpeed = (float)DefinitionResolver.Get<double>(Controller, Binding, file["WheelScrollSpeed"], 0);
            _rule = DefinitionResolver.Get<ScrollingService.ExceedRule>(Controller, Binding, file["ExceedRule"], ScrollingService.ExceedRule.Allow);

			_columns = new List<QuickColumnDefinition>();

			List<DefinitionFile> columnsDef = file["Columns"] as List<DefinitionFile>;

			if (columnsDef == null)
			{
				throw new Exception("Columns not defined!");
			}

			foreach (var cd in columnsDef)
			{
				QuickColumnDefinition columnDefinitionObj = cd.CreateInstance(Controller, Binding) as QuickColumnDefinition;
				_columns.Add(columnDefinitionObj);
			}

            DefinitionFile separatorDef = file["Separator"] as DefinitionFile;

            if(separatorDef != null)
            {
                _separator = separatorDef.CreateInstance(Controller, Binding) as QuickSeparator;
            }

			return true;
		}

        public void MoveListBy(int count)
        {
            int height = _rowHeight.Compute(Bounds.Height) + _separatorHeight.Compute(Bounds.Height);
            _scrollingService.ScrollPositionY += height * count;
            _scrollingService.Process();
        }

		protected override void Draw(ref UiViewDrawParameters parameters)
		{
			base.Draw(ref parameters);

			int position = 0;
			int startPosition = ScreenBounds.Top;

            int separatorHeight = _separator != null ? Math.Max(1,_separatorHeight.Compute(Bounds.Height)) : 0;
            int rowHeight = _rowHeight.Compute(Bounds.Height);

			Rectangle target = new Rectangle();


            float startIndexF = _scrollingService.ScrollPositionY / (float)(rowHeight + separatorHeight);

            int startIndex = (int)startIndexF;
            startPosition -= (int)_scrollingService.ScrollPositionY % (rowHeight + separatorHeight);

            if(_reversed)
            {
                startIndex = _items.Count - startIndex - 1;
            }

			Rectangle textTarget = new Rectangle();
            int maxY = ScreenBounds.Bottom;

            parameters.DrawBatch.PushClip(ScreenBounds);

            target = ScreenBounds;
            target.Y = startPosition;
            target.Height = separatorHeight;

            if (separatorHeight > 0)
            {
                for (int dataIndex = startIndex; dataIndex < _items.Count && dataIndex >= 0; )
                {
                    _separator.Draw(parameters.DrawBatch, target, parameters.Opacity);

                    target.Y += rowHeight + separatorHeight;

                    if (target.Y > maxY)
                    {
                        break;
                    }

                    dataIndex += _reversed ? -1 : 1;
                }
            }

            startPosition += separatorHeight;

			for (int columnIndex = 0; columnIndex < _columns.Count; ++columnIndex)
			{
				var column = _columns[columnIndex];
				int width = column.Width.Compute(Bounds.Width);

				UniversalFont font = column.Font.Font;
				float fontScale = column.Font.Scale;
				float fontSpacing = column.Font.Spacing;

				int lineHeight = column.LineHeight;

				TextAlign textAlign = column.TextAlign;

				target.X = position + column.TextMargin.Left + ScreenBounds.Left;
				target.Width = width - column.TextMargin.Width;

				target.Y = startPosition;
                target.Height = rowHeight;

				Margin textMargin = column.TextMargin;

                int dataIndex = startIndex;
                int count = _items.Count;

                for (int idx = 0; idx < count; idx++ )
                {
                    textTarget.X = target.X;
                    textTarget.Width = target.Width;

                    textTarget.Y = target.Y + textMargin.Top;
                    textTarget.Height = target.Height - textMargin.Height;

                    if (dataIndex >= 0 && dataIndex < count)
                    {
                        QuickDataRow row = (QuickDataRow)(_items.ElementAt(dataIndex));

                        Color color = row.Colors[columnIndex].Value;
                        string text = row.Labels[columnIndex];    

                        parameters.DrawBatch.DrawText(font, text, textTarget, textAlign, color, fontSpacing, lineHeight, fontScale);
                    }

                    target.Y += target.Height + separatorHeight;

                    if (target.Y > maxY)
                    {
                        break;
                    }

                    dataIndex += _reversed ? -1 : 1;
                }

				position += width;
			}

            parameters.DrawBatch.PopClip();
		}

        protected override void Update(float time)
        {
            base.Update(time);

            if(_recalculateScroll)
            {
                RecalculateScroll();
            }
        }

        void RecalculateScroll()
        {
            int height = (_rowHeight.Compute(Bounds.Height) + _separatorHeight.Compute(Bounds.Height)) * _items.Count;

            _maxScroll.X = 0;
            _maxScroll.Y = height;
        }

        Rectangle IScrolledElement.ScreenBounds { get { return ScreenBounds; } }

        int IScrolledElement.MaxScrollX { get { return _maxScroll.X; } }
        int IScrolledElement.MaxScrollY { get { return _maxScroll.Y; } }

        ScrollingService IScrolledElement.ScrollingService { get { return _scrollingService; } }

        void IItemsConsumer.Added(object item, int index)
        {
            _recalculateScroll = true;
        }

        void IItemsConsumer.Removed(object item)
        {
            _recalculateScroll = true;
        }

        void IItemsConsumer.RemovedAll()
        {
            _recalculateScroll = true;
        }

        void IItemsConsumer.Recalculate()
        {
            _recalculateScroll = true;
        }
    }
}

