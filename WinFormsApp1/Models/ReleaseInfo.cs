namespace VelopackUpdateTest.Models;

public record ReleaseInfo
{
    public required string Version { get; init; }
    public required string Title { get; init; }
    public required DateTime ReleaseDate { get; init; }
    public required string Summary { get; init; }
    public required string SizeDisplay { get; init; }
    public bool IsMandatory { get; init; }
    public string? MandatoryReason { get; init; }
    public string Channel { get; init; } = "stable";
    public List<string> Features { get; init; } = [];
    public List<string> Improvements { get; init; } = [];
    public List<string> Fixes { get; init; } = [];
    public List<string> SecurityUpdates { get; init; } = [];

    public string ToMarkdown()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"# {Title} ({Version})");
        sb.AppendLine($"*Released: {ReleaseDate:yyyy-MM-dd} • Channel: {Channel} • Size: {SizeDisplay}*");
        sb.AppendLine();
        sb.AppendLine(Summary);
        sb.AppendLine();

        if (IsMandatory)
        {
            sb.AppendLine("> [!IMPORTANT]");
            sb.AppendLine($"> **MANDATORY UPDATE**: {MandatoryReason ?? "This version is required to continue using VelopackUpdateTest."}");
            sb.AppendLine();
        }

        if (Features.Count > 0)
        {
            sb.AppendLine("### 🚀 New Features");
            foreach (var f in Features) sb.AppendLine($"- {f}");
            sb.AppendLine();
        }

        if (Improvements.Count > 0)
        {
            sb.AppendLine("### ⚡ Improvements & Performance");
            foreach (var imp in Improvements) sb.AppendLine($"- {imp}");
            sb.AppendLine();
        }

        if (Fixes.Count > 0)
        {
            sb.AppendLine("### 🐛 Bug Fixes");
            foreach (var fix in Fixes) sb.AppendLine($"- {fix}");
            sb.AppendLine();
        }

        if (SecurityUpdates.Count > 0)
        {
            sb.AppendLine("### 🔒 Security Patches");
            foreach (var sec in SecurityUpdates) sb.AppendLine($"- {sec}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static readonly ReleaseInfo CurrentVersion = new()
    {
        Version = "1.0.0",
        Title = "VelopackUpdateTest Initial Release",
        ReleaseDate = new DateTime(2026, 10, 19),
        Summary = "Initial release of VelopackUpdateTest featuring full Velopack integration, interactive update simulation, diagnostic dashboards, and GitHub Actions CI/CD automation.",
        SizeDisplay = "22.4 MB",
        IsMandatory = false,
        Features =
        [
            "Full Velopack integration with VelopackApp.Build().Run() bootstrap",
            "Real UpdateManager feed checking against GitHub Releases",
            "Complete update simulation engine for testing without publishing",
            "Cool modern update modal with changelog, animated progress, and forced update enforcement",
            "High-DPI double-buffered dark UI components"
        ],
        Improvements =
        [
            "Silky smooth WinForms rendering with hardware-accelerated drawing",
            "Instantaneous diagnostic information retrieval for Velopack locator"
        ],
        Fixes =
        [
            "Initial base build with zero known issues"
        ]
    };

    public static readonly ReleaseInfo ReleaseV110 = new()
    {
        Version = "1.1.0",
        Title = "Feature Pack: Cloud Sync & Faster Delta Updates",
        ReleaseDate = new DateTime(2026, 10, 25),
        Summary = "A feature-packed minor update introducing delta compression improvements, dark mode polish, and cloud sync telemetry.",
        SizeDisplay = "4.2 MB (Delta)",
        IsMandatory = false,
        Features =
        [
            "Enhanced Delta compression reducing update download payloads by up to 70%",
            "Background update checker with configurable intervals and toast notifications",
            "Exportable update diagnostics reports in JSON and Markdown formats"
        ],
        Improvements =
        [
            "35% faster package verification checksum calculations",
            "Refined progress bar easing animations with smooth velocity interpolation",
            "Lower memory footprint during update decompression"
        ],
        Fixes =
        [
            "Resolved edge-case where network drop during delta chunking required full restart",
            "Fixed DPI scale artifacting on multi-monitor setups with different refresh rates"
        ]
    };

    public static readonly ReleaseInfo ReleaseV200Mandatory = new()
    {
        Version = "2.0.0",
        Title = "Major Release: Critical Security & Protocol Overhaul",
        ReleaseDate = new DateTime(2026, 11, 01),
        Summary = "CRITICAL MANDATORY UPDATE: VelopackUpdateTest v2.0.0 introduces a brand new cryptographic handshake and API backend. Older clients will be disconnected from backend services.",
        SizeDisplay = "28.6 MB",
        IsMandatory = true,
        MandatoryReason = "This update patches a critical protocol vulnerability and migrates to our new high-speed distribution nodes. You must update to version 2.0.0 to continue using the application.",
        Features =
        [
            "New Next-Gen End-to-End Cryptographic Handshake (v2 Protocol)",
            "Automated rollback safeguard in case of unexpected host OS crash during install",
            "Zero-downtime hot swap update apply mechanism"
        ],
        Improvements =
        [
            "Rewritten IPC communication bus between application and Velopack helper processes",
            "Optimized startup latency by deferring non-critical asset warmups"
        ],
        Fixes =
        [
            "Patched critical buffer validation vulnerability in legacy network handler",
            "Fixed potential lock contention when checking updates on application shutdown"
        ],
        SecurityUpdates =
        [
            "CVE-2026-9901: Remote metadata parsing boundary verification enforcement",
            "Upgraded hash validation to SHA-384 with secondary signature validation"
        ]
    };

    public static readonly ReleaseInfo ReleaseV101Patch = new()
    {
        Version = "1.0.1",
        Title = "Maintenance & Stability Patch",
        ReleaseDate = new DateTime(2026, 10, 20),
        Summary = "A fast delta patch addressing minor visual glitches and improving update check response times.",
        SizeDisplay = "1.8 MB (Delta)",
        IsMandatory = false,
        Improvements =
        [
            "Reduced HTTP connection timeout for update servers",
            "Improved button hover state contrast"
        ],
        Fixes =
        [
            "Fixed minor tooltip positioning glitch on high resolution monitors"
        ]
    };

    public static readonly IReadOnlyList<ReleaseInfo> AllReleases =
    [
        ReleaseV200Mandatory,
        ReleaseV110,
        ReleaseV101Patch,
        CurrentVersion,
        new ReleaseInfo
        {
            Version = "0.9.0-beta",
            Title = "Public Beta Release",
            ReleaseDate = new DateTime(2026, 10, 10),
            Summary = "First public beta testing Velopack installer integration and GitHub workflows.",
            SizeDisplay = "21.0 MB",
            Features = ["Initial preview of Velopack packaging workflow"],
            Fixes = ["Fixed assembly manifest generation"]
        }
    ];
}
