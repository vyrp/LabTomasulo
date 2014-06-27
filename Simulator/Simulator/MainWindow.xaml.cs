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
    enum State { None, Ready, Running, Completed }

    enum WindowAction { LoadFile, Step, Play, Pause, Stop, Time }

    public partial class MainWindow : Window
    {
        /* Fields */

        private Simulator simulator;
        private State state = State.None;

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

        private void UpdateState(WindowAction action)
        {
            if (state == State.None || state == State.Completed || state == State.Running && action != WindowAction.Time)
            {
                state = State.Ready;
            }
            else if (state == State.Ready && action == WindowAction.Step && simulator.Completed || state == State.Running && action == WindowAction.Time)
            {
                state = State.Completed;
            }
            else if (state == State.Ready && action == WindowAction.Play)
            {
                state = State.Running;
            }

            StepBtn.IsEnabled = (state == State.Ready);
            PlayBtn.IsEnabled = (state == State.Ready);
            PauseBtn.IsEnabled = (state == State.Running);
            StopBtn.IsEnabled = (state != State.None);
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
                UpdateValues();
                UpdateState(WindowAction.LoadFile);
            }
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateValues();
            UpdateState(WindowAction.Play);
        }

        private void StepBtn_Click(object sender, RoutedEventArgs e)
        {
            simulator.Next();
            UpdateValues();
            UpdateState(WindowAction.Step);
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(WindowAction.Pause);
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(WindowAction.Stop);
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
