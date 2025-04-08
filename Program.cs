using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace DDOOCP_Assignment
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => HandleFatalError(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                HandleFatalError(e.ExceptionObject as Exception);

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                using (var LoginForm = new LoginForm(""))
                {
                    Application.Run(new LoginForm(""));
                }
            }
            catch (Exception ex)
            {
                HandleFatalError(ex);
            }
            finally
            {
                Application.Exit();
                Environment.Exit(0);
            }
        }

        private static void HandleFatalError(Exception ex)
        {
            Debug.WriteLine($"Fatal error: {ex}");

            MessageBox.Show(
                "A critical error occurred. The application will now close.\n\n" +
                $"Error: {ex.Message}",
                "Fatal Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            Environment.Exit(1);
        }
    }
}