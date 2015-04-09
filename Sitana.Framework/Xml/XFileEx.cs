using Sitana.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Xml
{
    public class XFileEx: ContentLoader.AdditionalType
    {
        public static void Register()
        {
            RegisterType(typeof(XFile), Load, true);
        }

        // <summary>
        /// Loads content object
        /// </summary>
        /// <param name="name">name of resource</param>
        /// <param name="contentLoader">content loader to load additional resources and files</param>
        /// <returns></returns>
        public static Object Load(string name)
        {
            return FromPath(name);
        }

        public static XFile FromPath(string name)
        {
            name = name + ".xml";

            try
            {
                // Open font definition file and load info.
                using (Stream stream = ContentLoader.Current.Open(name))
                {
                    return XFile.FromStream(stream, name);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("{0}: error: The XML file has invalid elements.", name), ex);
            }
        }
    }
}
