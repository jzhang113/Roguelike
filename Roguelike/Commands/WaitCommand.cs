﻿using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Systems;

namespace Roguelike.Commands
{
    class WaitCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; }
        public IAnimation Animation { get; } = null;

        public WaitCommand(Actor source)
        {
            Source = source;
            EnergyCost = source.RefreshRate;
        }

        public WaitCommand(int waitTime)
        {
            EnergyCost = waitTime;
        }

        public RedirectMessage Validate()
        {
            return new RedirectMessage(true);
        }

        public void Execute()
        {
            Game.Map.Refresh();
        }
    }
}
