using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.GameSystems;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Screen;
using MonoNet.Testing.UI;
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

        private byte? loadLevelRequest = null;

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
            GameManager.screenDimensions = new Vector2(monoNet.GraphicsDevice.Viewport.Width, monoNet.GraphicsDevice.Viewport.Height);

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

        protected abstract void OnGameQuit();

        public virtual void LoadContent() { }

        public virtual void Update(GameTime gameTime)
        {
            if (Input.IsKeyDownThisFrame(Keys.Escape))
            {
                monoNet.ScreenManager.SetScreen(new MainMenu(monoNet));
                Physic.Instance.collisionRules.Clear();
                OnGameQuit();
                return;
            }

            if (loadLevelRequest != null)
                LoadLevel(loadLevelRequest.Value);

            stage.Update();
        }

        public virtual void Draw(SpriteBatch batch)
        {
            monoNet.GraphicsDevice.Clear(Color.CornflowerBlue);

            batch.Begin(transformMatrix: camera.GetViewMatrix());
            stage.Draw(batch);
            batch.End();

            batch.Begin();
            UI.Draw(batch);
            batch.End();
        }

        public void RequestLevelChange(byte levelNumber)
        {
            loadLevelRequest = levelNumber;

            stage.DeleteAllActors(true);
        }

        protected virtual void LoadLevel(byte levelNumber)
        {
            loadLevelRequest = null;

            TiledMapComponent[] components = tiledBase.AddMap(stage, GameManager.LevelIDForLocation[levelNumber], true, true);
        }

        public virtual void UnloadContent()
        {
            ComponentFactory.Instance.UnloadAll();
            stage.Dispose();
        }
    }
}
