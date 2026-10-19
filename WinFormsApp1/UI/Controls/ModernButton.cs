using System.Drawing.Drawing2D;

namespace VelopackUpdateTest.UI.Controls;

public enum ButtonStyle
{
    Primary,
    Secondary,
    Success,
    Danger
}

public class ModernButton : Button
{
    private bool _isHovered;
    private bool _isPressed;

    public ButtonStyle Style { get; set; } = ButtonStyle.Primary;
    public int BorderRadius { get; set; } = 8;
    public Color CustomColor { get; set; } = Color.Empty;

    public ModernButton()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor |
                 ControlStyles.UserPaint, true);

        DoubleBuffered = true;
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        BackColor = Color.Transparent;
        Cursor = Cursors.Hand;
        Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        ForeColor = Color.White;
        Height = 40;
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _isHovered = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _isHovered = false;
        _isPressed = false;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        base.OnMouseDown(mevent);
        _isPressed = true;
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        base.OnMouseUp(mevent);
        _isPressed = false;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        var g = pevent.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var rect = ClientRectangle;
        rect.Width -= 1;
        rect.Height -= 1;

        if (rect.Width <= 0 || rect.Height <= 0) return;

        Color bg = GetBackgroundColor();
        Color textColor = Enabled ? ForeColor : ModernTheme.TextMuted;

        using var path = ModernTheme.GetRoundedRectangle(rect, BorderRadius);

        // Fill background
        using (var brush = new SolidBrush(bg))
        {
            g.FillPath(brush, path);
        }

        // Draw border for secondary button
        if (Style == ButtonStyle.Secondary)
        {
            Color borderColor = _isHovered ? ModernTheme.Primary : ModernTheme.Border;
            using var pen = new Pen(borderColor, 1.2f);
            g.DrawPath(pen, path);
        }

        // Draw Text
        var textRect = ClientRectangle;
        if (_isPressed)
        {
            textRect.Offset(0, 1);
        }

        TextRenderer.DrawText(g, Text, Font, textRect, textColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }

    private Color GetBackgroundColor()
    {
        if (!Enabled)
            return Color.FromArgb(40, 50, 70);

        if (CustomColor != Color.Empty)
            return _isPressed ? ControlPaint.Dark(CustomColor) : (_isHovered ? ControlPaint.Light(CustomColor) : CustomColor);

        return Style switch
        {
            ButtonStyle.Primary => _isPressed ? ModernTheme.PrimaryActive : (_isHovered ? ModernTheme.PrimaryHover : ModernTheme.Primary),
            ButtonStyle.Success => _isPressed ? ModernTheme.SuccessHover : (_isHovered ? ControlPaint.Light(ModernTheme.Success) : ModernTheme.Success),
            ButtonStyle.Danger => _isPressed ? Color.FromArgb(185, 28, 28) : (_isHovered ? ModernTheme.DangerHover : ModernTheme.Danger),
            ButtonStyle.Secondary => _isPressed ? Color.FromArgb(40, 52, 72) : (_isHovered ? Color.FromArgb(51, 65, 85) : ModernTheme.CardBackground),
            _ => ModernTheme.Primary
        };
    }
}
