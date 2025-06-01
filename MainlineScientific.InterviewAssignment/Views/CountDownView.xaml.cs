using CommunityToolkit.Mvvm.ComponentModel;
using MainlineScientific.InterviewAssignment.ViewModel;
using Serilog;
using Serilog.Debugging;
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

namespace MainlineScientific.InterviewAssignment.Views
{
    /// <summary>
    /// Interaction logic for CountDownView.xaml
    /// </summary>
    public partial class CountDownView : Window
    {
        //[ObservableProperty]
        private int countdown = 10;

        private readonly CancellationTokenSource _ctsComplexOpTask = new CancellationTokenSource();

        /// <summary>
        /// Synchonization primitive for pausing/resuming Countdown Timer
        /// To pause timer, call timerEvent.Reset()
        /// To resume timer, call timerEvent.Set()
        /// </summary>
        /// <param>can be true (resume thread) or false (pause thread)</param>
        static ManualResetEventSlim timerEvent = new ManualResetEventSlim(true); 

        public CountDownView(CancellationTokenSource cts)
        {
            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "-->CountDownView(cts) - " + this.GetType().ToString());

            InitializeComponent();
            this.DataContext = new CountDownViewModel();

            _ctsComplexOpTask = cts;
            timerEvent.Set();
            Task.Run(() => StartCountdown());

            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "<--CountDownView(cts) - " + this.GetType().ToString());
        }

        public void UpdateCountDownValue(int countdownvalue)
        {
            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "-->UpdateCountDownValue(" + countdownvalue + ") --" + this.GetType().ToString());

            tbCountDownValue.Text = countdownvalue.ToString();

            if (countdownvalue == 0)
            {
                Log.Warning($"[{Environment.CurrentManagedThreadId}]\t Countdown value is zero, closing Countdown window ");

                //countdown has completed, close window
                this.Close();
            }
            else if (countdownvalue>0 && (TaskManager.BackgroundTask?.IsCanceled == true || TaskManager.BackgroundTask?.IsCompleted == true)) 
            {
                Log.Warning($"[{Environment.CurrentManagedThreadId}]\t The complex operation thread has already been cancelled/completed even if the timer is still on");
            }

            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "<--UpdateCountDownValue() " + this.GetType().ToString());
        }

        public async void StartCountdown()
        {
            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "-->StartCountdown() - " + this.GetType().ToString());

            try
            {
                for (int i = countdown; i >= 0; i--)
                {
                    timerEvent.Wait();  //wait here if paused
                    Dispatcher.Invoke(() => UpdateCountDownValue(i));

                    Log.Information($"[{Environment.CurrentManagedThreadId}]\t Countdown Value: " + i);
                    await Task.Delay(1000); //need to add a cancellation token?
                }
            }
            catch (Exception ex)
            {
                Log.Information("Exception Error: " + ex.Message);
            }

            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "<--StartCountdown() - " + this.GetType().ToString());
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "-->btnCancel_Click(sender, e) - " + this.GetType().ToString());

            btnCancel.IsEnabled = false;

            PauseTimer();            

            var cancelComplexOpView = new CancelComplexOpView();

            cancelComplexOpView.CancelRequested += (s, args) =>
            {
                try
                {
                    if (!(TaskManager.BackgroundTask?.Status == TaskStatus.Running))
                    {
                        Log.Error($"Complex Operation Thread execution has already been either completed or cancelled!");
                    }
                    else
                    {
                        _ctsComplexOpTask.Cancel(true);
                    }

                    Log.Information($"[{Environment.CurrentManagedThreadId}]\t Background Task Cancelled");
                    cancelComplexOpView.Close();
                }
                catch (Exception ex)
                {
                    Log.Error($"Complex Operation had already been cancelled or completed! Error Message: {ex.Message}");
                }
                finally
                {
                }

                this.Close();
            };

            cancelComplexOpView.CancelRequestCancelled += (s, args) =>
            {
                Log.Information($"[{Environment.CurrentManagedThreadId}]\t Cancellation Request for [Complex Operation Thread] cancelled");
                cancelComplexOpView.Close();
                ResumeTimer();
            };
            
            cancelComplexOpView.ShowDialog();
          
           btnCancel.IsEnabled = true;

           Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "<--btnCancel_Click(sender, e) - " + this.GetType().ToString());
        }

        public void PauseTimer()
        {
            Log.Information("[" + Environment.CurrentManagedThreadId + "] " +"--> PauseTimer() - " + this.GetType().ToString());

            try
            {
                timerEvent.Reset(); //pause timer

                Log.Information($"[{Environment.CurrentManagedThreadId}]\t Timer has been paused");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }


            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "<-- PauseTimer() - " + this.GetType().ToString());
        }

        public void ResumeTimer()
        {
            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "--> ResumeTimer() - " + this.GetType().ToString());

            timerEvent.Set(); //resume timer

            Log.Information($"[{Environment.CurrentManagedThreadId}]\t Timer has been resumed");

            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "--> ResumeTimer() - " + this.GetType().ToString());
        }
    }
}