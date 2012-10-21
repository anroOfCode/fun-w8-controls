using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Fusao.Controls
{
    public sealed class AnimatedTextBlock : Control
    {
        private TextBlock _textBlock1;
        private TextBlock _textBlock2;
        private Canvas _mainCanvas;
        private Size _boundingSize;
        private TextBlocks _currentTextBlock = TextBlocks.One;
        private Dictionary<AnimationType, Action<DoubleAnimation, TextBlock>> _mappedAnimationFunctions =
            new Dictionary<AnimationType, Action<DoubleAnimation, TextBlock>>();

        private enum TextBlocks { One, Two };
        public enum AnimationType { None, HorizontalSlide, VerticalSlide };

        public AnimatedTextBlock()
        {
            this.DefaultStyleKey = typeof(AnimatedTextBlock);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _textBlock1 = GetTemplateChild("TextBlock1") as TextBlock;
            _textBlock2 = GetTemplateChild("TextBlock2") as TextBlock;
            _mainCanvas = GetTemplateChild("MainCanvas") as Canvas;

            if (!IsSurfaceBound()) return;

            Canvas.SetLeft(_textBlock1, 0);
            Canvas.SetTop(_textBlock1, 0);
            Canvas.SetLeft(_textBlock2, 0);
            Canvas.SetTop(_textBlock2, 0);

            _textBlock1.Text = this.Text;
            _textBlock2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            _currentTextBlock = TextBlocks.One;
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            Size boundingSize = availableSize;

            if (IsSurfaceBound())
            {
                _mainCanvas.Measure(availableSize);

                boundingSize.Height = Math.Max(_textBlock1.DesiredSize.Height, _textBlock2.DesiredSize.Height);


                if (boundingSize.Width == Double.PositiveInfinity || boundingSize.Width == 0.0)
                {
                    boundingSize.Width = Math.Max(_textBlock1.DesiredSize.Width, _textBlock2.DesiredSize.Width);
                }

                _mainCanvas.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, boundingSize.Width, boundingSize.Height) };
                _boundingSize = boundingSize;

                return boundingSize;
            }
            else
            {
                return base.MeasureOverride(availableSize);
            }
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            if (IsSurfaceBound())
            {
                _mainCanvas.Arrange(new Rect(new Point(0, 0), finalSize));
            }
            return finalSize;
        }

        private bool IsSurfaceBound()
        {
            return _textBlock1 != null && _textBlock2 != null && _mainCanvas != null;
        }

        #region TextBlock Chooser Helpers
        private TextBlock VisibleTextBlock
        {
            get
            {
                return _currentTextBlock == TextBlocks.One ? _textBlock1 : _textBlock2;
            }
        }

        private TextBlock HiddenTextBlock
        {
            get
            {
                return _currentTextBlock != TextBlocks.One ? _textBlock1 : _textBlock2;
            }
        }

        private void FlipCurrentTextBlock()
        {
            if (_currentTextBlock == TextBlocks.One)
            {
                _currentTextBlock = TextBlocks.Two;
            }
            else
            {
                _currentTextBlock = TextBlocks.One;
            }
        }

        private TextBlock GetTextBlock(TextBlocks whichTextblock)
        {
            if (whichTextblock == TextBlocks.One)
                return _textBlock1;
            else
                return _textBlock2;
        }
        #endregion

        private void PerformAnimation()
        {
            DoubleAnimation visibleTextAnimation = SetupAndGenerateAnimation(this.VisibleTextBlock);
            DoubleAnimation hiddenTextAnimation = SetupAndGenerateAnimation(this.HiddenTextBlock);

            FlipCurrentTextBlock();

            Storyboard sb = new Storyboard();
            sb.Children.Add(visibleTextAnimation);
            sb.Children.Add(hiddenTextAnimation);
            sb.Begin(); 
        }

        public DoubleAnimation SetupAndGenerateAnimation(TextBlock targetTextblock)
        {
            DoubleAnimation returnAnimation = new DoubleAnimation();
            returnAnimation.Duration = this.Duration;
            Storyboard.SetTarget(returnAnimation, targetTextblock);

            if (!_mappedAnimationFunctions.ContainsKey(this.Animation))
            {
                MethodInfo animationFunction = typeof(AnimatedTextBlock).GetRuntimeMethod("Build" + this.Animation.ToString(), 
                    new Type[] { typeof(DoubleAnimation), typeof(TextBlock) });

                if (animationFunction == null)
                {
                    throw new NotImplementedException();
                }

                Action<DoubleAnimation, TextBlock> a = (Action<DoubleAnimation, TextBlock>)
                    animationFunction.CreateDelegate(typeof(Action<DoubleAnimation, TextBlock>), this);
                _mappedAnimationFunctions.Add(this.Animation, a);
            }

            _mappedAnimationFunctions[this.Animation](returnAnimation, targetTextblock);
            return returnAnimation;
        }

        #region AnimationBuilders
        public void BuildHorizontalSlide(DoubleAnimation animation, TextBlock textBlock)
        {
            Storyboard.SetTargetProperty(animation, "(Canvas.Left)");

            if (textBlock == this.VisibleTextBlock)
            {
                animation.To = -_boundingSize.Width;
                animation.From = 0;
                animation.Completed += delegate
                {
                    textBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                };
            }
            else
            {
                textBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;

                animation.To = 0;
                animation.From = _boundingSize.Width;
                animation.Completed += delegate
                {
                    Canvas.SetLeft(textBlock, 0);
                };
            }
        }

        public void BuildVerticalSlide(DoubleAnimation animation, TextBlock textBlock)
        {
            Storyboard.SetTargetProperty(animation, "(Canvas.Top)");

            if (textBlock == this.VisibleTextBlock)
            {
                animation.To = _boundingSize.Height;
                animation.From = 0;
                animation.Completed += delegate
                {
                    textBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                };
            }
            else
            {
                textBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;

                animation.To = 0;
                animation.From = -_boundingSize.Height;
                animation.Completed += delegate
                {
                    Canvas.SetTop(textBlock, 0);
                };
            }
        }

        public void BuildNone(DoubleAnimation animation, TextBlock textBlock)
        {
            Storyboard.SetTargetProperty(animation, "(Canvas.Left)");

            if (textBlock == this.VisibleTextBlock)
            {
                animation.To = -_boundingSize.Width;
                animation.From = -_boundingSize.Width;
                animation.Completed += delegate
                {
                    textBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                };
            }
            else
            {
                textBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;

                animation.To = 0;
                animation.From = 0;
                animation.Completed += delegate
                {
                    Canvas.SetLeft(textBlock, 0);
                };
            }
        }
        #endregion

        #region DependencyProperties
        public static DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", 
            typeof(string), 
            typeof(AnimatedTextBlock), 
            new PropertyMetadata("", AnimatedTextBlock.TextPropertyChanged));

        public string Text
        {
            get
            {
                return (string) GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AnimatedTextBlock)d).TextPropertyChanged(e);
        }

        private void TextPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!IsSurfaceBound()) return;
            this.HiddenTextBlock.Text = this.Text;
            PerformAnimation();
        }

        public static DependencyProperty AnimationProperty = DependencyProperty.Register(
            "Animation",
            typeof(AnimationType),
            typeof(AnimatedTextBlock),
            new PropertyMetadata(AnimationType.HorizontalSlide));

        public AnimationType Animation
        {
            get
            {
                return (AnimationType)GetValue(AnimationProperty);
            }
            set
            {
                SetValue(AnimationProperty, value);
            }
        }

        public static DependencyProperty DurationProperty = DependencyProperty.Register(
            "Duration",
            typeof(TimeSpan),
            typeof(AnimatedTextBlock),
            new PropertyMetadata(TimeSpan.FromMilliseconds(250)));

        public TimeSpan Duration
        {
            get
            {
                return (TimeSpan)GetValue(DurationProperty);
            }
            set
            {
                SetValue(DurationProperty, value);
            }
        }
        #endregion
    }
}
