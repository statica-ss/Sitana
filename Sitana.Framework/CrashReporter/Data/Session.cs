using System;
using Ebatianos.DataTransfer;

namespace Ebatianos.CrashReporter.Data
{
	public class Session: IEcsStructure
	{
		public int? Id;
		public string ApplicationGuid;
		public string DeviceGuid;
		public DateAndTime Time;

        void IEcsStructure.Read(EcsReader reader)
		{
			Id = reader.HasField(0) ? (int?)reader.ReadInt32(0) : null;
			ApplicationGuid = reader.ReadString(1);
			DeviceGuid = reader.ReadString(2);
			Time = reader.ReadStructure<DateAndTime>(3);
		}

        void IEcsStructure.Write(EcsWriter writer)
		{
			if (Id!=null) writer.Write(0, Id.Value);
			writer.Write(1, ApplicationGuid);
			writer.Write(2, DeviceGuid);
			writer.Write(3, Time);
		}
	}
}