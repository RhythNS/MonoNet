using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoNet.Network
{
    /// <summary>
    /// Utility methods for basic repeating tasks to do with networking.
    /// </summary>
    public class NetUtils
    {
        /// <summary>
        /// Gets the next float value from a given package. Automatically increments the pointer
        /// to the next data index.
        /// </summary>
        /// <param name="data">The package as a byte array.</param>
        /// <param name="pointerAt">The current location of the array.</param>
        /// <returns>The converted float.</returns>
        public static float GetNextFloat(byte[] data, ref int pointerAt)
        {
            float value = BitConverter.ToSingle(data, pointerAt);
            pointerAt += 4;
            return value;
        }

        /// <summary>
        /// Gets an array of float values from a given package. Automatically increments the pointer
        /// to the next data index.
        /// </summary>
        /// <param name="data">The package as a byte array.</param>
        /// <param name="pointerAt">The current location of the array.</param>
        /// <param name="amount">The amount of floats to be read.</param>
        /// <returns>The converted float array.</returns>
        public static float[] GetNextFloats(byte[] data, ref int pointerAt, int amount)
        {
            float[] values = new float[amount];

            for (int i = 0; i < amount; i++)
                values[i] = GetNextFloat(data, ref pointerAt);

            return values;
        }

        /// <summary>
        /// Gets the next short value from a given package. Automatically increments the pointer
        /// to the next data index.
        /// </summary>
        /// <param name="data">The package as a byte array.</param>
        /// <param name="pointerAt">The current location of the array.</param>
        /// <returns>The converted short.</returns>
        public static short GetNextShort(byte[] data, ref int pointerAt)
        {
            short value = BitConverter.ToInt16(data, pointerAt);
            pointerAt += 4;
            return value;
        }

        /// <summary>
        /// Gets an array of short values from a given package. Automatically increments the pointer
        /// to the next data index.
        /// </summary>
        /// <param name="data">The package as a byte array.</param>
        /// <param name="pointerAt">The current location of the array.</param>
        /// <param name="amount">The amount of shorts to be read.</param>
        /// <returns>The converted short array.</returns>
        public static short[] GetNextShorts(byte[] data, ref int pointerAt, int amount)
        {
            short[] values = new short[amount];

            for (int i = 0; i < amount; i++)
                values[i] = GetNextShort(data, ref pointerAt);

            return values;
        }

        /// <summary>
        /// Gets the next vector2 value from a given package. Automatically increments the pointer
        /// to the next data index.
        /// </summary>
        /// <param name="data">The package as a byte array.</param>
        /// <param name="pointerAt">The current location of the array.</param>
        /// <returns>The converted Vector.</returns>
        public static Vector2 GetNextVector(byte[] data, ref int pointerAt)
            => new Vector2(GetNextFloat(data, ref pointerAt), GetNextFloat(data, ref pointerAt));

        /// <summary>
        /// Gets an array of vector2 values from a given package. Automatically increments the pointer
        /// to the next data index.
        /// </summary>
        /// <param name="data">The package as a byte array.</param>
        /// <param name="pointerAt">The current location of the array.</param>
        /// <param name="amount">The amount of vectors to be read.</param>
        /// <returns>The converted vector2 array.</returns>
        public static Vector2[] GetNextVectors(byte[] data, ref int pointerAt, int amount)
        {
            float[] values = GetNextFloats(data, ref pointerAt, amount * 2);
            Vector2[] vectors = new Vector2[amount];

            for (int i = 0; i < amount; i++)
                vectors[i] = new Vector2(values[i * 2], values[(i * 2) + 1]);

            return vectors;
        }

        /// <summary>
        /// Converts a float to a byte array and saves it to the list.
        /// </summary>
        /// <param name="value">The value to be converted into a byte array.</param>
        /// <param name="toAddTo">The list to where the value is stored to.</param>
        public static void AddFloatToList(float value, List<byte> toAddTo)
        {
            byte[] converted = BitConverter.GetBytes(value);

            if (converted.Length != 4)
                throw new Exception();

            for (int i = 0; i < converted.Length; i++)
                toAddTo.Add(converted[i]);
        }

        /// <summary>
        /// Converts any amount of floats to a byte array and saves it to the list.
        /// </summary>
        /// <param name="toAddTo">The list to where the value is stored to.</param>
        /// <param name="values">The values to be converted into a byte array.</param>
        public static void AddFloatsToList(List<byte> toAddTo, params float[] values)
        {
            for (int i = 0; i < values.Length; i++)
                AddFloatToList(values[i], toAddTo);
        }

        /// <summary>
        /// Converts a short to a byte array and saves it to the list.
        /// </summary>
        /// <param name="value">The value to be converted into a byte array.</param>
        /// <param name="toAddTo">The list to where the value is stored to.</param>
        public static void AddShortToList(short value, List<byte> toAddTo)
        {
            byte[] converted = BitConverter.GetBytes(value);

            for (int i = 0; i < converted.Length; i++)
                toAddTo.Add(converted[i]);
        }

        /// <summary>
        /// Converts any amount of shorts to a byte array and saves it to the list.
        /// </summary>
        /// <param name="toAddTo">The list to where the value is stored to.</param>
        /// <param name="values">The values to be converted into a byte array.</param>
        public static void AddShortsToList(List<byte> toAddTo, params short[] values)
        {
            for (int i = 0; i < values.Length; i++)
                AddFloatToList(values[i], toAddTo);
        }

        /// <summary>
        /// Converts a vecto2 to a byte array and saves it to the list.
        /// </summary>
        /// <param name="value">The value to be converted into a byte array.</param>
        /// <param name="toAddTo">The list to where the value is stored to.</param>
        public static void AddVectorToList(Vector2 vector, List<byte> toAddTo)
            => AddFloatsToList(toAddTo, vector.X, vector.Y);

        /// <summary>
        /// Converts any amount of vectors to a byte array and saves it to the list.
        /// </summary>
        /// <param name="toAddTo">The list to where the value is stored to.</param>
        /// <param name="values">The values to be converted into a byte array.</param>
        public static void AddVectorsToList(List<byte> toAddTo, params Vector2[] vectors)
        {
            for (int i = 0; i < vectors.Length; i++)
                AddFloatsToList(toAddTo, vectors[i].X, vectors[i].Y);
        }

        // TODO: Optimize me
        /// <summary>
        /// Gets the lowest avilable id from a given list of connected clients.
        /// </summary>
        /// <param name="clients">The current connected clients.</param>
        /// <returns>The lowest avilable id.</returns>
        public static byte GetLowestAvailableId(List<ConnectedClient> clients)
        {
            List<ConnectedClient> temp = new List<ConnectedClient>(clients);
            temp.OrderBy(x => x.id);
            byte id = 0;
            for (int i = 0; i < temp.Count - 1; i++)
            {
                if (id < temp[i].id && id < temp[i + 1].id)
                    return id;
                id = (byte)(temp[i].id + 1);
            }

            if (id < temp[temp.Count - 1].id)
                return id;

            return (byte)(temp[temp.Count - 1].id + 1);
        }
    }
}
