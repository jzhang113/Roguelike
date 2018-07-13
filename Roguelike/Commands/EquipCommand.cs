using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Commands
{
    class EquipCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost => Utils.Constants.FULL_TURN;
        public IAnimation Animation => null;

        private readonly IEquippable _equipableItem;

        public EquipCommand(Actor source, IEquippable item)
        {
            Source = source;

            _equipableItem = item;
        }

        public RedirectMessage Validate()
        {
            return new RedirectMessage(Source is IEquipped);
        }

        public void Execute()
        {
            _equipableItem.Equip(Source as IEquipped);
        }
    }
}
