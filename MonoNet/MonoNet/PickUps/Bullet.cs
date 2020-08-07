using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.GameSystems.PickUps;
using MonoNet.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.PickUps
{
    public class Bullet : Component, Interfaces.IUpdateable
    {
        protected override void OnInitialize()
        {
            Actor.GetComponent<Rigidbody>().OnTriggerEnter += OnTriggerEnter;
        }

        public void Update()
        {
           if (OfScreen())
           {
                Actor.Stage.DeleteActor(Actor);
           }
        }

        public void OnTriggerEnter(Rigidbody other)
        {
            if (other.Actor.GetComponent<PlayerManager>() != null)
            {
                other.Actor.GetComponent<PlayerManager>().TakeDamage();
            }
            Actor.Stage.DeleteActor(Actor);
        }

        private bool OfScreen()
        {
            bool ofScreen = false;
            float dist;

            dist = Actor.GetComponent<Transform2>().WorldPosition.X * Actor.GetComponent<Transform2>().WorldPosition.X
                   + Actor.GetComponent<Transform2>().WorldPosition.Y * Actor.GetComponent<Transform2>().WorldPosition.Y;

            if (dist > 50000)
            {
                ofScreen = true;
            }
            return ofScreen;
        }
    }
}
