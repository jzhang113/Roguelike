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
            // draw borders
            Terminal.Color(Colors.BorderColor);
            layer.Put(0, 0, '╟'); // 199
            layer.Put(layer.Width - 1, 0, '╢'); // 182
            layer.Put(0, layer.Height - 1, '╩'); // 202
            layer.Put(layer.Width - 1, layer.Height - 1, '╩'); // 202

            for (int x = 1; x < layer.Width - 1; x++)
            {
                layer.Put(x, 0, '─'); // 196
                layer.Put(x, layer.Height - 1, '═'); // 205
            }

            for (int y = 1; y < layer.Height - 1; y++)
            {
                layer.Put(0, y, '║'); // 186
                layer.Put(layer.Width - 1, y, '║');
            }

            Terminal.Color(Colors.Text);
            layer.Print(0, "[[SYSTEM LOG]]", System.Drawing.ContentAlignment.TopCenter);

            // draw messages
            int maxCount = Math.Min(_messages.Count, layer.Height - 2);
            int yPos = layer.Height - 2;

            for (int i = 0; i < maxCount; i++)
            {
                layer.Print(yPos, _messages[_messages.Count - i - 1]);
                yPos--;
            }
        }
    }
}
