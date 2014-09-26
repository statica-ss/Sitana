using System;
using Sitana.Framework.DataTransfer;

namespace Sitana.Framework.CrashReporter.Data
{
	public class Device: IEcsStructure
	{
		public int? Id;
		public string Guid;
		public string Name;
		public string Platform;
		public string OsVersion;

        void IEcsStructure.Read(EcsReader reader)
		{
			Id = reader.HasField(0) ? (int?)reader.ReadInt32(0) : null;
			Guid = reader.ReadString(1);
			Name = reader.ReadString(2);
			Platform = reader.ReadString(3);
			OsVersion = reader.ReadString(4);
		}

        void IEcsStructure.Write(EcsWriter writer)
		{
			if (Id!=null) writer.Write(0, Id.Value);
			writer.Write(1, Guid);
			writer.Write(2, Name);
			writer.Write(3, Platform);
			writer.Write(4, OsVersion);
		}
	}
}