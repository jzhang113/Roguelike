using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Commands
{
    class EquipCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = Utils.Constants.FULL_TURN;
        public IAnimation Animation { get; } = null;

        private readonly IEquipable _equipableItem;

        public EquipCommand(Actor source, IEquipable item)
        {
            Source = source;

            _equipableItem = item;
        }

        public RedirectMessage Validate()
        {
            return new RedirectMessage(true);
        }

        public void Execute()
        {
            _equipableItem.Equip(Source);
        }
    }
}
