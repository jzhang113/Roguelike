using Roguelike.Interfaces;
using System.Collections.Generic;

namespace Roguelike.Core
{
    class Skeleton : Actor
    {
        public Skeleton()
        {
            Awareness = 10;
            Name = "Skeleton";
            Color = Colors.TextHeading;
            Symbol = 'S';

            HP = 50;
            SP = 20;
            MP = 20;
            BasicAttack = new DamageSkill();
        }

        public override IEnumerable<IAction> Act()
        {
            IAction action = SimpleAI.GetAction(this, new System.Random());

            if (action != null)
                yield return action;

            yield break;
        }
    }
}
