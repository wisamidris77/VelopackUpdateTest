using System.Drawing.Drawing2D;

namespace VelopackUpdateTest.UI.Controls;

public class ModernProgressBar : Control
{
    private int _value = 0;
    private int _maximum = 100;
    private Color _barColor = ModernTheme.Primary;
    private Color _barColorEnd = ModernTheme.AccentPurple;
    private bool _showPercentage = true;

    public int Value
    {
        get => _value;
        set
        {
            int clamped = Math.Clamp(value, 0, _maximum);
            if (_value != clamped)
            {
                _value = clamped;
                Invalidate();
            }
        }
    }

    public int Maximum
    {
        get => _maximum;
        set
        {
            if (value > 0)
            {
                _maximum = value;
                Invalidate();
            }
        }
    }

    public Color BarColor
    {
        get => _barColor;
        set { _barColor = value; Invalidate(); }
    }

    public Color BarColorEnd
    {
        get => _barColorEnd;
        set { _barColorEnd = value; Invalidate(); }
    }

    public bool ShowPercentage
    {
        get => _showPercentage;
        set { _showPercentage = value; Invalidate(); }
    }

    public int BorderRadius { get; set; } = 6;

    public ModernProgressBar()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint, true);

        DoubleBuffered = true;
        Height = 14;
        Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = ClientRectangle;
        rect.Width -= 1;
        rect.Height -= 1;

        if (rect.Width <= 0 || rect.Height <= 0) return;

        // Background track
        using (var trackPath = ModernTheme.GetRoundedRectangle(rect, BorderRadius))
        using (var trackBrush = new SolidBrush(Color.FromArgb(30, 41, 59)))
        using (var trackPen = new Pen(ModernTheme.Border, 1f))
        {
            g.FillPath(trackBrush, trackPath);
            g.DrawPath(trackPen, trackPath);
        }

        // Filled bar
        float percent = (float)_value / _maximum;
        int fillWidth = (int)(rect.Width * percent);

        if (fillWidth > 4)
        {
            var fillRect = new Rectangle(rect.X, rect.Y, fillWidth, rect.Height);
            using var fillPath = ModernTheme.GetRoundedRectangle(fillRect, BorderRadius);

            Color startCol = _value >= 100 ? ModernTheme.Success : _barColor;
            Color endCol = _value >= 100 ? Color.FromArgb(52, 211, 153) : _barColorEnd;

            using var brush = new LinearGradientBrush(
                new Point(rect.X, rect.Y),
                new Point(rect.Right, rect.Y),
                startCol,
                endCol);

            g.FillPath(brush, fillPath);
        }

        // Optional percentage text inside or above
        if (_showPercentage && Height >= 20)
        {
            string txt = $"{_value}%";
            TextRenderer.DrawText(g, txt, Font, rect, ModernTheme.TextPrimary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}
