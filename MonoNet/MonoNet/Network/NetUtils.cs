using Microsoft.Xna.Framework;
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

        public static Vector2 GetNextVector(byte[] data, ref int pointerAt)
            => new Vector2(GetNextFloat(data, ref pointerAt), GetNextFloat(data, ref pointerAt));

        public static Vector2[] GetNextVectors(byte[] data, ref int pointerAt, int amount)
        {
            float[] values = GetNextFloats(data, ref pointerAt, amount * 2);
            Vector2[] vectors = new Vector2[amount];

            for (int i = 0; i < amount; i++)
                vectors[i] = new Vector2(values[i * 2], values[(i * 2) + 1]);
            
            return vectors;
        }

        public static void AddFloatToList(float value, List<byte> toAddTo)
        {
            byte[] converted = BitConverter.GetBytes(value);

            if (converted.Length != 4)
                throw new Exception();

            for (int i = 0; i < converted.Length; i++)
                toAddTo.Add(converted[i]);
        }

        public static void AddFloatsToList(List<byte> toAddTo, params float[] values)
        {
            for (int i = 0; i < values.Length; i++)
                AddFloatToList(values[i], toAddTo);
        }

        public static void AddVectorToList(Vector2 vector, List<byte> toAddTo)
            => AddFloatsToList(toAddTo, vector.X, vector.Y);

        public static void AddVectorsToList(List<byte> toAddTo, params Vector2[] vectors)
        {
            for (int i = 0; i < vectors.Length; i++)
                AddFloatsToList(toAddTo, vectors[i].X, vectors[i].Y);
        }
    }
}
