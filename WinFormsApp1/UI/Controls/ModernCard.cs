using System.Drawing.Drawing2D;

namespace VelopackUpdateTest.UI.Controls;

public class ModernCard : Panel
{
    public int BorderRadius { get; set; } = 12;
    public Color CardColor { get; set; } = ModernTheme.CardBackground;
    public Color BorderColor { get; set; } = ModernTheme.Border;
    public bool ShowBorder { get; set; } = true;

    public ModernCard()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor |
                 ControlStyles.UserPaint, true);

        DoubleBuffered = true;
        BackColor = Color.Transparent;
        Padding = new Padding(16);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = ClientRectangle;
        rect.Width -= 1;
        rect.Height -= 1;

        if (rect.Width <= 0 || rect.Height <= 0) return;

        using var path = ModernTheme.GetRoundedRectangle(rect, BorderRadius);

        // Fill background
        using (var brush = new SolidBrush(CardColor))
        {
            g.FillPath(brush, path);
        }

        // Draw border
        if (ShowBorder)
        {
            using var pen = new Pen(BorderColor, 1.2f);
            g.DrawPath(pen, path);
        }
    }
}
