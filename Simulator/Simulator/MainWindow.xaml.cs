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
    /// 

    public class Stations
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public bool Busy { get; set; }
        public string Instruction { get; set; }
        public string State { get; set; }
        public string Vj { get; set; }
        public string Vk { get; set; }
        public string Qj { get; set; }
        public string Qk { get; set; }
        public string A { get; set; }
    }

    public partial class MainWindow : Window
    {
        /* Fields */

        private Simulator simulator;
        public string filePath;

        /* Constructor */

        public MainWindow()
        {
            InitializeComponent();

            PlayBtn.Content = "\u25B8";
            StepBtn.Content = "\u25B8\u2759";
            StopBtn.Content = "\u25FE";
            PauseBtn.Content = "\u2759\u2759";

            simulator = new Simulator(); // Chamar funções nele

        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text documents|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
                FilePath_lbl.Content = filePath;
            }

        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            ReserveStations.ItemsSource = simulator.RS.ToList();
            Registers.ItemsSource = simulator.RegisterStat.ToList();
            CurrentClock.ItemsSource = simulator.RS.ToList(); //Alterar essa parada
            RecentMemory.ItemsSource = simulator.RegisterStat.ToList(); //Alterar essa parada
        }

        private void StepBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Step Forward");
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pause");
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            filePath = "Escolha um arquivo para compilar";
            FilePath_lbl.Content = filePath;
        }

        private void HelpBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Foi mal, ainda não posso te ajudar", "Help");
        }


    }
}
