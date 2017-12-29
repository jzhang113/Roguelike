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

        public IAction Execute(Actor origin, Actor target)
        {
            if (target != null)
            {
                Game.MessageHandler.AddMessage(origin.Name + " attacked " + target.Name);
            }

            return new AttackAction(origin, target, 20, 10);
        }
    }
}
