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
        #region Command DependencyProperty
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
        #endregion

        #region IsRotated DependencyProperty
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
            if (e.OldValue == e.NewValue)
            {
                return;
            }

            PerformRotationChange();
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
        #endregion

        #region IsVertical DependencyProperty
        public static DependencyProperty IsVerticalProperty = DependencyProperty.Register(
            "IsVertical",
            typeof(bool),
            typeof(RotateButton),
            new PropertyMetadata(false, IsVerticalChanged)
        );

        private static void IsVerticalChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            ((RotateButton)sender).IsVerticalChanged(e);
        }

        private void IsVerticalChanged(DependencyPropertyChangedEventArgs e)
        {
            PerformOrientationChange();     
        }

        public bool IsVertical
        {
            get
            {
                return (bool)GetValue(IsVerticalProperty);
            }
            set
            {
                SetValue(IsVerticalProperty, value);
            }
        }
        #endregion

        private Button _mainButton = null;

        public RotateButton()
        {
            this.DefaultStyleKey = typeof(RotateButton);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _mainButton = (Button)GetTemplateChild("MainButton");
            PerformOrientationChange();
        }

        private void PerformOrientationChange()
        {
            if (_mainButton == null)
            {
                return;
            }

            RotateTransform rTransform = _mainButton.RenderTransform as RotateTransform;
            if (rTransform != null)
            {
                rTransform.Angle = LookupToAngle();
            }
        }

        private void PerformRotationChange()
        {
            if (_mainButton == null)
            {
                return;
            }

            double from = LookupFromAngle();
            double to = LookupToAngle();

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

            d.Completed += delegate
            {
                RotateTransform rTransform = _mainButton.RenderTransform as RotateTransform;
                if (rTransform != null)
                {
                    rTransform.Angle = to;
                }
                s.Stop();
            };           
        }

        private double LookupToAngle()
        {
            if (this.IsRotated)
            {
                if (this.IsVertical)
                {
                    return 90;
                }
                else
                {
                    return 180;
                }
            }
            else
            {
                if (this.IsVertical)
                {
                    return 270;
                }
                else
                {
                    return 0;
                }
            }
        }

        private double LookupFromAngle()
        {
            if (this.IsRotated)
            {
                if (this.IsVertical)
                {
                    return 270;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (this.IsVertical)
                {
                    return 90;
                }
                else
                {
                    return 180;
                }
            }
        }
    }
}
