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
            attack.Execute(target);
        }

        public string Message(Actor origin)
        {
            return string.Format("{0} attacked {1} for {2} damage", origin.Name, origin.X, origin.Y);
        }
    }
}
