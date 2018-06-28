using RLNET;
using Roguelike.Commands;

namespace Roguelike.State
{
    interface IState
    {
        // Handle keyboard inputs.
        ICommand HandleKeyInput(RLKeyPress keyPress);

        // Handle mouse inputs.
        ICommand HandleMouseInput(RLMouse mouse);

        // Update state information.
        void Update();

        // Draw to the screen.
        void Draw();
    }
}
