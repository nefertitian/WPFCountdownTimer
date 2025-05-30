using MainlineScientific.InterviewAssignment.ViewModel;
using Serilog;
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
    /// Interaction logic for CancelComplexOpView.xaml
    /// </summary>
    public partial class CancelComplexOpView : Window
    {
        public event EventHandler? CancelRequested;

        public event EventHandler? CancelRequestCancelled;

        public CancelComplexOpView()
        {
            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "-->CancelComplexOpView() - " + this.GetType().ToString());

            InitializeComponent();
            this.DataContext = new CountDownViewModel();

            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "<--CancelComplexOpView() - " + this.GetType().ToString());
        }

        private void btnAbortCancellation_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "-->btnAbortCancellation_Click(sender, e) - " + this.GetType().ToString());

            Log.Information($"[{Environment.CurrentManagedThreadId}]\t Cancellation Request Aborted");

            CancelRequestCancelled?.Invoke(this, EventArgs.Empty);

            this.Close();

            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "<--btnAbortCancellation_Click(sender, e) - " + this.GetType().ToString());
        }

        private void btnConfirmCancellation_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "-->btnConfirmCancellation_Click(sender, e) - " + this.GetType().ToString());

            Log.Information($"[{Environment.CurrentManagedThreadId}]\t Cancelling Background Task");

            CancelRequested?.Invoke(this, EventArgs.Empty);

            Log.Information("[" + Environment.CurrentManagedThreadId + "] " + "<--btnConfirmCancellation_Click(sender, e) - " + this.GetType().ToString());
        }
    }
}
