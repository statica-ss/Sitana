using System;
using Sitana.Framework.DataTransfer;

namespace Sitana.Framework.CrashReporter.Data
{
	public class CrashReport: IEcsStructure
	{
		public string Crash;
		public DateAndTime Time;

        void IEcsStructure.Read(EcsReader reader)
		{
			Crash = reader.ReadString(0);
			Time = reader.ReadStructure<DateAndTime>(1);
		}

        void IEcsStructure.Write(EcsWriter writer)
		{
			writer.Write(0, Crash);
			writer.Write(1, Time);
		}
	}
}