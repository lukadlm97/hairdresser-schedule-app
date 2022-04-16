using System.IO;
using Microsoft.IO;

namespace HairdresserScheduleApp.BusinessLogic.Utilities
{
    public class MemoryStreamPool: IMemoryStreamPool
    {
        private readonly RecyclableMemoryStreamManager RmsManager = new RecyclableMemoryStreamManager();

        public MemoryStream GetStream()
        {
            return RmsManager.GetStream();
        }

        public MemoryStream GetStream(byte[] byteArray)
        {
            return RmsManager.GetStream(byteArray);
        }
    }
    public interface IMemoryStreamPool
    {
        MemoryStream GetStream();

        MemoryStream GetStream(byte[] byteArray);
    }
}
