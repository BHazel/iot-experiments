using System.Text.Json.Serialization;

namespace BWHazel.Apps.IntelligentScene.Model;

/// <summary>
/// Represent a colour in RGB colour space.
/// </summary>
public struct RgbColour
{
    /// <summary>
    /// Gets or sets the red value.
    /// </summary>
    [JsonPropertyName("r")]
    public double R { get; set;}

    /// <summary>
    /// Gets or sets the green value.
    /// </summary>
    [JsonPropertyName("g")]
    public double G { get; set;}

    /// <summary>
    /// Gets or sets the blue value.
    /// </summary>
    [JsonPropertyName("b")]
    public double B { get; set;}
}