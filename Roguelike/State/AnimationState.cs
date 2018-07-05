using RLNET;
using Roguelike.Animations;
using Roguelike.Commands;
using System;

namespace Roguelike.State
{
    class AnimationState : IState
    {
        private readonly IAnimation _animation;
        private readonly Func<ICommand> _callback;

        public AnimationState(IAnimation animation, Func<ICommand> callback)
        {
            _animation = animation;
            _callback = callback;
        }

        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            return null;
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            return null;
        }

        public void Update()
        {
            if (!_animation.Done)
            {
                _animation.Update();
                Game.ForceRender();
            }
            else
            {
                Game.StateHandler.PopState();
            }
        }

        public void Draw()
        {
            _animation.Draw();
        }
    }
}
