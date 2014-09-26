using System;
using Ebatianos.DataTransfer;

namespace Ebatianos.CrashReporter.Data
{
	public class Application: IEcsStructure
	{
		public int? Id;
		public string Guid;
		public string Name;

        void IEcsStructure.Read(EcsReader reader)
		{
			Id = reader.HasField(0) ? (int?)reader.ReadInt32(0) : null;
			Guid = reader.ReadString(1);
			Name = reader.ReadString(2);
		}

        void IEcsStructure.Write(EcsWriter writer)
		{
			if (Id!=null) writer.Write(0, Id.Value);
			writer.Write(1, Guid);
			writer.Write(2, Name);
		}
	}
}