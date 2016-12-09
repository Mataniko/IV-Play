using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IVPlay.View
{
    /// <summary>
    /// Interaction logic for ColorComboBoxControl.xaml
    /// </summary>
    public partial class ColorComboBoxControl : UserControl
    {
        public ColorComboBoxControl()
        {
            InitializeComponent();            
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ColorComboBoxControl), new PropertyMetadata(string.Empty));



        public Color Value
        {
            get { return (Color)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Color), typeof(ColorComboBoxControl), new PropertyMetadata(Color.FromRgb(0,0,0)));

        private void cmbColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = (sender as ComboBox);            
            Console.WriteLine("Selection Changed");
        }
    }
}
