using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.PickUps
{
    public class Knife : Weapon
    {
        public static Knife Instance { get; private set; }

        public Knife()
        {
            Instance = this;
        }

        /// <summary>
        /// stabs player infront of you
        /// </summary>
        public override void CoreMethod()
        {
            throw new NotImplementedException();
            //Stab infront of Player
        }
    }
}
