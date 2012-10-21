using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Fusao.Controls.Utilities
{
    public class PriorityScheduler
    {
        private LightPriorityQueue<DateTime, SchedulerEntry> _workQueue;

        private SchedulerEntry _currentEntry = null;
        private CancellationTokenSource _cts = null;

        public PriorityScheduler()
        {
            _workQueue = new LightPriorityQueue<DateTime, SchedulerEntry>();
        }

        public SchedulerEntry Add(DateTime date, Action action)
        {
            lock (this)
            {
                SchedulerEntry entry = new SchedulerEntry(date, action);
                _workQueue.Add(date, entry);

                if (_currentEntry != _workQueue.Min)
                {
                    if (_currentEntry != null)
                    {
                        UnsafeCancelCurrent();
                    }
                    UnsafeScheduleNext();
                }

                return entry;
            }
        }

        private void UnsafeCancelCurrent()
        {
            _cts.Cancel();
            _currentEntry = null;
            _cts = null;
        }

        private void UnsafeScheduleNext()
        {
            if (_workQueue.Count == 0)
            {
                return;
            }

            _cts = new CancellationTokenSource();
            _currentEntry = _workQueue.Min;

            CancellationToken token = _cts.Token;
            Task.Delay(BoundToZero(_currentEntry.Date - DateTime.Now), _cts.Token).ContinueWith(new Action<Task>((task) =>
            {
                lock(this)
                {
                    if (!token.IsCancellationRequested)
                    {
                        _currentEntry.RunTask();
                        _workQueue.Remove(_currentEntry);
                        UnsafeScheduleNext();
                    }
                }
            }), 
            _cts.Token, 
            TaskContinuationOptions.OnlyOnRanToCompletion, 
            TaskScheduler.Default);
        }

        private TimeSpan BoundToZero(TimeSpan span)
        {
            if (span.Ticks < 0)
            {
                return TimeSpan.FromMilliseconds(0);
            }
            else
            {
                return span;
            }
        }

    }

    public class SchedulerEntry
    {
        public DateTime Date { get { return _date; } }
        public bool HasRun { get { return Interlocked.CompareExchange(ref _runState, 0, 0) == 3; } }

        private DateTime _date;
        private Action _action;

        // 0- Pending, 1- Cancalled, 2- Running, 3- Completed
        private int  _runState = 0;

        public SchedulerEntry(DateTime date, Action action)
        {
            _date = date;
            _action = action;
        }

        public void RunTask()
        {
            bool shouldRun = Interlocked.CompareExchange(ref _runState, 2, 0) == 0;
            
            if (shouldRun)
            {
                _action.Invoke();
            }

            _runState = 3;
        }

        /// <summary>
        /// Cancels the task, returns false if task has already executed or ran. 
        /// </summary>
        /// <returns></returns>
        public bool Cancel()
        {
            return Interlocked.CompareExchange(ref _runState, 1, 0) == 0;
        }
    }
}
