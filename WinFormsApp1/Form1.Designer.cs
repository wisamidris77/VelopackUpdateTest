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
    private Label _lblDiagLastCheck;
    private TextBox _txtRepoUrl;

    // Production Update Policies Card
    private ModernCard _cardPolicies;
    private CheckBox _chkAutoCheckGitHub;
    private ModernButton _btnManualCheck;

    // Real Changelog Explorer Card
    private ModernCard _cardChangelog;
    private Label _lblChangelogTitle;
    private Label _lblChangelogMeta;
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
        Text = "VelopackUpdateTest - Production Auto-Update Suite";
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
            Text = "Production Auto-Update & SHA-256 Integrity Verification Dashboard",
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
            BadgeText = "GitHub Feed Active",
            BadgeColor = ModernTheme.Success,
            Width = 160,
            Location = new Point(510, 22)
        };

        _btnQuickCheck = new ModernButton
        {
            Text = "⚡ Check for Updates",
            Style = ButtonStyle.Primary,
            Width = 160,
            Height = 36,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(ClientSize.Width - 185, 18)
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
            Height = 260
        };

        var lblDiagHeader = new Label
        {
            Text = "RUNTIME & UPDATE ENVIRONMENT",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, 12),
            AutoSize = true
        };

        int rowY = 38;
        _lblDiagAppId = AddDiagField(_cardDiagnostics, "App Package Id:", "VelopackUpdateTest", ref rowY);
        _lblDiagVersion = AddDiagField(_cardDiagnostics, "Current Installed Version:", "1.0.0", ref rowY);
        _lblDiagInstalled = AddDiagField(_cardDiagnostics, "Velopack Runtime Mode:", "Checking...", ref rowY, ModernTheme.Success);
        _lblDiagChannel = AddDiagField(_cardDiagnostics, "Update Feed Channel:", "stable", ref rowY);
        _lblDiagLastCheck = AddDiagField(_cardDiagnostics, "Last Update Check:", "Not yet checked", ref rowY, ModernTheme.TextMuted);

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

        // --- LEFT COLUMN: Card 2 Policies & Actions ---
        _cardPolicies = new ModernCard
        {
            Location = new Point(0, 274),
            Width = 440,
            Height = 410
        };

        var lblPoliciesHeader = new Label
        {
            Text = "DELTA UPDATE & SHA-256 POLICIES",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, 12),
            AutoSize = true
        };

        _chkAutoCheckGitHub = new CheckBox
        {
            Text = "Check for updates automatically on startup",
            Checked = true,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = ModernTheme.Success,
            Location = new Point(18, 38),
            AutoSize = true
        };

        _btnManualCheck = new ModernButton
        {
            Text = "🔍 Check for Updates Now",
            Style = ButtonStyle.Primary,
            Location = new Point(16, 74),
            Width = 408,
            Height = 42,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold)
        };

        var lblRulesTitle = new Label
        {
            Text = "AUTOMATED UPDATE LOGIC:",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, 132),
            AutoSize = true
        };

        var lblRulesText = new Label
        {
            Text = "1. Strict 1-Version Apart Policy:\n   • Exactly 1 version apart (e.g. 1.0.0 ➔ 1.1.0) downloads a tiny delta patch (~500 KB).\n   • Greater than 1 version apart (e.g. 1.0.0 ➔ 1.0.2) automatically enforces the clean full standalone setup.\n\n2. Pre-Delta SHA-256 Integrity Verification:\n   • Before any delta download, all existing local binaries on disk are scanned with SHA-256.\n   • If any local file is modified, corrupted, or missing, the delta is rejected and automatically switched to the full installer.",
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, 156),
            Size = new Size(408, 230)
        };

        _cardPolicies.Controls.Add(lblPoliciesHeader);
        _cardPolicies.Controls.Add(_chkAutoCheckGitHub);
        _cardPolicies.Controls.Add(_btnManualCheck);
        _cardPolicies.Controls.Add(lblRulesTitle);
        _cardPolicies.Controls.Add(lblRulesText);
        pnlLeftColumn.Controls.Add(_cardPolicies);

        // --- RIGHT COLUMN: Card 3 Changelog Explorer ---
        _cardChangelog = new ModernCard
        {
            Location = new Point(10, 0),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Width = 620,
            Height = 350
        };

        _lblChangelogTitle = new Label
        {
            Text = "LATEST GITHUB RELEASE & CHANGELOG",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, 12),
            AutoSize = true
        };

        _lblChangelogMeta = new Label
        {
            Text = "Synchronized from GitHub Releases feed",
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = ModernTheme.TextMuted,
            Location = new Point(16, 32),
            AutoSize = true
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
            Font = new Font("Segoe UI", 9.5f)
        };

        _cardChangelog.Controls.Add(_lblChangelogTitle);
        _cardChangelog.Controls.Add(_lblChangelogMeta);
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
            Text = "LIVE ACTIVITY & SHA-256 CONSOLE",
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
