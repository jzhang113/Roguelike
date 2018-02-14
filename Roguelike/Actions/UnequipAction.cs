using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike.Actions
{
    class UnequipAction : IAction
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 120;

        public UnequipAction(Actor source, char key)
        {
            Source = source;
        }

        public RedirectMessage Validate()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
