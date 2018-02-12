using Roguelike.Actors;

namespace Roguelike.Items
{
    public abstract class Armor : Item
    {
        public override char Symbol { get; set; } = '[';

        public override void Equip(Actor actor)
        {
            System.Diagnostics.Debug.Assert(actor != null && Carrier == null);

            // TODO: same as weapons - make sure carrier is null
            Carrier = actor;
            Carrier.Armor = this;

            Game.MessageHandler.AddMessage("You put on the armor.", Systems.OptionHandler.MessageLevel.Normal);
        }

        public override void Unequip()
        {
            Carrier.Armor = null;
            Carrier = null;

            Game.MessageHandler.AddMessage("You take off the armor.", Systems.OptionHandler.MessageLevel.Normal);
        }
    }
}
