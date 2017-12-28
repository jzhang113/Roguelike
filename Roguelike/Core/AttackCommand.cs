using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class AttackCommand : ICommand
    {
        private IAction attack;

        public AttackCommand(IAction attack)
        {
            this.attack = attack;
        }

        public void Execute(Actor origin, Actor target)
        {
            if (target != null)
            {
                Game.MessageHandler.AddMessage(origin.Name + " attacked " + target.Name);
            }

            attack.Execute(target);
        }
    }
}
