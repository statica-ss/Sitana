using System;
using Sitana.Framework.DataTransfer;

namespace Sitana.Framework.CrashReporter.Data
{
	public class DateAndTime: IEcsStructure
	{
		public int Year;
		public int Month;
		public int Day;
		public int Hour;
		public int Minute;
		public int Second;

        public DateAndTime()
        {
        }

        public DateAndTime(DateTime dateTime)
        {
            Year = dateTime.Year;
            Month = dateTime.Month;
            Day = dateTime.Day;

            Hour = dateTime.Hour;
            Minute = dateTime.Minute;
            Second = dateTime.Second;
        }

        void IEcsStructure.Read(EcsReader reader)
		{
			Year = reader.ReadInt32(0);
			Month = reader.ReadInt32(1);
			Day = reader.ReadInt32(2);
			Hour = reader.ReadInt32(3);
			Minute = reader.ReadInt32(4);
			Second = reader.ReadInt32(5);
		}

        void IEcsStructure.Write(EcsWriter writer)
		{
			writer.Write(0, Year);
			writer.Write(1, Month);
			writer.Write(2, Day);
			writer.Write(3, Hour);
			writer.Write(4, Minute);
			writer.Write(5, Second);
		}
	}
}