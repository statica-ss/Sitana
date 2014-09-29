using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Diagnostics;
using Sitana.Framework.Xml;

namespace Sitana.Framework.Ui.Core
{
    public class StylesManager: Singleton<StylesManager>
    {
        Dictionary<string, DefinitionFile> _styles = new Dictionary<string, DefinitionFile>();

        public void RegisterStyle(string name, DefinitionFile file, bool overwrite)
        {
            if (_styles.ContainsKey(name))
            {
                if (overwrite)
                {
                    _styles.Remove(name);
                }
                else
                {
                    throw new Exception(String.Format("Style {0} is already defined.", name));
                }
            }

            _styles.Add(name, file);
        }

        public DefinitionFile FindStyle(string name, Type type)
        {
            if (name == null)
            {
                return null;
            }

            DefinitionFile file;
            if ( _styles.TryGetValue(name, out file))
            {
                if ( file.Class == type || file.Class.IsSubclassOf(type) )
                {
                    return file;
                }

                ConsoleEx.WriteLine("Warning: Style {0} type {1} doesn't match desired type {2}.", name, file.Class.FullName, type.FullName);
            }

            return null;
        }

        public void LoadStyles(string filename, bool overwrite)
        {
            XFile file = new XFile(filename);
            DefinitionFile def = DefinitionFile.LoadFile(file);

            if ( def.Class != typeof(StylesContainer))
            {
                throw new Exception(String.Format("Invalid style sheet."));
            }

            Dictionary<string, DefinitionFile> dict = def["Styles"] as Dictionary<string, DefinitionFile>;

            if (dict != null)
            {
                foreach (var el in dict)
                {
                    RegisterStyle(el.Key, el.Value, overwrite);
                }
            }
        }
    }
}
