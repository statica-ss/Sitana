using Sitana.Framework.Cs;
using Microsoft.Xna.Framework;
using System.IO;
using Sitana.Framework.DataTransfer;
using Sitana.Framework.Games.Scene;
using Sitana.Framework.Games.Elements;

namespace Editor
{
    public class Document: Singleton<Document>
    {
        public PpScene Scene {get; private set;}

        public string Filename { get; private set; }

        public Document()
        {
            New();
        }

        public void New()
        {
            Filename = null;
            Scene = new PpScene();
        }

        public void Open(string path)
        {
            try
            {
                using (Stream stream = new FileStream(path, FileMode.Open))
                {
                    int length = (int)stream.Length;
                    byte[] data = new byte[length];

                    stream.Read(data, 0, length);

                    var scene = EcsProtocol.GetData<PpScene>(data, 0, data.Length, null, PpScene.SceneTypes);

                    Scene = scene;
                    Filename = path;
                }
            }
            catch
            {
            }
        }

        public void Save(string path)
        {
            try
            {
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    byte[] data = EcsProtocol.ToBytes(Scene);

                    stream.Write(data, 0, data.Length);
                    stream.Flush();

                    Filename = path;
                }
            }
            catch
            {
            }
        }

        public PpElement FindTop(Vector2 pos)
        {
            PpElement element = null;

            foreach (var el in Scene)
            {
                bool result = el.Polygon.PointInPolygonAngle(ref pos);
                if (result)
                {
                    element = el;
                }
            }

            return element;
        }
    }
}
