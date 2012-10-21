using Fusao.Controls.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Fusao.Controls
{
    public sealed class ApproximateDateBlock : Control
    {
        private string _text;
        private AnimatedTextBlock _textBlock;

        private static PriorityScheduler _sched = new PriorityScheduler();
        private SchedulerEntry _currentEntry = null;

        public ApproximateDateBlock()
        {
            this.DefaultStyleKey = typeof(ApproximateDateBlock);
            Update();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _textBlock = GetTemplateChild("TextElement") as AnimatedTextBlock;
            Update();
        }

        #region Dependency Properties
        public static DependencyProperty DateProperty = 
            DependencyProperty.Register("Date", typeof(DateTime), typeof(ApproximateDateBlock), new PropertyMetadata(DateTime.Now, DatePropertyChanged));

        public DateTime Date 
        {
            get
            {
                return (DateTime)GetValue(DateProperty);
            }
            set
            {
                SetValue(DateProperty, value);
            }
        }
        
        public static void DatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ApproximateDateBlock)d).DatePropertyChanged(e);
        }

        private void DatePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_currentEntry != null)
            {
                _currentEntry.Cancel();
            }

            AnimatedTextBlock.AnimationType prevAnimation = AnimatedTextBlock.AnimationType.None;

            if (_textBlock != null)
            {
                prevAnimation = _textBlock.Animation;
                _textBlock.Animation = AnimatedTextBlock.AnimationType.None;
            }

            Update();

            if (_textBlock != null)
            {
                _textBlock.Animation = prevAnimation;
            }
        }

        #endregion

        #region DateBlockScheduler Interface
        public DateTime NextUpdate { get; set; }

        public void Update()
        {
            string newText = FriendlyDateStrings.GetString(this.Date);
            this.NextUpdate = FriendlyDateStrings.GetNextUpdateTime(this.Date);
            _text = newText;
            if (_textBlock != null)
            {
                if (newText != _textBlock.Text)
                {
                    _textBlock.Text = newText;
                }
            }

            _currentEntry = _sched.Add(this.NextUpdate, new Action(() =>
            {
                IAsyncAction i = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Update);
            }));
        }
        #endregion


    }
}
