using System;
using Microsoft.Xna.Framework;
using System.IO;

namespace Sitana.Framework.Games
{
    public abstract class Layer
    {
        public string Name;
        public Vector2 ScrollSpeed = Vector2.One;

        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(ScrollSpeed.X);
            writer.Write(ScrollSpeed.Y);
        }

        public virtual void Deserialize(BinaryReader reader)
        {
            Name = reader.ReadString();
            ScrollSpeed.X = reader.ReadSingle();
            ScrollSpeed.Y = reader.ReadSingle();
        }
    }
}

