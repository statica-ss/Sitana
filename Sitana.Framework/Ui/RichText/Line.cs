using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.RichText
{
    public struct Line
    {
        public void AddString(ref TagProperties props, string text)
        {
            Entities.Add(new Entity()
            {
                Font = props.FontType,
                Size = props.FontSize,
                Text = text,
                Url = props.Url
            });
        }

        public List<Entity> Entities;
        public int SpaceSize;
        public int LineHeight;
    }
}
