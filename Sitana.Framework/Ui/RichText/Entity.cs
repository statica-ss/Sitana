using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.RichText
{
    public struct Entity
    {
        public readonly EntityType Type;
        public readonly FontType Font;
        public readonly SizeType Size;
        public readonly object Data;
        public readonly string Url;

        public object Extra;

        public Entity(EntityType type, ref TagProperties props, string text)
        {
            Type = type;
            Font = props.FontType;
            Size = props.FontSize;
            Url = props.Url;
            Data = null;
            Extra = null;

            switch (type)
            {
                case EntityType.ListNumber:
                    Data = props.ListIndex;
                    break;

                case EntityType.Indent:
                    Data = props.IndentLevel;
                    break;

                case EntityType.Image:
                case EntityType.String:
                    Data = text;
                    break;
            }
        }

        public static Entity? MergeString(Entity previous, ref TagProperties props, string text)
        {
            if (previous.Type == EntityType.String && previous.Font == props.FontType && previous.Size == props.FontSize && previous.Url == props.Url)
            {
                string prev = previous.Data as string;

                return new Entity(EntityType.String, ref props, prev + text);
            }

            return null;
        }
    }
}
