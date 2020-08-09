using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Network;
using MonoNet.PickUps;
using MonoNet.Player;
using MonoNet.Util.Datatypes;

namespace MonoNet.LevelManager
{
    public class ComponentFactory
    {
        public static ComponentFactory Instance { get; private set; }

        private ContentManager content;

        public TextureRegion playerTex;
        public TextureRegion bulletTex;
        public Fast2DArray<TextureRegion> gunRegions;

        public ComponentFactory(ContentManager content)
        {
            Instance = this;
            this.content = content;
            LoadAll();
        }

        private void LoadAll()
        {
            TextureRegion[] loadedRegions = TextureRegion.CreateAllFromSheet(content.Load<Texture2D>("Test/guns"), 32, 15);
            gunRegions = new Fast2DArray<TextureRegion>(3, 6);
            for (int i = 0; i < loadedRegions.Length; i++)
                gunRegions.Set(loadedRegions[i], i % 3, i / 6);

            playerTex = new TextureRegion(content.Load<Texture2D>("Test/testingLayers"), 0, 0, 20, 20);
            bulletTex = new TextureRegion(content.Load<Texture2D>("Test/testingLayers"), 0, 0, 10, 10);
        }

        public void UnloadAll()
        {
            content.Unload();
        }

        public static PlayerManager CreateNetPlayer(byte netId, string name)
        {
            Actor actor = NetManager.Instance.GetNetSyncComponent(netId).Actor;
            TextureRegion textureRegion = Instance.playerTex;

            actor.AddComponent<Transform2>();
            Rigidbody body = actor.AddComponent<Rigidbody>();

            DrawTextureRegionComponent drawTexture = actor.AddComponent<DrawTextureRegionComponent>();
            drawTexture.region = textureRegion;

            PlayerManager player = actor.AddComponent<PlayerManager>();
            player.name = name;
            actor.RemoveComponent<PlayerInput>();

            body.Set(width: textureRegion.sourceRectangle.Width, height: textureRegion.sourceRectangle.Height, collisionLayer: GameManager.physicsPlayerLayer, isStatic: false, isSquare: true, isTrigger: false);

            return player;
        }

        public static Actor CreatePreparedBullet(byte netId)
        {
            Actor bullet = NetManager.Instance.GetNetSyncComponent(netId).Actor;
            bullet.AddComponent<Transform2>();

            Rigidbody body = bullet.AddComponent<Rigidbody>();

            TextureRegion tex = Instance.bulletTex;
            DrawTextureRegionComponent drawTexture = bullet.AddComponent<DrawTextureRegionComponent>();
            drawTexture.region = tex;

            body.Set(width: tex.sourceRectangle.Width, height: tex.sourceRectangle.Height, collisionLayer: GameManager.physicsBulletLayer, isStatic: false, isSquare: true, isTrigger: false, ignoreGravity: true);

            return bullet;
        }

        public static Weapon CreateWeapon(byte netId, byte weaponId)
        {
            Actor weaponActor = NetManager.Instance.GetNetSyncComponent(netId).Actor;

            Transform2 weaponTrans = weaponActor.AddComponent<Transform2>();
            Rigidbody body = weaponActor.AddComponent<Rigidbody>();
            DrawTextureRegionComponent drawComponent = weaponActor.AddComponent<DrawTextureRegionComponent>();
            Weapon weapon = (Weapon)weaponActor.AddComponent(GameManager.GunIDForType[weaponId]);

            TextureRegion tex = Instance.gunRegions.Get(weapon.XIndexInTilesheet, weapon.YIndexInTilesheet);
            drawComponent.region = tex;

            body.Set(width: tex.sourceRectangle.Width, height: tex.sourceRectangle.Height, collisionLayer: 2, isStatic: false, isSquare: true, isTrigger: false);

            return weapon;
        }
    }
}
