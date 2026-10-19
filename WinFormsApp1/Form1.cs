using System.Diagnostics;
using VelopackUpdateTest.Models;
using VelopackUpdateTest.Services;
using VelopackUpdateTest.UI;
using VelopackUpdateTest.UI.Controls;

namespace VelopackUpdateTest;

public partial class Form1 : Form
{
    private readonly VelopackUpdateService _updateService = VelopackUpdateService.Instance;

    // Top Navigation
    private Panel _pnlTopNav = null!;
    private Label _lblAppTitle = null!;
    private Label _lblAppSubtitle = null!;
    private ModernBadge _badgeCurrentVersion = null!;
    private ModernBadge _badgeStatusMode = null!;
    private ModernButton _btnQuickCheck = null!;

    // In-App Notification Banner
    private Panel _pnlBanner = null!;
    private Label _lblBannerText = null!;
    private ModernButton _btnBannerAction = null!;
    private ModernButton _btnBannerDismiss = null!;
    private ReleaseInfo? _pendingBannerRelease;

    // Main layout
    private TableLayoutPanel _tblMain = null!;

    // Diagnostics Card Controls
    private Label _lblDiagVersion = null!;
    private Label _lblDiagAppId = null!;
    private Label _lblDiagInstalled = null!;
    private Label _lblDiagChannel = null!;
    private TextBox _txtRepoUrl = null!;

    // Simulation Studio Controls
    private ComboBox _cmbSimTarget = null!;
    private CheckBox _chkForceUpdate = null!;
    private CheckBox _chkSimulateError = null!;
    private ModernButton _btnLaunchModal = null!;
    private ModernButton _btnCheckLive = null!;
    private ModernButton _btnTriggerBanner = null!;

    // Changelog Explorer Controls
    private ComboBox _cmbChangelogVersion = null!;
    private RichTextBox _rtbChangelogView = null!;
    private Label _lblChangelogMeta = null!;

    // Activity Log Controls
    private RichTextBox _rtbActivityLog = null!;
    private ModernButton _btnClearLog = null!;
    private ModernButton _btnCopyLog = null!;

    public Form1()
    {
        InitializeComponent();
        BuildModernUi();
        HookEvents();
        LoadInitialData();
    }

    private void BuildModernUi()
    {
        SuspendLayout();

        BackColor = ModernTheme.Background;
        ForeColor = ModernTheme.TextPrimary;
        Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

        // 1. Top Navigation Bar
        _pnlTopNav = new Panel
        {
            Dock = DockStyle.Top,
            Height = 72,
            BackColor = ModernTheme.BackgroundDark,
            Padding = new Padding(24, 12, 24, 12)
        };

        var pnlTitleGroup = new Panel
        {
            Location = new Point(24, 14),
            Size = new Size(380, 48),
            BackColor = Color.Transparent
        };

        _lblAppTitle = new Label
        {
            Text = "VelopackUpdateTest",
            Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(0, 0)
        };

        _lblAppSubtitle = new Label
        {
            Text = "Auto-Update Diagnostic Suite & Simulation Sandbox",
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = ModernTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(1, 24)
        };

        pnlTitleGroup.Controls.Add(_lblAppTitle);
        pnlTitleGroup.Controls.Add(_lblAppSubtitle);

        _badgeCurrentVersion = new ModernBadge
        {
            BadgeText = $"v{_updateService.CurrentVersion}",
            BadgeColor = ModernTheme.Primary,
            Width = 90,
            Location = new Point(410, 22)
        };

        _badgeStatusMode = new ModernBadge
        {
            BadgeText = _updateService.IsInstalled ? "Velopack Installed" : "Simulation Sandbox",
            BadgeColor = _updateService.IsInstalled ? ModernTheme.Success : ModernTheme.Warning,
            Width = 160,
            Location = new Point(510, 22)
        };

        _btnQuickCheck = new ModernButton
        {
            Text = "⚡ Check Updates",
            Style = ButtonStyle.Primary,
            Width = 145,
            Height = 36,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(ClientSize.Width - 170, 18)
        };
        _btnQuickCheck.Click += async (s, e) => await CheckForUpdatesAsync(useSimulation: !_updateService.IsInstalled);

        _pnlTopNav.Controls.Add(pnlTitleGroup);
        _pnlTopNav.Controls.Add(_badgeCurrentVersion);
        _pnlTopNav.Controls.Add(_badgeStatusMode);
        _pnlTopNav.Controls.Add(_btnQuickCheck);

        // 2. In-App Notification Banner (Collapsible)
        _pnlBanner = new Panel
        {
            Dock = DockStyle.Top,
            Height = 48,
            BackColor = ModernTheme.CardElevated,
            Visible = false,
            Padding = new Padding(24, 8, 24, 8)
        };

        _lblBannerText = new Label
        {
            Text = "Update Notice",
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(24, 14)
        };

        _btnBannerAction = new ModernButton
        {
            Text = "Update Now",
            Style = ButtonStyle.Primary,
            Height = 30,
            Width = 110,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(ClientSize.Width - 210, 9)
        };
        _btnBannerAction.Click += (s, e) =>
        {
            if (_pendingBannerRelease != null)
            {
                LaunchUpdateScreen(_pendingBannerRelease, !_updateService.IsInstalled, _chkSimulateError.Checked);
            }
        };

        _btnBannerDismiss = new ModernButton
        {
            Text = "✕",
            Style = ButtonStyle.Secondary,
            Height = 30,
            Width = 34,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(ClientSize.Width - 90, 9)
        };
        _btnBannerDismiss.Click += (s, e) => _pnlBanner.Visible = false;

        _pnlBanner.Controls.Add(_lblBannerText);
        _pnlBanner.Controls.Add(_btnBannerAction);
        _pnlBanner.Controls.Add(_btnBannerDismiss);

        // 3. Main Two-Column Table Layout
        _tblMain = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = ModernTheme.Background,
            Padding = new Padding(20, 16, 20, 16)
        };
        _tblMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 460F));
        _tblMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        // Left Container Panel (Holds Diagnostics Card and Simulation Studio)
        var pnlLeftColumn = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(0, 0, 10, 0)
        };

        // Right Container Panel (Holds Changelog Card and Activity Log)
        var pnlRightColumn = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(10, 0, 0, 0)
        };

        // --- LEFT COLUMN: Card 1 Diagnostics ---
        var cardDiagnostics = CreateDiagnosticsCard();
        cardDiagnostics.Location = new Point(0, 0);
        cardDiagnostics.Width = 440;
        cardDiagnostics.Height = 230;

        // --- LEFT COLUMN: Card 2 Simulation Studio ---
        var cardSimulation = CreateSimulationCard();
        cardSimulation.Location = new Point(0, 244);
        cardSimulation.Width = 440;
        cardSimulation.Height = 410;

        pnlLeftColumn.Controls.Add(cardDiagnostics);
        pnlLeftColumn.Controls.Add(cardSimulation);

        // --- RIGHT COLUMN: Card 3 Changelog Explorer ---
        var cardChangelog = CreateChangelogCard();
        cardChangelog.Location = new Point(10, 0);
        cardChangelog.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        cardChangelog.Width = 600;
        cardChangelog.Height = 350;

        // --- RIGHT COLUMN: Card 4 Activity Console ---
        var cardConsole = CreateConsoleCard();
        cardConsole.Location = new Point(10, 364);
        cardConsole.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        cardConsole.Width = 600;
        cardConsole.Height = 290;

        pnlRightColumn.Controls.Add(cardChangelog);
        pnlRightColumn.Controls.Add(cardConsole);

        _tblMain.Controls.Add(pnlLeftColumn, 0, 0);
        _tblMain.Controls.Add(pnlRightColumn, 1, 0);

        Controls.Add(_tblMain);
        Controls.Add(_pnlBanner);
        Controls.Add(_pnlTopNav);

        ResumeLayout(false);
    }

    private ModernCard CreateDiagnosticsCard()
    {
        var card = new ModernCard();

        var lblTitle = new Label
        {
            Text = "VELPOACK RUNTIME & ENVIRONMENT",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, 12),
            AutoSize = true
        };

        int rowY = 38;
        AddDiagRow(card, "App Assembly Title:", "VelopackUpdateTest", ref rowY);
        _lblDiagVersion = AddDiagRow(card, "Current Version:", _updateService.CurrentVersion, ref rowY);
        _lblDiagAppId = AddDiagRow(card, "Velopack AppId:", _updateService.AppId, ref rowY);
        _lblDiagInstalled = AddDiagRow(card, "Installed Mode:", _updateService.IsInstalled ? "Yes (Active)" : "No (Local Debug / Simulation)", ref rowY,
            _updateService.IsInstalled ? ModernTheme.Success : ModernTheme.Warning);
        _lblDiagChannel = AddDiagRow(card, "Release Channel:", _updateService.Channel, ref rowY);

        var lblRepo = new Label
        {
            Text = "GitHub Feed Source:",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, rowY),
            AutoSize = true
        };
        card.Controls.Add(lblRepo);

        _txtRepoUrl = new TextBox
        {
            Text = "https://github.com/your-username/VelopackUpdateTest",
            Location = new Point(16, rowY + 20),
            Width = 408,
            BackColor = ModernTheme.BackgroundDark,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 9f)
        };
        card.Controls.Add(_txtRepoUrl);

        card.Controls.Add(lblTitle);
        return card;
    }

    private Label AddDiagRow(ModernCard card, string label, string value, ref int y, Color? valColor = null)
    {
        var lblKey = new Label
        {
            Text = label,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, y),
            AutoSize = true
        };

        var lblVal = new Label
        {
            Text = value,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = valColor ?? ModernTheme.TextPrimary,
            Location = new Point(180, y),
            AutoSize = true
        };

        card.Controls.Add(lblKey);
        card.Controls.Add(lblVal);
        y += 24;
        return lblVal;
    }

    private ModernCard CreateSimulationCard()
    {
        var card = new ModernCard();

        var lblTitle = new Label
        {
            Text = "UPDATE SIMULATION STUDIO",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, 12),
            AutoSize = true
        };

        var lblDesc = new Label
        {
            Text = "Simulate all Velopack client update phases (discovery, download progress, forced updates) without publishing releases.",
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = ModernTheme.TextMuted,
            Location = new Point(16, 34),
            Size = new Size(408, 38)
        };

        var lblTarget = new Label
        {
            Text = "Select Simulation Scenario:",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, 76),
            AutoSize = true
        };

        _cmbSimTarget = new ComboBox
        {
            Location = new Point(16, 98),
            Width = 408,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.BackgroundDark,
            ForeColor = ModernTheme.TextPrimary,
            Font = new Font("Segoe UI", 9.5f)
        };
        _cmbSimTarget.Items.Add("v1.1.0 - Feature Pack (Optional Update)");
        _cmbSimTarget.Items.Add("v2.0.0 - Major Overhaul (🚨 MANDATORY FORCE UPDATE)");
        _cmbSimTarget.Items.Add("v1.0.1 - Stability Patch (Quick Delta)");
        _cmbSimTarget.SelectedIndex = 1; // Default to v2.0.0 to showcase forced update!

        _chkForceUpdate = new CheckBox
        {
            Text = "Enforce Mandatory Mode (Lock skip & require install)",
            Checked = true,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = ModernTheme.Danger,
            Location = new Point(18, 140),
            AutoSize = true
        };

        _chkSimulateError = new CheckBox
        {
            Text = "Simulate Network Interruption / Drop at 50%",
            Checked = false,
            Font = new Font("Segoe UI", 9f),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(18, 170),
            AutoSize = true
        };

        _cmbSimTarget.SelectedIndexChanged += (s, e) =>
        {
            _chkForceUpdate.Checked = _cmbSimTarget.SelectedIndex == 1;
        };

        _btnLaunchModal = new ModernButton
        {
            Text = "🚀 Launch Cool Update Screen (Simulation)",
            Style = ButtonStyle.Primary,
            Location = new Point(16, 210),
            Width = 408,
            Height = 44,
            Font = new Font("Segoe UI", 10.5f, FontStyle.Bold)
        };
        _btnLaunchModal.Click += (s, e) => LaunchCurrentSimulation();

        _btnTriggerBanner = new ModernButton
        {
            Text = "🔔 Test Top In-App Update Banner",
            Style = ButtonStyle.Secondary,
            Location = new Point(16, 264),
            Width = 408,
            Height = 38
        };
        _btnTriggerBanner.Click += (s, e) => TriggerInAppBannerSimulation();

        _btnCheckLive = new ModernButton
        {
            Text = "🌐 Query Live GitHub Release Feed (Velopack API)",
            Style = ButtonStyle.Secondary,
            Location = new Point(16, 312),
            Width = 408,
            Height = 38
        };
        _btnCheckLive.Click += async (s, e) => await CheckForUpdatesAsync(useSimulation: false);

        var lblHelp = new Label
        {
            Text = "💡 Tip: Selecting v2.0.0 tests mandatory update locking where the user cannot bypass the update.",
            Font = new Font("Segoe UI", 8.2f, FontStyle.Italic),
            ForeColor = ModernTheme.TextMuted,
            Location = new Point(16, 360),
            Size = new Size(408, 36)
        };

        card.Controls.Add(lblTitle);
        card.Controls.Add(lblDesc);
        card.Controls.Add(lblTarget);
        card.Controls.Add(_cmbSimTarget);
        card.Controls.Add(_chkForceUpdate);
        card.Controls.Add(_chkSimulateError);
        card.Controls.Add(_btnLaunchModal);
        card.Controls.Add(_btnTriggerBanner);
        card.Controls.Add(_btnCheckLive);
        card.Controls.Add(lblHelp);

        return card;
    }

    private ModernCard CreateChangelogCard()
    {
        var card = new ModernCard();

        var lblTitle = new Label
        {
            Text = "CURRENT VERSION & CHANGELOG EXPLORER",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, 12),
            AutoSize = true
        };

        _lblChangelogMeta = new Label
        {
            Text = "Version History & Release Highlights",
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = ModernTheme.TextMuted,
            Location = new Point(16, 32),
            AutoSize = true
        };

        _cmbChangelogVersion = new ComboBox
        {
            Location = new Point(340, 10),
            Width = 240,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.BackgroundDark,
            ForeColor = ModernTheme.TextPrimary,
            Font = new Font("Segoe UI", 9f)
        };

        foreach (var rel in ReleaseInfo.AllReleases)
        {
            string label = rel.Version == ReleaseInfo.CurrentVersion.Version
                ? $"v{rel.Version} (Current Installed Version)"
                : (rel.IsMandatory ? $"v{rel.Version} (🚨 Mandatory Preview)" : $"v{rel.Version} ({rel.Title})");

            _cmbChangelogVersion.Items.Add(label);
        }
        _cmbChangelogVersion.SelectedIndex = 3; // v1.0.0 Current Version initially!

        _cmbChangelogVersion.SelectedIndexChanged += (s, e) =>
        {
            if (_cmbChangelogVersion.SelectedIndex >= 0 && _cmbChangelogVersion.SelectedIndex < ReleaseInfo.AllReleases.Count)
            {
                DisplayChangelog(ReleaseInfo.AllReleases[_cmbChangelogVersion.SelectedIndex]);
            }
        };

        _rtbChangelogView = new RichTextBox
        {
            Location = new Point(16, 56),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Width = 566,
            Height = 276,
            BackColor = ModernTheme.BackgroundDark,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.None,
            ReadOnly = true,
            Font = new Font("Segoe UI", 9.2f)
        };

        card.Controls.Add(lblTitle);
        card.Controls.Add(_lblChangelogMeta);
        card.Controls.Add(_cmbChangelogVersion);
        card.Controls.Add(_rtbChangelogView);

        return card;
    }

    private ModernCard CreateConsoleCard()
    {
        var card = new ModernCard();

        var lblTitle = new Label
        {
            Text = "LIVE VELOPACK ACTIVITY CONSOLE",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, 12),
            AutoSize = true
        };

        _btnClearLog = new ModernButton
        {
            Text = "Clear",
            Style = ButtonStyle.Secondary,
            Location = new Point(440, 8),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Width = 64,
            Height = 28,
            Font = new Font("Segoe UI", 8.2f)
        };
        _btnClearLog.Click += (s, e) => _rtbActivityLog.Clear();

        _btnCopyLog = new ModernButton
        {
            Text = "Copy",
            Style = ButtonStyle.Secondary,
            Location = new Point(512, 8),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Width = 70,
            Height = 28,
            Font = new Font("Segoe UI", 8.2f)
        };
        _btnCopyLog.Click += (s, e) =>
        {
            if (!string.IsNullOrEmpty(_rtbActivityLog.Text))
            {
                Clipboard.SetText(_rtbActivityLog.Text);
                _updateService.Log("CONSOLE", "Activity logs copied to clipboard.", ModernTheme.Success);
            }
        };

        _rtbActivityLog = new RichTextBox
        {
            Location = new Point(16, 42),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Width = 566,
            Height = 230,
            BackColor = ModernTheme.BackgroundDark,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.None,
            ReadOnly = true,
            Font = new Font("Consolas", 9f)
        };

        card.Controls.Add(lblTitle);
        card.Controls.Add(_btnClearLog);
        card.Controls.Add(_btnCopyLog);
        card.Controls.Add(_rtbActivityLog);

        return card;
    }

    private void HookEvents()
    {
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
    }

    private void LoadInitialData()
    {
        _updateService.Log("APP_START", "VelopackUpdateTest initialized successfully.", ModernTheme.Success);
        _updateService.Log("INFO", $"Current Version: {_updateService.CurrentVersion} | Locator AppId: {_updateService.AppId}", ModernTheme.Primary);

        if (_cmbChangelogVersion.Items.Count > 0)
        {
            DisplayChangelog(ReleaseInfo.CurrentVersion);
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

        // Allow overriding mandatory flag from UI
        return baseRelease with
        {
            IsMandatory = _chkForceUpdate.Checked,
            MandatoryReason = _chkForceUpdate.Checked 
                ? (baseRelease.MandatoryReason ?? "Mandatory update policy enforced via simulation studio.")
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
            _btnBannerDismiss.Visible = false; // Cannot dismiss mandatory update!
        }
        else
        {
            _pnlBanner.BackColor = ModernTheme.CardElevated;
            _lblBannerText.Text = $"🎉 New Version Available: VelopackUpdateTest v{release.Version} ({release.Title})";
            _btnBannerAction.Text = "View & Update";
            _btnBannerAction.Style = ButtonStyle.Primary;
            _btnBannerDismiss.Visible = true;
        }

        _updateService.Log("BANNER", $"In-app update banner triggered for v{release.Version} (Mandatory: {release.IsMandatory})", ModernTheme.Primary);
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

    private void LaunchUpdateScreen(ReleaseInfo targetRelease, bool simulationMode, bool simulateFailure)
    {
        _updateService.Log("MODAL", $"Displaying modern update screen for v{targetRelease.Version} (Mandatory: {targetRelease.IsMandatory})", ModernTheme.Primary);

        using var dialog = new UpdateDialogForm(targetRelease, simulationMode, simulateFailure);
        dialog.ShowDialog(this);
    }
}
