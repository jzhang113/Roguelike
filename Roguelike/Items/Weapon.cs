using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Skills;
using Roguelike.Systems;

namespace Roguelike.Items
{
    public abstract class Weapon : Item
    {
        public override char Symbol { get; set; } = '(';

        public override void Equip(Actor actor)
        {
            System.Diagnostics.Debug.Assert(actor != null && Carrier == null);

            // TODO: ensure carrier is actually null
            Carrier = actor;
            Carrier.Weapon = this;

            Game.MessageHandler.AddMessage(string.Format("You wield a {0}.", Name), OptionHandler.MessageLevel.Normal);
        }
        
        public override void Unequip()
        {
            Carrier.Weapon = null;
            Carrier = null;

            Game.MessageHandler.AddMessage(string.Format("You unwield a {0}.", Name), OptionHandler.MessageLevel.Normal);
        }
    }
}
