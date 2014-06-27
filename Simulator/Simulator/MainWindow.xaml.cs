using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LabTomasulo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /* Fields */

        private Simulator simulator;

        /* Constructor */

        public MainWindow()
        {
            InitializeComponent();

            PlayBtn.Content = "\u25B8";
            StepBtn.Content = "\u25B8\u2759";
            StopBtn.Content = "\u25FE";
            PauseBtn.Content = "\u2759\u2759";

            simulator = new Simulator();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents|*.txt";

            if (dialog.ShowDialog() == true)
            {
                FilePath_lbl.Content = dialog.FileName;
                simulator.LoadFile(dialog.FileName);
            }
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateValues();
        }

        private void StepBtn_Click(object sender, RoutedEventArgs e)
        {
            simulator.Next();
            UpdateValues();
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pause");
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            FilePath_lbl.Content = "Escolha um arquivo para compilar";
        }

        private void HelpBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Foi mal, ainda não posso te ajudar", "Help");
        }

        private void UpdateValues()
        {
            ReserveStations.ItemsSource = simulator.RS.Skip(1);
            Registers.ItemsSource = null;
            CurrentClock.ItemsSource = null;
            RecentMemory.ItemsSource = null;
        }
    }
}
