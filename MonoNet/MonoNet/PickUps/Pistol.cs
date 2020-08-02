using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.PickUps
{
    class Pistol : Weapon
    {
        private int damage;

        public Pistol()
        {
            damage = 1; //example
        }

        public override void CoreMethod()
        {
            //shoot
        }
    }
}
