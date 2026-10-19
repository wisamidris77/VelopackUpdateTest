using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text.Json;
using Velopack;
using Velopack.Sources;
using VelopackUpdateTest.Models;

namespace VelopackUpdateTest.Services;

public class VelopackUpdateService
{
    private static VelopackUpdateService? _instance;
    public static VelopackUpdateService Instance => _instance ??= new VelopackUpdateService();

    private static readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(15),
        DefaultRequestHeaders = { { "User-Agent", "VelopackUpdateTest-Client" } }
    };

    public event Action<string, string, Color>? LogGenerated;

    private UpdateManager? _updateManager;
    private UpdateInfo? _lastRealUpdateInfo;
    private ReleaseInfo? _lastDownloadedRelease;

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
            _updateManager = new UpdateManager("https://github.com/wisamidris77/VelopackUpdateTest");
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
                UpdateExePath = "Running uninstalled (bin/Debug - Direct GitHub Release Hook Active)";
                Log("INIT", $"Development/Debug Mode active. Direct GitHub Release API & Simulation sandbox enabled.", Color.FromArgb(245, 158, 11));
            }
        }
        catch (Exception ex)
        {
            IsInstalled = false;
            Log("INIT_WARN", $"Velopack detection note: {ex.Message}", Color.FromArgb(148, 163, 184));
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
        if (string.IsNullOrWhiteSpace(gitHubRepoUrl))
        {
            gitHubRepoUrl = "https://github.com/wisamidris77/VelopackUpdateTest";
        }

        if (simulationMode)
        {
            Log("SIMULATION", "Checking simulation feed...", Color.FromArgb(99, 102, 241));
            await Task.Delay(600, ct);
            var simTarget = simulatedRelease ?? ReleaseInfo.ReleaseV110;
            Log("SIMULATION", $"Update ready: v{simTarget.Version} - {simTarget.Title}", Color.FromArgb(16, 185, 129));
            return simTarget;
        }

        Log("UPDATE_CHECK", $"Checking real GitHub Releases feed: {gitHubRepoUrl}...", Color.FromArgb(99, 102, 241));

        // 1. If running as an installed package, use native Velopack UpdateManager
        if (IsInstalled)
        {
            try
            {
                var source = new GithubSource(gitHubRepoUrl, null, false);
                _updateManager = new UpdateManager(source);
                _lastRealUpdateInfo = await _updateManager.CheckForUpdatesAsync();

                if (_lastRealUpdateInfo == null)
                {
                    Log("VELOPACK", "App is on latest version according to Velopack feed.", Color.FromArgb(16, 185, 129));
                    return null;
                }

                var targetAsset = _lastRealUpdateInfo.TargetFullRelease;
                var targetVersion = targetAsset.Version.ToString();
                var sizeMb = (targetAsset.Size / (1024.0 * 1024.0)).ToString("0.0") + " MB";

                Log("VELOPACK", $"Velopack update available: v{targetVersion} ({sizeMb})", Color.FromArgb(16, 185, 129));

                return new ReleaseInfo
                {
                    Version = targetVersion,
                    Title = $"Velopack Release v{targetVersion}",
                    ReleaseDate = DateTime.Now,
                    Summary = targetAsset.NotesMarkdown ?? "Velopack package update ready to apply.",
                    SizeDisplay = sizeMb,
                    IsMandatory = false,
                    Features = [$"Package file: {targetAsset.FileName}"]
                };
            }
            catch (Exception ex)
            {
                Log("WARN", $"Velopack package check note: {ex.Message}. Falling back to GitHub Releases API...", Color.FromArgb(245, 158, 11));
            }
        }

        // 2. Query GitHub Releases API directly (works in both debug and installed mode!)
        try
        {
            string ownerRepo = ExtractOwnerRepo(gitHubRepoUrl);
            string apiUrl = $"https://api.github.com/repos/{ownerRepo}/releases";
            Log("API", $"Querying GitHub Releases API: {apiUrl}", Color.FromArgb(148, 163, 184));

            using var response = await _httpClient.GetAsync(apiUrl, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
            {
                Log("API", "No releases published yet on GitHub repository.", Color.FromArgb(245, 158, 11));
                return null;
            }

            // Inspect the latest non-draft release
            foreach (var releaseElem in doc.RootElement.EnumerateArray())
            {
                bool draft = releaseElem.TryGetProperty("draft", out var d) && d.GetBoolean();
                if (draft) continue;

                string rawTag = releaseElem.GetProperty("tag_name").GetString() ?? "";
                string cleanVer = rawTag.TrimStart('v');
                string title = releaseElem.TryGetProperty("name", out var n) ? (n.GetString() ?? cleanVer) : cleanVer;
                string body = releaseElem.TryGetProperty("body", out var b) ? (b.GetString() ?? "") : "";
                DateTime pubDate = releaseElem.TryGetProperty("published_at", out var pDate) && pDate.TryGetDateTime(out var dt) ? dt : DateTime.Now;

                // Compare version with current
                if (IsNewerVersion(cleanVer, CurrentVersion))
                {
                    Log("API", $"Discovered newer version on GitHub: {rawTag} (Current: {CurrentVersion})", Color.FromArgb(16, 185, 129));

                    bool isMandatory = title.Contains("MANDATORY", StringComparison.OrdinalIgnoreCase) ||
                                       title.Contains("CRITICAL", StringComparison.OrdinalIgnoreCase) ||
                                       body.Contains("MANDATORY", StringComparison.OrdinalIgnoreCase);

                    string sizeDisplay = "24.5 MB";
                    string? downloadUrl = null;

                    if (releaseElem.TryGetProperty("assets", out var assetsElem) && assetsElem.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var asset in assetsElem.EnumerateArray())
                        {
                            string assetName = asset.GetProperty("name").GetString() ?? "";
                            if (assetName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                            {
                                downloadUrl = asset.GetProperty("browser_download_url").GetString();
                                if (asset.TryGetProperty("size", out var sBytes))
                                {
                                    sizeDisplay = (sBytes.GetInt64() / (1024.0 * 1024.0)).ToString("0.0") + " MB";
                                }
                                break;
                            }
                        }
                    }

                    var lines = body.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
                    var features = lines.Where(l => l.TrimStart().StartsWith('-') || l.TrimStart().StartsWith('*'))
                                        .Select(l => l.Trim().TrimStart('-', '*', ' '))
                                        .ToList();

                    if (features.Count == 0 && !string.IsNullOrWhiteSpace(body))
                    {
                        features.Add(body.Trim());
                    }

                    return new ReleaseInfo
                    {
                        Version = cleanVer,
                        Title = string.IsNullOrWhiteSpace(title) ? $"Release {rawTag}" : title,
                        ReleaseDate = pubDate,
                        Summary = string.IsNullOrWhiteSpace(body) ? "New release published on GitHub." : body,
                        SizeDisplay = sizeDisplay,
                        IsMandatory = isMandatory,
                        MandatoryReason = isMandatory ? "Critical update published on GitHub repository." : null,
                        DownloadUrl = downloadUrl,
                        Features = features
                    };
                }
                else
                {
                    Log("API", $"Latest release on GitHub is {rawTag}. Current app is up-to-date ({CurrentVersion}).", Color.FromArgb(16, 185, 129));
                    return null;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Log("ERROR", $"GitHub API check error: {ex.Message}", Color.FromArgb(239, 68, 68));
            return null;
        }
    }

    private static string ExtractOwnerRepo(string url)
    {
        var trimmed = url.Trim().TrimEnd('/');
        var parts = trimmed.Split('/');
        if (parts.Length >= 2)
        {
            return $"{parts[^2]}/{parts[^1]}";
        }
        return "wisamidris77/VelopackUpdateTest";
    }

    private static bool IsNewerVersion(string remote, string current)
    {
        if (Version.TryParse(remote, out var vRemote) && Version.TryParse(current, out var vCurrent))
        {
            return vRemote > vCurrent;
        }
        return string.Compare(remote, current, StringComparison.OrdinalIgnoreCase) > 0;
    }

    public async Task<bool> DownloadUpdateAsync(
        ReleaseInfo target,
        bool simulationMode,
        bool simulateFailure,
        Action<int, string, double> progressReport,
        CancellationToken ct = default)
    {
        _lastDownloadedRelease = target;
        Log("DOWNLOAD", $"Downloading v{target.Version} ({target.SizeDisplay})...", Color.FromArgb(99, 102, 241));

        // Real download from GitHub Releases if available and not installed
        if (!simulationMode && !string.IsNullOrEmpty(target.DownloadUrl))
        {
            try
            {
                Log("DOWNLOAD_LIVE", $"Downloading live installer from GitHub: {target.DownloadUrl}", Color.FromArgb(99, 102, 241));
                return await DownloadHttpFileAsync(target, progressReport, ct);
            }
            catch (Exception ex)
            {
                Log("WARN", $"Live download error: {ex.Message}. Falling back to simulation.", Color.FromArgb(245, 158, 11));
            }
        }

        if (IsInstalled && _lastRealUpdateInfo != null)
        {
            return await DownloadVelopackNativeAsync(progressReport, ct);
        }

        return await SimulateDownloadAsync(target, simulateFailure, progressReport, ct);
    }

    private async Task<bool> DownloadHttpFileAsync(
        ReleaseInfo target,
        Action<int, string, double> progressReport,
        CancellationToken ct)
    {
        using var response = await _httpClient.GetAsync(target.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        long? totalBytes = response.Content.Headers.ContentLength;
        string tempDir = Path.Combine(Path.GetTempPath(), "VelopackUpdateTest");
        Directory.CreateDirectory(tempDir);
        string tempFile = Path.Combine(tempDir, $"VelopackUpdateTest-Setup-{target.Version}.exe");

        await using var contentStream = await response.Content.ReadAsStreamAsync(ct);
        await using var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

        var buffer = new byte[8192];
        long totalRead = 0;
        int bytesRead;
        var sw = Stopwatch.StartNew();

        while ((bytesRead = await contentStream.ReadAsync(buffer, ct)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
            totalRead += bytesRead;

            if (totalBytes.HasValue && totalBytes.Value > 0)
            {
                int percent = (int)((totalRead * 100) / totalBytes.Value);
                double elapsed = Math.Max(0.1, sw.Elapsed.TotalSeconds);
                double speedMBs = (totalRead / (1024.0 * 1024.0)) / elapsed;
                progressReport(percent, $"Downloading GitHub Release Asset ({percent}%)...", Math.Round(speedMBs, 1));
            }
        }

        target.InstallerPath = tempFile;
        progressReport(100, "Download complete! Ready to launch setup.", 0);
        Log("SUCCESS", $"Installer saved to: {tempFile}", Color.FromArgb(16, 185, 129));
        return true;
    }

    private async Task<bool> DownloadVelopackNativeAsync(
        Action<int, string, double> progressReport,
        CancellationToken ct)
    {
        if (_updateManager == null || _lastRealUpdateInfo == null) return false;

        var sw = Stopwatch.StartNew();
        long totalBytes = _lastRealUpdateInfo.TargetFullRelease.Size;

        await _updateManager.DownloadUpdatesAsync(_lastRealUpdateInfo, p =>
        {
            double elapsedSec = Math.Max(0.1, sw.Elapsed.TotalSeconds);
            double downloadedBytes = totalBytes * (p / 100.0);
            double speedMBs = (downloadedBytes / (1024.0 * 1024.0)) / elapsedSec;
            progressReport(p, $"Applying delta package ({p}%)...", Math.Round(speedMBs, 1));
        }, ct);

        Log("DOWNLOAD", "Velopack delta packages applied successfully!", Color.FromArgb(16, 185, 129));
        return true;
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
        await Task.Delay(250, ct);

        while (progress < 90)
        {
            ct.ThrowIfCancellationRequested();
            int step = random.Next(5, 10);
            progress = Math.Min(90, progress + step);

            double speed = Math.Round(14.0 + (random.NextDouble() * 11.0), 1);
            string phase = progress < 45 
                ? $"Downloading update payload ({progress}%)..." 
                : $"Unpacking delta diffs ({progress}%)...";

            progressReport(progress, phase, speed);

            if (simulateFailure && progress >= 50)
            {
                await Task.Delay(300, ct);
                Log("SIM_ERROR", "Simulated Network Drop: Connection reset at 52%!", Color.FromArgb(239, 68, 68));
                throw new HttpRequestException("Simulated connection timeout: Failed to fetch delta chunk");
            }

            await Task.Delay(random.Next(140, 240), ct);
        }

        progressReport(95, "Verifying package cryptographic checksum (SHA-256)...", 28.5);
        await Task.Delay(350, ct);

        progressReport(100, "Update ready to install!", 0);
        Log("SUCCESS", $"Package v{target.Version} staged and ready.", Color.FromArgb(16, 185, 129));
        return true;
    }

    public void RestartAndApply(bool simulationMode)
    {
        Log("APPLY", "Initiating restart sequence...", Color.FromArgb(16, 185, 129));

        // If a real installer was downloaded from GitHub Releases
        if (_lastDownloadedRelease?.InstallerPath != null && File.Exists(_lastDownloadedRelease.InstallerPath))
        {
            Log("LAUNCH_SETUP", $"Executing installer: {_lastDownloadedRelease.InstallerPath}", Color.FromArgb(16, 185, 129));
            Process.Start(new ProcessStartInfo
            {
                FileName = _lastDownloadedRelease.InstallerPath,
                UseShellExecute = true
            });
            Environment.Exit(0);
            return;
        }

        // Native Velopack apply
        if (!simulationMode && IsInstalled && _updateManager != null)
        {
            _updateManager.ApplyUpdatesAndRestart(_lastRealUpdateInfo?.TargetFullRelease, null);
            return;
        }

        // Simulated restart: relaunch current executable
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
