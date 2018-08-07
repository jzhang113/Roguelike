using System;
using System.Collections.Generic;

namespace Roguelike.Statuses
{
    [Serializable]
    public class StatusHandler
    {
        private IDictionary<StatusType, StatusInfo> _statuses;

        public StatusHandler()
        {
            _statuses = new Dictionary<StatusType, StatusInfo>();
        }

        public void AddStatus(StatusType type, int timeout)
        {
            if (_statuses.TryGetValue(type, out StatusInfo status))
            {
                if (status.Expires && timeout >= 0)
                    status.Timeout += timeout;
                else if (status.Expires && timeout < 0)
                    _statuses[type] = new StatusInfo(-timeout - 1);

                // TODO: does putting on equipment and then taking it off clear statuses?
            }
            else
            {
                _statuses.Add(type, new StatusInfo(timeout));
            }
        }

        public bool RemoveStatus(StatusType type)
        {
            return _statuses.Remove(type);
        }

        public bool TryGetStatus(StatusType type, out StatusInfo status)
        {
            return _statuses.TryGetValue(type, out status);
        }

        public void Process()
        {
            IList<StatusType> remove = new List<StatusType>();

            foreach (var kvp in _statuses)
            {
                StatusType type = kvp.Key;
                StatusInfo status = kvp.Value;

                switch (type)
                {
                    case StatusType.Burning:
                        Console.WriteLine("tick");
                        break;
                }

                if (status.Expires && --status.Timeout < 0)
                    remove.Add(type);
            }

            foreach (StatusType type in remove)
            {
                RemoveStatus(type);
            }
        }
    }
}
