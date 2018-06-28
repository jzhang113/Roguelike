using RLNET;
using Roguelike.Commands;

namespace Roguelike.State
{
    public delegate ICommand CommandEventHandler<TEventArgs>(object sender, TEventArgs e);

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
