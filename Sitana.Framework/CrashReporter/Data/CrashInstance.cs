using System;
using Ebatianos.DataTransfer;

namespace Ebatianos.CrashReporter.Data
{
	public class CrashInstance: IEcsStructure
	{
		public int? Id;
		public int CrashId;
		public DateAndTime Time;

        void IEcsStructure.Read(EcsReader reader)
		{
			Id = reader.HasField(0) ? (int?)reader.ReadInt32(0) : null;
			CrashId = reader.ReadInt32(1);
			Time = reader.ReadStructure<DateAndTime>(2);
		}

        void IEcsStructure.Write(EcsWriter writer)
		{
			if (Id!=null) writer.Write(0, Id.Value);
			writer.Write(1, CrashId);
			writer.Write(2, Time);
		}
	}
}