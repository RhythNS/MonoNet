using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
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
    public class Equip : Component, Interfaces.IUpdateable, IDisposable
    {
        public Actor equipHolder;

        private KeyboardState input;

        private Weapon weapon;
        private Weapon knife;

        private List<PickUp> powerUps;

        public Equip()
        {
            weapon = new Weapon();
            knife = Knife.Instance;

            powerUps = new List<PickUp>();
        }

        /// <summary>
        /// Gets Called when weapon gets picked up
        /// </summary>
        /// <param name="newWeapon">picked up weapon</param>
        public void RegisterWeapon(Weapon newWeapon)
        {
            if (weapon != newWeapon)
            {
                DeRegisterWeapon(weapon);
                newWeapon.OnEquip(equipHolder);
                weapon = newWeapon;
            }
        }

        /// <summary>
        /// Gets Called when weapon gets droped
        /// </summary>
        /// <param name="oldWeapon">weapon which gets droped </param>
        public void DeRegisterWeapon(Weapon oldWeapon)
        {
            if (weapon != knife)
            {
                oldWeapon.OnDeEquip();
                weapon = knife;
                oldWeapon.OnDeEquip();
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

        /// <summary>
        /// checks for inputs every frame and calls core methods of the equipment when the right input is given
        /// </summary>
        public void Update()
        {
            if (input.IsKeyDown(weapon.activeKey))
            {
                weapon.CoreMethod();
            }
            else if (input.IsKeyDown(weapon.dropKey))
            {
                DeRegisterWeapon(weapon);
            }

            for (int i = 0; i < powerUps.Count; i++)
            {
                if (input.IsKeyDown(powerUps[i].activeKey))
                {
                    powerUps[i].CoreMethod();
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
