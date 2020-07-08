using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GoogleCloud_TTS_STT.Core
{
    public static class TextBoxHelper
    {

        public static string GetSelectedText(DependencyObject obj)
        {
            return (string)obj.GetValue(SelectedTextProperty);
        }

        public static void SetSelectedText(DependencyObject obj, string value)
        {
            obj.SetValue(SelectedTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.RegisterAttached(
                "SelectedText",
                typeof(string),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedTextChanged));

        private static void SelectedTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TextBox tb)
            {
                switch (e.OldValue)
                {
                    case null when e.NewValue != null:
                        tb.SelectionChanged += Tb_SelectionChanged;
                        break;
                    default:
                        if (e.OldValue != null && e.NewValue == null)
                        {
                            tb.SelectionChanged -= Tb_SelectionChanged;
                        }

                        break;
                }


                if (e.NewValue is string newValue && newValue != tb.SelectedText)
                {
                    tb.SelectedText = newValue;
                }
            }
        }

        static void Tb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                SetSelectedText(tb, tb.SelectedText);
            }
        }

    }
}
