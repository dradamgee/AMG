using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AMG.Physics
{
    public class UniversalTime {
        private readonly IEnumerable<TimeDependentAction> _actions;
        private readonly CancellationToken _cancelToken;
        private const int _interval = 25;
        private const double _intervalInSecounds = 0.025d;
        private Task time;

        public UniversalTime(IEnumerable<TimeDependentAction> actions, CancellationToken cancelToken)
        {
            _actions = actions;
            _cancelToken = cancelToken;
            time = new Task(Work);
        }

        public void Start()
        {
            time.Start();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        private void Work()
        {
            while (true)
            {
                if (_cancelToken.IsCancellationRequested) {
                    return;
                }
                var start = DateTime.Now;
                //Debug.WriteLine("Time Tick - start - " + start);
                foreach (TimeDependentAction timeDependentAction in _actions) {
                    timeDependentAction.Act(_intervalInSecounds);
                }
                var end = DateTime.Now;
                //Debug.WriteLine("Time Tick - 2 - " + DateTime.Now);

                var totalMilliseconds = new TimeSpan(end.Ticks - start.Ticks).TotalMilliseconds;
                if (totalMilliseconds > _interval) 
                {
                    Debug.WriteLine("Warning tick took " + totalMilliseconds);
                }

                
                _cancelToken.WaitHandle.WaitOne(_interval);
            }
        }
        
    }
}