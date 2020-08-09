namespace MonoNet.PickUps
{
    class DefaultRifle : Weapon
    {
        public override float BulletVelocity => 700;

        public override float DelayAfterShoot => 0.3f;

        public override int XIndexInTilesheet => 0;

        public override int YIndexInTilesheet => 3;
    }
}
