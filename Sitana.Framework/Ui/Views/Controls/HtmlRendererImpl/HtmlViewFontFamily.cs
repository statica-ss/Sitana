using TheArtOfDev.HtmlRenderer.Adapters;

namespace Sitana.Framework.Ui.Views.HtmlRendererImpl
{
    public class HtmlViewFontFamily : RFontFamily
    {
        string _name;

        public HtmlViewFontFamily(string name)
        {
            _name = name;
        }

        public override string Name { get { return _name; } }
    }
}
