using System;

namespace Ebatianos.DataTransfer
{
	public class EcsResponse: IEcsStructure
	{
        public object StateObject;
        public int Status;

        public string Message;
        public IEcsStructure Data;

        void IEcsStructure.Read(EcsReader reader)
		{
			Status = reader.ReadInt32(0);
            Message = reader.ReadString(1);
            
            if (reader.HasField(2))
            {
                Data = reader.ReadStructure(2);
            }
		}

        void IEcsStructure.Write(EcsWriter writer)
		{
            throw new NotImplementedException("This structure is for read only.");
		}
	}
}