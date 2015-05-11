using Sitana.Framework.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Serialization
{
    public interface IXSerializable
    {
        void Deserialize(XNode node);
        void Serialize(XNode node);
    }
}
