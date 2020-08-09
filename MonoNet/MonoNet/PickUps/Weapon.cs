using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.LevelManager;
using MonoNet.Network;
using MonoNet.Player;

namespace MonoNet.PickUps
{
    public abstract class Weapon : Pickable, Interfaces.IUpdateable
    {
        public abstract int XIndexInTilesheet { get; }
        public abstract int YIndexInTilesheet { get; }
        public abstract float BulletVelocity { get; }
        public abstract float DelayAfterShoot { get; }
        public bool CanShoot => shootTimer <= 0;

        private float shootTimer;

        private Transform2 weaponTrans;
        private Vector2 size;

        private Transform2 holderTrans;
        public PlayerManager holder;

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
                if (shootTimer > 0)
                    shootTimer -= Time.Delta;

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

            if (NetManager.Instance.IsServer == false)
                return;

            Vector2 screenDims = GameManager.screenDimensions;
            Vector2 pos = weaponTrans.WorldPosition;

            if (pos.X + size.X < 0 || pos.X > screenDims.X || pos.Y + size.Y > screenDims.Y || pos.Y < 0)
                ServerConnectionComponent.Instance.DestroySyncable(Actor.GetComponent<NetSyncComponent>().Id);
        }
        /// <summary>
        /// Activates the main function of the weapon
        /// </summary>
        public void CoreMethod()
        {
            if (CanShoot == false)
                return;

            if (NetManager.Instance.IsServer == true)
                ServerConnectionComponent.Instance.CreateBullet(holder);
            else
                ClientConnectionComponent.Instance.RequestBullet(holder);
        }

        /// <summary>
        /// Gets called when the weapon is equiped
        /// </summary>
        /// <param name="holder">Actor of the one who picks up the weapon </param>
        public void OnEquip(Actor holder)
        {
            Rectangle rec = Actor.GetComponent<DrawTextureRegionComponent>().region.sourceRectangle;
            size = new Vector2(rec.Width, rec.Height);

            shootTimer = -1;
            isEquiped = true;

            this.holder = holder.GetComponent<PlayerManager>();
            holderTrans = holder.GetComponent<Transform2>();

            if (NetManager.Instance.IsServer == true)
            {
                ServerConnectionComponent.Instance.ParentTransform(weaponTrans, holderTrans);
                Physic.Instance.DeRegister(body);
            }
            else
                weaponTrans.Parent = holderTrans;


            // change graphic to player holding the weapon
        }

        /// <summary>
        /// Gets called when the weapon gets droped
        /// </summary>
        public void OnDeEquip()
        {
            isEquiped = false;
            holder = null;

            if (NetManager.Instance.IsServer == false)
                return;

            Vector2 curWeaponPos = weaponTrans.WorldPosition;
            ServerConnectionComponent.Instance.ParentTransform(weaponTrans, null);
            weaponTrans.WorldPosition = curWeaponPos;

            Physic.Instance.Register(body);
            body.velocity = new Vector2();

            //change graphic to laying on the stage
        }

    }
}
