using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Fusao.Controls
{
    public class SchedulerEntry
    {
        private ApproximateDateBlock _dateBlock;
        protected DateTime _cachedUpdateTime;

        public event EventHandler DateUpdated;

        public SchedulerEntry(ApproximateDateBlock dateBlock)
        {
            if (dateBlock == null)
            {
                throw new ArgumentNullException();
            }

            _dateBlock = dateBlock;
            _dateBlock.DateUpdated += _dateBlock_DateUpdated;
            _cachedUpdateTime = dateBlock.NextUpdate;
        }

        protected SchedulerEntry()
        {

        }

        public void Update()
        {
            IAsyncAction action = _dateBlock.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                _dateBlock.Update();
            });
            action.AsTask().Wait();

            _cachedUpdateTime = _dateBlock.NextUpdate;
        }

        public DateTime CachedNextUpdate
        {
            get
            {
                return _cachedUpdateTime;
            }
        }

        private void _dateBlock_DateUpdated(object sender, EventArgs e)
        {
            Task.Run(new Action(() =>
            {
                Update();
                if (DateUpdated != null)
                {
                    DateUpdated(this, EventArgs.Empty);
                }
            }));
        }
    }
}
