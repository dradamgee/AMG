using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AMG.Physics
{
    public class UniversalTime {
        private readonly IEnumerable<TimeDependentAction> _actions;
        private readonly CancellationToken _cancelToken;
        private int _interval = 10;

        public UniversalTime(IEnumerable<TimeDependentAction> actions, CancellationToken cancelToken)
        {
            _actions = actions;
            _cancelToken = cancelToken;

            Task time = new Task(Start);
            time.Start();
        }

        private void Start()
        {
            while (true)
            {
                if (_cancelToken.IsCancellationRequested) {
                    return;
                }

                foreach (TimeDependentAction timeDependentAction in _actions) {
                    timeDependentAction.Act();
                }
                _cancelToken.WaitHandle.WaitOne(_interval);
            }
        }
        
    }
}