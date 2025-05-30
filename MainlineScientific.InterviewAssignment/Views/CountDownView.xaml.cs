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

        //public event EventHandler PauseComplexOpThread; //use Observable object?
        //public event EventHandler ResumeComplexOpThread; //use Observable object instead?

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
            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "-->UpdateCountDownValue(" + countdownvalue + "cts) --" + this.GetType().ToString());

            tbCountDownValue.Text = countdownvalue.ToString();

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
                _ctsComplexOpTask.Cancel();
                Log.Information($"[{Environment.CurrentManagedThreadId}]\t Background Task Cancelled");
                cancelComplexOpView.Close();
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
                //first pause complexop thread
                //PauseComplexOpThread?.Invoke(this, EventArgs.Empty);

                //second pause timer displayed on CountDownView
                timerEvent.Reset();

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

            //resume timer displayed on CountDownView
            timerEvent.Set();

            Log.Information($"[{Environment.CurrentManagedThreadId}]\t Timer has been resumed");

            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "--> ResumeTimer() - " + this.GetType().ToString());
        }
    }
}