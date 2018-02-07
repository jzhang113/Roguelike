using RLNET;
using Roguelike.Core;
using System;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    // Accepts messages from various systems and displays it in the message console. Also keeps a 
    // rolling history of messages that can be displayed.
    class MessageHandler
    {
        public bool Redraw { get; private set; }

        private readonly int _maxSize;
        private readonly IList<string> messages;

        public MessageHandler(int maxSize)
        {
            _maxSize = maxSize;
            messages = new List<string>();
        }

        // Place a new message onto the message log.
        public void AddMessage(string text)
        {
            messages.Add(text);

            if (messages.Count > _maxSize)
                messages.RemoveAt(0);

            Redraw = true;
        }

        // Modify the last message by adding additional text.
        public void AppendMessage(string text)
        {
            int prev = messages.Count - 1;
            messages[prev] += " " + text;

            Redraw = true;
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

            Redraw = false;
        }
    }
}
