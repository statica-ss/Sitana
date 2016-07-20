using Sitana.Framework.Content;
using Sitana.Framework.Ui.Views.HtmlRendererImpl;
using System.IO;
using TheArtOfDev.HtmlRenderer.Core;

namespace Sitana.Framework.Ui.Views.Controls.HtmlRendererImpl
{
    public class LoadCssDataFromCss: ContentLoader.AdditionalType
    {
        public static void Register()
        {
            RegisterType(typeof(CssData), Load, true);
        }

        public static object Load(string name)
        {
            using (var stream = ContentLoader.Current.Open(name + ".css"))
            {
                StreamReader reader = new StreamReader(stream);
                string css = reader.ReadToEnd();

                return CssData.Parse(HtmlViewAdapter.Instance, css, true);
            }
        }
    }
}
