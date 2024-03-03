using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using BWHazel.Apps.IntelligentScene.Model;
using HueApi;
using HueApi.Models;
using HueApi.Models.Requests;
using static System.Console;

const string BridgeIpAddressConfigKey = "PhilipsHue:BridgeIpAddress";
const string AppUsernameConfigKey = "PhilipsHue:AppUsername";
const string ZoneConfigKey = "PhilipsHue:Zone";
const string OpenAiApiKeyConfigKey = "OpenAi:ApiKey";
const string OpenAiModelConfigKey = "OpenAi:Model";

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true, true)
    .Build();

ILoggerFactory loggerFactory =
    LoggerFactory.Create(
        builder => builder.AddSimpleConsole(options =>
        {
            options.SingleLine = true;
        }));

ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("Connecting to Hue Bridge on IP address {BridgeIpAddress}...", configuration[BridgeIpAddressConfigKey]);
LocalHueApi localHueApi;
try
{
    localHueApi = new(configuration[BridgeIpAddressConfigKey]!, configuration[AppUsernameConfigKey]);
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to connect to Hue Bridge.  Exiting...");
    return;
}

logger.LogInformation("Successfully connected to Hue Bridge.");
logger.LogInformation("Getting zones...");
List<Zone> zones = (await localHueApi.GetZonesAsync()).Data;
logger.LogInformation("{ZoneCount} zones found:", zones.Count);
zones.ForEach(zone => logger.LogInformation("- {ZoneName}", zone.Metadata!.Name));
logger.LogInformation("Getting zone '{Zone}' as specified in configuration...", configuration[ZoneConfigKey]);
Zone? zone = zones.FirstOrDefault(z => z.Metadata!.Name == configuration[ZoneConfigKey]!);
if (zone is null)
{
    logger.LogError("Zone '{Zone}' was not found.  Exiting...", configuration[ZoneConfigKey]);
    return;
}

logger.LogInformation("Zone '{Zone}' found.  Getting lights in zone...", zone.Metadata!.Name);
List<Light?> zoneLights = zone!.Children
    .Where(child => child.Rtype == "light")
    .Select(rid => localHueApi.GetLightAsync(rid.Rid))
    .Select(task => task.Result.Data.FirstOrDefault())
    .ToList();

logger.LogInformation("{LightCount} lights found:", zoneLights.Count);
zoneLights.ForEach(light => logger.LogInformation("- {LightName}", light!.Metadata!.Name));

logger.LogInformation("Building kernel with Open AI integration using model {OpenAiModel}...", configuration[OpenAiModelConfigKey]);
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(configuration[OpenAiModelConfigKey]!, configuration[OpenAiApiKeyConfigKey]!);
Kernel kernel = kernelBuilder.Build();
logger.LogInformation("Successfully built kernel.");

string pluginsPath = Path.Combine(Environment.CurrentDirectory, "Plugins");
logger.LogInformation("Loading kernel plugins from '{PluginsPath}'...", pluginsPath);
KernelPlugin kernelPlugins = kernel.ImportPluginFromPromptDirectory(pluginsPath);
logger.LogInformation("{PluginCount} plugins loaded:", kernelPlugins.Count());
kernelPlugins.ToList().ForEach(plugin => logger.LogInformation("- {PluginName}", plugin.Name));

logger.LogInformation("Starting user interaction prompt...");
WriteLine("*** Intelligent Scene ***");
WriteLine("* Scene Setup *");
string location = zone!.Metadata!.Name;
int lights = zoneLights.Count;
WriteLine($"Location: {location}");
WriteLine($"Lights: {lights}");

string situation = string.Empty;
bool exit = false;

do
{
    while (string.IsNullOrWhiteSpace(situation))
    {
        Write("Situation: ");
        situation = ReadLine() ?? string.Empty;
    }

    logger.LogInformation("Creating kernel arguments...");
    KernelArguments kernelArguments = new()
    {
        ["location"] = location,
        ["lights"] = lights,
        ["situation"] = situation
    };

    logger.LogInformation("Successfully created kernel arguments.  Invoking kernel plugin 'IntelligentScene'... ");
    FunctionResult intelligentSceneResult = await kernel.InvokeAsync(kernelPlugins["IntelligentScene"], kernelArguments);
    string intelligentSceneRaw = intelligentSceneResult.ToString();
    logger.LogWarning("Response contains Markdown markers for JSON.  Removing...");
    string intelligentScene = intelligentSceneRaw
        .Replace("```json", string.Empty)
        .Replace("```", string.Empty)
        .Trim();
    
    logger.LogInformation("Successfully removed Markdown JSON markers.");
    logger.LogInformation("JSON scene response from Open AI: {IntelligentScene}", intelligentScene);
    logger.LogInformation("Parsing JSON scene response...");
    SceneInfo? scene = JsonSerializer.Deserialize<SceneInfo>(intelligentScene);
    Dictionary<Light, ColourInfo> lightSettings = zoneLights
        .Zip(scene!.Colours, (light, colour) => new KeyValuePair<Light, ColourInfo>(light!, colour))
        .ToDictionary(lightColourPair => lightColourPair.Key, pair => pair.Value);

    logger.LogInformation("Setting lights to theme colours...");
    foreach (KeyValuePair<Light, ColourInfo> lightSetting in lightSettings)
    {
        Light light = lightSetting.Key;
        ColourInfo colour = lightSetting.Value;
        UpdateLight updateLightRequest = new UpdateLight()
            .TurnOn()
            .SetBrightness(colour.Brightness)
            .SetColor(colour.Xy.X, colour.Xy.Y);
        
        logger.LogInformation("- Setting light '{LightName}' to colour '{ColourName}'...", light.Metadata!.Name, colour.Name);
        await localHueApi.UpdateLightAsync(light.Id, updateLightRequest);
    }

    WriteLine("* Scene Result *");
    WriteLine($"Scene: {scene.Name}");
    WriteLine($"Description: {scene.Description}");
    WriteLine("Lights:");
    lightSettings.ToList().ForEach(lightSetting => WriteLine($"- {lightSetting.Key.Metadata!.Name}: {lightSetting.Value.Name}"));

    Write("* Press Y to create another scene... *");
    exit = ReadKey().Key != ConsoleKey.Y;
    situation = string.Empty;
    WriteLine();
}
while (!exit);

logger.LogInformation("Exiting...");