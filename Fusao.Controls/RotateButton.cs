using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Fusao.Controls
{
    [TemplatePart(Name = "MainButton", Type = typeof(Button))]
    public sealed class RotateButton : Control
    {
        public static DependencyProperty CommandProperty = DependencyProperty.Register(
           "Command",
           typeof(ICommand),
           typeof(RotateButton),
           new PropertyMetadata(null)
       );

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public static DependencyProperty IsRotatedProperty = DependencyProperty.Register(
            "IsRotated",
            typeof(bool),
            typeof(RotateButton),
            new PropertyMetadata(false, IsRotatedChanged)
        );

        private static void IsRotatedChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            RotateButton instance = (RotateButton)sender;
            instance.IsRotatedChanged(e);
        }

        private void IsRotatedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_mainButton == null)
            {
                return;
            }

            if (e.OldValue == e.NewValue)
            {
                return;
            }

            double from = 0;
            double to = 0;

            if ((bool)e.OldValue)
            {
                to = 180;
            }
            else
            {
                from = 180;
            }

            DoubleAnimation d = new DoubleAnimation()
            {
               BeginTime = new TimeSpan(0),
               Duration = new Duration(TimeSpan.FromMilliseconds(450)),
               From = from,
               To = to,
               FillBehavior = FillBehavior.HoldEnd
            };

            Storyboard s = new Storyboard();
            s.Children.Add(d);
            Storyboard.SetTarget(d, _mainButton);
            Storyboard.SetTargetProperty(d, "(UIElement.RenderTransform).(RotateTransform.Angle)");
            s.Begin();
        }

        public bool IsRotated
        {
            get
            {
                return (bool)GetValue(IsRotatedProperty);
            }
            set
            {
                SetValue(IsRotatedProperty, value);
            }
        }

        private Button _mainButton = null;
        public RotateButton()
        {
            this.DefaultStyleKey = typeof(RotateButton);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _mainButton = (Button)GetTemplateChild("MainButton");
        }
    }
}
