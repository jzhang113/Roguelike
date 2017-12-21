using RLNET;
using System.Collections;
using Roguelike.Core;

namespace Roguelike.Systems
{
    class MessageHandler
    {
        Queue messages = new Queue();
        int maxSize = 5;

        public void AddMessage(string message)
        {
            messages.Enqueue(message);

            if (messages.Count > maxSize)
                messages.Dequeue();
        }

        public void Draw(RLConsole console)
        {
            foreach (string s in messages)
            {
                console.Print(0, 0, s, Colors.TextHeading);
            }
        }
    }
}
