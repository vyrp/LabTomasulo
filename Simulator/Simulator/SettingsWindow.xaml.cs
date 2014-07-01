using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LabTomasulo
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            UseCacheChk.IsChecked = MainWindow.UseCache;
            NotInstPlayChk.IsChecked = MainWindow.IsNotInstantaneousPlay;
            SpeedSlider.Value = MainWindow.PlaySpeed;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.UseCache = UseCacheChk.IsChecked == true;
            MainWindow.IsNotInstantaneousPlay = NotInstPlayChk.IsChecked == true;
            MainWindow.PlaySpeed = (int)SpeedSlider.Value;
            DialogResult = true;
        }
    }
}
