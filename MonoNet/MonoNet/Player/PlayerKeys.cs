using Microsoft.Xna.Framework.Input;

namespace MonoNet.Player
{
    public class PlayerKeys
    {
        public Keys jump;
        public Keys left;
        public Keys right;
        public Keys lookRight;
        public Keys lookUp;
        public Keys lookLeft;
        public Keys weaponFire;
        public Keys weaponDrop;
        public Keys pickup;

        public PlayerKeys()
        {
            jump = Keys.Space;
            left = Keys.A;
            right = Keys.D;
            lookRight = Keys.Right;
            lookUp = Keys.Up;
            lookLeft = Keys.Left;
            weaponFire = Keys.F;
            weaponDrop = Keys.Q;
            pickup = Keys.E;
        }

        public PlayerKeys(Keys jump, Keys left, Keys right, Keys lookRight, Keys lookUp, Keys lookLeft, Keys weaponFire, Keys weaponDrop, Keys pickup)
        {
            Set(jump, left, right, lookRight, lookUp, lookLeft, weaponFire, weaponDrop, pickup);
        }

        public void Set(Keys jump, Keys left, Keys right, Keys lookRight, Keys lookUp, Keys lookLeft, Keys weaponFire, Keys weaponDrop, Keys pickup)
        {
            this.jump = jump;
            this.left = left;
            this.right = right;
            this.lookRight = lookRight;
            this.lookUp = lookUp;
            this.lookLeft = lookLeft;
            this.weaponFire = weaponFire;
            this.weaponDrop = weaponDrop;
            this.pickup = pickup;
        }

    }
}
