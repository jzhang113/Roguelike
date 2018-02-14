using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.Systems;

namespace Roguelike.Actions
{
    class DropAction : IAction
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 0;

        private char _key;

        public DropAction(Actor source, char key)
        {
            Source = source;
            _key = key;
        }

        public RedirectMessage Validate()
        {
            if (!Source.Inventory.HasKey(_key))
            {
                Game.MessageHandler.AddMessage("No such item to drop.", OptionHandler.MessageLevel.Normal);
                return new RedirectMessage(false);
            }

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            Item item = Source.Inventory.GetItem(_key);
            Source.Inventory.Remove(item);
            item.Carrier = null;

            item.X = Source.X;
            item.Y = Source.Y;
            Game.Map.AddItem(item);
            Game.MessageHandler.AddMessage(string.Format("You drop a {0}.", item.Name), OptionHandler.MessageLevel.Normal);
        }
    }
}
