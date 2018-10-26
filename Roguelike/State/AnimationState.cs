using Roguelike.Animations;
using Roguelike.Commands;
using Roguelike.Core;

namespace Roguelike.State
{
    internal class AnimationState : IState
    {
        private readonly IAnimation _animation;

        public AnimationState(IAnimation animation)
        {
            _animation = animation;
        }

        public ICommand HandleKeyInput(int key)
        {
            throw new System.NotImplementedException();
        }

        public ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            throw new System.NotImplementedException();
        }

        public void Update()
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
