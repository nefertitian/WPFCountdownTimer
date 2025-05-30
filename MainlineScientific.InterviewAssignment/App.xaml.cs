using Serilog;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;

namespace MainlineScientific.InterviewAssignment
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Application Starting Up");

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Application Exiting");
            Log.CloseAndFlush();
            base.OnExit(e);
            FreeConsole();
        }

    }

}
