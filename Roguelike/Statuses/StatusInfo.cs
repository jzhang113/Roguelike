using System;

namespace Roguelike.Statuses
{
    [Serializable]
    public class StatusInfo
    {
        public bool Expires { get; }
        public int Timeout { get; set; }

        public StatusInfo(int timeout)
        {
            if (timeout < 0)
            {
                Expires = false;
                Timeout = 0;
            }
            else
            {
                Expires = true;
                Timeout = timeout;
            }
        }
    }
}
