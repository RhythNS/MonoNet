using System;
using System.Collections.Generic;

namespace MonoNet.Network
{
    public class NetUtils
    {
        public static float GetNextFloat(byte[] data, ref int pointerAt)
        {
            float value = BitConverter.ToSingle(data, pointerAt);
            pointerAt += 4;
            return value;
        }

        public static float[] GetNextFloats(byte[] data, ref int pointerAt, int amount)
        {
            float[] values = new float[amount];

            for (int i = 0; i < amount; i++)
                values[i] = GetNextFloat(data, ref pointerAt);

            return values;
        }

        public static void AddFloatToList(float value, List<byte> toAddTo)
        {
            byte[] converted = BitConverter.GetBytes(value);

            for (int i = 0; i < converted.Length; i++)
                toAddTo.Add(converted[i]);
        }

        public static void AddFloatsToList(List<byte> toAddTo, params float[] values)
        {
            for (int i = 0; i < values.Length; i++)
                AddFloatToList(values[i], toAddTo);
        }
    }
}
