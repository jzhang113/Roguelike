namespace Roguelike.Items
{
    public abstract class Armor : Item
    {
        public override char Symbol { get; set; } = '[';

        public override void Equip()
        {
            Game.MessageHandler.AddMessage("You put on the armor.");
        }

        public override void Unequip()
        {
            Game.MessageHandler.AddMessage("You take off the armor.");
        }
    }
}
