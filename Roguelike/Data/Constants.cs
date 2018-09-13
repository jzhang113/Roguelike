﻿namespace Roguelike.Data
{
    static class Constants
    {
        public const int DEFAULT_REFRESH_RATE = 120;
        public const int MIN_TURN_ENERGY = 0;

        public const int FULL_TURN = DEFAULT_REFRESH_RATE;
        public const int HALF_TURN = FULL_TURN / 2;
        public const int DOUBLE_TURN = FULL_TURN * 2;

        public const string SAVE_FILE = "save.dat";

        public const int DEFAULT_MELEE_RANGE = 1;
        public const int DEFAULT_THROW_RANGE = 1;
        public const int DEFAULT_DAMAGE = 100;

        public const int FIRE_DAMAGE = 10;
        public const double LOW_BURN_PERCENT = 0.2;
        public const double MEDIUM_BURN_PERCENT = 0.4;
        public const double HIGH_BURN_PERCENT = 0.8;

        public const float MIN_VISIBLE_LIGHT_LEVEL = 0.25f;
        public const double LIGHT_DECAY = 0.1;
    }
}
