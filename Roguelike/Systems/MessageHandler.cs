using BearLib;
using Roguelike.Core;
using System;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    public enum MessageLevel
    {
        Minimal,
        Normal,
        Verbose
    }

    // Accepts messages from various systems and displays it in the message console. Also keeps a 
    // rolling history of messages that can be displayed.
    public class MessageHandler
    {
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
        }

        // Modify the last message by adding additional text.
        public void AppendMessage(string text)
        {
            int prev = _messages.Count - 1;
            _messages[prev] += " " + text;
        }

        public void Clear()
        {
            _messages.Clear();
        }

        public void Draw(LayerInfo layer)
        {
            int maxCount = Math.Min(_messages.Count, layer.Height - 1);
            int yPos = layer.Height - 1;

            for (int i = 0; i < maxCount; i++)
            {
                Terminal.Color(Colors.Text);
                layer.Print(yPos, _messages[_messages.Count - i - 1]);
                yPos--;
            }
        }
    }
}
