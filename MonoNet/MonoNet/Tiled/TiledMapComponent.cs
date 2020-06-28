using Microsoft.Xna.Framework;
using MonoNet.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace MonoNet.Tiled
{
    public class TiledMapComponent : Component
    {
        public TmxMap Map { get; private set; }
        private Vector2 offset;

        public void Set(TmxMap map)
        {
            Map = map;
        }

    }
}
