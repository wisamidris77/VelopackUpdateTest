using System.Drawing.Drawing2D;

namespace VelopackUpdateTest.UI.Controls;

public static class ModernTheme
{
    // Dark Theme Palette
    public static readonly Color Background = Color.FromArgb(15, 23, 42);          // Slate 900
    public static readonly Color BackgroundDark = Color.FromArgb(10, 15, 30);      // Slate 950
    public static readonly Color CardBackground = Color.FromArgb(30, 41, 59);       // Slate 800
    public static readonly Color CardElevated = Color.FromArgb(51, 65, 85);         // Slate 700
    public static readonly Color Border = Color.FromArgb(51, 65, 85);               // Slate 700
    public static readonly Color BorderSubtle = Color.FromArgb(39, 50, 71);
    public static readonly Color TextPrimary = Color.FromArgb(248, 250, 252);      // Slate 50
    public static readonly Color TextSecondary = Color.FromArgb(148, 163, 184);    // Slate 400
    public static readonly Color TextMuted = Color.FromArgb(100, 116, 139);        // Slate 500

    // Accent colors
    public static readonly Color Primary = Color.FromArgb(99, 102, 241);           // Indigo 500
    public static readonly Color PrimaryHover = Color.FromArgb(79, 70, 229);       // Indigo 600
    public static readonly Color PrimaryActive = Color.FromArgb(67, 56, 202);      // Indigo 700

    public static readonly Color Success = Color.FromArgb(16, 185, 129);           // Emerald 500
    public static readonly Color SuccessHover = Color.FromArgb(5, 150, 105);
    public static readonly Color SuccessBg = Color.FromArgb(6, 78, 59);

    public static readonly Color Warning = Color.FromArgb(245, 158, 11);           // Amber 500
    public static readonly Color WarningBg = Color.FromArgb(120, 53, 15);

    public static readonly Color Danger = Color.FromArgb(239, 68, 68);             // Rose 500
    public static readonly Color DangerHover = Color.FromArgb(220, 38, 38);
    public static readonly Color DangerBg = Color.FromArgb(127, 29, 29);

    public static readonly Color AccentPurple = Color.FromArgb(168, 85, 247);      // Purple 500

    public static GraphicsPath GetRoundedRectangle(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        int diameter = radius * 2;
        var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

        // Top Left
        path.AddArc(arc, 180, 90);

        // Top Right
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);

        // Bottom Right
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);

        // Bottom Left
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);

        path.CloseFigure();
        return path;
    }
}
