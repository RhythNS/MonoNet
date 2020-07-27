using System.Collections.Generic;

namespace MonoNet.Network
{
    public class NetState
    {
        private readonly byte[][] rawState = new byte[256][];

        public void Set(int index, byte[] array) => rawState[index] = array;

        public List<byte> GetDif(NetState fromState, List<byte> difList)
        {
            for (int i = 0; i < rawState.Length; i++)
            {
                if (rawState[i] == null)
                    continue;

                if (fromState.rawState[i] != null)
                {
                    if (rawState[i].Length != fromState.rawState[i].Length)
                        continue;

                    bool shouldContinue = false;

                    for (int j = 0; j < rawState[i].Length; j++)
                    {
                        if (rawState[i] != fromState.rawState[j])
                        {
                            shouldContinue = true;
                            break;
                        }
                    }

                    if (shouldContinue == true)
                        continue;
                }

                difList.Add((byte)i);
                difList.AddRange(rawState[i]);
            }
            return difList;
        }
    }
}
