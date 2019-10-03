using System;
using System.Diagnostics;

namespace AMG.Physics
{
    public class TimeDependentActionable : ITimeDependentAction
    {
        private readonly Action<double> _action;

        public TimeDependentActionable(Action<double> action)
        {
            _action = action;
        }

        public void Act(double interval)
        {
            _action(interval);
        }
    }

    public interface ITimeDependentAction {
        void Act(double interval);
    }
}