using VelopackUpdateTest.UI.Controls;

namespace VelopackUpdateTest;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    // Top Navigation Controls
    private Panel _pnlTopNav;
    private Label _lblAppTitle;
    private Label _lblAppSubtitle;
    private ModernBadge _badgeCurrentVersion;
    private ModernBadge _badgeStatusMode;
    private ModernButton _btnQuickCheck;

    // In-App Notification Banner
    private Panel _pnlBanner;
    private Label _lblBannerText;
    private ModernButton _btnBannerAction;
    private ModernButton _btnBannerDismiss;

    // Main Layout
    private TableLayoutPanel _tblMain;

    // Diagnostics Card Controls
    private ModernCard _cardDiagnostics;
    private Label _lblDiagVersion;
    private Label _lblDiagAppId;
    private Label _lblDiagInstalled;
    private Label _lblDiagChannel;
    private TextBox _txtRepoUrl;

    // Simulation Studio Controls
    private ModernCard _cardSimulation;
    private ComboBox _cmbSimTarget;
    private CheckBox _chkForceUpdate;
    private CheckBox _chkSimulateError;
    private CheckBox _chkAutoCheckGitHub;
    private ModernButton _btnLaunchModal;
    private ModernButton _btnCheckLive;
    private ModernButton _btnTriggerBanner;

    // Changelog Explorer Controls
    private ModernCard _cardChangelog;
    private Label _lblChangelogMeta;
    private ComboBox _cmbChangelogVersion;
    private RichTextBox _rtbChangelogView;

    // Activity Log Controls
    private ModernCard _cardConsole;
    private RichTextBox _rtbActivityLog;
    private ModernButton _btnClearLog;
    private ModernButton _btnCopyLog;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();

        // Form settings
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1160, 780);
        Text = "VelopackUpdateTest - Auto-Update Simulation & Diagnostics Suite";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1040, 700);
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
            BadgeText = "v1.0.0",
            BadgeColor = ModernTheme.Primary,
            Width = 90,
            Location = new Point(410, 22)
        };

        _badgeStatusMode = new ModernBadge
        {
            BadgeText = "Simulation Sandbox",
            BadgeColor = ModernTheme.Warning,
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

        _pnlTopNav.Controls.Add(pnlTitleGroup);
        _pnlTopNav.Controls.Add(_badgeCurrentVersion);
        _pnlTopNav.Controls.Add(_badgeStatusMode);
        _pnlTopNav.Controls.Add(_btnQuickCheck);

        // 2. In-App Notification Banner
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

        var pnlLeftColumn = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(0, 0, 10, 0)
        };

        var pnlRightColumn = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(10, 0, 0, 0)
        };

        // --- LEFT COLUMN: Card 1 Diagnostics ---
        _cardDiagnostics = new ModernCard
        {
            Location = new Point(0, 0),
            Width = 440,
            Height = 230
        };

        var lblDiagHeader = new Label
        {
            Text = "VELOPACK RUNTIME & ENVIRONMENT",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, 12),
            AutoSize = true
        };

        int rowY = 38;
        AddDiagField(_cardDiagnostics, "App Assembly Title:", "VelopackUpdateTest", ref rowY);
        _lblDiagVersion = AddDiagField(_cardDiagnostics, "Current Version:", "1.0.0", ref rowY);
        _lblDiagAppId = AddDiagField(_cardDiagnostics, "Velopack AppId:", "VelopackUpdateTest", ref rowY);
        _lblDiagInstalled = AddDiagField(_cardDiagnostics, "Installed Mode:", "Checking...", ref rowY, ModernTheme.Warning);
        _lblDiagChannel = AddDiagField(_cardDiagnostics, "Release Channel:", "stable", ref rowY);

        var lblRepo = new Label
        {
            Text = "GitHub Feed Source:",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, rowY),
            AutoSize = true
        };
        _cardDiagnostics.Controls.Add(lblRepo);

        _txtRepoUrl = new TextBox
        {
            Text = "https://github.com/wisamidris77/VelopackUpdateTest",
            Location = new Point(16, rowY + 20),
            Width = 408,
            BackColor = ModernTheme.BackgroundDark,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 9f)
        };
        _cardDiagnostics.Controls.Add(_txtRepoUrl);
        _cardDiagnostics.Controls.Add(lblDiagHeader);
        pnlLeftColumn.Controls.Add(_cardDiagnostics);

        // --- LEFT COLUMN: Card 2 Simulation Studio ---
        _cardSimulation = new ModernCard
        {
            Location = new Point(0, 244),
            Width = 440,
            Height = 440
        };

        var lblSimHeader = new Label
        {
            Text = "UPDATE SIMULATION STUDIO",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, 12),
            AutoSize = true
        };

        var lblSimDesc = new Label
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
        _cmbSimTarget.SelectedIndex = 1;

        _chkForceUpdate = new CheckBox
        {
            Text = "Enforce Mandatory Mode (Lock skip & require install)",
            Checked = true,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = ModernTheme.Danger,
            Location = new Point(18, 138),
            AutoSize = true
        };

        _chkSimulateError = new CheckBox
        {
            Text = "Simulate Network Interruption / Drop at 50%",
            Checked = false,
            Font = new Font("Segoe UI", 8.8f),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(18, 164),
            AutoSize = true
        };

        _chkAutoCheckGitHub = new CheckBox
        {
            Text = "Auto-Check GitHub Releases on Startup (Background)",
            Checked = true,
            Font = new Font("Segoe UI", 8.8f),
            ForeColor = ModernTheme.Success,
            Location = new Point(18, 190),
            AutoSize = true
        };

        _btnLaunchModal = new ModernButton
        {
            Text = "🚀 Launch Cool Update Screen (Simulation)",
            Style = ButtonStyle.Primary,
            Location = new Point(16, 226),
            Width = 408,
            Height = 44,
            Font = new Font("Segoe UI", 10.5f, FontStyle.Bold)
        };

        _btnTriggerBanner = new ModernButton
        {
            Text = "🔔 Test Top In-App Update Banner",
            Style = ButtonStyle.Secondary,
            Location = new Point(16, 278),
            Width = 408,
            Height = 36
        };

        _btnCheckLive = new ModernButton
        {
            Text = "🌐 Query Live GitHub Release Feed (Velopack API)",
            Style = ButtonStyle.Secondary,
            Location = new Point(16, 322),
            Width = 408,
            Height = 36
        };

        var lblHelp = new Label
        {
            Text = "💡 Tip: When a release is published on GitHub, the app auto-discovers it and opens the update screen automatically.",
            Font = new Font("Segoe UI", 8.2f, FontStyle.Italic),
            ForeColor = ModernTheme.TextMuted,
            Location = new Point(16, 368),
            Size = new Size(408, 48)
        };

        _cardSimulation.Controls.Add(lblSimHeader);
        _cardSimulation.Controls.Add(lblSimDesc);
        _cardSimulation.Controls.Add(lblTarget);
        _cardSimulation.Controls.Add(_cmbSimTarget);
        _cardSimulation.Controls.Add(_chkForceUpdate);
        _cardSimulation.Controls.Add(_chkSimulateError);
        _cardSimulation.Controls.Add(_chkAutoCheckGitHub);
        _cardSimulation.Controls.Add(_btnLaunchModal);
        _cardSimulation.Controls.Add(_btnTriggerBanner);
        _cardSimulation.Controls.Add(_btnCheckLive);
        _cardSimulation.Controls.Add(lblHelp);
        pnlLeftColumn.Controls.Add(_cardSimulation);

        // --- RIGHT COLUMN: Card 3 Changelog Explorer ---
        _cardChangelog = new ModernCard
        {
            Location = new Point(10, 0),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Width = 620,
            Height = 350
        };

        var lblChangelogTitle = new Label
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
            Location = new Point(360, 10),
            Width = 244,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = ModernTheme.BackgroundDark,
            ForeColor = ModernTheme.TextPrimary,
            Font = new Font("Segoe UI", 9f)
        };

        _rtbChangelogView = new RichTextBox
        {
            Location = new Point(16, 56),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Width = 588,
            Height = 276,
            BackColor = ModernTheme.BackgroundDark,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.None,
            ReadOnly = true,
            Font = new Font("Segoe UI", 9.2f)
        };

        _cardChangelog.Controls.Add(lblChangelogTitle);
        _cardChangelog.Controls.Add(_lblChangelogMeta);
        _cardChangelog.Controls.Add(_cmbChangelogVersion);
        _cardChangelog.Controls.Add(_rtbChangelogView);
        pnlRightColumn.Controls.Add(_cardChangelog);

        // --- RIGHT COLUMN: Card 4 Activity Console ---
        _cardConsole = new ModernCard
        {
            Location = new Point(10, 364),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Width = 620,
            Height = 320
        };

        var lblConsoleTitle = new Label
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
            Location = new Point(460, 8),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Width = 64,
            Height = 28,
            Font = new Font("Segoe UI", 8.2f)
        };

        _btnCopyLog = new ModernButton
        {
            Text = "Copy",
            Style = ButtonStyle.Secondary,
            Location = new Point(532, 8),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Width = 70,
            Height = 28,
            Font = new Font("Segoe UI", 8.2f)
        };

        _rtbActivityLog = new RichTextBox
        {
            Location = new Point(16, 42),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Width = 588,
            Height = 260,
            BackColor = ModernTheme.BackgroundDark,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.None,
            ReadOnly = true,
            Font = new Font("Consolas", 9f)
        };

        _cardConsole.Controls.Add(lblConsoleTitle);
        _cardConsole.Controls.Add(_btnClearLog);
        _cardConsole.Controls.Add(_btnCopyLog);
        _cardConsole.Controls.Add(_rtbActivityLog);
        pnlRightColumn.Controls.Add(_cardConsole);

        _tblMain.Controls.Add(pnlLeftColumn, 0, 0);
        _tblMain.Controls.Add(pnlRightColumn, 1, 0);

        Controls.Add(_tblMain);
        Controls.Add(_pnlBanner);
        Controls.Add(_pnlTopNav);

        ResumeLayout(false);
    }

    private Label AddDiagField(ModernCard card, string label, string value, ref int y, Color? valColor = null)
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

    #endregion
}
