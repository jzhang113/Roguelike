using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Systems;
using System;

namespace Roguelike.Actions
{
    class DropAction : IAction
    {
        public Actor Source { get; }

        public int EnergyCost { get; } = 0;

        public DropAction(Actor source)
        {
            Source = source;
        }

        public void Execute()
        {
            InputHandler.GetKeyPress();
        }
    }
}
