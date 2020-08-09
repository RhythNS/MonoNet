using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.Interfaces;
using MonoNet.PickUps;
using MonoNet.Player;
using System;
using System.Collections.Generic;

namespace MonoNet.LevelManager
{
    public delegate void GameEnd(PlayerManager winningPlayer);

    public class GameManager : Component, Interfaces.IUpdateable
    {
        public static GameManager Instance { get; private set; }

        public List<PlayerManager> Players { get; private set; } = new List<PlayerManager>();

        public static Random Random { get; private set; } = new Random();

        public static int playerLayer = 5;
        public static int bulletLayer = 5;

        public static int physicsPlayerLayer = 3;
        public static int physicsBulletLayer = 1;
        public static int physicsWeaponLayer = 2;

        public static Vector2 screenDimensions;

        public static int currentLevel;

        public static Dictionary<byte, string> LevelIDForLocation = new Dictionary<byte, string>
        {
            { 0, "maps/level1" },
            { 1, "maps/level2" },
            { 2, "maps/level3" }
        };
        public static byte GetRandomLevelNumber() => (byte)Random.Next(0, LevelIDForLocation.Count);

        public static Dictionary<byte, Type> GunIDForType = new Dictionary<byte, Type>()
        {
            {0, typeof(DefaultRifle) }
        };

        public event GameEnd OnGameEnd;
        public bool RoundStarted
        {
            get => roundStarted;
            set
            {
                roundStarted = value;
                if (value == true)
                    SpawnStartWeapons();
            }
        }
        private bool roundStarted = false;

        protected override void OnInitialize()
        {
            Instance = this;
        }

        public static void RegisterPlayer(PlayerManager player) => Instance.Players.Add(player);

        public static void DeRegisterPlayer(PlayerManager player) => Instance.Players.Remove(player);

        public void SpawnStartWeapons()
        {
            List<Vector2> locs = GunSpawnLocations.Instance.GunLocations;
            for (int i = 0; i < locs.Count; i++)
            {
                ServerConnectionComponent.Instance.CreateWeapon((byte)Random.Next(0, GunIDForType.Count), locs[i]);
            }
        }

        public void Update()
        {
            if (RoundStarted == false)
                return;

            int playersAlive = 0;
            PlayerManager lastPlayerAlive = null;
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].Dead == false)
                {
                    ++playersAlive;
                    lastPlayerAlive = Players[i];
                }
            }

            if (playersAlive <= 1)
            {
                OnGameEnd?.Invoke(lastPlayerAlive);
                RoundStarted = false;
            }

            //TODO: Weapon timer spawner?
        }
    }
}
