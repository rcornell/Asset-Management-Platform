using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class TimerMessage
    {
        public bool StartTimer;
        public bool StopTimer;
        public TimeSpan Span;

        public TimerMessage(TimeSpan span, bool startTimer, bool stopTimer)
        {
            Span = span;
            StartTimer = startTimer;
            StopTimer = stopTimer;
        }
    }
}
