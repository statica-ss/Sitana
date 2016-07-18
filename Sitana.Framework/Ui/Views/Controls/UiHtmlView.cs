using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Cs;
using Sitana.Framework.Input.TouchPad;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views.HtmlRendererImpl;
using Sitana.Framework.Ui.Views.Parameters;
using Sitana.Framework.Xml;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.Core.Entities;

namespace Sitana.Framework.Ui.Views
{
    public class UiHtmlView : UiView
    {
        public delegate void ImageLoadedDelegate(Texture2D image);

        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);
            file["Content"] = parser.ParseString("Content");
            file["CssStyles"] = parser.ParseString("CssStyles");

            file["LinkClick"] = parser.ParseDelegate("LinkClick");

            file["LoadImage"] = parser.ParseDelegate("LoadImage");

            file["ClickedLinkColor"] = parser.ParseColor("ClickedLinkColor");

            file["SelectionEnabled"] = parser.ParseBoolean("SelectionEnabled");

            file["GestureMargin"] = parser.ParseMargin("GestureMargin");
        }

        SharedString _content;

        CssData _cssData;

        HtmlContainerInt _container = new HtmlContainerInt(HtmlViewAdapter.Instance);

        HtmlViewControl _control = new HtmlViewControl();

        Point _lastSize = Point.Zero;

        SharedValue<bool> _enabledSelection;

        Rectangle _clickedArea = Rectangle.Empty;

        ColorWrapper _clickedColor;
        string _clickedHref;

        int _documentHeight = 10;

        protected Margin _gestureMargin;

        HtmlViewGraphics _graphics;

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiHtmlView));

            string styles = DefinitionResolver.GetString(Controller, Binding, file["CssStyles"]);

            _cssData = CssData.Parse(HtmlViewAdapter.Instance, styles, true);

            _content = DefinitionResolver.GetSharedString(Controller, Binding, file["Content"]);

            _clickedColor = DefinitionResolver.GetColorWrapper(Controller, Binding, file["ClickedLinkColor"]) ?? UiLabel.DefaultTextColor;

            _enabledSelection = DefinitionResolver.GetShared(Controller, Binding, file["SelectionEnabled"], false);

            _gestureMargin = DefinitionResolver.Get<Margin>(Controller, Binding, file["GestureMargin"], Margin.None);

            _content.ValueChanged += _content_ValueChanged;

            _container.ImageLoad += _container_ImageLoad;

            _container.AvoidAsyncImagesLoading = false;
            _container.AvoidImagesLateLoading = false;

            _container.SetHtml(_content.StringValue, _cssData);

            _container.Refresh += (o, e) =>
            {
                Recalculate();
            };

            EnabledGestures = GestureType.Tap | GestureType.Down | GestureType.Up | GestureType.Move | GestureType.MouseMove | GestureType.DoubleTap;

            RegisterDelegate("LinkClick", file["LinkClick"]);
            RegisterDelegate("LoadImage", file["LoadImage"]);

            _container.IsSelectionEnabled = _enabledSelection.Value;

            _enabledSelection.ValueChanged += (value)=>
            {
                _container.IsSelectionEnabled = value;
            };

            _graphics = new HtmlViewGraphics(AppMain.Current.DrawBatch);
            return true;
        }

        private void _container_ImageLoad(object sender, HtmlImageLoadEventArgs e)
        {
            if (HasDelegate("LoadImage"))
            {
                ImageLoadedDelegate onImageLoaded = (texture) =>
                {
                    e.Callback(texture);
                };

                e.Handled = CallDelegate<bool>("LoadImage", new InvokeParam("callback", onImageLoaded), new InvokeParam("src", e.Src));
            }
        }

        private void _content_ValueChanged()
        {
            _container.SetHtml(_content.StringValue, _cssData);
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();

            _content.ValueChanged -= _content_ValueChanged;
            _container.Dispose();
            _graphics.Dispose();
        }

        void Recalculate()
        {
            
            _container.MaxSize = new RSize(Bounds.Width, double.MaxValue);
            var maxSize = HtmlRendererUtils.MeasureHtmlByRestrictions(_graphics, _container, new RSize(10, 10), new RSize(Bounds.Width, double.MaxValue));
            _container.MaxSize = maxSize;

            _documentHeight = (int)(maxSize.Height+1);

            if (maxSize.Height != Bounds.Height)
            {
                Parent.RecalcLayout(this);
            }

            _lastSize = Bounds.Size;
        }

        public override Point ComputeSize(int width, int height)
        {
            Point size = base.ComputeSize(width, height);
            size.Y = _documentHeight;
            return size;
        }

        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();
            Recalculate();
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            if(_lastSize != Bounds.Size)
            {
                Recalculate();
            }

            base.Draw(ref parameters);

            _graphics.Prepare(ScreenBounds.Location);

            _graphics.AddActiveArea(_clickedArea, _clickedColor.Value);
            

            _container.PerformPaint(_graphics);

        }

        Rectangle GetLinkTouchRectangle(RRect area)
        {
            Rectangle bounds = new Rectangle();

            bounds.X = (int)(area.X - _gestureMargin.Left);
            bounds.Width = (int)(area.Width + _gestureMargin.Width);

            bounds.Y = (int)(area.Y -_gestureMargin.Top);
            bounds.Height = (int)(area.Height + _gestureMargin.Height);

            return bounds;
        }

        protected override void OnGesture(Gesture gesture)
        {
            Point position = gesture.Position.ToPoint() - ScreenBounds.Location;
            RPoint point = position.ToHtmlRendererPoint();

            switch (gesture.GestureType)
            {
                case GestureType.CapturedByOther:
                    _clickedArea = Rectangle.Empty;
                    break;

                case GestureType.Up:
                    _control.Update(false, point);
                    _container.HandleMouseUp(_control, point, new RMouseEvent(false));

                    if (_clickedArea != Rectangle.Empty)
                    {
                        var areaToCheck = _clickedArea;
                        if(GetLinkTouchRectangle(_clickedArea.ToHtmlRendererRect()).Contains(position))
                        {
                            CallDelegate("LinkClick", new InvokeParam("link", _clickedHref));
                        }
                    }
                    
                    _clickedArea = Rectangle.Empty;
                    break;

                case GestureType.Down:
                    _control.Update(true, point);
                    _container.HandleMouseDown(_control, point);

                    {
                        foreach(var link in _container.GetLinks())
                        {
                            if(GetLinkTouchRectangle(link.Rectangle).Contains(position))
                            {
                                _clickedArea = link.Rectangle.ToXnaRectangle();
                                _clickedHref = link.Href;
                                break;
                            }
                        }
                    }

                    break;

                case GestureType.DoubleTap:
                    _control.Update(false, point);
                    _container.HandleMouseDoubleClick(_control, point);
                    break;

                case GestureType.Move:
                    _control.Update(true, point);
                    _container.HandleMouseMove(_control, point);

                    if (!GetLinkTouchRectangle(_clickedArea.ToHtmlRendererRect()).Contains(position))
                    {
                        _clickedArea = Rectangle.Empty;
                    }
                    break;

                case GestureType.MouseMove:
                    _control.Update(false, point);
                    _container.HandleMouseMove(_control, point);
                    break;
            }
        }
    }
}
