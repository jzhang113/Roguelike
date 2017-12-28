using Roguelike.Interfaces;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    class EventScheduler
    {
        Queue<IAction> eventQueue = new Queue<IAction>();

        public void Schedule(IAction action, double time)
        {
            eventQueue.Enqueue(action);
        }
    }
}
