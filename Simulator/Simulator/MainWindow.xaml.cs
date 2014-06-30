using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
        private const int SleepTime = 100;

        /* Fields */

        private Simulator simulator;
        private State state = State.None;
        private Label[] Qis = new Label[32];
        private Label[] Vis = new Label[32];
        private bool allowedToRun;
        private string fileName;

        /* Constructor */

        public MainWindow()
        {
            InitializeComponent();

            #region Registers Labels
            Label lbl;
            for (int i = 0; i < 4; i++)
            {
                lbl = new Label() { Content = "Qi" };
                Grid.SetColumn(lbl, i * 3 + 1);
                Grid.SetRow(lbl, 1);
                Registers.Children.Add(lbl);

                lbl = new Label() { Content = "Vi" };
                Grid.SetColumn(lbl, i * 3 + 2);
                Grid.SetRow(lbl, 1);
                Registers.Children.Add(lbl);
            }
            for (int i = 0; i < 32; i++)
            {
                lbl = new Label() { Content = "R" + i };
                Grid.SetColumn(lbl, i / 8 * 3);
                Grid.SetRow(lbl, i % 8 + 2);
                Registers.Children.Add(lbl);

                Qis[i] = new Label() { Content = "-" };
                Grid.SetColumn(Qis[i], i / 8 * 3 + 1);
                Grid.SetRow(Qis[i], i % 8 + 2);
                Registers.Children.Add(Qis[i]);

                Vis[i] = new Label() { Content = "-" };
                Grid.SetColumn(Vis[i], i / 8 * 3 + 2);
                Grid.SetRow(Vis[i], i % 8 + 2);
                Registers.Children.Add(Vis[i]);
            }
            #endregion

            PlayBtn.Content = "\u25B8";
            StepBtn.Content = "\u25B8\u2759";
            StopBtn.Content = "\u25FE";
            PauseBtn.Content = "\u2759\u2759";

            simulator = new Simulator();

            ReserveStations.ItemsSource = simulator.RS.Skip(1);
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

            if (dialog.ShowDialog() == true)
            {
                fileName = dialog.FileName;
                FilePath_lbl.Content = fileName;
                simulator.LoadFile(fileName);
                UpdateValues();
                UpdateState(WindowAction.LoadFile);
            }
        }

        private async void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            allowedToRun = true;
            UpdateState(WindowAction.Play);
            await Task.Run(() =>
            {
                while (!simulator.Completed && allowedToRun)
                {
                    simulator.Next();
                    Dispatcher.Invoke(() =>
                    {
                        UpdateValues();
                    });
                    Thread.Sleep(SleepTime);
                }
            });
            if (simulator.Completed)
            {
                UpdateState(WindowAction.Time);
            }
        }

        private void StepBtn_Click(object sender, RoutedEventArgs e)
        {
            simulator.Next();
            UpdateValues();
            UpdateState(WindowAction.Step);
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            allowedToRun = false;
            Thread.Sleep(SleepTime / 2);
            UpdateState(WindowAction.Pause);
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (state == State.Running)
            {
                allowedToRun = false;
                Thread.Sleep(SleepTime / 2);
            }
            simulator.LoadFile(fileName);
            UpdateValues();
            UpdateState(WindowAction.Stop);
        }

        private void HelpBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Foi mal, ainda não posso te ajudar", "Help");
        }

        private void UpdateValues()
        {
            ReserveStations.ItemsSource = simulator.RS.Skip(1);
            
            for (int i = 0; i < 32; i++)
            {
                Qis[i].Content = simulator.RS[simulator.RegisterStat[i].Qi].ID;
                Vis[i].Content = simulator.Regs[i];
            }

            Clock_Lbl.Content = simulator.Clock;
            PC_Lbl.Content = simulator.PC;
            CompletedInstructions_Lbl.Content = simulator.CompletedInstructions;
            CPI_Lbl.Content = simulator.CPI;
        }
    }
}
