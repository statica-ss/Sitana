using System;

namespace Ebatianos.DataTransfer
{
	public class EcsRequest: IEcsStructure
	{
        public object StateObject;

		public string Action;
		public IEcsStructure Data;

        internal EcsHttpClient.ResponseHandler Handler;

        void IEcsStructure.Read(EcsReader reader)
		{
            throw new NotImplementedException("This structure is only to send.");
		}

        void IEcsStructure.Write(EcsWriter writer)
		{
			writer.Write(0, Action);
			writer.Write(1, Data);
		}
	}
}