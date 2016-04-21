using System;
using System.Diagnostics;

namespace AMG.Physics
{
    public class TimeDependentActionable : TimeDependentAction
    {
        private readonly Action<double> _action;

        public TimeDependentActionable(Action<double> action)
        {
            _action = action;
        }

        public override void Act()
        {
            _action(ResetInterval());
        }
    }

    public abstract class TimeDependentAction {
        public TimeDependentAction()
        {
            _lastActedAt = Now;
        }

        public double ResetInterval() {
            var now = Now;
            
            var resetInterval = ((double)now - _lastActedAt) / ticksPerSecond;
            _lastActedAt = now;
            return resetInterval;
        }

        protected long _lastActedAt;
        protected long ticksPerSecond = TimeSpan.FromSeconds(1).Ticks;
        protected long Now { get { return DateTime.Now.Ticks; } }
        public abstract void Act();
    }
}