using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.Network.Commands
{
    public class PlayerJoined : Command
    {
        public override Type CommandType => Type.PlayerJoined;

        public override void Execute(GameVariables variables)
        {
            throw new NotImplementedException();
        }

        public override void GetSync(List<byte> data)
        {
            throw new NotImplementedException();
        }
    }
}
