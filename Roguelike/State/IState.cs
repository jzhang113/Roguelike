using Roguelike.Commands;
using Roguelike.Core;

namespace Roguelike.State
{
    public interface IState
    {
        bool Nonblocking { get; }

        // Handle keyboard inputs.
        ICommand HandleKeyInput(int key);

        // Handle mouse inputs.
        ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick);

        // Update state information.
        void Update(ICommand command);

        // Draw to the screen.
        void Draw(LayerInfo layer);
    }
}
