namespace Roguelike.Utils
{
    static class Constants
    {
        public const int DEFAULT_REFRESH_RATE = 120;
        public const int MIN_TURN_ENERGY = 0;

        public const int FULL_TURN = DEFAULT_REFRESH_RATE;
        public const int HALF_TURN = FULL_TURN / 2;
        public const int DOUBLE_TURN = FULL_TURN * 2;

        public const string SAVE_FILE = "save.dat";

        public const double DEFAULT_MELEE_RANGE = 1.5;
        public const double DEFAULT_THROW_RANGE = 1.5;
        public const int DEFAULT_DAMAGE = 100;

        public const int FIRE_DAMAGE = 10;
    }
}
