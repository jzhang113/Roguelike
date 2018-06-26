using RLNET;
using Roguelike.Commands;

namespace Roguelike.State
{
    interface IState
    {
        // Perform any setup needed.
        // void Initialize();

        // Perform any cleanup needed.
        // void Cleanup();

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
