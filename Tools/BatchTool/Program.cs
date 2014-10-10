using System;
using System.IO;
using Sitana.Framework.Xml;

namespace BatchTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                XFile file = null;
                using (Stream stream = new FileStream(args[0], FileMode.Open))
                {
                    file = XFile.FromStream(stream, args[0]);
                }

                Parse(file);
            }
        }

        static void Parse(XNode node)
        {
            if (node.Tag != "Script")
            {
                Console.WriteLine(node.NodeError("\nInvalid node name. Expected: Script"));
                return;
            }

            foreach (var cn in node.Nodes)
            {
                ParseNode(cn);
            }
        }

        static void ParseNode(XNode node)
        {
            switch (node.Tag)
            {
                case "PackTextures":
                    TexturePacker.Pack(node);
                    break;
            }
        }
    }
}
