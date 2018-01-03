using RLNET;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Core
{
    class Player : Actor
    {
        private RLRootConsole _rootConsole;

        public Player(RLRootConsole console)
        {
            _rootConsole = console;

            Awareness = 100;
            Name = "Player";
            Color = Colors.Player;
            Symbol = '@';

            HP = 100;
            SP = 50;
            MP = 50;
            BasicAttack = new DamageSkill();
        }

        public override ICommand Act()
        {
            return InputHandler.HandleInput(_rootConsole);
        }

        public override void TriggerDeath() => Game.GameOver();
    }
}
