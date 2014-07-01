using System.Windows;

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
