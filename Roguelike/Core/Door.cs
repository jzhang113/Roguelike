using Roguelike.Actors;
using Roguelike.Commands;
using System;

namespace Roguelike.Core
{
    [Serializable]
    public class Door : Actor
    {
        public bool IsOpen { get; set; }

        public Door(bool open = false) : base(new ActorParameters("Door"), Colors.Door, '+')
        {
            IsOpen = open;
        }

        public override ICommand Act()
        {
            if (IsDead)
                TriggerDeath();

            return new WaitCommand(this);
        }

        public void Open()
        {
            DrawingComponent.Symbol = '-';
            IsOpen = true;

            Game.Map.Field[X, Y].IsOccupied = false;
            Game.Map.Field[X, Y].BlocksLight = false;

            // reflect that the door is open immediately
            Game.Map.UpdatePlayerFov();
        }
    }
}
