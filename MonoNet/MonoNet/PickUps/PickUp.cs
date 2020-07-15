using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.PickUps
{
    public class PickUp
    {
        public Actor powerUpHolder;
        public Keys activeKey;
        public bool isEquiped;

        public PickUp()
        {
            isEquiped = false;
        }

        /// <summary>
        /// Activates the main function of the Power Up
        /// </summary>
        public virtual void CoreMethod() { }

        /// <summary>
        /// Gets called when Power up is Equiped
        /// </summary>
        /// <param name="holder"></param>
        public virtual void OnEquip(Actor holder) { }
    }
}
