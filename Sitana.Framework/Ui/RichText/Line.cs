using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.RichText
{
    public struct Line
    {
        public void Add(Entity entity)
        {
            Entities.Add(entity);
        }

        public List<Entity> Entities;
    }
}
