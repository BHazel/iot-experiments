using System.Text.Json.Serialization;

namespace BWHazel.Apps.IntelligentScene.Model;

/// <summary>
/// Information about a scene.
/// </summary>
public class SceneInfo
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the colours.
    /// </summary>
    [JsonPropertyName("colours")]
    public ColourInfo[] Colours { get; set; } = [];
}