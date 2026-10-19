using VelopackUpdateTest.Models;
using VelopackUpdateTest.Services;
using VelopackUpdateTest.UI;
using VelopackUpdateTest.UI.Controls;

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
        // Top Nav Check
        _btnQuickCheck.Click += async (s, e) => await CheckForUpdatesAsync();

        // Policies Card Manual Check
        _btnManualCheck.Click += async (s, e) => await CheckForUpdatesAsync();

        // Top Banner Actions
        _btnBannerAction.Click += (s, e) =>
        {
            if (_pendingRelease != null)
            {
                LaunchUpdateScreen(_pendingRelease);
            }
        };
        _btnBannerDismiss.Click += (s, e) => _pnlBanner.Visible = false;

        // Activity Console Actions
        _btnClearLog.Click += (s, e) => _rtbActivityLog.Clear();
        _btnCopyLog.Click += (s, e) =>
        {
            if (!string.IsNullOrEmpty(_rtbActivityLog.Text))
            {
                Clipboard.SetText(_rtbActivityLog.Text);
                _updateService.Log("CONSOLE", "Logs copied to clipboard.", ModernTheme.Success);
            }
        };

        // Live Log Event Binding
        _updateService.LogGenerated += (category, message, color) =>
        {
            if (InvokeRequired)
            {
                Invoke(() => AppendLog(category, message, color));
            }
            else
            {
                AppendLog(category, message, color);
            }
        };

        // Automatic Background Check on Startup
        Shown += async (s, e) =>
        {
            if (_chkAutoCheckGitHub.Checked)
            {
                await Task.Delay(1200);
                await CheckForUpdatesAsync(isSilentStartup: true);
            }
        };

        // Periodic Background Check Timer (Every 5 minutes)
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
        _lblDiagVersion.Text = $"v{_updateService.CurrentVersion}";
        _lblDiagAppId.Text = _updateService.AppId;
        _lblDiagInstalled.Text = _updateService.IsInstalled ? "Yes (Velopack Engine Active)" : "No (Development / Uninstalled)";
        _lblDiagInstalled.ForeColor = _updateService.IsInstalled ? ModernTheme.Success : ModernTheme.Warning;
        _lblDiagChannel.Text = _updateService.Channel;
        _lblDiagLastCheck.Text = "Not yet checked";
        _badgeCurrentVersion.BadgeText = $"v{_updateService.CurrentVersion}";
        _badgeStatusMode.BadgeText = _updateService.IsInstalled ? "Velopack Active" : "Direct GitHub Feed";
        _badgeStatusMode.BadgeColor = _updateService.IsInstalled ? ModernTheme.Success : ModernTheme.Primary;

        // Initial Changelog Card placeholder
        DisplayCurrentVersionInfo();

        _updateService.Log("STARTUP", "VelopackUpdateTest application running in production mode.", ModernTheme.Success);
        _updateService.Log("DIAGNOSTICS", $"Version: {_updateService.CurrentVersion} | AppId: {_updateService.AppId} | Channel: {_updateService.Channel}", ModernTheme.Primary);
    }

    private void DisplayCurrentVersionInfo()
    {
        _lblChangelogTitle.Text = $"CURRENT APPLICATION VERSION: v{_updateService.CurrentVersion}";
        _lblChangelogMeta.Text = $"Assembly Version: {_updateService.CurrentVersion} • Release Channel: {_updateService.Channel}";

        _rtbChangelogView.Clear();
        AppendChangelogLine($"🚀 VelopackUpdateTest (Version {_updateService.CurrentVersion})\n\n", Color.FromArgb(168, 85, 247), true, 11f);
        AppendChangelogLine("The application is initialized and synchronized with your GitHub Releases repository.\n\n", ModernTheme.TextPrimary, false, 9.5f);
        AppendChangelogLine("ACTIVE POLICIES:\n", ModernTheme.Success, true, 9.5f);
        AppendChangelogLine("  • 1-Version Apart: Tiny Delta update (~500 KB) after pre-flight local SHA-256 integrity verification.\n", ModernTheme.TextSecondary, false, 9f);
        AppendChangelogLine("  • Greater than 1 version apart: Automatically enforces clean full standalone package.\n", ModernTheme.TextSecondary, false, 9f);
        AppendChangelogLine("  • Automatic GitHub Release trigger: Publishing a release on GitHub automatically builds, packs, and attaches Velopack assets.\n", ModernTheme.TextSecondary, false, 9f);
    }

    private void DisplayReleaseChangelog(ReleaseInfo release)
    {
        // Use the exact GitHub Release Name for the title!
        _lblChangelogTitle.Text = release.Title;
        _lblChangelogMeta.Text = $"Target Version: v{release.Version} • Published: {release.ReleaseDate:yyyy-MM-dd} • Size: {release.SizeDisplay}";

        _rtbChangelogView.Clear();
        AppendChangelogLine($"🚀 {release.Title}\n\n", Color.FromArgb(168, 85, 247), true, 11f);

        // Display the exact GitHub release description (body)
        AppendChangelogLine($"{release.Summary}\n\n", ModernTheme.TextPrimary, false, 9.5f);

        if (release.IsMandatory)
        {
            AppendChangelogLine("⚠️ MANDATORY POLICY ENFORCEMENT:\n", ModernTheme.Danger, true, 9.5f);
            AppendChangelogLine($"  {release.MandatoryReason}\n\n", Color.FromArgb(254, 202, 202), false, 9f);
        }

        if (release.Features.Count > 0)
        {
            AppendChangelogLine("HIGHLIGHTS & ASSETS:\n", ModernTheme.Success, true, 9.5f);
            foreach (var f in release.Features)
            {
                AppendChangelogLine($"  • {f}\n", ModernTheme.TextSecondary, false, 9f);
            }
        }
    }

    private async Task CheckForUpdatesAsync(bool isSilentStartup = false)
    {
        try
        {
            _btnQuickCheck.Enabled = false;
            _btnManualCheck.Enabled = false;

            string source = _txtRepoUrl.Text.Trim();
            var update = await _updateService.CheckForUpdatesAsync(source);

            _lblDiagLastCheck.Text = DateTime.Now.ToString("HH:mm:ss");

            if (update != null)
            {
                _pendingRelease = update;
                DisplayReleaseChangelog(update);

                _pnlBanner.Visible = true;
                if (update.IsMandatory)
                {
                    _pnlBanner.BackColor = ModernTheme.DangerBg;
                    _lblBannerText.Text = $"🚨 CRITICAL UPDATE REQUIRED: {update.Title} ({update.SizeDisplay})";
                    _btnBannerAction.Text = "Install Required Update";
                    _btnBannerAction.Style = ButtonStyle.Danger;
                    _btnBannerDismiss.Visible = false;

                    // Automatically pop up mandatory update modal
                    LaunchUpdateScreen(update);
                }
                else
                {
                    _pnlBanner.BackColor = ModernTheme.CardElevated;
                    _lblBannerText.Text = $"🎉 New Update Available: {update.Title} ({update.SizeDisplay})";
                    _btnBannerAction.Text = "View & Update";
                    _btnBannerAction.Style = ButtonStyle.Primary;
                    _btnBannerDismiss.Visible = true;

                    if (!isSilentStartup)
                    {
                        LaunchUpdateScreen(update);
                    }
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
                        $"VelopackUpdateTest is completely up to date (v{_updateService.CurrentVersion})!\nNo new releases were found in the feed.",
                        "Up to Date",
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
                    "Check Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        finally
        {
            _btnQuickCheck.Enabled = true;
            _btnManualCheck.Enabled = true;
        }
    }

    private void LaunchUpdateScreen(ReleaseInfo targetRelease)
    {
        _updateService.Log("MODAL", $"Opening update dialog for {targetRelease.Title} (Mandatory: {targetRelease.IsMandatory})", ModernTheme.Primary);

        using var dialog = new UpdateDialogForm(targetRelease);
        dialog.ShowDialog(this);
    }

    private void AppendChangelogLine(string text, Color color, bool bold, float size)
    {
        _rtbChangelogView.SelectionStart = _rtbChangelogView.TextLength;
        _rtbChangelogView.SelectionLength = 0;
        _rtbChangelogView.SelectionColor = color;
        _rtbChangelogView.SelectionFont = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular);
        _rtbChangelogView.AppendText(text);
        _rtbChangelogView.SelectionColor = _rtbChangelogView.ForeColor;
    }

    private void AppendLog(string category, string message, Color color)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");

        _rtbActivityLog.SelectionStart = _rtbActivityLog.TextLength;
        _rtbActivityLog.SelectionLength = 0;
        _rtbActivityLog.SelectionColor = ModernTheme.TextMuted;
        _rtbActivityLog.AppendText($"[{timestamp}] ");

        _rtbActivityLog.SelectionColor = color;
        _rtbActivityLog.AppendText($"[{category}] ");

        _rtbActivityLog.SelectionColor = ModernTheme.TextPrimary;
        _rtbActivityLog.AppendText($"{message}\n");

        _rtbActivityLog.ScrollToCaret();
    }
}
