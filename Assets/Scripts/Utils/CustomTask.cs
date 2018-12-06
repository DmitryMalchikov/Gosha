using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Utils
{
    public class CustomTask
    {
        public CustomTask()
        {
            _isReady = false;
        }

        public bool Ready
        {
            get
            {
                return _isReady;
            }
            set
            {
                _isReady = value;
            }
        }

        private bool _isReady = false;

        public static bool TasksReady(IEnumerable<CustomTask> tasks)
        {
            return !tasks.Any(t => t._isReady == false);
        }
    }
}
