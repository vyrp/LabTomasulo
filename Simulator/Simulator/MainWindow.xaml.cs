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
        /* Fields */

        private Simulator simulator;
        private State state = State.None;
        private Label[,] RS = new Label[12, 10];
        private Label[] Qis = new Label[32];
        private Label[] Vis = new Label[32];
        private Label[,] recentMemoryAccesses = new Label[Simulator.RecentMemorySize, 2];
        private bool allowedToRun;
        private string fileName;

        /* Properties */

        public static bool UseCache { get; set; }
        public static bool IsNotInstantaneousPlay { get; set; }
        public static int PlaySpeed { get; set; }

        /* Constructor */

        public MainWindow()
        {
            InitializeComponent();
            UseCache = false;
            IsNotInstantaneousPlay = false;
            PlaySpeed = 100;
            simulator = new Simulator();

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

            #region Recent Memory
            for (int i = 0; i < Simulator.RecentMemorySize; i++)
            {
                recentMemoryAccesses[i, 0] = new Label() { Content = "-", HorizontalAlignment = HorizontalAlignment.Center };
                Grid.SetColumn(recentMemoryAccesses[i, 0], 0);
                Grid.SetRow(recentMemoryAccesses[i, 0], i + 2);
                RecentMemory.Children.Add(recentMemoryAccesses[i, 0]);

                recentMemoryAccesses[i, 1] = new Label() { Content = "-", HorizontalAlignment = HorizontalAlignment.Center };
                Grid.SetColumn(recentMemoryAccesses[i, 1], 1);
                Grid.SetRow(recentMemoryAccesses[i, 1], i + 2);
                RecentMemory.Children.Add(recentMemoryAccesses[i, 1]);
            }
            #endregion

            #region RS
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    RS[i, j] = new Label() { Content = GetRSElement(i, j, false), HorizontalAlignment = HorizontalAlignment.Center };
                    Grid.SetColumn(RS[i, j], j);
                    Grid.SetRow(RS[i, j], i + 2);
                    ReserveStations.Children.Add(RS[i, j]);
                }
            }
            #endregion

            PlayBtn.Content = "\u25B8";
            StepBtn.Content = "\u25B8\u2759";
            StopBtn.Content = "\u25FE";
            PauseBtn.Content = "\u2759\u2759";
        }

        private string GetRSElement(int i, int j, bool show)
        {
            if (j >= 2 && !show)
            {
                return "-";
            }

            ReserveStation station = simulator.RS[i+1];
            switch (j)
            {
                case 0:
                    return station.ID;
                case 1:
                    return station.Type.ToString();
                case 2:
                    return station.Busy.ToString();
                case 3:
                    return station.Instruction == null ? "-" : station.Instruction.ToString();
                case 4:
                    return station.Phase.ToString();
                case 5:
                    return station.Vj.ToString();
                case 6:
                    return station.Vk.ToString();
                case 7:
                    return simulator.RS[station.Qj].ID;
                case 8:
                    return simulator.RS[station.Qk].ID;
                case 9:
                    return station.A.ToString();
                default:
                    return "-";
            }
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

        private async void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            allowedToRun = true;
            UpdateState(WindowAction.Play);
            await Task.Run(() =>
            {
                while (!simulator.Completed && allowedToRun)
                {
                    simulator.Next();
                    if (IsNotInstantaneousPlay)
                    {
                        Dispatcher.Invoke(() => { UpdateValues(); });
                        Thread.Sleep(PlaySpeed);
                    }
                }
            });
            UpdateValues();
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
            Thread.Sleep(PlaySpeed / 2);
            UpdateState(WindowAction.Pause);
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (state == State.Running)
            {
                allowedToRun = false;
                Thread.Sleep(PlaySpeed / 2);
            }
            simulator.LoadFile(fileName);
            UpdateValues();
            UpdateState(WindowAction.Stop);
        }

        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Programa desenvolvido para o projeto 2 da matéria de CES-25", "About");
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow() { Owner = this }.ShowDialog();
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
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

        private void UpdateValues()
        {
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    RS[i, j].Content = GetRSElement(i, j, true);
                }
            }
            for (int i = 0; i < 32; i++)
            {
                Qis[i].Content = simulator.RS[simulator.RegisterStat[i].Qi].ID;
                Vis[i].Content = simulator.Regs[i];
            }

            int counter = 0;
            foreach (var rm in simulator.RecentMemory)
            {
                recentMemoryAccesses[counter, 0].Content = rm.Address;
                recentMemoryAccesses[counter, 1].Content = rm.Value;
                counter++;
            }
            for (int i = counter; i < Simulator.RecentMemorySize; i++)
            {
                recentMemoryAccesses[i, 0].Content = "-";
                recentMemoryAccesses[i, 1].Content = "-";
            }

            Clock_Lbl.Content = simulator.Clock;
            PC_Lbl.Content = simulator.PC;
            CompletedInstructions_Lbl.Content = simulator.CompletedInstructions;
            CPI_Lbl.Content = (float.IsNaN(simulator.CPI) ? "-" : simulator.CPI.ToString());
        }
    }
}
