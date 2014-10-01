using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.DataTransfer;
using Sitana.Framework.Games.Elements;

namespace Sitana.Framework.Games.Scene
{
    public class PpScene: List<PpElement>, IEcsStructure
    {
        public static readonly Type[] SceneTypes = new Type[] {typeof(PpScene), typeof(PpRectangle), typeof(PpCircle), typeof(PpPolygon)};

        void IEcsStructure.Read(EcsReader reader)
        {
            byte[] data = reader.ReadByteArray(0);

            int offset = 0;

            while(offset < data.Length)
            {
                int size = EcsReader.GetStructureSize(data, offset+1);
                EcsReader r = new EcsReader(data, offset, size+1, reader);

                Add(r.ReadStructure<PpElement>(0));

                offset += size+1;
            }
        }

        void IEcsStructure.Write(EcsWriter writer)
        {
            List<Byte> data = new List<Byte>();

            foreach (var element in this)
            {
                EcsWriter w = new EcsWriter();
                w.Write(0, element);

                data.AddRange(w.Data);
            }

            writer.Write(0, data.ToArray());
        }

        public void MoveToBack(PpElement element)
        {
            int index = IndexOf(element);

            if (index > 0)
            {
                RemoveAt(index);
                Insert(index - 1, element);
            }
        }

        public void MoveToFront(PpElement element)
        {
            int index = IndexOf(element);

            if (index < Count-1)
            {
                RemoveAt(index);
                Insert(index+1, element);
            }
        }

        public void SendToBack(PpElement element)
        {
            int index = IndexOf(element);

            RemoveAt(index);
            Insert(0, element);            
        }

        public void BringToFront(PpElement element)
        {
            int index = IndexOf(element);

            RemoveAt(index);
            Add(element);
        }

        public void Overwrite(PpElement oldElement, PpElement newElement)
        {
            int index = IndexOf(oldElement);

            RemoveAt(index);
            Insert(index, newElement);
        }
    }
}
