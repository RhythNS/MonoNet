using MonoNet.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.Network.Commands
{
    public abstract class Command
    {
        public enum Type : byte
        {
            PlayerJoined = 0,
            PlayerDisconnected = 1
        }

        public abstract Type CommandType { get; }
        
        public static Command Parse(byte[] data, ref int pointerAt)
        {
            Type type = (Type)data[pointerAt];
            pointerAt++;
            switch (type)
            {
                default:
                    break;
            }

            return null;
        }

        public abstract void Execute(GameVariables variables);

        public abstract void GetSync(List<byte> data);
    }
}
