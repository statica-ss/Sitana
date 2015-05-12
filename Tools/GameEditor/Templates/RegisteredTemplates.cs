using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.Binding;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Sitana.Framework.Xml;
using Sitana.Framework;

namespace GameEditor
{
    public class RegisteredTemplates: Singleton<RegisteredTemplates>
    {
        public class Template
        {
            public string Name {get;set;}
            public string Guid { get; set; }
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
                Guid = "",
                Path = null,
                ShortPath = ""
            });

			LoadTemplatesList();
        }

        public Template FindTemplate(string guid)
        {
            for(int idx = 0; idx < Templates.Count; ++idx)
            {
                if(Templates[idx].Guid == guid)
                {
                    return Templates[idx];
                }
            }

            return null;
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
                        XNode node = XFile.Create(zipStream, entry.Name);

                        if (node.Tag != "Template")
                        {
                            return;
                        }

                        foreach (var cn in node.Nodes)
                        {
                            if (cn.Tag == "Properties")
                            {
                                string name = cn.Attribute("Name");
                                string guid = cn.Attribute("Guid");

                                string shortPath = path;

                                int maxLength = 34;

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
                                    Guid = guid,
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

		public void SaveTemplatesList()
		{
			using (var store = Platform.GetUserStoreForApplication())
			{
				using (var stream = store.OpenFile("RegisteredTemplates", FileMode.Create))
				{
					BinaryWriter writer = new BinaryWriter(stream);
					writer.Write(Templates.Count - 1);

					for (int idx = 1; idx < Templates.Count; ++idx)
					{
						writer.Write(Templates[idx].Path);
					}
				}
			}
		}

		void LoadTemplatesList()
		{
			try
			{
				using (var store = Platform.GetUserStoreForApplication())
				{
					using (var stream = store.OpenFile("RegisteredTemplates", FileMode.Open))
					{
						BinaryReader reader = new BinaryReader(stream);

						int count = reader.ReadInt32();

						while(count>0)
						{
							count--;
							string path = reader.ReadString();

							try
							{
								Register(path);
							}
							catch{}
						}
					}
				}
			}
			catch{}
		}
    }
}
