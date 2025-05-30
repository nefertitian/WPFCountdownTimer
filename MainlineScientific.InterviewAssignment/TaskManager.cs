using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainlineScientific.InterviewAssignment
{
    public static class TaskManager
    {
        private static readonly object _lock = new object();

        private static Task? _backgroundTask;

        public static Task? BackgroundTask
        { 
            get
            {
                lock(_lock)
                {
                    return _backgroundTask;
                }
            }
            set
            {
                lock(_lock)
                {
                    _backgroundTask = value;
                }
            }
        }
    }
}
