using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class AttackCommand : ICommand
    {
        private ISkill _attack;

        public AttackCommand(ISkill attack)
        {
            _attack = attack;
        }

        public IAction Resolve(Actor origin, Actor target)
        {
            if (target != null)
            {
                Game.MessageHandler.AddMessage(origin.Name + " attacked " + target.Name);
            }

            return new AttackAction(origin, target, _attack);
        }
    }
}
