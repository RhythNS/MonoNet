using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.GameSystems.PickUps;
using MonoNet.Graphics;
using MonoNet.Player;
using MonoNet.Testing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.PickUps
{
    public class Weapon : Component, Interfaces.IUpdateable
    {
        public Actor weaponHolder;
        public Vector2 looking;
        private Transform2 weaponTrans;
        private Transform2 holderTrans;
        public bool isEquiped;
        //public Rigidbody weaponRB;

        protected override void OnInitialize()
        {
            isEquiped = false;
            weaponTrans = Actor.GetComponent<Transform2>();
            Actor.GetComponent<Rigidbody>().OnTriggerEnter += OnTriggerEnter;
            Actor.GetComponent<Rigidbody>().OnTriggerExit += OnTriggerExit;
        }

        public void Update()
        {
            if (isEquiped == true)
            {
                looking = weaponHolder.GetComponent<PlayerManager>().LookingAt;
                weaponTrans.WorldPosition = holderTrans.WorldPosition + looking * 15;
            }
        }

        public void OnTriggerEnter(Rigidbody holderRB)
        {
            if (holderRB.Actor.GetComponent<Equip>() != null && holderRB.Actor.GetComponent<PlayerInput>() != null)
            {
                holderRB.Actor.GetComponent<Equip>().standingOnWeapon = this;
                holderRB.Actor.GetComponent<PlayerInput>().onWeapon = true;
            }
        }

        public void OnTriggerExit(Rigidbody holderRB)
        {
            if (holderRB.Actor.GetComponent<Equip>() != null && holderRB.Actor.GetComponent<PlayerInput>() != null)
            {
                holderRB.Actor.GetComponent<Equip>().standingOnWeapon = null;
                holderRB.Actor.GetComponent<PlayerInput>().onWeapon = false;
            }
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
            holderTrans = holder.GetComponent<Transform2>();
            // change graphic to player holding the weapon
        }

        /// <summary>
        /// Gets called when the weapon gets droped
        /// </summary>
        public void OnDeEquip()
        {
            weaponTrans.WorldPosition = weaponHolder.GetComponent<PlayerInput>().lastGround;
            isEquiped = false;
            weaponHolder = null;
            //change graphic to laying on the stage
        }

    }
}
