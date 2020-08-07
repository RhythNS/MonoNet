using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.GameSystems.PickUps;
using MonoNet.Graphics;

namespace MonoNet.Player
{
    public delegate void OnDeath();

    public class PlayerManager : Component
    {
        public event OnDeath OnDeath;

        public PlayerInput PlayerInput { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public Equip Equip { get; private set; }
        public DrawTextureRegionComponent DrawComponent { get; private set; }

        public float XSpeed { get; private set; } = 150;
        public float XMaxSpeed { get; private set; } = 350;

        public enum LookingAt
        {
            Right, Left, Up
        }

        public float JumpForce { get; private set; } = 250;
        public bool Dead { get; private set; }

        public int Health { get; private set; } = 3;

        public LookingAt lookingAt;
        private PlayerKeys binding;

        protected override void OnInitialize()
        {
            Equip = Actor.AddComponent<Equip>();
            PlayerInput = Actor.AddComponent<PlayerInput>();
            Rigidbody = Actor.GetComponent<Rigidbody>();
            DrawComponent = Actor.GetComponent<DrawTextureRegionComponent>();
        }

        public void TakeDamage()
        {
            Health--;
            if (Health <= 0)
            {
                Dead = true;
                Actor.Stage.DeleteActor(Actor);
            }
        }

        public Vector2 GetLookingVector()
        {
            if (lookingAt == LookingAt.Right)
            {
                return Vector2.UnitX;
            }
            else if (lookingAt == LookingAt.Up)
            {
                return Vector2.UnitY * -1;
            }
            else
            {
                return Vector2.UnitX * -1;
            }
        }
    }
}
