using System.Text.Json.Serialization;

namespace BWHazel.Apps.IntelligentScene.Model;

/// <summary>
/// Represents a colour in XY colour space.
/// </summary>
public struct XyColour
{
    /// <summary>
    /// Gets or sets the X value.
    /// </summary>
    [JsonPropertyName("x")]
    public double X { get; set; }

    /// <summary>
    /// Gets or sets the Y value.
    /// </summary>
    [JsonPropertyName("y")]
    public double Y { get; set; }
}