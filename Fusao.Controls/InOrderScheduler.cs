using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading; 

using Fusao.Controls.Utilities;
using Windows.Foundation;

namespace Fusao.Controls
{
    class InOrderScheduler
    {
        private LightPriorityQueue<DateTime, SchedulerEntry> _set;
        private Dictionary<ApproximateDateBlock, SchedulerEntry> _lookupDict;

        private DateTime _nextScheduledRuntime;
        private SchedulerEntry _nextEntry;
        private CancellationTokenSource _cancalTokenSource;

        private static InOrderScheduler _instance;

        private InOrderScheduler()
        {
            _set = new LightPriorityQueue<DateTime, SchedulerEntry>();
            _lookupDict = new Dictionary<ApproximateDateBlock, SchedulerEntry>();
            
        }

        public static void Register(ApproximateDateBlock dateBlock)
        {
            lock (typeof(InOrderScheduler))
            {
                if (_instance == null) _instance = new InOrderScheduler();
            }
            Task.Run(new Action(() =>
            {
                _instance.InternalRegister(dateBlock);
            }));
        }

        public static void Unregister(ApproximateDateBlock dateBlock)
        {
            lock (typeof(InOrderScheduler))
            {
                if (_instance == null) _instance = new InOrderScheduler();
            }
            Task.Run(new Action(() =>
            {
                _instance.InternalUnregister(dateBlock);
            }));
        }

        private void InternalRegister(ApproximateDateBlock dateBlock)
        {
            lock (this)
            {
                CancelCurrentUpdate();
                SchedulerEntry schedulerEntry = new SchedulerEntry(dateBlock);
                schedulerEntry.DateUpdated += OnDateUpdated;
                _set.Add(schedulerEntry.CachedNextUpdate, schedulerEntry);
                _lookupDict.Add(dateBlock, schedulerEntry);
                ScheduleNextUpdate();
            }
        }

        private void InternalUnregister(ApproximateDateBlock dateBlock)
        {
            SchedulerEntry schedulerEntry;
            lock (this)
            {
                if (_lookupDict.TryGetValue(dateBlock, out schedulerEntry))
                {
                    CancelCurrentUpdate();
                    _lookupDict.Remove(dateBlock);
                    _set.Remove(schedulerEntry);
                    ScheduleNextUpdate();
                }
            }
        }

        private void CancelCurrentUpdate()
        {
            lock (this)
            {
                if (_cancalTokenSource != null)
                {
                    _cancalTokenSource.Cancel();
                    _cancalTokenSource = null;
                }
                _nextScheduledRuntime = default(DateTime);
                _nextEntry = null;
            }
        }

        private TimeSpan BoundToZero(TimeSpan input)
        {
            if (input.TotalMilliseconds < 0)
            {
                return TimeSpan.FromMilliseconds(0);
            }
            else
            {
                return input;
            }
        }

        private void ScheduleNextUpdate()
        {
            lock (this)
            {
                if (_set.Count > 0)
                {
                    SchedulerEntry nextEntry = _set.Min;
                    _nextScheduledRuntime = nextEntry.CachedNextUpdate;
                    _cancalTokenSource = new CancellationTokenSource();

                    TimeSpan toNextRun = BoundToZero(nextEntry.CachedNextUpdate - DateTime.Now);
                    Task t = Task.Delay(toNextRun, _cancalTokenSource.Token).ContinueWith(new Action<Task>((taskArg) =>
                    {
                        lock (this)
                        {
                            if (taskArg.IsCanceled) return;
                            _set.Remove(nextEntry);
                            nextEntry.Update();
                            _set.Add(nextEntry.CachedNextUpdate, nextEntry);
                            ScheduleNextUpdate();
                        }
                    }));
                   // t.Start();
                }
            }
        }

        private void OnDateUpdated(object sender, EventArgs e)
        {
            lock (this)
            {
                SchedulerEntry entry = (SchedulerEntry)sender;
                CancelCurrentUpdate();
                _set.Remove(entry);
                entry.Update();
                _set.Add(entry.CachedNextUpdate, entry);
                ScheduleNextUpdate();
            }
        }
    }
}