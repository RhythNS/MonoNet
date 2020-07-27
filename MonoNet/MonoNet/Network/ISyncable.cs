using System.Collections.Generic;

namespace MonoNet.Network
{
    public interface ISyncable
    {
        void Sync(byte[] data, ref int pointerAt);

        void GetSync(List<byte> data);
    }
}
