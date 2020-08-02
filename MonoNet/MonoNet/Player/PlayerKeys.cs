using Microsoft.Xna.Framework.Input;

namespace MonoNet.Player
{
    public class PlayerKeys
    {
        public Keys jump;
        public Keys left;
        public Keys right;
        public Keys weaponFire;
        public Keys weaponDrop;
        public Keys pickup;

        public PlayerKeys()
        {
            jump = Keys.Space;
            left = Keys.A;
            right = Keys.D;
            weaponFire = Keys.LeftControl;
            weaponDrop = Keys.Q;
            pickup = Keys.E;
        }

        public PlayerKeys(Keys jump, Keys left, Keys right, Keys weaponFire, Keys weaponDrop, Keys pickup)
        {
            Set(jump, left, right, weaponFire, weaponDrop, pickup);
        }

        public void Set(Keys jump, Keys left, Keys right, Keys weaponFire, Keys weaponDrop, Keys pickup)
        {
            this.jump = jump;
            this.left = left;
            this.right = right;
            this.weaponFire = weaponFire;
            this.weaponDrop = weaponDrop;
            this.pickup = pickup;
        }

    }
}
