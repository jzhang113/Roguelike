using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Actors
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
        }

        public override ICommand Act()
        {
            if (Game.GameMode == Game.Mode.Targetting)
                return InputHandler.HandleInput();

            if (ActiveSequence)
            {
                if (ActionSequence.HasAction())
                {
                    return new ActionCommand(this, ActionSequence.GetAction());
                }
                else
                {
                    ActiveSequence = false;
                }
            }

            return InputHandler.HandleInput();
        }

        public override void TriggerDeath() => Game.GameOver();
    }
}
