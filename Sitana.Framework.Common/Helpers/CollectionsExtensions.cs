using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework
{
    public static class CollectionsExtensions
    {
        public delegate void ForElementDelegate<T>(T element);

        public static void ForEach<T>(this List<T> list, ForElementDelegate<T> forElement)
        {
            for(int idx = 0; idx < list.Count; ++idx)
            {
                forElement(list[idx]);
            }
        }
    }
}
