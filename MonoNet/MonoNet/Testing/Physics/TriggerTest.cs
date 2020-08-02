using MonoNet.ECS;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Util;

namespace MonoNet.Testing.Physics
{
    public class TriggerTest : Component
    {
        public string name;

        protected override void OnInitialize()
        {
            Rigidbody body = Actor.GetComponent<Rigidbody>();
            body.OnTriggerEnter += OnTriggerEnter;
            body.OnTriggerExit += OnTriggerExit;
            body.OnTriggerStay += OnTriggerStay;
        }

        public void OnTriggerEnter(Rigidbody other)
        {
            Log.Info("Enter on " + name);
        }

        public void OnTriggerExit(Rigidbody other)
        {
            Log.Info("Exit on " + name);
        }

        public void OnTriggerStay(Rigidbody other)
        {
            Log.Info("stay on " + name);
        }

    }
}
