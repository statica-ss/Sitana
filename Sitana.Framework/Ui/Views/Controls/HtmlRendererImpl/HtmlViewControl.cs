using Sitana.Framework.Ui.Core;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace Sitana.Framework.Ui.Views.HtmlRendererImpl
{
    public class HtmlViewControl : RControl
    {
        bool _down;
        RPoint _location;

        public HtmlViewControl() : base(HtmlViewAdapter.Instance)
        {
        }

        public override bool LeftMouseButton
        {
            get
            {
                return _down;
            }
        }

        public override RPoint MouseLocation
        {
            get
            {
                return _location;
            }
        }

        public override bool RightMouseButton
        {
            get
            {
                return false;
            }
        }

        public override void DoDragDropCopy(object dragDropData)
        {
            
        }

        public override void Invalidate()
        {
            AppMain.Redraw(true);
        }

        public void Update(bool down, RPoint location)
        {
            _down = down;
            _location = location;
        }

        public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth)
        {
            //var theFont = font as HtmlViewFont;
            //var size = theFont.UiFont.MeasureString(str);

            charFit = 0;
            charFitWidth = 0;
        }

        public override void SetCursorDefault()
        {
            
        }

        public override void SetCursorHand()
        {
            
        }

        public override void SetCursorIBeam()
        {
            
        }
    }
}
