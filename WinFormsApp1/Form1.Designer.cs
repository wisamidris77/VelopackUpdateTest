namespace VelopackUpdateTest;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    // In-App Notification Strip
    private Panel _pnlBanner;
    private Label _lblBannerText;
    private Button _btnBannerAction;
    private Button _btnBannerDismiss;

    // Status Group
    private GroupBox _grpStatus;
    private Label _lblDiagVersion;
    private Label _lblDiagInstalled;
    private Label _lblDiagChannel;
    private Label _lblDiagLastCheck;
    private TextBox _txtRepoUrl;
    private CheckBox _chkAutoCheckGitHub;
    private Button _btnCheckUpdates;

    // Changelog Group
    private GroupBox _grpChangelog;
    private Label _lblChangelogTitle;
    private Label _lblChangelogMeta;
    private TextBox _txtChangelogView;

    // Activity Log Group
    private GroupBox _grpLog;
    private TextBox _txtActivityLog;
    private Button _btnClearLog;
    private Button _btnCopyLog;

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
        SuspendLayout();

        // Form settings (Standard Classic Windows 7 UI)
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(780, 620);
        Text = "VelopackUpdateTest";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(720, 560);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);

        // 1. Top In-App Notification Banner (Classic Alert Strip)
        _pnlBanner = new Panel
        {
            Dock = DockStyle.Top,
            Height = 40,
            BackColor = SystemColors.Info,
            ForeColor = SystemColors.InfoText,
            BorderStyle = BorderStyle.FixedSingle,
            Visible = false,
            Padding = new Padding(10, 6, 10, 6)
        };

        _lblBannerText = new Label
        {
            Text = "A new update is available.",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(10, 10)
        };

        _btnBannerAction = new Button
        {
            Text = "Update Now",
            Size = new Size(95, 26),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(ClientSize.Width - 150, 6),
            UseVisualStyleBackColor = true
        };

        _btnBannerDismiss = new Button
        {
            Text = "✕",
            Size = new Size(28, 26),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(ClientSize.Width - 45, 6),
            UseVisualStyleBackColor = true
        };

        _pnlBanner.Controls.Add(_lblBannerText);
        _pnlBanner.Controls.Add(_btnBannerAction);
        _pnlBanner.Controls.Add(_btnBannerDismiss);

        // Main Container Panel
        var pnlContainer = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            AutoScroll = true
        };

        // 2. Status GroupBox
        _grpStatus = new GroupBox
        {
            Text = "Application & Update Status",
            Location = new Point(12, 10),
            Width = 740,
            Height = 150,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        var lblVerKey = new Label { Text = "Current Version:", Location = new Point(16, 26), AutoSize = true };
        _lblDiagVersion = new Label { Text = "1.0.0", Font = new Font("Segoe UI", 9F, FontStyle.Bold), Location = new Point(130, 26), AutoSize = true };

        var lblModeKey = new Label { Text = "Runtime Status:", Location = new Point(16, 50), AutoSize = true };
        _lblDiagInstalled = new Label { Text = "Checking...", Location = new Point(130, 50), AutoSize = true };

        var lblChannelKey = new Label { Text = "Release Channel:", Location = new Point(360, 26), AutoSize = true };
        _lblDiagChannel = new Label { Text = "stable", Location = new Point(480, 26), AutoSize = true };

        var lblCheckKey = new Label { Text = "Last Checked:", Location = new Point(360, 50), AutoSize = true };
        _lblDiagLastCheck = new Label { Text = "Never", Location = new Point(480, 50), AutoSize = true };

        var lblRepoKey = new Label { Text = "GitHub Source:", Location = new Point(16, 80), AutoSize = true };
        _txtRepoUrl = new TextBox
        {
            Text = "https://github.com/wisamidris77/VelopackUpdateTest",
            Location = new Point(130, 77),
            Width = 430
        };

        _btnCheckUpdates = new Button
        {
            Text = "Check for Updates",
            Location = new Point(570, 75),
            Size = new Size(150, 26),
            UseVisualStyleBackColor = true
        };

        _chkAutoCheckGitHub = new CheckBox
        {
            Text = "Automatically check for updates on application startup",
            Checked = true,
            Location = new Point(130, 110),
            AutoSize = true
        };

        _grpStatus.Controls.Add(lblVerKey);
        _grpStatus.Controls.Add(_lblDiagVersion);
        _grpStatus.Controls.Add(lblModeKey);
        _grpStatus.Controls.Add(_lblDiagInstalled);
        _grpStatus.Controls.Add(lblChannelKey);
        _grpStatus.Controls.Add(_lblDiagChannel);
        _grpStatus.Controls.Add(lblCheckKey);
        _grpStatus.Controls.Add(_lblDiagLastCheck);
        _grpStatus.Controls.Add(lblRepoKey);
        _grpStatus.Controls.Add(_txtRepoUrl);
        _grpStatus.Controls.Add(_btnCheckUpdates);
        _grpStatus.Controls.Add(_chkAutoCheckGitHub);
        pnlContainer.Controls.Add(_grpStatus);

        // 3. Changelog GroupBox
        _grpChangelog = new GroupBox
        {
            Text = "Release Information & Changelog",
            Location = new Point(12, 168),
            Width = 740,
            Height = 220,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        _lblChangelogTitle = new Label
        {
            Text = "Release: Current (v1.0.0)",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Location = new Point(16, 24),
            AutoSize = true
        };

        _lblChangelogMeta = new Label
        {
            Text = "No pending updates available.",
            ForeColor = SystemColors.GrayText,
            Location = new Point(16, 44),
            AutoSize = true
        };

        _txtChangelogView = new TextBox
        {
            Location = new Point(16, 68),
            Width = 708,
            Height = 138,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = SystemColors.Window,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        _grpChangelog.Controls.Add(_lblChangelogTitle);
        _grpChangelog.Controls.Add(_lblChangelogMeta);
        _grpChangelog.Controls.Add(_txtChangelogView);
        pnlContainer.Controls.Add(_grpChangelog);

        // 4. Activity Log GroupBox
        _grpLog = new GroupBox
        {
            Text = "Update Log & SHA-256 Verification",
            Location = new Point(12, 396),
            Width = 740,
            Height = 190,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        _txtActivityLog = new TextBox
        {
            Location = new Point(16, 24),
            Width = 708,
            Height = 125,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            BackColor = SystemColors.Window,
            Font = new Font("Consolas", 8.5f),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        _btnClearLog = new Button
        {
            Text = "Clear",
            Size = new Size(75, 25),
            Location = new Point(568, 155),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            UseVisualStyleBackColor = true
        };

        _btnCopyLog = new Button
        {
            Text = "Copy",
            Size = new Size(75, 25),
            Location = new Point(649, 155),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            UseVisualStyleBackColor = true
        };

        _grpLog.Controls.Add(_txtActivityLog);
        _grpLog.Controls.Add(_btnClearLog);
        _grpLog.Controls.Add(_btnCopyLog);
        pnlContainer.Controls.Add(_grpLog);

        Controls.Add(pnlContainer);
        Controls.Add(_pnlBanner);

        ResumeLayout(false);
    }

    #endregion
}
