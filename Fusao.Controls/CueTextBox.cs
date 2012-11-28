using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Fusao.Controls
{
    public sealed class CueTextBox : TextBox 
    {
        private enum TextBoxState { Empty, Typing, HasText };
        private TextBoxState _currentState = TextBoxState.Empty;

        private TextBox _mainBox;

        public CueTextBox()
        {
            this.DefaultStyleKey = typeof(CueTextBox);
            this.Text = "";
            this.ActualTextBrush = this.CueTextBrush; 
        }

        protected override void OnApplyTemplate()
        {
            _mainBox = (TextBox)GetTemplateChild("MainTextBox");
            _mainBox.GotFocus += CueTextBox_GotFocus;
            _mainBox.LostFocus += CueTextBox_LostFocus;
            _mainBox.TextChanged += OnTextChanged;
            _mainBox.Text = this.CueText;
            base.OnApplyTemplate();
        }

        private void CueTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_currentState == TextBoxState.Empty)
            {
                _mainBox.Text = "";
                this.ActualTextBrush = this.Foreground;
            }
            _currentState = TextBoxState.Typing;
        }

        private void CueTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(_mainBox.Text))
            {
                _currentState = TextBoxState.Empty;
                _mainBox.Text = this.CueText;
                this.ActualTextBrush = this.CueTextBrush;
            }
            else
            {
                _currentState = TextBoxState.HasText;
            }
        }

        public static DependencyProperty CueTextProperty = DependencyProperty.Register("CueText", typeof(string), typeof(CueTextBox), new PropertyMetadata("", OnCueTextChanged));

        private static void OnCueTextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CueTextBox c = (CueTextBox)sender;
            c.OnCueTextChanged(e);
        }

        private void OnCueTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_mainBox != null && _currentState == TextBoxState.Empty)
            {
                _mainBox.Text = this.CueText;
                this.ActualTextBrush = this.CueTextBrush;                
            }
        }

        public string CueText
        {
            get
            {
                return (string)GetValue(CueTextProperty);
            }
            set
            {
                SetValue(CueTextProperty, value);
            }
        }

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            if (_mainBox != null && _currentState == TextBoxState.Typing)
            {
                this.Text = _mainBox.Text;
            }
        }

        public static DependencyProperty ActualTextBrushProperty = DependencyProperty.Register("ActualTextBrush", typeof(Brush), typeof(CueTextBox), new PropertyMetadata(null));

        public Brush ActualTextBrush
        {
            get
            {
                return (Brush)GetValue(ActualTextBrushProperty);
            }

            set
            {
                SetValue(ActualTextBrushProperty, value);
            }
        }

        public static DependencyProperty CueTextBrushProperty = DependencyProperty.Register("CueTextBrush", typeof(Brush), typeof(CueTextBox), new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Gray)));

        public Brush CueTextBrush
        {
            get
            {
                return (Brush)GetValue(CueTextBrushProperty);
            }

            set
            {
                SetValue(CueTextBrushProperty, value);
            }
        }
    }
}
