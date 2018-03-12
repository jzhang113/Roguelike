using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Systems;
using System;

namespace Roguelike.Commands
{
    class UnequipCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 120;

        public UnequipCommand(Actor source, char key)
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
