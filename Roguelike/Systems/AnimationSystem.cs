using Roguelike.Animations;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    public class AnimationSystem
    {
        private readonly ICollection<IAnimation> _currentAnimations;
        private readonly ICollection<IAnimation> _finishedAnimations;

        public AnimationSystem()
        {
            _currentAnimations = new List<IAnimation>();
            _finishedAnimations = new List<IAnimation>();
        }

        public void Add(IAnimation animation)
        {
            _currentAnimations.Add(animation);
        }

        public void Clear()
        {
            _currentAnimations.Clear();
            _finishedAnimations.Clear();
        }

        public void Update()
        {
            foreach (IAnimation animation in _currentAnimations)
            {
                if (animation.Update() || EventScheduler.Turn > animation.Turn)
                    _finishedAnimations.Add(animation);
            }

            foreach (IAnimation animation in _finishedAnimations)
            {
                _currentAnimations.Remove(animation);
            }

            _finishedAnimations.Clear();
        }

        public void Draw()
        {
            foreach (IAnimation animation in _currentAnimations)
            {
                animation.Draw();
            }
        }
    }
}
