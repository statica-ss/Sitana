using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.Binding;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Sitana.Framework.Xml;

namespace GameEditor
{
    public class RegisteredTemplates: Singleton<RegisteredTemplates>
    {
        public class Template
        {
            public string Name {get;set;}
            public string Path { get; set; }
            public string ShortPath { get; set; }
        }

        public ItemsList<Template> Templates { get; private set; }

        public RegisteredTemplates()
        {
            Templates = new ItemsList<Template>();

            Templates.Add(new Template()
            {
                Name="Sample",
                Path = null,
                ShortPath = ""
            });
        }

        public void Register(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                ZipFile zf = new ZipFile(stream);
                ZipEntry entry = zf.GetEntry("Definition.xml");

                if (entry != null)
                {
                    using(Stream zipStream = zf.GetInputStream(entry))
                    {
                        XNode node = XFile.FromStream(zipStream, entry.Name);

                        if (node.Tag != "Template")
                        {
                            return;
                        }

                        foreach (var cn in node.Nodes)
                        {
                            if (cn.Tag == "Properties")
                            {
                                string name = cn.Attribute("Name");
                                string shortPath = path;

                                int maxLength = 48;

                                if (shortPath.Length > maxLength)
                                {
                                    string fname = Path.GetFileName(shortPath);
                                    string dir = Path.GetDirectoryName(shortPath);

                                    int len = maxLength - fname.Length - 4;

                                    if (len > 0)
                                    {
                                        shortPath = dir.Substring(0, len) + "/../" + fname;
                                    }
                                    else
                                    {
                                        shortPath = "../" + fname.Substring(fname.Length - (maxLength - 3));
                                    }
                                }

                                InvalidateTemplate(path);

                                Templates.Add(new Template()
                                {
                                    Name = name,
                                    Path = path,
                                    ShortPath = shortPath.Replace('\\','/')
                                });
                            }
                        }
                    }
                }
            }
        }

        public void InvalidateTemplate(string path)
        {
            for (int idx = 0; idx < Templates.Count; ++idx)
            {
                if (Templates[idx].Path == path)
                {
                    Templates.RemoveAt(idx);
                    return;
                }
            }
        }
    }
}
