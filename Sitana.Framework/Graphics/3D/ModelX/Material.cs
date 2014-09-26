using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace Ebatianos.Graphics.Model
{
    public class Material
    {
        public String Texture;
        public Vector3 Diffuse;
        public Vector3 Ambient;
        public Vector3 Specular;
        public Vector3 Emissive;
        public Single SpecularExponent;

        public Single Opacity;

        public MaterialTextures Textures { set; get;}

        public Material(String texture): this(texture, Vector3.One) { }
        public Material(String texture, Vector3 diffuse) : this(texture, diffuse, Vector3.One) { }
        public Material(String texture, Vector3 diffuse, Vector3 ambient) : this(texture, diffuse, ambient, Vector3.One, 0) { }
        public Material(String texture, Vector3 diffuse, Vector3 ambient, Vector3 specular, Single specularExponent) : this(texture, diffuse, ambient, specular, specularExponent, Vector3.Zero) { }

        public Material(String texture, Vector3 diffuse, Vector3 ambient, Vector3 specular, Single specularExponent, Vector3 emissive): this(texture, diffuse, ambient, specular, specularExponent, emissive, 1){}

        public Material(String texture, Vector3 diffuse, Vector3 ambient, Vector3 specular, Single specularExponent, Vector3 emissive, Single opacity)
        {
            Texture = texture;

            Diffuse = diffuse;
            Ambient = ambient;
            Specular = specular;
            Emissive = emissive;

            SpecularExponent = specularExponent;

            Opacity = opacity;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write( Texture == null ? String.Empty : Texture );

            SerializeColor(writer, Diffuse);
            SerializeColor(writer, Ambient);
            SerializeColor(writer, Specular);
            SerializeColor(writer, Emissive);

            writer.Write(SpecularExponent);
            writer.Write(Opacity);
        }

        private void SerializeColor(BinaryWriter writer, Vector3 color)
        {
            Color serialize = new Color(color.X, color.Y, color.Z);

            writer.Write(serialize.R);
            writer.Write(serialize.G);
            writer.Write(serialize.B);
        }

        private static Vector3 DeserializeColor(BinaryReader reader)
        {
            return new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte()).ToVector3();
        }

        public static Material Deserialize(BinaryReader reader)
        {
            String texture = reader.ReadString();

            if (String.IsNullOrWhiteSpace(texture))
            {
                texture = null;
            }

            Vector3 diffuse = DeserializeColor(reader);
            Vector3 ambient = DeserializeColor(reader);
            Vector3 specular = DeserializeColor(reader);
            Vector3 emmisive = DeserializeColor(reader);
            Single specularExponent = reader.ReadSingle();
            Single opacity = reader.ReadSingle();

            return new Material(texture, diffuse, ambient, specular, specularExponent, emmisive, opacity);
        }
    }
}
