using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using VelopackUpdateTest.Models;
using VelopackUpdateTest.Services;
using VelopackUpdateTest.UI.Controls;

namespace VelopackUpdateTest.UI;

public class UpdateDialogForm : Form
{
    private readonly ReleaseInfo _targetRelease;
    private CancellationTokenSource? _downloadCts;

    // UI Controls
    private Panel _pnlHeader = null!;
    private Label _lblAppTitle = null!;
    private Label _lblHeaderSubtitle = null!;
    private ModernButton _btnCloseHeader = null!;

    private ModernCard _cardHero = null!;
    private ModernBadge _badgeStatus = null!;
    private Label _lblVersionTitle = null!;
    private Label _lblVersionFlow = null!;
    private Label _lblReleaseDateAndSize = null!;

    private ModernCard? _cardMandatoryNotice;
    private Label? _lblMandatoryText;

    private ModernCard _cardChangelog = null!;
    private RichTextBox _rtbChangelog = null!;

    private ModernCard _cardProgress = null!;
    private Label _lblProgressPhase = null!;
    private Label _lblProgressStats = null!;
    private ModernProgressBar _progressBar = null!;

    private Panel _pnlFooter = null!;
    private ModernBadge _badgeType = null!;
    private ModernButton _btnLater = null!;
    private ModernButton _btnUpdate = null!;

    private bool _isDownloading = false;
    private bool _isDownloaded = false;

    public UpdateDialogForm(ReleaseInfo targetRelease)
    {
        _targetRelease = targetRelease;

        InitializeComponent();
        ApplyReleaseData();
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        Text = _targetRelease.IsMandatory
            ? "🚨 Required Update Available - VelopackUpdateTest"
            : $"Update Available - {_targetRelease.Title}";

        BackColor = ModernTheme.Background;
        ForeColor = ModernTheme.TextPrimary;
        Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = true;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(650, 700);

        // Header Panel
        _pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 64,
            BackColor = ModernTheme.BackgroundDark,
            Padding = new Padding(20, 12, 20, 12)
        };

        _lblAppTitle = new Label
        {
            Text = "VelopackUpdateTest",
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = ModernTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 14)
        };

        _lblHeaderSubtitle = new Label
        {
            Text = _targetRelease.IsMandatory
                ? "Mandatory update required by policy"
                : "A new version has been published on GitHub",
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = _targetRelease.IsMandatory ? ModernTheme.Danger : ModernTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(20, 36)
        };

        _btnCloseHeader = new ModernButton
        {
            Text = "✕",
            Style = ButtonStyle.Secondary,
            Size = new Size(32, 32),
            Location = new Point(ClientSize.Width - 52, 16),
            BorderRadius = 6,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold)
        };
        _btnCloseHeader.Click += (s, e) => HandleCloseAttempt();

        _pnlHeader.Controls.Add(_lblAppTitle);
        _pnlHeader.Controls.Add(_lblHeaderSubtitle);
        _pnlHeader.Controls.Add(_btnCloseHeader);

        // Footer Panel
        _pnlFooter = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 68,
            BackColor = ModernTheme.BackgroundDark,
            Padding = new Padding(20, 14, 20, 14)
        };

        _badgeType = new ModernBadge
        {
            BadgeText = _targetRelease.IsDelta ? "1-Version Delta (~500 KB)" : "Full Installer (~50 MB)",
            BadgeColor = _targetRelease.IsDelta ? ModernTheme.Success : ModernTheme.Primary,
            Width = 190,
            Location = new Point(20, 20)
        };

        _btnLater = new ModernButton
        {
            Text = _targetRelease.IsMandatory ? "Skip (Locked)" : "Remind Me Later",
            Style = ButtonStyle.Secondary,
            Width = 140,
            Height = 38,
            Location = new Point(350, 15),
            Enabled = !_targetRelease.IsMandatory
        };
        _btnLater.Click += (s, e) => Close();

        _btnUpdate = new ModernButton
        {
            Text = _targetRelease.IsMandatory ? "🚨 Install Required Update" : "Update Now",
            Style = _targetRelease.IsMandatory ? ButtonStyle.Danger : ButtonStyle.Primary,
            Width = 140,
            Height = 38,
            Location = new Point(500, 15)
        };
        _btnUpdate.Click += async (s, e) => await HandleUpdateClickAsync();

        _pnlFooter.Controls.Add(_badgeType);
        _pnlFooter.Controls.Add(_btnLater);
        _pnlFooter.Controls.Add(_btnUpdate);

        // Content Scrollable Container
        var pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(20, 14, 20, 14),
            BackColor = ModernTheme.Background
        };

        int curY = 14;

        // Hero Card
        _cardHero = new ModernCard
        {
            Location = new Point(20, curY),
            Width = 590,
            Height = 110,
            CardColor = ModernTheme.CardBackground,
            BorderColor = _targetRelease.IsMandatory ? ModernTheme.Danger : ModernTheme.Border
        };

        _badgeStatus = new ModernBadge
        {
            BadgeText = _targetRelease.IsMandatory ? "CRITICAL MANDATORY UPDATE" : "NEW RELEASE READY",
            BadgeColor = _targetRelease.IsMandatory ? ModernTheme.Danger : ModernTheme.Primary,
            Width = 230,
            Location = new Point(16, 12)
        };

        // Use the exact GitHub Release Name for the title!
        _lblVersionTitle = new Label
        {
            Text = _targetRelease.Title,
            Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(16, 42)
        };

        _lblVersionFlow = new Label
        {
            Text = $"Current: v{VelopackUpdateService.Instance.CurrentVersion}  ──►  Target: v{_targetRelease.Version}",
            Font = new Font("Segoe UI", 8.8f, FontStyle.Bold),
            ForeColor = ModernTheme.Primary,
            AutoSize = true,
            Location = new Point(16, 72)
        };

        _lblReleaseDateAndSize = new Label
        {
            Text = $"Size: {_targetRelease.SizeDisplay}  •  Released: {_targetRelease.ReleaseDate:MMM dd, yyyy}",
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = ModernTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(320, 72)
        };

        _cardHero.Controls.Add(_badgeStatus);
        _cardHero.Controls.Add(_lblVersionTitle);
        _cardHero.Controls.Add(_lblVersionFlow);
        _cardHero.Controls.Add(_lblReleaseDateAndSize);
        pnlContent.Controls.Add(_cardHero);
        curY += 122;

        // Mandatory Notice Card (if mandatory)
        if (_targetRelease.IsMandatory)
        {
            _cardMandatoryNotice = new ModernCard
            {
                Location = new Point(20, curY),
                Width = 590,
                Height = 70,
                CardColor = ModernTheme.DangerBg,
                BorderColor = ModernTheme.Danger
            };

            _lblMandatoryText = new Label
            {
                Text = $"⚠️ Required Update: {_targetRelease.MandatoryReason}\nThis version is mandatory to ensure application integrity and connectivity.",
                Font = new Font("Segoe UI", 8.8f, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new Point(14, 12),
                Size = new Size(560, 46)
            };

            _cardMandatoryNotice.Controls.Add(_lblMandatoryText);
            pnlContent.Controls.Add(_cardMandatoryNotice);
            curY += 80;
        }

        // Changelog Card (Displays GitHub Release Description)
        _cardChangelog = new ModernCard
        {
            Location = new Point(20, curY),
            Width = 590,
            Height = 230,
            CardColor = ModernTheme.CardBackground,
            BorderColor = ModernTheme.Border
        };

        var lblChangelogTitle = new Label
        {
            Text = "RELEASE NOTES & CHANGELOG (FROM GITHUB)",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(16, 12),
            AutoSize = true
        };

        _rtbChangelog = new RichTextBox
        {
            Location = new Point(16, 36),
            Width = 558,
            Height = 178,
            BackColor = ModernTheme.CardBackground,
            ForeColor = ModernTheme.TextPrimary,
            BorderStyle = BorderStyle.None,
            ReadOnly = true,
            ScrollBars = RichTextBoxScrollBars.Vertical,
            Font = new Font("Segoe UI", 9.5f)
        };

        _cardChangelog.Controls.Add(lblChangelogTitle);
        _cardChangelog.Controls.Add(_rtbChangelog);
        pnlContent.Controls.Add(_cardChangelog);
        curY += 242;

        // Progress Card (revealed during download)
        _cardProgress = new ModernCard
        {
            Location = new Point(20, curY),
            Width = 590,
            Height = 84,
            CardColor = ModernTheme.BackgroundDark,
            BorderColor = ModernTheme.Border,
            Visible = false
        };

        _lblProgressPhase = new Label
        {
            Text = "Ready to download...",
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = ModernTheme.TextPrimary,
            Location = new Point(16, 12),
            AutoSize = true
        };

        _lblProgressStats = new Label
        {
            Text = "0% • 0 MB/s",
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = ModernTheme.TextSecondary,
            Location = new Point(440, 14),
            AutoSize = true
        };

        _progressBar = new ModernProgressBar
        {
            Location = new Point(16, 40),
            Width = 558,
            Height = 18,
            Value = 0,
            BarColor = _targetRelease.IsMandatory ? ModernTheme.Danger : ModernTheme.Primary,
            BarColorEnd = _targetRelease.IsMandatory ? ModernTheme.Warning : ModernTheme.AccentPurple,
            ShowPercentage = true
        };

        _cardProgress.Controls.Add(_lblProgressPhase);
        _cardProgress.Controls.Add(_lblProgressStats);
        _cardProgress.Controls.Add(_progressBar);
        pnlContent.Controls.Add(_cardProgress);

        Controls.Add(pnlContent);
        Controls.Add(_pnlHeader);
        Controls.Add(_pnlFooter);

        ResumeLayout(false);
    }

    private void ApplyReleaseData()
    {
        _rtbChangelog.Clear();

        // Release Title
        AppendFormattedLine($"🚀 {_targetRelease.Title}\n\n", Color.FromArgb(168, 85, 247), true, 11f);

        // Release Description (Changelog content from GitHub)
        AppendFormattedLine($"{_targetRelease.Summary}\n\n", ModernTheme.TextPrimary, false, 9.5f);

        if (_targetRelease.Features.Count > 0)
        {
            AppendFormattedLine("HIGHLIGHTS:\n", ModernTheme.Success, true, 9.5f);
            foreach (var f in _targetRelease.Features)
            {
                AppendFormattedLine($"  • {f}\n", ModernTheme.TextSecondary, false, 9f);
            }
        }
    }

    private void AppendFormattedLine(string text, Color color, bool bold, float size)
    {
        _rtbChangelog.SelectionStart = _rtbChangelog.TextLength;
        _rtbChangelog.SelectionLength = 0;
        _rtbChangelog.SelectionColor = color;
        _rtbChangelog.SelectionFont = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular);
        _rtbChangelog.AppendText(text);
        _rtbChangelog.SelectionColor = _rtbChangelog.ForeColor;
    }

    private async Task HandleUpdateClickAsync()
    {
        if (_isDownloaded)
        {
            _btnUpdate.Enabled = false;
            _btnUpdate.Text = "Restarting...";
            VelopackUpdateService.Instance.RestartAndApply();
            return;
        }

        if (_isDownloading) return;

        _isDownloading = true;
        _btnUpdate.Enabled = false;
        _btnLater.Enabled = false;
        _cardProgress.Visible = true;
        _progressBar.Value = 0;

        _downloadCts = new CancellationTokenSource();

        try
        {
            bool success = await VelopackUpdateService.Instance.DownloadUpdateAsync(
                _targetRelease,
                (percent, phase, speed) =>
                {
                    if (InvokeRequired)
                    {
                        Invoke(() => UpdateProgressUi(percent, phase, speed));
                    }
                    else
                    {
                        UpdateProgressUi(percent, phase, speed);
                    }
                },
                _downloadCts.Token);

            if (success)
            {
                _isDownloaded = true;
                _isDownloading = false;
                _lblProgressPhase.Text = "✅ Update verified clean with SHA-256 and ready to apply!";
                _lblProgressPhase.ForeColor = ModernTheme.Success;
                _lblProgressStats.Text = "100% Complete";

                _btnUpdate.Text = "🚀 Restart & Apply";
                _btnUpdate.Style = ButtonStyle.Success;
                _btnUpdate.Enabled = true;

                if (_targetRelease.IsMandatory)
                {
                    MessageBox.Show(
                        this,
                        $"Mandatory update for {_targetRelease.Title} downloaded successfully.\nClick 'Restart & Apply' to finalize installation.",
                        "Update Ready",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _isDownloading = false;
            _btnUpdate.Enabled = true;
            _btnLater.Enabled = !_targetRelease.IsMandatory;
            _lblProgressPhase.Text = "Download cancelled.";
            _lblProgressPhase.ForeColor = ModernTheme.Warning;
        }
        catch (Exception ex)
        {
            _isDownloading = false;
            _btnUpdate.Enabled = true;
            _btnLater.Enabled = !_targetRelease.IsMandatory;
            _lblProgressPhase.Text = "❌ Download encountered an issue.";
            _lblProgressPhase.ForeColor = ModernTheme.Danger;

            MessageBox.Show(
                this,
                $"Update failed: {ex.Message}\n\nYou can retry the update anytime.",
                "Update Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void UpdateProgressUi(int percent, string phase, double speed)
    {
        _progressBar.Value = percent;
        _lblProgressPhase.Text = phase;
        _lblProgressStats.Text = $"{percent}% • {speed:0.0} MB/s";
    }

    private void HandleCloseAttempt()
    {
        if (_targetRelease.IsMandatory && !_isDownloaded)
        {
            var res = MessageBox.Show(
                this,
                $"⚠️ MANDATORY UPDATE REQUIRED\n\n{_targetRelease.Title} is required to continue.\nExiting will close the entire application.\n\nDo you want to exit the application?",
                "Mandatory Update",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (res == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
            return;
        }

        Close();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_targetRelease.IsMandatory && !_isDownloaded && e.CloseReason == CloseReason.UserClosing)
        {
            var res = MessageBox.Show(
                this,
                $"⚠️ MANDATORY UPDATE REQUIRED\n\nYou must update to continue using VelopackUpdateTest.\n\nDo you want to exit the application completely?",
                "Mandatory Update Required",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (res == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
                return;
            }
        }

        _downloadCts?.Cancel();
        base.OnFormClosing(e);
    }
}
