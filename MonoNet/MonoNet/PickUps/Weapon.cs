using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.PickUps
{
    public class Weapon
    {
        public Actor weaponHolder;
        public Keys activeKey;
        public Keys dropKey;
        public bool isEquiped;

        public Weapon()
        {
            isEquiped = false;
            dropKey = Keys.R;
        }

        /// <summary>
        /// Activates the main function of the weapon
        /// </summary>
        public virtual void CoreMethod() { }

        /// <summary>
        /// Gets called when the weapon is equiped
        /// </summary>
        /// <param name="holder">Actor of the one who picks up the weapon </param>
        public void OnEquip(Actor holder) 
        {
            isEquiped = true;
            weaponHolder = holder;
            // change graphic to player holding the weapon
        }

        /// <summary>
        /// Gets called when the weapon gets droped
        /// </summary>
        public void OnDeEquip() 
        {
            isEquiped = false;
            weaponHolder = null;
            //change graphic to laying on the stage
        }
    }
}
