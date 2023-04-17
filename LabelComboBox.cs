using System.Windows;
using System.Windows.Controls;

namespace PTGui_Language_Editor
{
    public class LabelComboBox : ComboBox
    {
        /// <summary>
        /// The Label
        /// </summary>
        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        /// <summary>
        /// The LabelProperty
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(LabelComboBox), new PropertyMetadata(string.Empty));
    }
}
