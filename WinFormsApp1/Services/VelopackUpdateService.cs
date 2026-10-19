using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using Velopack;
using Velopack.Sources;
using VelopackUpdateTest.Models;

namespace VelopackUpdateTest.Services;

public class VelopackUpdateService
{
    private static VelopackUpdateService? _instance;
    public static VelopackUpdateService Instance => _instance ??= new VelopackUpdateService();

    public event Action<string, string, Color>? LogGenerated;

    private UpdateManager? _updateManager;
    private UpdateInfo? _lastRealUpdateInfo;

    public bool IsInstalled { get; private set; }
    public string AppId { get; private set; } = "VelopackUpdateTest";
    public string CurrentVersion { get; private set; } = "1.0.0";
    public string RootAppDir { get; private set; } = string.Empty;
    public string UpdateExePath { get; private set; } = "N/A (Dev Mode)";
    public string Channel { get; private set; } = "stable";
    public bool IsUpdatePendingRestart => _updateManager?.UpdatePendingRestart != null;

    public VelopackUpdateService()
    {
        InitializeLocatorInfo();
    }

    private void InitializeLocatorInfo()
    {
        try
        {
            var asmVersion = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = asmVersion != null ? $"{asmVersion.Major}.{asmVersion.Minor}.{asmVersion.Build}" : "1.0.0";
            RootAppDir = AppDomain.CurrentDomain.BaseDirectory;

            // Probe Velopack UpdateManager safely
            _updateManager = new UpdateManager("https://github.com/placeholder/repo");
            IsInstalled = _updateManager.IsInstalled;
            AppId = _updateManager.AppId ?? "VelopackUpdateTest";

            var locator = Velopack.Locators.VelopackLocator.Current;
            if (locator != null)
            {
                Channel = locator.Channel ?? "stable";
                RootAppDir = locator.RootAppDir ?? AppDomain.CurrentDomain.BaseDirectory;
            }

            if (IsInstalled)
            {
                if (_updateManager.CurrentVersion != null)
                {
                    CurrentVersion = _updateManager.CurrentVersion.ToString();
                }
                UpdateExePath = locator?.UpdateExePath ?? "Velopack Locator Active";
                Log("INIT", $"Velopack active in INSTALLED mode. Version: {CurrentVersion}, AppId: {AppId}", Color.FromArgb(16, 185, 129));
            }
            else
            {
                UpdateExePath = "Running uninstalled (bin/Debug - Simulation Ready)";
                Log("INIT", $"Running in Development/Debug Mode. Real Velopack updates require installed package; Simulation Mode is fully active.", Color.FromArgb(245, 158, 11));
            }
        }
        catch (Exception ex)
        {
            IsInstalled = false;
            Log("INIT_WARN", $"Velopack initial detection note: {ex.Message}", Color.FromArgb(148, 163, 184));
        }
    }

    public void Log(string category, string message, Color color)
    {
        LogGenerated?.Invoke(category, message, color);
    }

    public async Task<ReleaseInfo?> CheckForUpdatesAsync(
        string? gitHubRepoUrl,
        bool simulationMode,
        ReleaseInfo? simulatedRelease,
        CancellationToken ct = default)
    {
        Log("UPDATE_CHECK", $"Initiating update check (Mode: {(simulationMode ? "Simulation" : "Velopack Live API")})...", Color.FromArgb(99, 102, 241));

        if (simulationMode || !IsInstalled)
        {
            if (!simulationMode && !IsInstalled)
            {
                Log("INFO", "Velopack is not running in an installed package directory. Automatically using Simulation Engine.", Color.FromArgb(245, 158, 11));
            }

            // Simulate network latency
            Log("SIMULATION", "Contacting remote release metadata server...", Color.FromArgb(148, 163, 184));
            await Task.Delay(800, ct);

            var target = simulatedRelease ?? ReleaseInfo.ReleaseV110;
            Log("SIMULATION", $"Update discovered! Version {target.Version} - {target.Title} (Mandatory: {target.IsMandatory})", Color.FromArgb(16, 185, 129));
            return target;
        }

        // Real Velopack Live API Check
        try
        {
            if (string.IsNullOrWhiteSpace(gitHubRepoUrl))
            {
                gitHubRepoUrl = "https://github.com/wisamidris77/VelopackUpdateTest";
            }

            Log("VELOPACK", $"Querying GitHub Releases feed: {gitHubRepoUrl}", Color.FromArgb(99, 102, 241));
            var source = new GithubSource(gitHubRepoUrl, null, false);
            _updateManager = new UpdateManager(source);

            _lastRealUpdateInfo = await _updateManager.CheckForUpdatesAsync();

            if (_lastRealUpdateInfo == null)
            {
                Log("VELOPACK", "Application is already on the latest version. No updates pending.", Color.FromArgb(16, 185, 129));
                return null;
            }

            var targetAsset = _lastRealUpdateInfo.TargetFullRelease;
            var targetVersion = targetAsset.Version.ToString();
            var sizeMb = (targetAsset.Size / (1024.0 * 1024.0)).ToString("0.0") + " MB";

            Log("VELOPACK", $"Found remote update: v{targetVersion} ({sizeMb})", Color.FromArgb(16, 185, 129));

            return new ReleaseInfo
            {
                Version = targetVersion,
                Title = $"Velopack Release v{targetVersion}",
                ReleaseDate = DateTime.Now,
                Summary = targetAsset.NotesMarkdown ?? "Velopack package update available from GitHub Releases.",
                SizeDisplay = sizeMb,
                IsMandatory = false,
                Features = [$"Downloaded package: {targetAsset.FileName}", $"SHA256: {targetAsset.SHA256}"]
            };
        }
        catch (Exception ex)
        {
            Log("ERROR", $"Failed to check live Velopack updates: {ex.Message}", Color.FromArgb(239, 68, 68));
            throw;
        }
    }

    public async Task<bool> DownloadUpdateAsync(
        ReleaseInfo target,
        bool simulationMode,
        bool simulateFailure,
        Action<int, string, double> progressReport,
        CancellationToken ct = default)
    {
        Log("DOWNLOAD", $"Starting download for version {target.Version} ({target.SizeDisplay})...", Color.FromArgb(99, 102, 241));

        if (simulationMode || !IsInstalled)
        {
            return await SimulateDownloadAsync(target, simulateFailure, progressReport, ct);
        }

        try
        {
            if (_updateManager == null || _lastRealUpdateInfo == null)
            {
                throw new InvalidOperationException("No update info cached. Call CheckForUpdatesAsync first.");
            }

            var sw = Stopwatch.StartNew();
            long totalBytes = _lastRealUpdateInfo.TargetFullRelease.Size;

            await _updateManager.DownloadUpdatesAsync(_lastRealUpdateInfo, p =>
            {
                double elapsedSec = Math.Max(0.1, sw.Elapsed.TotalSeconds);
                double downloadedBytes = totalBytes * (p / 100.0);
                double speedMBs = (downloadedBytes / (1024.0 * 1024.0)) / elapsedSec;

                progressReport(p, $"Downloading update payload ({p}%)...", Math.Round(speedMBs, 1));
            }, ct);

            Log("DOWNLOAD", "Update download and delta verification completed successfully!", Color.FromArgb(16, 185, 129));
            return true;
        }
        catch (Exception ex)
        {
            Log("DOWNLOAD_ERROR", $"Real download failed: {ex.Message}", Color.FromArgb(239, 68, 68));
            throw;
        }
    }

    private async Task<bool> SimulateDownloadAsync(
        ReleaseInfo target,
        bool simulateFailure,
        Action<int, string, double> progressReport,
        CancellationToken ct)
    {
        var random = new Random();
        int progress = 0;

        progressReport(0, "Initiating secure TLS connection...", 0);
        await Task.Delay(300, ct);

        progressReport(5, "Resolving CDN distribution mirror...", 12.4);
        await Task.Delay(300, ct);

        while (progress < 90)
        {
            ct.ThrowIfCancellationRequested();

            int step = random.Next(4, 9);
            progress = Math.Min(90, progress + step);

            double speed = Math.Round(14.0 + (random.NextDouble() * 11.0), 1);
            string phase = progress < 40 
                ? $"Downloading base payload chunks ({progress}%)..." 
                : $"Applying delta package diffs ({progress}%)...";

            progressReport(progress, phase, speed);
            Log("PROGRESS", $"{phase} @ {speed} MB/s", Color.FromArgb(148, 163, 184));

            if (simulateFailure && progress >= 50)
            {
                await Task.Delay(400, ct);
                Log("SIM_ERROR", "Simulated Network Drop: Connection reset by peer at 52%!", Color.FromArgb(239, 68, 68));
                throw new HttpRequestException("Simulated connection timeout: Failed to fetch delta chunk #14");
            }

            await Task.Delay(random.Next(160, 260), ct);
        }

        progressReport(94, "Verifying package cryptographic checksum (SHA-256)...", 28.5);
        await Task.Delay(400, ct);

        progressReport(98, "Staging binaries for seamless update swap...", 32.1);
        await Task.Delay(350, ct);

        progressReport(100, "Update ready to install!", 0);
        Log("SUCCESS", $"Package v{target.Version} successfully downloaded and staged for restart.", Color.FromArgb(16, 185, 129));
        return true;
    }

    public void RestartAndApply(bool simulationMode)
    {
        Log("APPLY", "Applying updates and initiating restart sequence...", Color.FromArgb(16, 185, 129));

        if (!simulationMode && IsInstalled && _updateManager != null)
        {
            _updateManager.ApplyUpdatesAndRestart(_lastRealUpdateInfo?.TargetFullRelease, null);
            return;
        }

        // In simulation mode: restart the current executable to simulate the seamless restart experience!
        var processPath = Environment.ProcessPath;
        if (!string.IsNullOrEmpty(processPath))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = processPath,
                UseShellExecute = true
            });
        }
        Environment.Exit(0);
    }
}
