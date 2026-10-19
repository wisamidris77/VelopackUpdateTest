namespace VelopackUpdateTest.Models;

public record ReleaseInfo
{
    public required string Version { get; init; }
    public required string Title { get; init; }
    public required DateTime ReleaseDate { get; init; }
    public required string Summary { get; init; }
    public required string SizeDisplay { get; set; }
    public bool IsMandatory { get; init; }
    public string? MandatoryReason { get; init; }
    public string Channel { get; init; } = "stable";
    public List<string> Features { get; init; } = [];
    public string? DownloadUrl { get; set; }
    public string? FullDownloadUrl { get; set; }
    public string? DeltaDownloadUrl { get; set; }
    public string? InstallerPath { get; set; }
    public bool IsDelta { get; set; }
    public string? Sha256Checksum { get; init; }
}
