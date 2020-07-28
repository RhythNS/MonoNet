using MonoNet.ECS;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.GameSystems.PickUps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.Player
{
    public delegate void OnDeath();

    public class PlayerManager : Component
    {
        public event OnDeath OnDeath;

        public PlayerInput PlayerInput { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public Equip Equip { get; private set; }

        public float XSpeed { get; private set; } = 150;
        public float XMaxSpeed { get; private set; } = 350;

        public float JumpForce { get; private set; } = 250;
        public bool Dead { get; private set; }

        private PlayerKeys binding;

        protected override void OnInitialize()
        {
            Equip = Actor.AddComponent<Equip>();
            PlayerInput = Actor.AddComponent<PlayerInput>();
            Rigidbody = Actor.GetComponent<Rigidbody>();
        }



    }
}
