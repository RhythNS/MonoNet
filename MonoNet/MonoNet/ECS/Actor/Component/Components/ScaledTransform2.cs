using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoNet.ECS.Components
{
    public class ScaledTransform2 : Transform2
    {
        public override Vector2 WorldPosition
        {
            get => base.WorldPosition * WorldScale;
            set => base.WorldPosition = value / WorldScale;
        }
    }
}
