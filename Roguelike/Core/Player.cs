﻿namespace Roguelike.Core
{
    class Player : Actor
    {
        public Player()
        {
            Awareness = 100;
            Name = "Player";
            Color = Colors.Player;
            Symbol = '@';

            HP = 100;
            SP = 50;
            MP = 50;
            BasicAttack = new Attack(20);
        }

        public override void TriggerDeath()
        {
            Game.GameOver();
        }
    }
}
