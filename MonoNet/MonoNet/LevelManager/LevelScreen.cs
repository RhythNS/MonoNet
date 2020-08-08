using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Screen;
using MonoNet.Tiled;
using MonoNet.Util.Datatypes;
using MonoNet.Util.Pools;

namespace MonoNet.LevelManager
{
    public abstract class LevelScreen : IScreen
    {
        public static LevelScreen Instance { get; private set; }
        public LevelUI UI { get; protected set; }

        protected MonoNet monoNet;

        protected Stage stage;
        protected Camera camera;
        protected GameManager gameManager;

        protected TiledBase tiledBase;
        protected HitboxLoader hitboxLoader;

        public LevelScreen(MonoNet monoNet)
        {
            Instance = this;
            this.monoNet = monoNet;
        }

        public virtual void Initialize()
        {
            ComponentFactory factory = new ComponentFactory(monoNet.Content);

            stage = new Stage(10, new Pool<Actor>(50, 10)); // TODO: LAYERS

            Actor serviceActor = stage.CreateActor(0);

            gameManager = serviceActor.AddComponent<GameManager>();

            Physic physic = Physic.Instance;
            physic.collisionRules.Add(new MultiKey<int>(GameManager.physicsPlayerLayer, GameManager.physicsWeaponLayer), false);
            physic.collisionRules.Add(new MultiKey<int>(GameManager.physicsBulletLayer, GameManager.physicsBulletLayer), false);

            tiledBase = serviceActor.AddComponent<TiledBase>();
            tiledBase.Set(monoNet.Content);

            hitboxLoader = new HitboxLoader(stage);
            tiledBase.OnCollisionHitboxLoaded += hitboxLoader.OnCollisionHitboxLoaded;

            camera = new Camera(monoNet.GraphicsDevice.Viewport);
            UI = new LevelUI();
        }

        public virtual void LoadContent()
        {

        }

        public virtual void Update(GameTime gameTime)
        {
            stage.Update();
        }

        public virtual void Draw(SpriteBatch batch)
        {
            monoNet.GraphicsDevice.Clear(Color.CornflowerBlue);

            batch.Begin(transformMatrix: camera.GetViewMatrix());
            stage.Draw(batch);
            UI.Draw(batch);
            batch.End();
        }

        public virtual void LoadLevel(byte levelNumber)
        {
            stage.DeleteAllActors(true);

            TiledMapComponent[] components = tiledBase.AddMap(stage, GameManager.LevelIDForLocation[levelNumber], true, true);
        }

        public virtual void UnloadContent()
        {
            ComponentFactory.Instance.UnloadAll();
        }
    }
}
