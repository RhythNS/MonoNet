﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.GameSystems;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.LevelManager;
using MonoNet.Network;
using MonoNet.Testing.World;
using MonoNet.Tiled;
using MonoNet.Util.Datatypes;
using System.Net;

namespace MonoNet.Testing.NetTest
{
    public class NetTestGame : TestGame
    {
        public static bool startDebug = false;

        NetManagerReciever reciever;
        NetManagerSender sender;
        PlayerSpawnLocations playerSpawns;

        protected override void AfterManagerPreStageUpdate(GameTime time)
        {
            if (reciever != null)
            {
                reciever.Recieve();
                reciever.Send();
            }
            else if (sender != null)
            {
                if (startDebug == true)
                    new Vector2();

                sender.UpdateCurrentState();
                sender.SendToAll();
            }
            else
            {
                if (Input.KeyDown(Keys.F1))
                {
                    sender = new NetManagerSender(25565);
                    stage.CreateActor(0).AddComponent<ServerConnectionComponent>().Set(playerSpawns, "Unknown", stage);

                }
                else if (Input.KeyDown(Keys.F2))
                {
                    reciever = new NetManagerReciever(new IPEndPoint(IPAddress.Parse("0:0:0:0:0:0:0:1"), 25565), "Unknown");
                    stage.CreateActor(0).AddComponent<ClientConnectionComponent>();

                }


            }

            if (Input.KeyDown(Keys.F5))
                startDebug = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();


            ComponentFactory factory = new ComponentFactory(Content);

            graphics.PreferredBackBufferWidth = 1920;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 1080;   // set this value to the desired height of your window
            graphics.ApplyChanges();

            TextureRegion orangeRegion = new TextureRegion(Content.Load<Texture2D>("Test/orangeSquare"), 0, 0, 32, 32);

            HitboxLoader hitboxLoader = new HitboxLoader(stage);
            BoxSpawn boxSpawn = new BoxSpawn(factory.playerWalk[0], stage);
            playerSpawns = new PlayerSpawnLocations();
            //GunSpawn gunSpawn = new GunSpawn(factory.gunRegions, stage);

            Physic.Instance.collisionRules.Add(new MultiKey<int>(1, 2), false);

            Actor baseActor = stage.CreateActor(0);
            TiledBase tiledBase = baseActor.AddComponent<TiledBase>();
            tiledBase.Set(Content);
            tiledBase.OnCollisionHitboxLoaded += hitboxLoader.OnCollisionHitboxLoaded;
            tiledBase.OnObjectLoaded += boxSpawn.OnObjectLoaded;
            tiledBase.OnObjectLoaded += playerSpawns.OnObjectLoaded;
            //tiledBase.OnObjectLoaded += gunSpawn.OnObjectLoaded;

            TiledMapComponent[] components = tiledBase.AddMap(stage, "maps/level1", true, true);

        }

    }
}
