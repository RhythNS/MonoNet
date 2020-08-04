using Microsoft.Xna.Framework;
using MonoNet.Util;
using System;
using System.CodeDom;
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
        /// Tries to get the next value specified by the Type type and returns it via the out variable parsed.
        /// Returns wheter it managed to parse it or not.
        /// </summary>
        /// <param name="data">The package as a byte array.</param>
        /// <param name="pointer">The current location of the array.</param>
        /// <param name="type">The type of the object that is trying to be parsed.</param>
        /// <param name="parsed">The parsed value.</param>
        /// <returns>Wheter it successeded or not.</returns>
        public static bool TryGetNextValue(byte[] data, ref int pointer, Type type, out object parsed)
        {
            if (type == typeof(int))
            {
                parsed = GetNextInt(data, ref pointer);
            }
            else if (type == typeof(float))
            {
                parsed = GetNextFloat(data, ref pointer);
            }
            else if (type == typeof(short))
            {
                parsed = GetNextShort(data, ref pointer);
            }
            else if (type == typeof(Vector2))
            {
                parsed = GetNextVector(data, ref pointer);
            }
            else
            {
                Log.Error("Could not parse " + type);
                parsed = default;
                return false;
            }
            return true;
        }

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
            pointerAt += 2;
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
        /// Gets the next int value from a given package. Automatically increments the pointer
        /// to the next data index.
        /// </summary>
        /// <param name="data">The package as a byte array.</param>
        /// <param name="pointerAt">The current location of the array.</param>
        /// <returns>The converted int.</returns>
        public static int GetNextInt(byte[] data, ref int pointerAt)
        {
            int value = BitConverter.ToInt32(data, pointerAt);
            pointerAt += 4;
            return value;
        }

        /// <summary>
        /// Gets an array of int values from a given package. Automatically increments the pointer
        /// to the next data index.
        /// </summary>
        /// <param name="data">The package as a byte array.</param>
        /// <param name="pointerAt">The current location of the array.</param>
        /// <param name="amount">The amount of shorts to be read.</param>
        /// <returns>The converted int array.</returns>
        public static int[] GetNextInts(byte[] data, ref int pointerAt, int amount)
        {
            int[] values = new int[amount];

            for (int i = 0; i < amount; i++)
                values[i] = GetNextInt(data, ref pointerAt);

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
        /// Converts a value to a byte array and saves it to the toAddTo List.
        /// </summary>
        /// <param name="toAdd">The object to be added to the list.</param>
        /// <param name="toAddTo">The list to where the parsed object is put into.</param>
        /// <returns>Wheter it successeded or not.</returns>
        public static bool TryAddValueToList(object toAdd, List<byte> toAddTo)
        {
            if (toAdd is int parsedInt)
            {
                AddIntToList(parsedInt, toAddTo);
            }
            else if (toAdd is float parsedFloat)
            {
                AddFloatToList(parsedFloat, toAddTo);
            }
            else if (toAdd is short parsedShort)
            {
                AddShortToList(parsedShort, toAddTo);
            }
            else if (toAdd is Vector2 parsedVector)
            {
                AddVectorToList(parsedVector, toAddTo);
            }
            else
            {
                Log.Error("Could not parse " + toAdd.GetType());
                return false;
            }
            return true;
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
                AddShortToList(values[i], toAddTo);
        }

        /// <summary>
        /// Converts a int to a byte array and saves it to the list.
        /// </summary>
        /// <param name="value">The value to be converted into a byte array.</param>
        /// <param name="toAddTo">The list to where the value is stored to.</param>
        public static void AddIntToList(int value, List<byte> toAddTo)
        {
            byte[] converted = BitConverter.GetBytes(value);

            for (int i = 0; i < converted.Length; i++)
                toAddTo.Add(converted[i]);
        }

        /// <summary>
        /// Converts any amount of ints to a byte array and saves it to the list.
        /// </summary>
        /// <param name="toAddTo">The list to where the value is stored to.</param>
        /// <param name="values">The values to be converted into a byte array.</param>
        public static void AddIntsToList(List<byte> toAddTo, params int[] values)
        {
            for (int i = 0; i < values.Length; i++)
                AddIntToList(values[i], toAddTo);
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

    public static class Extensions
    {
        /// <summary>
        /// Returns just specified part of a byte array.
        /// </summary>
        /// <param name="data">The original byte array.</param>
        /// <param name="index">The index to start at.</param>
        /// <param name="length">How many elements should be copied into the sub array.</param>
        /// <returns>The specified part of the array.</returns>
        public static byte[] SubArray(this byte[] data, int index, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Expands (or shortens) a string to the specified length.
        /// </summary>
        /// <param name="original">The string to expand / shorten.</param>
        /// <param name="length">The length the new string should be.</param>
        /// <returns>A new string at the exact length specified.</returns>
        public static string ExpandTo(this string original, int length)
        {
            if (length <= original.Length) return original.Substring(0, length);

            while (original.Length <= length)
            {
                original += " ";
            }

            return original;
        }
    }
}
