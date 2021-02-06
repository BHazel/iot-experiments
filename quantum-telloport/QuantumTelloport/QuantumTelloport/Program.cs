using static System.Console;
using Microsoft.Quantum.Simulation.Core;
using Microsoft.Quantum.Simulation.Simulators;
using BWHazel.Apps.QuantumTelloport.Control;

const long MinimumDroneDistance = 20;

QuantumSimulator quantumSimulator = new();
long axis = await DetermineAxis.Run(quantumSimulator);
long direction = await DetermineDirection.Run(quantumSimulator);
long distance = await DetermineDistance.Run(quantumSimulator);

long droneDistance = distance + MinimumDroneDistance;
string droneCommand = (axis, direction) switch
{
    (1, 1) => $"forward {droneDistance}",
    (1, 0) => $"backward {droneDistance}",
    (0, 1) => $"right {droneDistance}",
    (0, 0) => $"left {droneDistance}",
    (_, _) => $"land"
};

WriteLine(droneCommand);