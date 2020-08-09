using MonoNet.ECS;
using MonoNet.GameSystems;
using MonoNet.Interfaces;
using MonoNet.Player;
using System;

namespace MonoNet.LevelManager
{
    class HostGameEndComponent : Component, IUpdateable, IDisposable
    {
        protected override void OnInitialize()
        {
            GameManager.Instance.OnGameEnd += OnGameEnd;
        }

        private void OnGameEnd(PlayerManager winningPlayer)
        {
            if (winningPlayer != null)
                LevelUI.DisplayString("Player " + winningPlayer.name + " won!\nPress F6 to restart game!");
            else
                LevelUI.DisplayString("Nobody wins!\nPress F6 to restart game!");
        }

        public void Update()
        {
            if (Input.IsKeyDownThisFrame(Microsoft.Xna.Framework.Input.Keys.F6))
            {
                LevelUI.DisplayString("Nobody wins!\nPress F6 to restart game!");
                ServerConnectionComponent.Instance.ChangeLevel(GameManager.GetRandomLevelNumber());
            }
        }

        public void Dispose()
        {
            GameManager.Instance.OnGameEnd -= OnGameEnd;
        }
    }
}
