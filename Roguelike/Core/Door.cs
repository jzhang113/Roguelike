using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Interfaces;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Core
{
    [Serializable]
    public class Door : Actor
    {
        public Door()
        {
            Name = "door";
            DrawingComponent.Color = Swatch.DbBrightWood;
            DrawingComponent.Symbol = '+';
        }

        protected Door(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override ICommand Act()
        {
            if (IsDead)
                TriggerDeath();

            return new WaitCommand(this);
        }
    }
}
