using Roguelike.Animations;
using Roguelike.Commands;
using Roguelike.Core;

namespace Roguelike.State
{
    internal class AnimationState : IState
    {
        public bool Nonblocking => true;

        private readonly IAnimation _animation;

        public AnimationState(IAnimation animation)
        {
            _animation = animation;
        }

        public ICommand HandleKeyInput(int key)
        {
            return null;
        }

        public ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            return null;
        }

        public void Update(ICommand command)
        {
            if (!_animation.Done)
                _animation.Update();
            else
                Game.StateHandler.PopState();
        }

        public void Draw(LayerInfo layer)
        {
            _animation.Draw(layer);
        }
    }
}
