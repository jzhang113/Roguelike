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

        public IAction Resolve(IActor origin, IActor target)
        {
            return new AttackAction(origin, target, _attack);
        }
    }
}
