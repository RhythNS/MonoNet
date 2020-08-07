using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Player;

namespace MonoNet.PickUps
{
    public class Weapon : Pickable, Interfaces.IUpdateable
    {
        private Transform2 weaponTrans;

        private Transform2 holderTrans;
        protected PlayerManager holder;

        public bool isEquiped;
        private Rigidbody body;
        private DrawTextureRegionComponent drawComponent;

        protected override void OnInitialize()
        {
            isEquiped = false;
            weaponTrans = Actor.GetComponent<Transform2>();
            body = Actor.GetComponent<Rigidbody>();
            drawComponent = Actor.GetComponent<DrawTextureRegionComponent>();
        }

        public void Update()
        {
            if (isEquiped == true)
            {
                if (holder.lookingAt == PlayerManager.LookingAt.Left)
                {
                    drawComponent.mirror = true;
                    weaponTrans.LocalPosition = new Vector2(-drawComponent.region.sourceRectangle.Width + holder.DrawComponent.region.sourceRectangle.Width * 0.3f, 0);
                }
                else
                {
                    drawComponent.mirror = false;
                    weaponTrans.LocalPosition = new Vector2(holder.DrawComponent.region.sourceRectangle.Width * 0.7f, 0);
                }

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

            this.holder = holder.GetComponent<PlayerManager>();
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
            holder = null;

            Vector2 curWeaponPos = weaponTrans.WorldPosition;
            weaponTrans.Parent = null;
            weaponTrans.WorldPosition = curWeaponPos;

            Physic.Instance.Register(body);
            body.velocity = new Vector2();

            //change graphic to laying on the stage
        }

    }
}
