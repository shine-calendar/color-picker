using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorPickerSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            colorPicker.OwnerWindow = this;
        }

        private void colorPicker_ColorChanged(object sender, ShineColorPicker.Controls.ColorEventArgs e)
        {
            border.Background = new SolidColorBrush(e.Color);
        }
    }
}
