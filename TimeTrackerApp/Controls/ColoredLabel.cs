using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Controls
{
    internal class ColoredLabel :Label
    {
        public static readonly BindableProperty IsErrorMessageProperty =
            BindableProperty.Create(
                nameof(IsErrorMessage),
                typeof(bool),
                typeof(ColoredLabel),
                false, propertyChanged: OnColorChanged);

        public ColoredLabel()
        {
            TextColor = Colors.YellowGreen;
        }

        public bool IsErrorMessage
        {
            get => (bool)GetValue(IsErrorMessageProperty);
            set => SetValue(IsErrorMessageProperty, value);
        }

        private static void OnColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ColoredLabel l = bindable as ColoredLabel;
            l.TextColor = (bool)newValue ? Colors.Red : Colors.YellowGreen;
        }
    }
}
