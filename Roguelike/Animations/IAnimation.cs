namespace Roguelike.Animations
{
    interface IAnimation
    {
        bool Done { get; }

        void Update();
        void Draw();
    }
}
