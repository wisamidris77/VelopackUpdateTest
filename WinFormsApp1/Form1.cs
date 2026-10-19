using VelopackUpdateTest.Models;
using VelopackUpdateTest.Services;
using VelopackUpdateTest.UI;

namespace VelopackUpdateTest;

public partial class Form1 : Form
{
    private readonly VelopackUpdateService _updateService = VelopackUpdateService.Instance;
    private ReleaseInfo? _pendingRelease;
    private System.Windows.Forms.Timer? _periodicTimer;

    public Form1()
    {
        InitializeComponent();
        HookEvents();
        LoadInitialData();
    }

    private void HookEvents()
    {
        // Check for updates button
        _btnCheckUpdates.Click += async (s, e) => await CheckForUpdatesAsync();

        // Banner actions
        _btnBannerAction.Click += (s, e) =>
        {
            if (_pendingRelease != null)
            {
                LaunchUpdateScreen(_pendingRelease);
            }
        };
        _btnBannerDismiss.Click += (s, e) => _pnlBanner.Visible = false;

        // Log actions
        _btnClearLog.Click += (s, e) => _txtActivityLog.Clear();
        _btnCopyLog.Click += (s, e) =>
        {
            if (!string.IsNullOrEmpty(_txtActivityLog.Text))
            {
                Clipboard.SetText(_txtActivityLog.Text);
                _updateService.Log("APP", "Logs copied to clipboard.", Color.Empty);
            }
        };

        // Live Log binding
        _updateService.LogGenerated += (category, message, color) =>
        {
            if (InvokeRequired)
            {
                Invoke(() => AppendLog(category, message));
            }
            else
            {
                AppendLog(category, message);
            }
        };

        // Automatic background check on startup
        Shown += async (s, e) =>
        {
            if (_chkAutoCheckGitHub.Checked)
            {
                await Task.Delay(1000);
                await CheckForUpdatesAsync(isSilentStartup: true);
            }
        };

        // Periodic check timer (Every 5 minutes)
        _periodicTimer = new System.Windows.Forms.Timer { Interval = 1000 * 60 * 5 };
        _periodicTimer.Tick += async (s, e) =>
        {
            if (_chkAutoCheckGitHub.Checked)
            {
                await CheckForUpdatesAsync(isSilentStartup: true);
            }
        };
        _periodicTimer.Start();
    }

    private void LoadInitialData()
    {
        _lblDiagVersion.Text = _updateService.CurrentVersion;
        _lblDiagInstalled.Text = _updateService.IsInstalled ? "Installed (Velopack Engine Active)" : "Development / Uninstalled";
        _lblDiagChannel.Text = _updateService.Channel;
        _lblDiagLastCheck.Text = "Not yet checked";

        DisplayCurrentVersionInfo();

        _updateService.Log("STARTUP", $"Application running. Current version: {_updateService.CurrentVersion}", Color.Empty);
    }

    private void DisplayCurrentVersionInfo()
    {
        _lblChangelogTitle.Text = $"Current Application Version: v{_updateService.CurrentVersion}";
        _lblChangelogMeta.Text = $"Channel: {_updateService.Channel} • No pending updates.";

        _txtChangelogView.Text =
            $"VelopackUpdateTest (Version {_updateService.CurrentVersion})" + Environment.NewLine +
            "The application is synchronized with GitHub Releases." + Environment.NewLine + Environment.NewLine +
            "Update Rules:" + Environment.NewLine +
            "1. 1-Version Apart: Downloads a lightweight delta patch (~500 KB) after checking local file SHA-256 integrity." + Environment.NewLine +
            "2. Greater than 1 version apart: Enforces full standalone installer download (~50 MB)." + Environment.NewLine +
            "3. If any local file is modified or corrupted, automatically falls back to full installer.";
    }

    private void DisplayReleaseChangelog(ReleaseInfo release)
    {
        _lblChangelogTitle.Text = release.Title;
        _lblChangelogMeta.Text = $"Version: v{release.Version} | Size: {release.SizeDisplay} | Released: {release.ReleaseDate:yyyy-MM-dd}";

        _txtChangelogView.Text =
            $"{release.Title}" + Environment.NewLine +
            $"Target Version: v{release.Version}" + Environment.NewLine +
            $"Download Type: {(release.IsDelta ? "Fast Delta Update (~500 KB)" : "Full Standalone Installer (~50 MB)")}" + Environment.NewLine +
            new string('-', 60) + Environment.NewLine + Environment.NewLine +
            release.Summary;
    }

    private async Task CheckForUpdatesAsync(bool isSilentStartup = false)
    {
        try
        {
            _btnCheckUpdates.Enabled = false;

            string source = _txtRepoUrl.Text.Trim();
            var update = await _updateService.CheckForUpdatesAsync(source);

            _lblDiagLastCheck.Text = DateTime.Now.ToString("HH:mm:ss");

            if (update != null)
            {
                _pendingRelease = update;
                DisplayReleaseChangelog(update);

                _pnlBanner.Visible = true;
                _lblBannerText.Text = $"Update available: {update.Title} ({update.SizeDisplay})";

                if (update.IsMandatory)
                {
                    _lblBannerText.Text = $"REQUIRED UPDATE: {update.Title} must be installed.";
                    _btnBannerDismiss.Visible = false;
                    LaunchUpdateScreen(update);
                }
                else if (!isSilentStartup)
                {
                    LaunchUpdateScreen(update);
                }
            }
            else
            {
                _pnlBanner.Visible = false;
                DisplayCurrentVersionInfo();

                if (!isSilentStartup)
                {
                    MessageBox.Show(
                        this,
                        $"VelopackUpdateTest is up to date (v{_updateService.CurrentVersion}).",
                        "Update Check",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }
        catch (Exception ex)
        {
            if (!isSilentStartup)
            {
                MessageBox.Show(
                    this,
                    $"Failed to check update feed: {ex.Message}",
                    "Update Check Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        finally
        {
            _btnCheckUpdates.Enabled = true;
        }
    }

    private void LaunchUpdateScreen(ReleaseInfo targetRelease)
    {
        using var dialog = new UpdateDialogForm(targetRelease);
        dialog.ShowDialog(this);
    }

    private void AppendLog(string category, string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        string line = $"[{timestamp}] [{category}] {message}" + Environment.NewLine;
        _txtActivityLog.AppendText(line);
    }
}
