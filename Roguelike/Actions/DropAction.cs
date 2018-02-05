using RLNET;
using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Systems;
using System;
using System.Threading;

namespace Roguelike.Actions
{
    class DropAction : IAction
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 0;

        private RLKey _key;

        public DropAction(Actor source, RLKey key)
        {
            Source = source;
            _key = key;
        }

        public RedirectMessage Validate()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            _key.ToString();
        }
    }
}
