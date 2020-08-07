using Microsoft.Xna.Framework;

namespace MonoNet.PickUps
{
    class Pistol : Weapon
    {
        public override void CoreMethod()
        {
            LoadBullet.Shoot(Vector2.UnitX, 700, holder);
        }
    }
}
