using System.Text.Json.Serialization;

namespace BWHazel.Apps.IntelligentScene.Model;

/// <summary>
/// Information about a colour.
/// </summary>
public class ColourInfo
{
    /// <summary>
    /// Gets or sets the colour in XY colour space.
    /// </summary>
    [JsonPropertyName("xy")]
    public XyColour Xy { get; set; }

    /// <summary>
    /// Gets or sets the colour in RGB colour space.
    /// </summary>
    [JsonPropertyName("rgb")]
    public RgbColour Rgb { get; set; }

    /// <summary>
    /// Gets of sets the brightness.
    /// </summary>
    /// <remarks>This is a percentage value between 0 and 100.</remarks>
    [JsonPropertyName("brightness")]
    public double Brightness { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}