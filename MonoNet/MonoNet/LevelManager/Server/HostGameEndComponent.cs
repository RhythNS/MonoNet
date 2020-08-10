using MonoNet.ECS;
using MonoNet.GameSystems;
using MonoNet.Interfaces;
using MonoNet.Player;
using System;

namespace MonoNet.LevelManager
{
    /// <summary>
    /// Component for restarting a round when a round is finished.
    /// </summary>
    class HostGameEndComponent : Component, IUpdateable, IDisposable
    {
        protected override void OnInitialize()
        {
            GameManager.Instance.OnGameEnd += OnGameEnd;
        }

        private bool canRestart = false;

        private void OnGameEnd(PlayerManager winningPlayer)
        {
            canRestart = true;
            if (winningPlayer != null)
                LevelUI.DisplayString("Player " + winningPlayer.name + " won!\nPress F6 to restart game!");
            else
                LevelUI.DisplayString("Nobody wins!\nPress F6 to restart game!");
        }

        public void Update()
        {
            if (canRestart == true && Input.IsKeyDownThisFrame(Microsoft.Xna.Framework.Input.Keys.F6))
            {
                canRestart = false;
                LevelUI.DisplayString();
                ServerConnectionComponent.Instance.ChangeLevel(GameManager.GetRandomLevelNumber());
            }
        }

        public void Dispose()
        {
            GameManager.Instance.OnGameEnd -= OnGameEnd;
        }
    }
}
