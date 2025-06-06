﻿using MainlineScientific.InterviewAssignment.ViewModel;
using Serilog;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainlineScientific.InterviewAssignment.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource? _cts;

        private Task? _complexOperationTask;

        public MainWindow()
        {
            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "-->MainWindow() - " + this.GetType().ToString());

            InitializeComponent();
            this.DataContext = new MainWindowViewModel();

            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "<--MainWindow() -" + this.GetType().ToString());
        }

        private async void btnRunComplexOp_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "-->btnRunComplexOp_Click(sender, e) --" + this.GetType().ToString());

            btnRunComplexOp.IsEnabled = false;

            _cts = new CancellationTokenSource();

            try
            {
                TaskManager.BackgroundTask = Task.Run(() => DoBackgroundWork(_cts), _cts.Token);
                var countDownView = new CountDownView(_cts);
                countDownView.Show();
                await TaskManager.BackgroundTask;
            }
            catch (Exception ex)
            {
                Log.Error($"{Environment.CurrentManagedThreadId}\t{ex.Message}");
            } 
            finally
            {
                btnRunComplexOp.IsEnabled = true;
            }

            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "<--btnRunComplexOp_Click(sender, e) --" + this.GetType().ToString());
        }

        private void DoBackgroundWork(CancellationTokenSource cts)
        {
            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "-->DoBackgroundWork(cts) - " + this.GetType().ToString());
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    Log.Information($"[{Environment.CurrentManagedThreadId}]\t Complex Operation Thread - {10-i} seconds remaining");
                    //pauseComplexOpEvent.Wait();  // Wait here if paused
                    cts.Token.ThrowIfCancellationRequested();
                    Thread.Sleep(1000); //milliseconds
                }
                Log.Information("[" + Environment.CurrentManagedThreadId + "]\tComplex Operation Thread Execution has completed");
            }
            catch(OperationCanceledException ex)
            {
                Log.Warning($"[{Environment.CurrentManagedThreadId}]\t - User cancelled the operation: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"[{Environment.CurrentManagedThreadId}]\t{ex.Message}");
            }

            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "<--DoBackgroundWork(cts) - " + this.GetType().ToString());
        }
    }
}