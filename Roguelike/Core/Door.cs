using Roguelike.Actors;
using Roguelike.Commands;

namespace Roguelike.Core
{
    public class Door : Actor
    {
        public bool IsOpen { get; set; }

        public Door() : base(new ActorParameters("Door"), Colors.Door, '+')
        {
            IsOpen = false;
        }

        public Door(bool open) : base(new ActorParameters("Door"), Colors.Door, '+')
        {
            IsOpen = open;
            if (open)
                DrawingComponent.Symbol = '-';
        }

        public override ICommand GetAction()
        {
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
