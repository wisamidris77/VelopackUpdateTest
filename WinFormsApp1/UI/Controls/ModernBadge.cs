using System.Drawing.Drawing2D;

namespace VelopackUpdateTest.UI.Controls;

public class ModernBadge : Control
{
    private string _badgeText = "Badge";
    private Color _badgeColor = ModernTheme.Primary;
    private Color _textColor = ModernTheme.TextPrimary;
    private bool _showDot = true;

    public string BadgeText
    {
        get => _badgeText;
        set { _badgeText = value; Invalidate(); }
    }

    public Color BadgeColor
    {
        get => _badgeColor;
        set { _badgeColor = value; Invalidate(); }
    }

    public Color TextColor
    {
        get => _textColor;
        set { _textColor = value; Invalidate(); }
    }

    public bool ShowDot
    {
        get => _showDot;
        set { _showDot = value; Invalidate(); }
    }

    public ModernBadge()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor |
                 ControlStyles.UserPaint, true);

        DoubleBuffered = true;
        Height = 26;
        Width = 110;
        Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var rect = ClientRectangle;
        rect.Width -= 1;
        rect.Height -= 1;

        if (rect.Width <= 0 || rect.Height <= 0) return;

        int radius = rect.Height / 2;
        using var path = ModernTheme.GetRoundedRectangle(rect, radius);

        // Fill background with low alpha
        Color fillBg = Color.FromArgb(40, _badgeColor.R, _badgeColor.G, _badgeColor.B);
        using (var brush = new SolidBrush(fillBg))
        {
            g.FillPath(brush, path);
        }

        // Draw border
        using (var pen = new Pen(Color.FromArgb(120, _badgeColor), 1.2f))
        {
            g.DrawPath(pen, path);
        }

        int startX = 12;
        if (_showDot)
        {
            int dotSize = 7;
            int dotY = (rect.Height - dotSize) / 2;
            using var dotBrush = new SolidBrush(_badgeColor);
            g.FillEllipse(dotBrush, startX, dotY, dotSize, dotSize);
            startX += dotSize + 6;
        }

        var textRect = new Rectangle(startX, 0, rect.Width - startX - 8, rect.Height);
        TextRenderer.DrawText(g, _badgeText, Font, textRect, _textColor,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }
}
