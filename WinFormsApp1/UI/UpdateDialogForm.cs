using System.Diagnostics;
using VelopackUpdateTest.Models;
using VelopackUpdateTest.Services;

namespace VelopackUpdateTest.UI;

public class UpdateDialogForm : Form
{
    private readonly ReleaseInfo _targetRelease;
    private CancellationTokenSource? _downloadCts;

    // Controls
    private PictureBox _picIcon = null!;
    private Label _lblHeading = null!;
    private Label _lblReleaseTitle = null!;
    private Label _lblReleaseMeta = null!;
    private Label _lblNotesHeader = null!;
    private TextBox _txtNotes = null!;

    private Label _lblProgressStatus = null!;
    private ProgressBar _progressBar = null!;

    private Panel _pnlBottom = null!;
    private Button _btnCancel = null!;
    private Button _btnUpdate = null!;

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

        // Classic Windows 7 Dialog styling
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(540, 480);
        Text = _targetRelease.IsMandatory ? "Required Update - VelopackUpdateTest" : "Software Update";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = true;
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        BackColor = SystemColors.Window; // Classic top white dialog area

        // Icon (System icon)
        _picIcon = new PictureBox
        {
            Location = new Point(20, 20),
            Size = new Size(32, 32),
            SizeMode = PictureBoxSizeMode.CenterImage,
            Image = _targetRelease.IsMandatory
                ? SystemIcons.Shield.ToBitmap()
                : SystemIcons.Information.ToBitmap()
        };

        // Heading
        _lblHeading = new Label
        {
            Text = _targetRelease.IsMandatory ? "A mandatory update is required." : "A new version of VelopackUpdateTest is available.",
            Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
            ForeColor = _targetRelease.IsMandatory ? Color.FromArgb(180, 0, 0) : Color.FromArgb(0, 51, 153), // Windows classic blue header
            Location = new Point(64, 18),
            AutoSize = true
        };

        // Release Title (exact GitHub release name: e.g. "v1.1.0 - the small update")
        _lblReleaseTitle = new Label
        {
            Text = _targetRelease.Title,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Location = new Point(64, 44),
            AutoSize = true
        };

        // Meta info
        _lblReleaseMeta = new Label
        {
            Text = $"Current version: v{VelopackUpdateService.Instance.CurrentVersion}   ➔   New version: v{_targetRelease.Version} ({_targetRelease.SizeDisplay})",
            ForeColor = SystemColors.GrayText,
            Location = new Point(64, 66),
            AutoSize = true
        };

        // Notes Label
        _lblNotesHeader = new Label
        {
            Text = "Release notes:",
            Location = new Point(20, 96),
            AutoSize = true
        };

        // Changelog TextBox (Classic Multiline)
        _txtNotes = new TextBox
        {
            Location = new Point(20, 118),
            Width = 500,
            Height = 200,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = SystemColors.Window,
            Font = new Font("Segoe UI", 9F)
        };

        // Progress controls (Classic Windows 7 ProgressBar)
        _lblProgressStatus = new Label
        {
            Text = "Ready to download update...",
            Location = new Point(20, 328),
            Width = 500,
            Visible = false
        };

        _progressBar = new ProgressBar
        {
            Location = new Point(20, 350),
            Width = 500,
            Height = 22,
            Style = ProgressBarStyle.Continuous,
            Visible = false
        };

        // Bottom Button Bar (Classic SystemColors.Control gray panel)
        _pnlBottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 48,
            BackColor = SystemColors.Control,
            BorderStyle = BorderStyle.FixedSingle
        };

        _btnCancel = new Button
        {
            Text = _targetRelease.IsMandatory ? "Cancel (Locked)" : "Cancel",
            Size = new Size(85, 26),
            Location = new Point(340, 10),
            UseVisualStyleBackColor = true,
            Enabled = !_targetRelease.IsMandatory
        };
        _btnCancel.Click += (s, e) => Close();

        _btnUpdate = new Button
        {
            Text = _targetRelease.IsMandatory ? "Install Update" : "Install Update",
            Size = new Size(95, 26),
            Location = new Point(435, 10),
            UseVisualStyleBackColor = true
        };
        _btnUpdate.Click += async (s, e) => await HandleUpdateClickAsync();

        _pnlBottom.Controls.Add(_btnCancel);
        _pnlBottom.Controls.Add(_btnUpdate);

        Controls.Add(_picIcon);
        Controls.Add(_lblHeading);
        Controls.Add(_lblReleaseTitle);
        Controls.Add(_lblReleaseMeta);
        Controls.Add(_lblNotesHeader);
        Controls.Add(_txtNotes);
        Controls.Add(_lblProgressStatus);
        Controls.Add(_progressBar);
        Controls.Add(_pnlBottom);

        AcceptButton = _btnUpdate;
        CancelButton = _btnCancel;

        ResumeLayout(false);
    }

    private void ApplyReleaseData()
    {
        _txtNotes.Text = _targetRelease.Summary + Environment.NewLine + Environment.NewLine;

        if (_targetRelease.Features.Count > 0)
        {
            _txtNotes.AppendText("Highlights:" + Environment.NewLine);
            foreach (var f in _targetRelease.Features)
            {
                _txtNotes.AppendText($" • {f}" + Environment.NewLine);
            }
        }
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
        _btnCancel.Enabled = false;
        _lblProgressStatus.Visible = true;
        _progressBar.Visible = true;
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
                _lblProgressStatus.Text = "Update downloaded and verified clean. Ready to install.";
                _progressBar.Value = 100;

                _btnUpdate.Text = "Restart & Apply";
                _btnUpdate.Enabled = true;

                if (_targetRelease.IsMandatory)
                {
                    MessageBox.Show(
                        this,
                        $"The required update ({_targetRelease.Title}) has been downloaded.\nClick 'Restart & Apply' to complete the installation.",
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
            _btnCancel.Enabled = !_targetRelease.IsMandatory;
            _lblProgressStatus.Text = "Download cancelled.";
        }
        catch (Exception ex)
        {
            _isDownloading = false;
            _btnUpdate.Enabled = true;
            _btnCancel.Enabled = !_targetRelease.IsMandatory;
            _lblProgressStatus.Text = "Download failed: " + ex.Message;

            MessageBox.Show(
                this,
                $"An error occurred while downloading the update:\n{ex.Message}\n\nPlease try again later.",
                "Update Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void UpdateProgressUi(int percent, string phase, double speed)
    {
        _progressBar.Value = Math.Clamp(percent, 0, 100);
        _lblProgressStatus.Text = $"{phase} - {speed:0.0} MB/s";
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_targetRelease.IsMandatory && !_isDownloaded && e.CloseReason == CloseReason.UserClosing)
        {
            var res = MessageBox.Show(
                this,
                $"The update '{_targetRelease.Title}' is mandatory to continue using this application.\n\nDo you want to exit the application?",
                "Mandatory Update",
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
