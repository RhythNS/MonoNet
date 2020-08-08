using MonoNet.ECS;
using MonoNet.Interfaces;
using MonoNet.Player;
using System;
using System.Collections.Generic;

namespace MonoNet.LevelManager
{
    public delegate GameEnd GameEnd(PlayerManager winningPlayer);

    public class GameManager : Component, IUpdateable
    {
        public static GameManager Instance { get; private set; }

        public List<PlayerManager> Players { get; private set; } = new List<PlayerManager>();

        public static Random Random { get; private set; } = new Random();

        public static int playerLayer = 2;
        public static int bulletLayer = 2;

        public static int physicsPlayerLayer = 0;
        public static int physicsBulletLayer = 1;
        public static int physicsWeaponLayer = 2;

        public static Dictionary<byte, string> LevelIDForLocation = new Dictionary<byte, string>
            {
                { 0, "maps/level1" }
            };

        public event GameEnd OnGameEnd;
        public bool RoundStarted = false;

        protected override void OnInitialize()
        {
            Instance = this;
        }

        public static void RegisterPlayer(PlayerManager player) => Instance.Players.Add(player);

        public static void DeRegisterPlayer(PlayerManager player) => Instance.Players.Remove(player);

        public void Update()
        {
            if (Players.Count < 2 || RoundStarted == false)
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
        }
    }
}
