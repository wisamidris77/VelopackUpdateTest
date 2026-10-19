using Velopack;

namespace VelopackUpdateTest;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        // VelopackApp must be called as early as possible in Main().
        // It handles install, update, and uninstall lifecycle hooks and fast callbacks.
        VelopackApp.Build()
            .OnFirstRun(v =>
            {
                // Hook triggered on the first launch after install
            })
            .OnRestarted(v =>
            {
                // Hook triggered when restarted by Velopack after updating
            })
            .Run();

        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}