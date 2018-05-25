namespace Roguelike.State
{
    interface IState
    {
        // Perform any setup needed
        void Initialize();

        // Update the state. Returns true if the state is updated successfully
        bool Update();

        // Draw to the screen
        void Draw();
    }
}
