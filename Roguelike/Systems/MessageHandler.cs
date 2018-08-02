using RLNET;
using Roguelike.Core;
using System;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    enum MessageLevel
    {
        Minimal,
        Normal,
        Verbose
    }

    // Accepts messages from various systems and displays it in the message console. Also keeps a 
    // rolling history of messages that can be displayed.
    class MessageHandler
    {
        public bool Redraw { get; private set; }

        private readonly int _maxSize;
        private readonly IList<string> _messages;

        public MessageHandler(int maxSize)
        {
            _maxSize = maxSize;
            _messages = new List<string>();
        }

        // Place a new message onto the message log if its MessageLevel is lower than the currently
        // set level.
        public void AddMessage(string text, MessageLevel level = MessageLevel.Normal)
        {
            if (level > Game.Option.Verbosity)
                return;

            _messages.Add(text);

            if (_messages.Count > _maxSize)
                _messages.RemoveAt(0);

            Redraw = true;
            Game.ForceRender();
        }

        // Modify the last message by adding additional text.
        public void AppendMessage(string text)
        {
            int prev = _messages.Count - 1;
            _messages[prev] += " " + text;

            Redraw = true;
            Game.ForceRender();
        }

        public void Clear()
        {
            _messages.Clear();
            Redraw = true;
        }

        public void Draw(RLConsole console)
        {
            int viewSize = (console.Height - 1) / 2;
            int maxCount = Math.Min(_messages.Count, viewSize);
            int yPos = console.Height - 2;

            for (int i = 0; i < maxCount; i++)
            {
                console.Print(1, yPos, _messages[_messages.Count - i - 1], Colors.Text);
                yPos -= 2;
            }

            Redraw = false;
        }
    }
}
