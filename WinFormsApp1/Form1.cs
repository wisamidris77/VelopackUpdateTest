using VelopackUpdateTest.Models;
using VelopackUpdateTest.Services;
using VelopackUpdateTest.UI;
using VelopackUpdateTest.UI.Controls;

namespace VelopackUpdateTest;

public partial class Form1 : Form
{
    private readonly VelopackUpdateService _updateService = VelopackUpdateService.Instance;
    private ReleaseInfo? _pendingBannerRelease;
    private System.Windows.Forms.Timer? _periodicTimer;

    public Form1()
    {
        InitializeComponent();
        HookEvents();
        LoadInitialData();
    }

    private void HookEvents()
    {
        // Top Nav Actions
        _btnQuickCheck.Click += async (s, e) => await CheckForUpdatesAsync(useSimulation: false);

        // Banner Actions
        _btnBannerAction.Click += (s, e) =>
        {
            if (_pendingBannerRelease != null)
            {
                LaunchUpdateScreen(_pendingBannerRelease, simulationMode: !_updateService.IsInstalled, simulateFailure: _chkSimulateError.Checked);
            }
        };
        _btnBannerDismiss.Click += (s, e) => _pnlBanner.Visible = false;

        // Simulation Studio Actions
        _cmbSimTarget.SelectedIndexChanged += (s, e) =>
        {
            _chkForceUpdate.Checked = _cmbSimTarget.SelectedIndex == 1;
        };

        _btnLaunchModal.Click += (s, e) => LaunchCurrentSimulation();
        _btnTriggerBanner.Click += (s, e) => TriggerInAppBannerSimulation();
        _btnCheckLive.Click += async (s, e) => await CheckForUpdatesAsync(useSimulation: false);

        // Changelog Explorer Actions
        _cmbChangelogVersion.SelectedIndexChanged += (s, e) =>
        {
            if (_cmbChangelogVersion.SelectedIndex >= 0 && _cmbChangelogVersion.SelectedIndex < ReleaseInfo.AllReleases.Count)
            {
                DisplayChangelog(ReleaseInfo.AllReleases[_cmbChangelogVersion.SelectedIndex]);
            }
        };

        // Activity Console Actions
        _btnClearLog.Click += (s, e) => _rtbActivityLog.Clear();
        _btnCopyLog.Click += (s, e) =>
        {
            if (!string.IsNullOrEmpty(_rtbActivityLog.Text))
            {
                Clipboard.SetText(_rtbActivityLog.Text);
                _updateService.Log("CONSOLE", "Activity logs copied to clipboard.", ModernTheme.Success);
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
                await Task.Delay(1200); // Allow UI to render smoothly before checking
                await AutoCheckGitHubReleasesAsync();
            }
        };

        // Periodic Background Check Timer (Every 5 minutes)
        _periodicTimer = new System.Windows.Forms.Timer { Interval = 1000 * 60 * 5 };
        _periodicTimer.Tick += async (s, e) =>
        {
            if (_chkAutoCheckGitHub.Checked)
            {
                await AutoCheckGitHubReleasesAsync(isPeriodic: true);
            }
        };
        _periodicTimer.Start();
    }

    private void LoadInitialData()
    {
        _lblDiagVersion.Text = _updateService.CurrentVersion;
        _lblDiagAppId.Text = _updateService.AppId;
        _lblDiagInstalled.Text = _updateService.IsInstalled ? "Yes (Velopack Active)" : "No (Local Debug / Direct GitHub API)";
        _lblDiagInstalled.ForeColor = _updateService.IsInstalled ? ModernTheme.Success : ModernTheme.Warning;
        _lblDiagChannel.Text = _updateService.Channel;
        _badgeCurrentVersion.BadgeText = $"v{_updateService.CurrentVersion}";
        _badgeStatusMode.BadgeText = _updateService.IsInstalled ? "Velopack Installed" : "GitHub Hook Active";
        _badgeStatusMode.BadgeColor = _updateService.IsInstalled ? ModernTheme.Success : ModernTheme.Primary;

        // Populate Changelog dropdown
        _cmbChangelogVersion.Items.Clear();
        foreach (var rel in ReleaseInfo.AllReleases)
        {
            string label = rel.Version == ReleaseInfo.CurrentVersion.Version
                ? $"v{rel.Version} (Current Installed Version)"
                : (rel.IsMandatory ? $"v{rel.Version} (🚨 Mandatory Preview)" : $"v{rel.Version} ({rel.Title})");

            _cmbChangelogVersion.Items.Add(label);
        }
        _cmbChangelogVersion.SelectedIndex = 3; // v1.0.0 Current Version
        DisplayChangelog(ReleaseInfo.CurrentVersion);

        _updateService.Log("STARTUP", "VelopackUpdateTest application running.", ModernTheme.Success);
        _updateService.Log("DIAGNOSTICS", $"Version: {_updateService.CurrentVersion} | AppId: {_updateService.AppId} | Mode: {(_updateService.IsInstalled ? "Installed" : "Debug/Simulation")}", ModernTheme.Primary);
    }

    private async Task AutoCheckGitHubReleasesAsync(bool isPeriodic = false)
    {
        try
        {
            string source = _txtRepoUrl.Text.Trim();
            _updateService.Log("AUTO_CHECK", $"{(isPeriodic ? "Periodic" : "Startup")} checking GitHub Releases for updates...", Color.FromArgb(148, 163, 184));

            var update = await _updateService.CheckForUpdatesAsync(source, simulationMode: false, simulatedRelease: null);
            if (update != null)
            {
                _pendingBannerRelease = update;
                _pnlBanner.Visible = true;

                if (update.IsMandatory)
                {
                    _pnlBanner.BackColor = ModernTheme.DangerBg;
                    _lblBannerText.Text = $"🚨 CRITICAL REQUIRED UPDATE: VelopackUpdateTest v{update.Version} is available!";
                    _btnBannerAction.Text = "Install Required Update";
                    _btnBannerAction.Style = ButtonStyle.Danger;
                    _btnBannerDismiss.Visible = false;

                    // Automatically pop up mandatory update modal
                    LaunchUpdateScreen(update, simulationMode: !_updateService.IsInstalled, simulateFailure: false);
                }
                else
                {
                    _pnlBanner.BackColor = ModernTheme.CardElevated;
                    _lblBannerText.Text = $"🎉 New Release Available on GitHub: VelopackUpdateTest v{update.Version} ({update.Title})";
                    _btnBannerAction.Text = "View & Update";
                    _btnBannerAction.Style = ButtonStyle.Primary;
                    _btnBannerDismiss.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            _updateService.Log("AUTO_CHECK_ERR", $"Auto-check notice: {ex.Message}", ModernTheme.TextMuted);
        }
    }

    private async Task CheckForUpdatesAsync(bool useSimulation)
    {
        try
        {
            _btnQuickCheck.Enabled = false;
            _btnCheckLive.Enabled = false;

            var target = GetSelectedSimulationRelease();
            var update = await _updateService.CheckForUpdatesAsync(_txtRepoUrl.Text.Trim(), useSimulation, target);

            if (update != null)
            {
                LaunchUpdateScreen(update, useSimulation, _chkSimulateError.Checked);
            }
            else
            {
                MessageBox.Show(
                    this,
                    "VelopackUpdateTest is completely up to date!\nNo new releases were found in the feed.",
                    "Up to Date",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"Failed to check update feed: {ex.Message}",
                "Check Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            _btnQuickCheck.Enabled = true;
            _btnCheckLive.Enabled = true;
        }
    }

    private void DisplayChangelog(ReleaseInfo release)
    {
        _rtbChangelogView.Clear();
        _lblChangelogMeta.Text = $"Viewing release info for v{release.Version} ({release.ReleaseDate:yyyy-MM-dd}) • Channel: {release.Channel} • Size: {release.SizeDisplay}";

        AppendChangelogLine($"🚀 {release.Title} (Version {release.Version})\n", Color.FromArgb(168, 85, 247), true, 11f);
        AppendChangelogLine($"{release.Summary}\n\n", ModernTheme.TextSecondary, false, 9.2f);

        if (release.IsMandatory)
        {
            AppendChangelogLine("⚠️ MANDATORY UPDATE POLICY:\n", ModernTheme.Danger, true, 9.5f);
            AppendChangelogLine($"  {release.MandatoryReason}\n\n", Color.FromArgb(254, 202, 202), false, 9f);
        }

        if (release.Features.Count > 0)
        {
            AppendChangelogLine("✨ NEW FEATURES & HIGHLIGHTS:\n", ModernTheme.Success, true, 9.5f);
            foreach (var f in release.Features)
            {
                AppendChangelogLine($"  • {f}\n", ModernTheme.TextPrimary, false, 9f);
            }
            AppendChangelogLine("\n", ModernTheme.TextPrimary, false, 6f);
        }

        if (release.Improvements.Count > 0)
        {
            AppendChangelogLine("⚡ IMPROVEMENTS & PERFORMANCE:\n", ModernTheme.Primary, true, 9.5f);
            foreach (var imp in release.Improvements)
            {
                AppendChangelogLine($"  • {imp}\n", ModernTheme.TextPrimary, false, 9f);
            }
            AppendChangelogLine("\n", ModernTheme.TextPrimary, false, 6f);
        }

        if (release.Fixes.Count > 0)
        {
            AppendChangelogLine("🐛 BUG FIXES & STABILITY:\n", ModernTheme.Warning, true, 9.5f);
            foreach (var fix in release.Fixes)
            {
                AppendChangelogLine($"  • {fix}\n", ModernTheme.TextPrimary, false, 9f);
            }
            AppendChangelogLine("\n", ModernTheme.TextPrimary, false, 6f);
        }

        if (release.SecurityUpdates.Count > 0)
        {
            AppendChangelogLine("🔒 SECURITY ENHANCEMENTS:\n", ModernTheme.Danger, true, 9.5f);
            foreach (var sec in release.SecurityUpdates)
            {
                AppendChangelogLine($"  • {sec}\n", ModernTheme.TextPrimary, false, 9f);
            }
        }
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

    private ReleaseInfo GetSelectedSimulationRelease()
    {
        var baseRelease = _cmbSimTarget.SelectedIndex switch
        {
            0 => ReleaseInfo.ReleaseV110,
            1 => ReleaseInfo.ReleaseV200Mandatory,
            2 => ReleaseInfo.ReleaseV101Patch,
            _ => ReleaseInfo.ReleaseV110
        };

        return baseRelease with
        {
            IsMandatory = _chkForceUpdate.Checked,
            MandatoryReason = _chkForceUpdate.Checked 
                ? (baseRelease.MandatoryReason ?? "Mandatory update policy enforced.")
                : null
        };
    }

    private void LaunchCurrentSimulation()
    {
        var release = GetSelectedSimulationRelease();
        LaunchUpdateScreen(release, simulationMode: true, simulateFailure: _chkSimulateError.Checked);
    }

    private void TriggerInAppBannerSimulation()
    {
        var release = GetSelectedSimulationRelease();
        _pendingBannerRelease = release;

        _pnlBanner.Visible = true;
        if (release.IsMandatory)
        {
            _pnlBanner.BackColor = ModernTheme.DangerBg;
            _lblBannerText.Text = $"🚨 CRITICAL UPDATE REQUIRED: VelopackUpdateTest v{release.Version} must be installed.";
            _btnBannerAction.Text = "Install Required Update";
            _btnBannerAction.Style = ButtonStyle.Danger;
            _btnBannerDismiss.Visible = false;
        }
        else
        {
            _pnlBanner.BackColor = ModernTheme.CardElevated;
            _lblBannerText.Text = $"🎉 New Version Available: VelopackUpdateTest v{release.Version} ({release.Title})";
            _btnBannerAction.Text = "View & Update";
            _btnBannerAction.Style = ButtonStyle.Primary;
            _btnBannerDismiss.Visible = true;
        }

        _updateService.Log("BANNER", $"In-app update banner triggered for v{release.Version}", ModernTheme.Primary);
    }

    private void LaunchUpdateScreen(ReleaseInfo targetRelease, bool simulationMode, bool simulateFailure)
    {
        _updateService.Log("MODAL", $"Displaying modern update screen for v{targetRelease.Version} (Mandatory: {targetRelease.IsMandatory})", ModernTheme.Primary);

        using var dialog = new UpdateDialogForm(targetRelease, simulationMode, simulateFailure);
        dialog.ShowDialog(this);
    }
}
