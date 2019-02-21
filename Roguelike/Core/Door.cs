using Roguelike.Actors;
using Roguelike.Commands;
using System;

namespace Roguelike.Core
{
    [Serializable]
    public class Door : Actor
    {
        public bool IsOpen { get; set; }

        public Door(Loc pos) : base(new ActorParameters("Door"), Colors.Door, '+')
        {
            Loc = pos;
            IsOpen = false;
        }

        public override ICommand GetAction()
        {
            return new WaitCommand(this);
        }

        public void Open()
        {
            DrawingComponent.Symbol = '-';
            IsOpen = true;

            Tile tile = Game.Map.Field[Loc];
            tile.IsOccupied = false;
            tile.BlocksLight = false;

            // reflect that the door is open immediately
            Game.Map.UpdatePlayerFov();
        }
    }
}
