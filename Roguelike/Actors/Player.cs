using RLNET;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Actors
{
    class Player : Actor
    {
        private RLRootConsole _rootConsole;

        public Player(RLRootConsole console) : base()
        {
            _rootConsole = console;

            Awareness = 100;
            Name = "Player";
            Color = Colors.Player;
            Symbol = '@';

            HP = 100;
            SP = 50;
            MP = 50;
        }

        public override IAction Act() => InputHandler.HandleInput(_rootConsole);
        public override void TriggerDeath() => Game.GameOver();
    }
}
