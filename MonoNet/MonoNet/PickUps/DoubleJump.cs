using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.GameSystems.PickUps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.PickUps
{
    class DoubleJump : PickUp
    {
        private Rigidbody holderBody;
        private bool canJump;
        private Vector2 jumpVel;

        public DoubleJump()
        {
            canJump = true;
            jumpVel = new Vector2 (0, -50);
        }

        public override void OnEquip(Actor holder)
        {
            holderBody = holder.GetComponent<Rigidbody>();
            isEquiped = true;
        }


        /// <summary>
        /// jumps to a naximum of two times in a row
        /// </summary>
        public override void CoreMethod()
        {
            if (canJump)
            {
                canJump = false;
                if (holderBody.isGrounded)
                {
                    canJump = true;
                }

                holderBody.velocity += jumpVel;
            }
        }

        //on triger enter ...
    }
}
