using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;
using CLI;
using Classes;

namespace wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Length == 0)
            {
                // No command line arguments, so run as normal Windows application.
                var mainWindow = new MainWindow();
                // open GUI/WPF window
                mainWindow.ShowDialog();
            }
            else
            {
                // Command-line arguments were supplied, so run in console mode.
                try
                {
                    const int ATTACH_PARENT_PROCESS = -1;
                    if (AttachConsole(ATTACH_PARENT_PROCESS))
                    {
                        // If the argument is equal to CLI, Open CLI window
                        if (e.Args[0] == "cli"){
                            CommandLineVersionOfApp.ConsoleMain(e.Args);
                        }
                        else {
                            // Return invalid argument error
                            Console.WriteLine($"{string.Join(" ", e.Args)} is not a valid argument");
                        }
                        
                    }
                }
                finally
                {
                    FreeConsole();
                    Shutdown();
                }
            }
        }

        [DllImport("kernel32")]
        private static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32")]
        private static extern bool FreeConsole();
    }

    internal class CommandLineVersionOfApp
    {
        internal static void ConsoleMain(string[] args)
        {   
            //Console.WriteLine(string.Join(", ", args));
            Cli.Start();
        }
    }
}
