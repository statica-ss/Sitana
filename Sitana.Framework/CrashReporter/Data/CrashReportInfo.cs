using System;
using Ebatianos.DataTransfer;
using System.Collections.Generic;

namespace Ebatianos.CrashReporter.Data
{
	public class CrashReportInfo: IEcsStructure
	{
		public string DeviceGuid;
		public string ApplicationGuid;

        List<CrashReport> _crashes = new List<CrashReport>();

        const int _startIndex = 10;
        public const int MaxCapacity = 192;

        public void Add(CrashReport report)
        {
            if (_crashes.Count >= MaxCapacity)
            {
                throw new Exception("To much crashes in one report.");
            }

            _crashes.Add(report);
        }

        void IEcsStructure.Read(EcsReader reader)
		{
            throw new NotImplementedException("This structure is only to send.");
		}

        void IEcsStructure.Write(EcsWriter writer)
		{
            writer.Write(0, _crashes.Count);
			writer.Write(1, DeviceGuid);
			writer.Write(2, ApplicationGuid);

            writer.Write(3, _startIndex);

            for (int idx = 0; idx < _crashes.Count; ++idx)
            {
                writer.Write((byte)(idx+_startIndex), _crashes[idx]);
            }
		}
	}
}