using System;
using Ebatianos.DataTransfer;

namespace Ebatianos.CrashReporter.Data
{
	public class Crash: IEcsStructure
	{
		public int? Id;
		public string ApplicationGuid;
		public string Exception;

        void IEcsStructure.Read(EcsReader reader)
		{
			Id = reader.HasField(0) ? (int?)reader.ReadInt32(0) : null;
			ApplicationGuid = reader.ReadString(1);
			Exception = reader.ReadString(2);
		}

        void IEcsStructure.Write(EcsWriter writer)
		{
			if (Id!=null) writer.Write(0, Id.Value);
			writer.Write(1, ApplicationGuid);
			writer.Write(2, Exception);
		}
	}
}