using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Commands
{
    internal class UnequipCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost => Data.Constants.FULL_TURN;
        public IAnimation Animation => null;

        private readonly IEquippable _equipableItem;

        public UnequipCommand(Actor source, IEquippable item)
        {
            Source = source;
            _equipableItem = item;
        }

        public RedirectMessage Validate()
        {
            if (!(Source is IEquipped))
                return new RedirectMessage(false);

            // TODO: check item is equipped
            // if (Source.Equipment)

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            _equipableItem.Unequip(Source as IEquipped);
        }
    }
}
