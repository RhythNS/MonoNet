using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.PickUps;
using MonoNet.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.GameSystems.PickUps
{
    public class Equip : Component, IDisposable, Interfaces.IUpdateable
    {
        public Actor equipHolder;

        public Weapon standingOnWeapon;
        public Weapon ActiveWeapon { get; private set; }

        private List<PickUp> powerUps;

        protected override void OnInitialize()
        {
            equipHolder = Actor;
            powerUps = new List<PickUp>();
        }

        public void Update()
        {
            
        }

        /// <summary>
        /// Gets Called when weapon gets picked up
        /// </summary>
        /// <param name="newWeapon">picked up weapon</param>
        public void PickupWeapon(Weapon newWeapon)
        {
            if (ActiveWeapon != newWeapon)
            {
                DropWeapon();
                newWeapon.OnEquip(equipHolder);
                ActiveWeapon = newWeapon;
            }
        }

        /// <summary>
        /// Gets Called when weapon gets droped
        /// </summary>
        public void DropWeapon()
        {
            if (ActiveWeapon != null)
            {
                ActiveWeapon.OnDeEquip();
                ActiveWeapon = null;
            }
        }

        /// <summary>
        /// Gets Called if power up is picked up
        /// </summary>
        /// <param name="newPowerUp">Power Up which is picked up </param>
        public void RegisterPowerUP(PickUp newPowerUp)
        {
            if (!powerUps.Contains(newPowerUp))
            {
                newPowerUp.OnEquip(equipHolder);
                powerUps.Add(newPowerUp);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
