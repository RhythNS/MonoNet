using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Player;

namespace MonoNet.PickUps
{
    public class Weapon : Pickable, Interfaces.IUpdateable
    {
        public Actor weaponHolder;
        public Vector2 looking;
        private Transform2 weaponTrans;
        private Transform2 holderTrans;
        public bool isEquiped;
        private Rigidbody body;

        protected override void OnInitialize()
        {
            isEquiped = false;
            weaponTrans = Actor.GetComponent<Transform2>();
            body = Actor.GetComponent<Rigidbody>();
        }

        public void Update()
        {
            if (isEquiped == true)
            {
                looking = weaponHolder.GetComponent<PlayerManager>().LookingAt;
                weaponTrans.LocalPosition = looking * 15;
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
            weaponTrans.Parent = holderTrans;
            Physic.Instance.DeRegister(body);

            // change graphic to player holding the weapon
        }

        /// <summary>
        /// Gets called when the weapon gets droped
        /// </summary>
        public void OnDeEquip()
        {
            isEquiped = false;
            weaponHolder = null;

            Vector2 curWeaponPos = weaponTrans.WorldPosition;
            weaponTrans.Parent = null;
            weaponTrans.WorldPosition = curWeaponPos;

            Physic.Instance.Register(body);
            body.velocity = new Vector2();

            //change graphic to laying on the stage
        }

    }
}
