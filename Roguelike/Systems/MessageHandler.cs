using RLNET;
using Roguelike.Core;
using System;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    class MessageHandler
    {
        private readonly int _maxSize;
        private readonly IList<string> messages;

        public MessageHandler(int maxSize)
        {
            _maxSize = maxSize;
            messages = new List<string>();
        }

        public void AddMessage(string text)
        {
            messages.Add(text);

            if (messages.Count > _maxSize)
                messages.RemoveAt(0);
        }

        public void Draw(RLConsole console)
        {
            int viewSize = (console.Height - 1) / 2;
            int maxCount = Math.Min(messages.Count, viewSize);
            int yPos = console.Height - 2;

            for (int i = 0; i < maxCount; i++)
            {
                console.Print(1, yPos, messages[messages.Count - i - 1], Colors.TextHeading);
                yPos -= 2;
            }
        }
    }
}
